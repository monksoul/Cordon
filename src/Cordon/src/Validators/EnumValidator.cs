// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <inheritdoc cref="EnumValidator" />
/// </summary>
/// <typeparam name="TEnum">枚举类型</typeparam>
public class EnumValidator<TEnum> : EnumValidator
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    public EnumValidator() : base(typeof(TEnum))
    {
    }
}

/// <summary>
///     枚举验证器
/// </summary>
public class EnumValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="EnumValidator" />
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    /// <exception cref="ArgumentException"></exception>
    public EnumValidator(Type enumType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(enumType);

        // 检查是否是枚举
        if (!enumType.IsEnum)
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentException($"The type '{enumType.Name}' is not an enumeration type.", nameof(enumType));
        }

        EnumType = enumType;

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
    public bool SupportFlags { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value switch
        {
            null => true,
            Enum otherEnum when otherEnum.GetType() != EnumType => false,
            string stringValue when string.IsNullOrWhiteSpace(stringValue) => false,
            _ => IsEnumValueDefined(value)
        };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, EnumType.Name);

    /// <summary>
    ///     判断值是否为合法的枚举成员
    /// </summary>
    /// <remarks>支持 Flags 模式。</remarks>
    /// <param name="value">值</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool IsEnumValueDefined(object value)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(value);

        // 检查是否是非 Flags 模式或枚举类型无 [Flags]
        if (!SupportFlags || !EnumType.IsDefined(typeof(FlagsAttribute), false))
        {
            return Enum.IsDefined(EnumType, value);
        }

        // 显式检查枚举
        if (Enum.IsDefined(EnumType, value))
        {
            return true;
        }

        try
        {
            // 检查 Flags 模式下是否所有位都属于已定义的枚举值
            var allDefinedValues = Enum.GetValues(EnumType).Cast<object>().Select(Convert.ToUInt64)
                .Aggregate((a, b) => a | b);

            var input = Convert.ToUInt64(value);
            return input != 0 && (input & ~allDefinedValues) == 0;
        }
        catch
        {
            return false;
        }
    }

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