// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     敏感词验证器
/// </summary>
public class SensitiveWordValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="SensitiveWordValidator" />
    /// </summary>
    public SensitiveWordValidator() => UseResourceKey(GetResourceKey);

    /// <summary>
    ///     <inheritdoc cref="SensitiveWordValidator" />
    /// </summary>
    /// <param name="dictionaryName">敏感词字典名称</param>
    public SensitiveWordValidator(string dictionaryName)
        : this()
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(dictionaryName);

        DictionaryName = dictionaryName;
    }

    /// <summary>
    ///     <inheritdoc cref="SensitiveWordValidator" />
    /// </summary>
    /// <param name="sanitizer">
    ///     <see cref="SensitiveWordSanitizer" />
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    public SensitiveWordValidator(SensitiveWordSanitizer sanitizer)
        : this()
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(sanitizer);

        Sanitizer = sanitizer;
    }

    /// <summary>
    ///     敏感词清理器实例
    /// </summary>
    /// <remarks>直接注入的实例，优先级最高。设置后 <see cref="DictionaryName" /> 和 <see cref="ConfigureBuilder" /> 将作为备选。</remarks>
    public SensitiveWordSanitizer? Sanitizer { get; set; }

    /// <summary>
    ///     敏感词字典名称
    /// </summary>
    /// <remarks>
    ///     <para>用于从 <see cref="SensitiveWordSanitizerFactory.Get(string)" /> 中获取 <see cref="SensitiveWordSanitizer" /> 实例。</para>
    ///     <para>当 <see cref="Sanitizer" /> 未设置时，通过该名称从工厂获取已注册的实例。</para>
    ///     <para>若同时提供了 <see cref="ConfigureBuilder" />，则使用该名称作为缓存键进行构建（若工厂中尚未缓存）。</para>
    ///     <para>若未提供该名称且需要构建，将使用 <see cref="SensitiveWordOptions.DefaultDictionaryName" />。</para>
    /// </remarks>
    public string? DictionaryName { get; set; }

    /// <summary>
    ///     构建器配置委托
    /// </summary>
    /// <remarks>
    ///     <para>当 <see cref="Sanitizer" /> 和 <see cref="DictionaryName" />（已缓存）均无法提供实例时，使用此委托构建敏感词清理器。</para>
    ///     <para>
    ///         构建后的实例将通过
    ///         <see cref="SensitiveWordSanitizerFactory.GetOrCreate(string, Action{SensitiveWordSanitizerBuilder})" />
    ///         进行缓存，缓存键为 <see cref="DictionaryName" /> 或 <see cref="SensitiveWordOptions.DefaultDictionaryName" />。
    ///     </para>
    /// </remarks>
    public Action<SensitiveWordSanitizerBuilder>? ConfigureBuilder { get; set; }

    /// <summary>
    ///     是否在错误信息中显示命中的敏感词详情
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool ShowMatchedWords { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 检查值是否为字符串类型，且字符串不是由空白字符组成
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        // 获取敏感词清理器实例
        var sanitizer = GetSanitizer();

        return !sanitizer.Contains(text);
    }

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(object? value, IValidationContext? validationContext)
    {
        // 检查值是否为字符串类型，且字符串不是由空白字符组成
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        // 获取敏感词清理器实例
        var sanitizer = GetSanitizer();

        // 检查文本并返回所有命中词及精确位置
        return sanitizer.FindMatches(text) is not { Length: > 0 } matches
            ? null
            :
            [
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!, matches),
                    validationContext?.MemberNames)
            ];
    }

    /// <inheritdoc />
    public override void Validate(object? value, IValidationContext? validationContext)
    {
        // 检查值是否为字符串类型，且字符串不是由空白字符组成
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        // 获取敏感词清理器实例
        var sanitizer = GetSanitizer();

        // 检查文本并返回首个命中词及精确位置
        if (sanitizer.FindFirst(text) is { } firstMatch)
        {
            throw new ValidationException(
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!, [firstMatch]),
                    validationContext?.MemberNames), null, value);
        }
    }

    /// <summary>
    ///     格式化错误信息
    /// </summary>
    /// <param name="name">显示名称</param>
    /// <param name="matches"><see cref="MatchResult" />[]</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public virtual string? FormatErrorMessage(string name, MatchResult[]? matches)
    {
        var template = ErrorMessageString;

        // 空检查
        if (string.IsNullOrWhiteSpace(template))
        {
            return base.FormatErrorMessage(name);
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
        // 尝试获取 Sanitizer 实例
        if (Sanitizer is not null)
        {
            return Sanitizer;
        }

        // 尝试从工厂缓存获取
        if (!string.IsNullOrWhiteSpace(DictionaryName))
        {
            try
            {
                return SensitiveWordSanitizerFactory.Get(DictionaryName);
            }
            catch (InvalidOperationException) { }
        }

        // 尝试使用 ConfigureBuilder 构建
        if (ConfigureBuilder is not null)
        {
            // 获取有效的字典名称
            var effectiveName = !string.IsNullOrWhiteSpace(DictionaryName)
                ? DictionaryName
                : SensitiveWordOptions.DefaultDictionaryName;

            return SensitiveWordSanitizerFactory.GetOrCreate(effectiveName, ConfigureBuilder);
        }

        // 回退到默认字典（当未指定 DictionaryName 且未提供 ConfigureBuilder）
        if (string.IsNullOrWhiteSpace(DictionaryName))
        {
            try
            {
                return SensitiveWordSanitizerFactory.Get(SensitiveWordOptions.DefaultDictionaryName);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"No sensitive word source is configured for the {nameof(SensitiveWordValidator)}. Either set '{nameof(Sanitizer)}', '{nameof(DictionaryName)}', or '{nameof(ConfigureBuilder)}', or register the default dictionary '{SensitiveWordOptions.DefaultDictionaryName}' via `SensitiveWordSanitizerFactory.GetOrCreate`.");
            }
        }

        // DictionaryName 指定了但无缓存且无 ConfigureBuilder
        throw new InvalidOperationException(
            $"The dictionary '{DictionaryName}' has not been registered in the factory. Please register it first, or provide a '{nameof(ConfigureBuilder)}' to build it.");
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