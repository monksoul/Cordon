// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     最大值验证器
/// </summary>
public class MaxValidator : ComparisonValidator
{
    /// <summary>
    ///     <inheritdoc cref="MaxValidator" />
    /// </summary>
    /// <param name="maximum">允许的最大字段值</param>
    public MaxValidator(IComparable maximum)
        : base(maximum, nameof(ValidationMessages.MaxValidator_ValidationError))
    {
    }

    /// <inheritdoc />
    protected override bool IsValid(IComparable value, IValidationContext? validationContext) =>
        IsTypeMatchedToCompareValue(value) && value.CompareTo(CompareValue) <= 0;
}