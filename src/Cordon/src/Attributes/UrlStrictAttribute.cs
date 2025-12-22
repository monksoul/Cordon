// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     URL 地址增强版验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class UrlStrictAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="UrlStrictAttribute" />
    /// </summary>
    public UrlStrictAttribute()
    {
        Validator = new UrlValidator();

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     是否支持 FTP 协议
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool SupportsFtp
    {
        get;
        set
        {
            field = value;
            Validator.SupportsFtp = value;
        }
    }

    /// <summary>
    ///     <inheritdoc cref="UrlValidator" />
    /// </summary>
    protected UrlValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);

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