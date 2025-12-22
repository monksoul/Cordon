// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     最小值验证器
/// </summary>
public class MinValidator : ComparisonValidator
{
    /// <summary>
    ///     <inheritdoc cref="MinValidator" />
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    public MinValidator(IComparable minimum)
        : base(minimum, nameof(ValidationMessages.MinValidator_ValidationError))
    {
    }

    /// <inheritdoc />
    protected override bool IsValid(IComparable value) =>
        IsTypeMatchedToCompareValue(value) && value.CompareTo(CompareValue) >= 0;
}