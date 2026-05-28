// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     敏感词验证特性
/// </summary>
/// <remarks>
///     <para>支持通过 <see cref="DictionaryName" /> 或 <see cref="FilePath" /> 配置词库来源。</para>
///     <para>注意：<see cref="DictionaryName" /> 与 <see cref="FilePath" /> 互斥，只能设置其中一个。</para>
///     <para>若需注入 <see cref="SensitiveWordSanitizer" /> 实例，请通过 <see cref="SensitiveWordSanitizerFactory" /> 注册或手动构建。</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class SensitiveWordAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="SensitiveWordAttribute" />
    /// </summary>
    public SensitiveWordAttribute() => UseResourceKey(GetResourceKey);

    /// <summary>
    ///     敏感词字典名称
    /// </summary>
    /// <remarks>
    ///     <para>用于从 <see cref="SensitiveWordSanitizerFactory.Get(string)" /> 中获取 <see cref="SensitiveWordSanitizer" /> 实例。</para>
    ///     <para>对应 <see cref="SensitiveWordSanitizerFactory" /> 中缓存的字典名称。与 <see cref="FilePath" /> 互斥，只能设置其中一个。</para>
    /// </remarks>
    public string? DictionaryName { get; set; }

    /// <summary>
    ///     敏感词文件路径
    /// </summary>
    /// <remarks>
    ///     与 <see cref="DictionaryName" /> 互斥，只能设置其中一个。支持后续通过
    ///     <see cref="SensitiveWordSanitizerFactory.Refresh(string)" /> 进行刷新（热更新）。
    /// </remarks>
    public string? FilePath { get; set; }

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
        var dictionaryNameSet = !string.IsNullOrWhiteSpace(DictionaryName);
        var filePathSet = !string.IsNullOrWhiteSpace(FilePath);

        // 以下组合是非法的，会抛出 InvalidOperationException：
        // 1) 没有任何数据源被配置
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (!dictionaryNameSet && !filePathSet)
        {
            try
            {
                // 尝试使用默认的 SensitiveWordOptions.DefaultDictionaryName 回退
                return SensitiveWordSanitizerFactory.Get(SensitiveWordOptions.DefaultDictionaryName);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException(
                    $"No dictionary source is configured for the {nameof(SensitiveWordAttribute)}, and the default dictionary '{SensitiveWordOptions.DefaultDictionaryName}' has not been registered. Please either set the '{nameof(DictionaryName)}' or '{nameof(FilePath)}' property, or register the default dictionary via `SensitiveWordSanitizerFactory.GetOrCreateFromPath` at application startup.");
            }
        }

        // 2) 同时配置了多个数据源
        if (dictionaryNameSet && filePathSet)
        {
            throw new InvalidOperationException(
                $"Multiple dictionary sources are configured for the {nameof(SensitiveWordAttribute)}. Please set either '{nameof(DictionaryName)}' or '{nameof(FilePath)}'.");
        }

        // 如果设置了 DictionaryName，则从工厂缓存中获取
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (dictionaryNameSet)
        {
            return SensitiveWordSanitizerFactory.Get(DictionaryName!);
        }

        // 否则通过文件路径加载（使用 SensitiveWordOptions.DefaultDictionaryName 作为字典名称）
        return SensitiveWordSanitizerFactory.GetOrCreateFromPath(FilePath!);
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