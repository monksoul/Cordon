// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     域名验证器
/// </summary>
/// <remarks>不含协议（如 https/http）。</remarks>
public partial class DomainValidator : ValidatorBase
{
    /// <inheritdoc cref="IdnMapping" />
    internal readonly IdnMapping _idnMapping;

    /// <summary>
    ///     <inheritdoc cref="DomainValidator" />
    /// </summary>
    public DomainValidator()
    {
        _idnMapping = new IdnMapping { AllowUnassigned = true };

        UseResourceKey(() => nameof(ValidationMessages.DomainValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text) && ValidateDomain(text),
            _ => false
        };

    /// <summary>
    ///     验证域名有效性
    /// </summary>
    /// <param name="domain">域名</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ValidateDomain(string domain)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        try
        {
            // 将 Unicode 域名转换为 Punycode 格式
            var asciiDomain = _idnMapping.GetAscii(domain);

            // 检查域名总长度是否超过 RFC 1034 规定的 253 字节限制
            // 参考文献：https://www.rfc-editor.org/info/rfc1034
            return asciiDomain.Length <= 253 && Regex().IsMatch(asciiDomain);
        }
        // 转换失败（如包含非法字符、未分配 Unicode 等），视为无效域名
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     域名正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^([a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$")]
    private static partial Regex Regex();
}