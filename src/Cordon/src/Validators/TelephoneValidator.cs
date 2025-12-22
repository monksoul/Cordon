// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     座机（电话）验证器
/// </summary>
public partial class TelephoneValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="TelephoneValidator" />
    /// </summary>
    public TelephoneValidator() => UseResourceKey(() => nameof(ValidationMessages.TelephoneValidator_ValidationError));

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text) && Regex().IsMatch(text),
            _ => false
        };

    /// <summary>
    ///     座机（电话）正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^(?:(?:\d{3}-)?\d{8}|(?:\d{4}-)?\d{7,8})(?:-\d+)?$")]
    private static partial Regex Regex();
}