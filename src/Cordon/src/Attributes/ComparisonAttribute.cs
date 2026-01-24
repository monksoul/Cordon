// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     比较验证特性抽象基类
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public abstract class ComparisonAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="ComparisonAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <param name="resourceKey">资源属性名</param>
    protected ComparisonAttribute(IComparable compareValue, string resourceKey)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValue);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);

        CompareValue = compareValue;

        UseResourceKey(() => resourceKey);
    }

    /// <summary>
    ///     <inheritdoc cref="ComparisonAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <param name="errorMessageResourceAccessor">错误信息资源访问器</param>
    protected ComparisonAttribute(IComparable compareValue, Func<string> errorMessageResourceAccessor)
        : base(errorMessageResourceAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValue);

        CompareValue = compareValue;
    }

    /// <summary>
    ///     比较的值
    /// </summary>
    public IComparable CompareValue { get; }

    /// <summary>
    ///     检查对象是否合法
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    protected abstract bool IsValid(IComparable value);

    /// <inheritdoc />
    public sealed override bool IsValid(object? value) =>
        value switch
        {
            null => true,
            IComparable val => IsValid(val),
            _ => false
        };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, CompareValue);
}