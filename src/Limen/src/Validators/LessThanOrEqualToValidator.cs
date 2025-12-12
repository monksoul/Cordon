// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     小于等于验证器
/// </summary>
public class LessThanOrEqualToValidator : ComparisonValidator
{
    /// <summary>
    ///     <inheritdoc cref="LessThanOrEqualToValidator" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public LessThanOrEqualToValidator(IComparable compareValue)
        : base(compareValue, nameof(ValidationMessages.LessThanOrEqualToValidator_ValidationError))
    {
    }

    /// <inheritdoc />
    protected override bool IsValid(IComparable value) =>
        IsTypeMatchedToCompareValue(value) && value.CompareTo(CompareValue) <= 0;
}