// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     IP 地址验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class IpAddressAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="IpAddressValidator" />
    internal readonly IpAddressValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="IpAddressAttribute" />
    /// </summary>
    public IpAddressAttribute()
    {
        _validator = new IpAddressValidator();

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     是否允许 IPv6 地址
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowIPv6
    {
        get;
        set
        {
            field = value;
            _validator.AllowIPv6 = value;
        }
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

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
}