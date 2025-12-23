// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     <inheritdoc cref="EnumAttribute" />
/// </summary>
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
    ///     <inheritdoc cref="EnumAttribute" />
    /// </summary>
    public EnumAttribute(Type enumType)
    {
        EnumType = enumType;
        Validator = new EnumValidator(enumType);

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     枚举类型
    /// </summary>
    public Type EnumType { get; }

    /// <summary>
    ///     是否支持 Flags 模式
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool SupportFlags
    {
        get;
        set
        {
            field = value;
            Validator.SupportFlags = value;
        }
    }

    /// <summary>
    ///     <inheritdoc cref="EnumValidator" />
    /// </summary>
    protected EnumValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, EnumType.Name);

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        SupportFlags
            ? nameof(ValidationMessages.EnumValidator_ValidationError_SupportFlags)
            : nameof(ValidationMessages.EnumValidator_ValidationError);
}