// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     <inheritdoc cref="EnumAttribute" />
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class EnumAttribute<TEnum> : EnumAttribute
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    public EnumAttribute() : base(typeof(TEnum))
    {
    }
}

/// <summary>
///     枚举验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class EnumAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     是否支持 Flags 模式
    /// </summary>
    internal bool _supportFlags;

    /// <inheritdoc cref="EnumValidator" />
    internal EnumValidator? _validator;

    /// <summary>
    ///     <inheritdoc cref="EnumAttribute" />
    /// </summary>
    /// <remarks>自动推断枚举类型。</remarks>
    public EnumAttribute()
    {
        _supportFlags = false;

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     <inheritdoc cref="EnumAttribute" />
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    public EnumAttribute(Type enumType)
        : this()
    {
        EnumType = enumType;
        _validator = new EnumValidator(enumType);
    }

    /// <summary>
    ///     枚举类型
    /// </summary>
    public Type? EnumType { get; private set; }

    /// <summary>
    ///     是否支持 Flags 模式
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool SupportFlags
    {
        get => _supportFlags;
        set
        {
            _supportFlags = value;
            _validator?.SupportFlags = value;
        }
    }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // 空检查
        // ReSharper disable once InvertIf
        if (_validator is null)
        {
            // 从 ValidationContext 推断成员类型
            var memberType = GetMemberType(validationContext);

            EnumType = memberType;
            _validator = new EnumValidator(memberType!) { SupportFlags = _supportFlags };
        }

        return _validator.IsValid(value)
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName),
                validationContext.MemberName is null ? null : [validationContext.MemberName]);
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, EnumType?.Name ?? "Enum");

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        _supportFlags
            ? nameof(ValidationMessages.EnumValidator_ValidationError_SupportFlags)
            : nameof(ValidationMessages.EnumValidator_ValidationError);

    /// <summary>
    ///     从 <see cref="ValidationContext" /> 推断成员类型
    /// </summary>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext" />
    /// </param>
    /// <returns>
    ///     <see cref="Type" />
    /// </returns>
    internal static Type? GetMemberType(ValidationContext validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);

        // 检查该特性是否标注在参数上
        if (string.IsNullOrEmpty(validationContext.MemberName) || (Type?)validationContext.ObjectType is null)
        {
            return validationContext.ObjectType;
        }

        // 获取成员信息
        var memberInfo = validationContext.ObjectType
            .GetMember(validationContext.MemberName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

        // 获取成员类型
        var memberType = memberInfo switch
        {
            PropertyInfo pi => pi.PropertyType,
            FieldInfo fi => fi.FieldType,
            _ => null
        };

        // 处理可空类型
        return memberType is null ? memberType : Nullable.GetUnderlyingType(memberType) ?? memberType;
    }
}