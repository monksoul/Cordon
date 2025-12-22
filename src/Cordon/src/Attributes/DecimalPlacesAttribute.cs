// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     验证数值的小数位数验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DecimalPlacesAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="DecimalPlacesAttribute" />
    /// </summary>
    /// <param name="maxDecimalPlaces">允许的最大有效小数位数</param>
    public DecimalPlacesAttribute(int maxDecimalPlaces)
    {
        MaxDecimalPlaces = maxDecimalPlaces;
        Validator = new DecimalPlacesValidator(maxDecimalPlaces);

        UseResourceKey(() => nameof(ValidationMessages.DecimalPlacesValidator_ValidationError));
    }

    /// <summary>
    ///     允许的最大有效小数位数
    /// </summary>
    public int MaxDecimalPlaces { get; }

    /// <summary>
    ///     允许字符串数值
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowStringValues
    {
        get;
        set
        {
            field = value;
            Validator.AllowStringValues = value;
        }
    }

    /// <summary>
    ///     <inheritdoc cref="DecimalPlacesValidator" />
    /// </summary>
    protected DecimalPlacesValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaxDecimalPlaces);
}