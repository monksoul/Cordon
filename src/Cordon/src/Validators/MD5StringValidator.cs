// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     MD5 字符串验证器
/// </summary>
/// <remarks>支持 32 位标准格式，可选 16 字符截断格式。</remarks>
public partial class MD5StringValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="MD5StringValidator" />
    /// </summary>
    public MD5StringValidator() => UseResourceKey(() => nameof(ValidationMessages.MD5StringValidator_ValidationError));

    /// <summary>
    ///     是否允许截断的 128 位哈希值（16 字节的十六进制字符串，共 32 字符）
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowShortFormat { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value switch
        {
            null => true,
            string text when string.IsNullOrWhiteSpace(text) => false,
            string text when AllowShortFormat => text.Length is 16 or 32 && Regex16Or32().IsMatch(text),
            string text => text.Length == 32 && Regex32().IsMatch(text),
            _ => false
        };

    /// <summary>
    ///     匹配 32 位十六进制字符串（标准 MD5 格式）
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^[0-9a-fA-F]{32}$")]
    private static partial Regex Regex32();

    /// <summary>
    ///     匹配 16 或 32 位十六进制字符串（用于支持截断格式）
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^([0-9a-fA-F]{16}|[0-9a-fA-F]{32})$")]
    private static partial Regex Regex16Or32();
}