// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     以特定字符/字符串开头的验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class StartsWithAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="StartsWithValidator" />
    internal readonly StartsWithValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="StartsWithAttribute" />
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    public StartsWithAttribute(char searchValue)
        : this(searchValue.ToString())
    {
    }

    /// <summary>
    ///     <inheritdoc cref="StartsWithAttribute" />
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    public StartsWithAttribute(string searchValue)
    {
        SearchValue = searchValue;
        _validator = new StartsWithValidator(searchValue);

        UseResourceKey(() => nameof(ValidationMessages.StartsWithValidator_ValidationError));
    }

    /// <summary>
    ///     检索的值
    /// </summary>
    public string SearchValue { get; }

    /// <summary>
    ///     <inheritdoc cref="StringComparison" />
    /// </summary>
    /// <remarks>默认值为：<see cref="StringComparison.Ordinal" />。</remarks>
    public StringComparison Comparison
    {
        get;
        set
        {
            field = value;
            _validator.Comparison = value;
        }
    } = StringComparison.Ordinal;

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, SearchValue);
}