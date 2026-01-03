// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     非空白字符串验证器
/// </summary>
public class NotBlankValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="NotBlankValidator" />
    /// </summary>
    public NotBlankValidator() => UseResourceKey(() => nameof(ValidationMessages.NotBlankValidator_ValidationError));

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text),
            char c => !char.IsWhiteSpace(c) && c != '\0',
            _ => false
        };
}