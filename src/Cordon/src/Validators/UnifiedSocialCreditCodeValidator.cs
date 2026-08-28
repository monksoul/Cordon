// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     统一社会信用代码验证器
/// </summary>
public partial class UnifiedSocialCreditCodeValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="UnifiedSocialCreditCodeValidator" />
    /// </summary>
    public UnifiedSocialCreditCodeValidator() => UseResourceKey(GetResourceKey);

    /// <summary>
    ///     是否使用宽松匹配模式
    /// </summary>
    /// <remarks>允许 15/18/20 位数字或字母。默认值为：<c>false</c>。</remarks>
    public bool AllowLooseMatch { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 空检查
        if (value is null)
        {
            return true;
        }

        // 检查是否是字符串值
        if (value is not string stringValue)
        {
            return false;
        }

        return AllowLooseMatch ? LooseRegex().IsMatch(stringValue) : StrictRegex().IsMatch(stringValue);
    }

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

    /// <summary>
    ///     严格的统一社会信用代码正则表达式
    /// </summary>
    /// <remarks>18 位统一社会信用代码。</remarks>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^[0-9A-HJ-NPQRTUWXY]{2}\d{6}[0-9A-HJ-NPQRTUWXY]{10}$")]
    private static partial Regex StrictRegex();

    /// <summary>
    ///     宽松模式的统一社会信用代码正则表达式
    /// </summary>
    /// <remarks>15/18/20 位统一社会信用代码。</remarks>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^(([0-9A-Za-z]{15})|([0-9A-Za-z]{18})|([0-9A-Za-z]{20}))$")]
    private static partial Regex LooseRegex();
}