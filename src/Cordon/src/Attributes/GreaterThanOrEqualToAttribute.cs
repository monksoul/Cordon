// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     大于等于验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class GreaterThanOrEqualToAttribute : ComparisonAttribute
{
    /// <inheritdoc cref="GreaterThanOrEqualToValidator" />
    internal readonly GreaterThanOrEqualToValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="GreaterThanOrEqualToAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public GreaterThanOrEqualToAttribute(int compareValue)
        : this(compareValue as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="GreaterThanOrEqualToAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public GreaterThanOrEqualToAttribute(double compareValue)
        : this(compareValue as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="GreaterThanOrEqualToAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public GreaterThanOrEqualToAttribute(IComparable compareValue)
        : base(compareValue, nameof(ValidationMessages.GreaterThanOrEqualToValidator_ValidationError)) =>
        _validator = new GreaterThanOrEqualToValidator(compareValue);

    /// <inheritdoc />
    protected override bool IsValid(IComparable value) => _validator.IsValid(value);
}