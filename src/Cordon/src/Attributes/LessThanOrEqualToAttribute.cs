// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     小于等于验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class LessThanOrEqualToAttribute : ComparisonAttribute
{
    /// <inheritdoc cref="LessThanOrEqualToValidator" />
    internal readonly LessThanOrEqualToValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="LessThanOrEqualToAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public LessThanOrEqualToAttribute(int compareValue)
        : this(compareValue as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="LessThanOrEqualToAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public LessThanOrEqualToAttribute(double compareValue)
        : this(compareValue as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="LessThanOrEqualToAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public LessThanOrEqualToAttribute(IComparable compareValue)
        : base(compareValue, nameof(ValidationMessages.LessThanOrEqualToValidator_ValidationError)) =>
        _validator = new LessThanOrEqualToValidator(compareValue);

    /// <inheritdoc />
    protected override bool IsValid(IComparable value) => _validator.IsValid(value);
}