// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     验证数值是否为有效的 <see cref="decimal" /> 类型验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DecimalAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="DecimalValidator" />
    internal readonly DecimalValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="DecimalAttribute" />
    /// </summary>
    /// <param name="precision">总位数（精度），默认值为：<c>18</c></param>
    /// <param name="scale">小数位数（标度），默认值为：<c>2</c></param>
    public DecimalAttribute(int precision = 18, int scale = 2)
    {
        Precision = precision;
        Scale = scale;
        _validator = new DecimalValidator(precision, scale);

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
    public bool AllowNegative
    {
        get;
        set
        {
            field = value;
            _validator.AllowNegative = value;
        }
    }

    /// <summary>
    ///     是否允许字符串数值
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowStringValues
    {
        get;
        set
        {
            field = value;
            _validator.AllowStringValues = value;
        }
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Precision, Scale);

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