// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     最大值验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class MaxAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="MaxValidator" />
    internal readonly MaxValidator _validator;

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
    {
        CompareValue = maximum;
        _validator = new MaxValidator(maximum);

        UseResourceKey(() => nameof(ValidationMessages.MaxValidator_ValidationError));
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