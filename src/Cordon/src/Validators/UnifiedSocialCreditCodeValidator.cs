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

        // 检查是否是宽松模式
        if (AllowLooseMatch)
        {
            return LooseRegex().IsMatch(stringValue);
        }

        return StrictRegex().IsMatch(stringValue) && ValidateChecksum(stringValue);
    }

    /// <summary>
    ///     验证 18 位校验码是否合法
    /// </summary>
    /// <remarks>
    ///     参考文献：https://openstd.samr.gov.cn/bzgk/std/showGb?type=online&amp;hcno=24691C25985C1073D3A7C85629378AC0&amp;
    ///     request_locale=zh。
    /// </remarks>
    /// <param name="code">18 位统一社会信用代码</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool ValidateChecksum(string code)
    {
        // 检查是否是空值或长度不足 18 位
        if (string.IsNullOrWhiteSpace(code) || code.Length != 18)
        {
            return false;
        }

        // 初始化标准字符集和前 17 位加权因子
        const string chars = "0123456789ABCDEFGHJKLMNPQRTUWXY";
        int[] weights = [1, 3, 9, 27, 19, 26, 16, 17, 20, 29, 25, 13, 8, 24, 10, 30, 28];

        var sum = 0;

        for (var i = 0; i < 17; i++)
        {
            var value = chars.IndexOf(code[i]);

            // 检查前 17 位是否包含非法字符
            if (value < 0)
            {
                return false;
            }

            sum += value * weights[i];
        }

        // 计算校验值 C18 = 31 - (sum % 31)
        var mod = sum % 31;
        var checkValue = 31 - mod;

        // 根据标准映射为最终字符
        var expectedCheckChar = checkValue switch
        {
            31 => '0',
            30 => 'Y',
            _ => chars[checkValue]
        };

        return code[17] == expectedCheckChar;
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
    /// <remarks>18 位统一社会信用代码，字符集已排除 I、O、Z、S、V。</remarks>
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