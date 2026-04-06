// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证值是否为有效的 <see cref="decimal" /> 类型验证器
/// </summary>
/// <remarks>支持精度、标度和正负数配置。</remarks>
public class DecimalValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="DecimalValidator" />
    /// </summary>
    /// <param name="precision">总位数（精度），默认值为：<c>18</c></param>
    /// <param name="scale">小数位数（标度），默认值为：<c>2</c></param>
    public DecimalValidator(int precision = 18, int scale = 2)
    {
        // 检查总位数（精度）合法性
        if (precision <= 0)
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentOutOfRangeException(nameof(precision), "precision must be a positive number.");
        }

        // 检查小数位数（标度）合法性
        if (scale < 0 || scale > precision)
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentOutOfRangeException(nameof(scale), "scale must be between 0 and precision.");
        }

        Precision = precision;
        Scale = scale;

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     总位数（精度）
    /// </summary>
    public int Precision { get; }

    /// <summary>
    ///     小数位数（标度）
    /// </summary>
    public int Scale { get; }

    /// <summary>
    ///     是否允许负数
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowNegative { get; set; }

    /// <summary>
    ///     是否允许字符串数值
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowStringValues { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 空检查
        if (value is null)
        {
            return true;
        }

        decimal decimalValue;

        // 检查是否是否允许字符串数值
        if (AllowStringValues && value is string valueAsString)
        {
            if (!decimal.TryParse(valueAsString, NumberStyles.Number, CultureInfo.InvariantCulture, out decimalValue))
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

        // 检查是否允许负数
        if (!AllowNegative && decimalValue < 0)
        {
            return false;
        }

        // 检查小数位数（标度）
        var actualScale = GetActualScale(decimalValue);
        if (actualScale > Scale)
        {
            return false;
        }

        // 将数值转换为字符串
        var stringValue = decimalValue.ToString(CultureInfo.InvariantCulture);
        var digitCount = stringValue.Length;

        // 检查字符串数值是否是负数
        if (stringValue.StartsWith('-'))
        {
            digitCount--;
        }

        // 检查字符串数值是否包含小数点
        if (stringValue.Contains('.'))
        {
            digitCount--;
        }

        // 检查总位数（精度）
        return digitCount <= Precision;
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Precision, Scale);

    /// <summary>
    ///     获取 <see cref="decimal" /> 类型值的实际小数位数（标度）
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>
    ///     <see cref="int" />
    /// </returns>
    internal static int GetActualScale(decimal value)
    {
        var bits = decimal.GetBits(value);
        return (bits[3] >> 16) & 0x7FF;
    }

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        AllowNegative
            ? nameof(ValidationMessages.DecimalValidator_ValidationError_AllowNegative)
            : nameof(ValidationMessages.DecimalValidator_ValidationError);
}