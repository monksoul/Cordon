// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     大于验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class GreaterThanAttribute : ComparisonAttribute
{
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
        : base(compareValue, nameof(ValidationMessages.GreaterThanValidator_ValidationError)) =>
        Validator = new GreaterThanValidator(compareValue);

    /// <summary>
    ///     <inheritdoc cref="GreaterThanValidator" />
    /// </summary>
    protected GreaterThanValidator Validator { get; }

    /// <inheritdoc />
    protected override bool IsValid(IComparable value) => Validator.IsValid(value);
}