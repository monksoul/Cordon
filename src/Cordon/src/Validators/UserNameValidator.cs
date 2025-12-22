// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     用户名验证器
/// </summary>
/// <remarks>
///     长度 4-16 位，以字母开头，支持字母、数字、下划线、减号组合。
///     不允许包含空格或其他特殊字符，禁止连续特殊字符（如 __）。
/// </remarks>
public partial class UserNameValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="UserNameValidator" />
    /// </summary>
    public UserNameValidator() => UseResourceKey(() => nameof(ValidationMessages.UserNameValidator_ValidationError));

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text) && Regex().IsMatch(text),
            _ => false
        };

    /// <summary>
    ///     用户名正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^[a-zA-Z](?!.*[_-]{2})[\w-]{2,14}[a-zA-Z0-9]$")]
    private static partial Regex Regex();
}