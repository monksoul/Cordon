// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     <see cref="decimal" /> 类型比较验证特性抽象基类
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public abstract class NumericComparisonAttribute : ComparisonAttribute
{
    /// <inheritdoc />
    protected NumericComparisonAttribute(IComparable compareValue, string resourceKey)
        : base(compareValue, resourceKey)
    {
    }

    /// <inheritdoc />
    protected NumericComparisonAttribute(IComparable compareValue, Func<string> errorMessageResourceAccessor)
        : base(compareValue, errorMessageResourceAccessor)
    {
    }

    /// <inheritdoc />
    protected sealed override bool IsValid(IComparable value) =>
        IsValid(Convert.ToDecimal(value), Convert.ToDecimal(CompareValue));

    /// <summary>
    ///     检查对象合法性
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="compareValue">比较的值</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    protected abstract bool IsValid(decimal value, decimal compareValue);
}