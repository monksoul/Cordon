// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     验证数值的小数位数验证器
/// </summary>
public class DecimalPlacesValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="DecimalPlacesValidator" />
    /// </summary>
    /// <param name="maxDecimalPlaces">允许的最大有效小数位数</param>
    public DecimalPlacesValidator(int maxDecimalPlaces)
    {
        // 检查允许的最大有效小数位数合法性
        if (maxDecimalPlaces < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxDecimalPlaces),
                // ReSharper disable once LocalizableElement
                "maxDecimalPlaces must be a non-negative number.");
        }

        MaxDecimalPlaces = maxDecimalPlaces;

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
    public bool AllowStringValues { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value)
    {
        // 空检查
        if (value is null)
        {
            return true;
        }

        decimal decimalValue;

        // 检查是否允许字符串数值
        if (AllowStringValues && value is string stringValue)
        {
            if (!decimal.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimalValue))
            {
                return false;
            }
        }
        // 检查是否是数值类型
        else if (value.GetType().IsNumeric())
        {
            decimalValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        }
        else
        {
            return false;
        }

        // 获取 decimal 值的实际小数位数
        var decimalPlaces = GetDecimalPlaces(decimalValue);

        return decimalPlaces <= MaxDecimalPlaces;
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MaxDecimalPlaces);

    /// <summary>
    ///     获取 <see cref="decimal" /> 值的实际有效小数位数
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>
    ///     <see cref="int" />
    /// </returns>
    internal static int GetDecimalPlaces(decimal value)
    {
        var bits = decimal.GetBits(value);
        return (bits[3] >> 16) & 0x7FF;
    }
}