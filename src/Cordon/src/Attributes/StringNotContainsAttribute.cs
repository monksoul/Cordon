// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     不包含特定字符/字符串的验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class StringNotContainsAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="StringNotContainsValidator" />
    internal readonly StringNotContainsValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="StringNotContainsAttribute" />
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    public StringNotContainsAttribute(char searchValue)
        : this(searchValue.ToString())
    {
    }

    /// <summary>
    ///     <inheritdoc cref="StringNotContainsAttribute" />
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    public StringNotContainsAttribute(string searchValue)
    {
        SearchValue = searchValue;
        _validator = new StringNotContainsValidator(searchValue);

        UseResourceKey(() => nameof(ValidationMessages.StringNotContainsValidator_ValidationError));
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