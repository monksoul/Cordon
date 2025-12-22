// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     以特定字符/字符串结尾的验证器
/// </summary>
public class EndsWithValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="EndsWithValidator" />
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    public EndsWithValidator(char searchValue)
        : this(searchValue.ToString())
    {
    }

    /// <summary>
    ///     <inheritdoc cref="EndsWithValidator" />
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    public EndsWithValidator(string searchValue)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrEmpty(searchValue);

        SearchValue = searchValue;

        UseResourceKey(() => nameof(ValidationMessages.EndsWithValidator_ValidationError));
    }

    /// <summary>
    ///     检索的值
    /// </summary>
    public string SearchValue { get; }

    /// <summary>
    ///     <inheritdoc cref="StringComparison" />
    /// </summary>
    /// <remarks>默认值为：<see cref="StringComparison.Ordinal" />。</remarks>
    public StringComparison Comparison { get; set; } = StringComparison.Ordinal;

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value switch
        {
            null => true,
            string text => text.EndsWith(SearchValue, Comparison),
            _ => false
        };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, SearchValue);
}