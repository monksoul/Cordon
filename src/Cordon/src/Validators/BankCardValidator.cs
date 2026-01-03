// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     银行卡号验证器（Luhn 算法）
/// </summary>
/// <remarks>
///     <see href="https://baike.baidu.com/item/Luhn算法/22799984">Luhn 算法</see>
///     <see href="https://www.ee.unb.ca/cgi-bin/tervo/luhn.pl">Luhn 算法在线测试</see>
/// </remarks>
public partial class BankCardValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="BankCardValidator" />
    /// </summary>
    public BankCardValidator() => UseResourceKey(() => nameof(ValidationMessages.BankCardValidator_ValidationError));

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 空检查
        if (value is null)
        {
            return true;
        }

        // 清理输入：去除空格/特殊字符（如 '-' 或空格）
        var sanitized = value switch
        {
            string s => s.Replace(" ", "").Replace("-", ""),
            _ => value.ToString()?.Replace(" ", "").Replace("-", "")
        };

        // 格式验证 + Luhn 算法校验
        return !string.IsNullOrWhiteSpace(sanitized) && Regex().IsMatch(sanitized) && CheckLuhn(sanitized);
    }

    /// <summary>
    ///     Luhn 算法校验
    /// </summary>
    /// <param name="number">卡号</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool CheckLuhn(string number)
    {
        var sum = 0;
        var length = number.Length;

        for (var i = 0; i < length; i++)
        {
            var add = (number[i] - '0') * (2 - ((i + length) % 2));
            add -= add > 9 ? 9 : 0;
            sum += add;
        }

        return sum % 10 == 0;
    }

    /// <summary>
    ///     银行卡号正则表达式
    /// </summary>
    /// <returns>
    ///     <see cref="System.Text.RegularExpressions.Regex" />
    /// </returns>
    [GeneratedRegex(@"^[1-9]\d{11,18}$")]
    private static partial Regex Regex();
}