// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     手机号（中国）验证器
/// </summary>
/// <remarks>支持国际区号（如：13800138000 或 +8613800138000 或 008613800138000）。</remarks>
public partial class PhoneNumberValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="PhoneNumberValidator" />
    /// </summary>
    public PhoneNumberValidator() =>
        UseResourceKey(() => nameof(ValidationMessages.PhoneNumberValidator_ValidationError));

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text) && Regex().IsMatch(text),
            _ => false
        };

    /// <summary>
    ///     手机号正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^(?:(?:\+|00)86)?1(?:3\d|4[579]|5[0-35-9]|6[6-8]|7[0-8]|8\d|9[0-35-9])\d{8}$")]
    private static partial Regex Regex();
}