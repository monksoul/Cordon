// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     最小值验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class MinAttribute : ComparisonAttribute
{
    /// <inheritdoc cref="MinValidator" />
    internal readonly MinValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="MinAttribute" />
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    public MinAttribute(int minimum)
        : this(minimum as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="MinAttribute" />
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    public MinAttribute(double minimum)
        : this(minimum as IComparable)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="MinAttribute" />
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    public MinAttribute(IComparable minimum)
        : base(minimum, nameof(ValidationMessages.MinValidator_ValidationError)) =>
        _validator = new MinValidator(minimum);

    /// <inheritdoc />
    protected override bool IsValid(IComparable value) => _validator.IsValid(value);
}