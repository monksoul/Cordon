// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     最大值验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class MaxAttribute : ComparisonAttribute
{
    /// <summary>
    ///     <inheritdoc cref="MaxAttribute" />
    /// </summary>
    /// <param name="maximum">允许的最大字段值</param>
    public MaxAttribute(int maximum)
        : this(maximum as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="MaxAttribute" />
    /// </summary>
    /// <param name="maximum">允许的最大字段值</param>
    public MaxAttribute(double maximum)
        : this(maximum as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="MaxAttribute" />
    /// </summary>
    /// <param name="maximum">允许的最大字段值</param>
    public MaxAttribute(IComparable maximum)
        : base(maximum, nameof(ValidationMessages.MaxValidator_ValidationError)) =>
        Validator = new MaxValidator(maximum);

    /// <summary>
    ///     <inheritdoc cref="MaxValidator" />
    /// </summary>
    protected MaxValidator Validator { get; }

    /// <inheritdoc />
    protected override bool IsValid(IComparable value) => Validator.IsValid(value);
}