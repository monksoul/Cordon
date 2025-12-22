// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     中文姓名验证器
/// </summary>
public partial class ChineseNameValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="ChineseNameValidator" />
    /// </summary>
    public ChineseNameValidator() =>
        UseResourceKey(() => nameof(ValidationMessages.ChineseNameValidator_ValidationError));

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text) && Regex().IsMatch(text),
            _ => false
        };

    /// <summary>
    ///     中文姓名正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^(?:[\u4e00-\u9fa5·]{2,16})$")]
    private static partial Regex Regex();
}