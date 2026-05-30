// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     敏感词验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class SensitiveWordAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="SensitiveWordAttribute" />
    /// </summary>
    public SensitiveWordAttribute() => UseResourceKey(GetResourceKey);

    /// <summary>
    ///     <inheritdoc cref="SensitiveWordAttribute" />
    /// </summary>
    /// <param name="dictionaryName">敏感词字典名称</param>
    public SensitiveWordAttribute(string dictionaryName)
        : this()
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(dictionaryName);

        DictionaryName = dictionaryName;
    }

    /// <summary>
    ///     敏感词字典名称
    /// </summary>
    /// <remarks>
    ///     <para>用于从 <see cref="SensitiveWordSanitizerFactory.Get(string)" /> 中获取 <see cref="SensitiveWordSanitizer" /> 实例。</para>
    ///     <para>
    ///         该名称需已在应用启动时通过
    ///         <see cref="SensitiveWordSanitizerFactory.GetOrCreate(string, Action{SensitiveWordSanitizerBuilder})" /> 注册。
    ///     </para>
    ///     <para>若未提供该名称且需要构建，将使用 <see cref="SensitiveWordSanitizerFactory.DefaultName" />。</para>
    /// </remarks>
    public string? DictionaryName { get; set; }

    /// <summary>
    ///     是否在错误信息中显示命中的敏感词详情
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool ShowMatchedWords { get; set; }

    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // 检查值是否为字符串类型，且字符串不是由空白字符组成
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return ValidationResult.Success;
        }

        // 获取敏感词清理器实例
        var sanitizer = GetSanitizer();

        // 检查文本并返回所有命中词及精确位置
        return sanitizer.FindMatches(text) is not { Length: > 0 } matches
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!, matches));
    }

    /// <summary>
    ///     格式化错误信息
    /// </summary>
    /// <param name="name">显示名称</param>
    /// <param name="matches"><see cref="MatchResult" />[]</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public virtual string FormatErrorMessage(string name, MatchResult[]? matches)
    {
        var template = ErrorMessageString;

        // 空检查
        if (string.IsNullOrWhiteSpace(template))
        {
            return null!;
        }

        // 检查是否在错误信息中显示命中的敏感词详情
        if (!ShowMatchedWords)
        {
            return string.Format(CultureInfo.CurrentCulture, template, name);
        }

        // 将验证命中的匹配结果详情组合成字符串
        var wordsString = string.Join(", ", (matches ?? []).Select(u => u.ToString()));

        // 检查错误信息字符串是否包含 {1} 占位符
        if (template.Contains("{1}"))
        {
            return string.Format(CultureInfo.CurrentCulture, template, name, wordsString);
        }

        return string.Format(CultureInfo.CurrentCulture, template, name) + $" Matched: {wordsString}";
    }

    /// <summary>
    ///     获取敏感词清理器实例
    /// </summary>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    internal SensitiveWordSanitizer GetSanitizer()
    {
        // 尝试从工厂缓存获取
        if (!string.IsNullOrWhiteSpace(DictionaryName))
        {
            try
            {
                return SensitiveWordSanitizerFactory.Get(DictionaryName);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"The dictionary '{DictionaryName}' has not been registered. Please register it first using `SensitiveWordSanitizerFactory.GetOrCreate`.");
            }
        }

        // 回退到默认字典（当未指定 DictionaryName）
        try
        {
            return SensitiveWordSanitizerFactory.Get(SensitiveWordSanitizerFactory.DefaultName);
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException(
                $"No dictionary name is configured for the {nameof(SensitiveWordAttribute)}, and the default dictionary '{SensitiveWordSanitizerFactory.DefaultName}' has not been registered. Either set the '{nameof(DictionaryName)}' property, or register the default dictionary via `SensitiveWordSanitizerFactory.GetOrCreate`.");
        }
    }

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        ShowMatchedWords
            ? nameof(ValidationMessages.SensitiveWordValidator_ValidationError_ShowMatchedWords)
            : nameof(ValidationMessages.SensitiveWordValidator_ValidationError);
}