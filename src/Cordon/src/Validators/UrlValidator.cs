// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     URL 地址验证器
/// </summary>
public class UrlValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="UrlValidator" />
    /// </summary>
    public UrlValidator() => UseResourceKey(GetResourceKey);

    /// <summary>
    ///     是否支持 FTP 协议
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool SupportsFtp { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text) && ValidateUrl(text),
            _ => false
        };

    /// <summary>
    ///     验证 URL 地址有效性
    /// </summary>
    /// <param name="url">URL 地址</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ValidateUrl(string url)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        // 尝试创建一个新的 Uri 实例
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
        {
            return false;
        }

        return (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps ||
                (SupportsFtp && uriResult.Scheme == Uri.UriSchemeFtp)) &&
               !string.IsNullOrEmpty(uriResult.Host); // 不接受无主机名的 URL（如：http:///path）
    }

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        SupportsFtp
            ? nameof(ValidationMessages.UrlValidator_ValidationError_SupportsFtp)
            : nameof(ValidationMessages.UrlValidator_ValidationError);
}