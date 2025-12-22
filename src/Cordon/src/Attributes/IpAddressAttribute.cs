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
    /// <summary>
    ///     <inheritdoc cref="IpAddressAttribute" />
    /// </summary>
    public IpAddressAttribute()
    {
        Validator = new IpAddressValidator();

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
            Validator.AllowIPv6 = value;
        }
    }

    /// <summary>
    ///     <inheritdoc cref="IpAddressValidator" />
    /// </summary>
    protected IpAddressValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);

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