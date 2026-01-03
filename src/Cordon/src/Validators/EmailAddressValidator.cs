// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     邮箱地址验证器
/// </summary>
public partial class EmailAddressValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="EmailAddressValidator" />
    /// </summary>
    public EmailAddressValidator() =>
        UseResourceKey(() => nameof(ValidationMessages.EmailAddressValidator_ValidationError));

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text) && Regex().IsMatch(text),
            _ => false
        };

    /// <summary>
    ///     邮箱地址正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(
        """^(([^<>()\$$\].,;:\s@"]+(\.[^<>()\$$\].,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$""")]
    private static partial Regex Regex();
}