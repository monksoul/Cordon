// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     IP 地址验证器
/// </summary>
public partial class IpAddressValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="IpAddressValidator" />
    /// </summary>
    public IpAddressValidator() => UseResourceKey(GetResourceKey);

    /// <summary>
    ///     是否允许 IPv6 地址
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowIPv6 { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value switch
        {
            null => true,
            string ipString => CheckIpAddress(ipString, AllowIPv6),
            _ => false
        };

    /// <summary>
    ///     检查是否是有效的 IP 地址
    /// </summary>
    /// <param name="ipString">IP 地址字符串</param>
    /// <param name="allowIPv6">是否允许 IPv6 地址</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool CheckIpAddress(string ipString, bool allowIPv6)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(ipString))
        {
            return false;
        }

        // 如果不包含 ':'，则视为 IPv4，必须严格符合 x.x.x.x 格式
        if (!ipString.Contains(':') && !StrictIPv4Regex().IsMatch(ipString))
        {
            return false;
        }

        // 尝试解析 IP 地址
        if (!IPAddress.TryParse(ipString, out var address))
        {
            return false;
        }

        return address.AddressFamily == AddressFamily.InterNetwork ||
               (allowIPv6 && address.AddressFamily == AddressFamily.InterNetworkV6);
    }

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        AllowIPv6
            ? nameof(ValidationMessages.IpAddressValidator_ValidationError_AllowIPv6)
            : nameof(ValidationMessages.IpAddressValidator_ValidationError);

    /// <summary>
    ///     严格的 IPv4 地址正则表达式
    /// </summary>
    /// <remarks>IPv4 路径必须严格符合 <c>x.x.x.x</c> 格式。</remarks>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^(?:\d{1,3}\.){3}\d{1,3}$")]
    private static partial Regex StrictIPv4Regex();
}