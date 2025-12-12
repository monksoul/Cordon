// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     颜色值验证器
/// </summary>
public partial class ColorValueValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="ColorValueValidator" />
    /// </summary>
    public ColorValueValidator() =>
        UseResourceKey(() => nameof(ValidationMessages.ColorValueValidator_ValidationError));

    /// <summary>
    ///     是否启用完整模式
    /// </summary>
    /// <remarks>在完整模式下，支持的颜色格式包括：十六进制颜色、RGB、RGBA、HSL 和 HSLA。若未启用，则仅支持：十六进制颜色、RGB 和 RGBA。默认值为：<c>false</c>。</remarks>
    public bool FullMode { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text) && (FullMode ? Regex() : StandardRegex()).IsMatch(text),
            _ => false
        };

    /// <summary>
    ///     颜色值正则表达式（完整模式）
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(
        @"^(?:#(?:(?:[0-9a-fA-F]{3}){1,2}|[0-9a-fA-F]{8})|rgba?\((?:\s*\d+\%?\s*,){2}\s*(?:\d+\%?\s*(?:,\s*[0-9.]+\s*)?)?\)|hsla?\((?:\s*\d+\%?\s*,){2}\s*(?:\d+\%?\s*(?:,\s*[0-9.]+\s*)?)?\)|hwb\((?:\s*\d+\%?\s*,){2}\s*(?:\d+\%?\s*)?\)|lch\((?:\s*\d+\%?\s*,){2}\s*(?:\d+\%?\s*)?\)|oklch\((?:\s*\d+\%?\s*,){2}\s*(?:\d+\%?\s*)?\)|lab\((?:\s*[-+]?\d+\%?\s*,){2}\s*[-+]?\d+\%?\s*\)|oklab\((?:\s*[-+]?\d+\%?\s*,){2}\s*[-+]?\d+\%?\s*\))$",
        RegexOptions.IgnoreCase)]
    private static partial Regex Regex();

    /// <summary>
    ///     颜色值正则表达式（标准模式）
    /// </summary>
    /// <remarks>仅支持：十六进制颜色、RGB 和 RGBA。</remarks>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(
        @"^(?:#(?:[0-9a-fA-F]{3}){1,2}|rgba?\((?:\s*(?:\d+%?)\s*,){2}\s*(?:\d+%?)\s*(?:,\s*(?:\d+(?:\.\d+)?|\.\d+))?\))$",
        RegexOptions.IgnoreCase)]
    private static partial Regex StandardRegex();
}