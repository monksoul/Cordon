// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     大于验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class GreaterThanAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="GreaterThanValidator" />
    internal readonly GreaterThanValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="GreaterThanAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public GreaterThanAttribute(int compareValue)
        : this(compareValue as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="GreaterThanAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public GreaterThanAttribute(double compareValue)
        : this(compareValue as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="GreaterThanAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public GreaterThanAttribute(IComparable compareValue)
    {
        CompareValue = compareValue;
        _validator = new GreaterThanValidator(compareValue);

        UseResourceKey(() => nameof(ValidationMessages.GreaterThanValidator_ValidationError));
    }

    /// <summary>
    ///     比较的值
    /// </summary>
    public IComparable CompareValue { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, CompareValue);
}