// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     统一社会信用代码验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class UnifiedSocialCreditCodeAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="UnifiedSocialCreditCodeValidator" />
    internal readonly UnifiedSocialCreditCodeValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="UnifiedSocialCreditCodeAttribute" />
    /// </summary>
    public UnifiedSocialCreditCodeAttribute()
    {
        _validator = new UnifiedSocialCreditCodeValidator();

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     是否使用宽松匹配模式
    /// </summary>
    /// <remarks>允许 15/18/20 位数字或字母。默认值为：<c>false</c>。</remarks>
    public bool AllowLooseMatch
    {
        get;
        set
        {
            field = value;
            _validator.AllowLooseMatch = value;
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
        AllowLooseMatch
            ? nameof(ValidationMessages.UnifiedSocialCreditCodeValidator_ValidationError_AllowLooseMatch)
            : nameof(ValidationMessages.UnifiedSocialCreditCodeValidator_ValidationError);
}