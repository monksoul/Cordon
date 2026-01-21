// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     正则表达式验证器
/// </summary>
public class RegularExpressionValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="RegularExpressionValidator" />
    /// </summary>
    /// <param name="pattern">正则表达式模式</param>
    public RegularExpressionValidator([StringSyntax(StringSyntaxAttribute.Regex)] string pattern)
    {
        Pattern = pattern;
        MatchTimeoutInMilliseconds = 2000;

        UseResourceKey(() => nameof(ValidationMessages.RegularExpressionValidator_ValidationError));
    }

    /// <summary>
    ///     正则表达式模式
    /// </summary>
    public string Pattern { get; }

    /// <summary>
    ///     用于在操作超时前执行单个匹配操作的时间量
    /// </summary>
    /// <remarks>以毫秒为单位，默认值为：2000。</remarks>
    public int MatchTimeoutInMilliseconds { get; set; }

    /// <summary>
    ///     匹配正则表达式模式时要使用的超时值
    /// </summary>
    public TimeSpan MatchTimeout => TimeSpan.FromMilliseconds(MatchTimeoutInMilliseconds);

    /// <summary>
    ///     缓存正则表达式 <see cref="Regex" /> 实例
    /// </summary>
    internal Regex? Regex { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 确保 Regex 实例已初始化
        SetupRegex();

        // 将对象转换为字符串
        var stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);

        // 空检查
        if (string.IsNullOrEmpty(stringValue))
        {
            return true;
        }

        // 使用 EnumerateMatches 遍历所有匹配项
        foreach (var valueMatch in Regex!.EnumerateMatches(stringValue))
        {
            // 判断是否完全匹配
            return valueMatch.Index == 0 && valueMatch.Length == stringValue.Length;
        }

        return false;
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        // 确保 Regex 实例已初始化
        SetupRegex();

        return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Pattern);
    }

    /// <summary>
    ///     初始化 <see cref="Regex" /> 实例
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    internal void SetupRegex()
    {
        // 空检查
        if (Regex is not null)
        {
            return;
        }

        // 空检查
        if (string.IsNullOrEmpty(Pattern))
        {
            throw new InvalidOperationException("The pattern must be set to a valid regular expression.");
        }

        Regex = MatchTimeoutInMilliseconds == -1
            ? new Regex(Pattern)
            : new Regex(Pattern, default, TimeSpan.FromMilliseconds(MatchTimeoutInMilliseconds));
    }
}