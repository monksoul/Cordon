// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     敏感词验证器
/// </summary>
/// <remarks>
///     <para>支持多种词库加载方式：直接注入实例、引用工厂缓存名称、指定文件路径或输入流。</para>
///     <para>
///         优先级：<see cref="Sanitizer" /> > <see cref="DictionaryName" /> > <see cref="FilePath" />。
///     </para>
/// </remarks>
public class SensitiveWordValidator : ValidatorBase
{
    /// <summary>
    ///     最近一次验证命中的匹配结果详情
    /// </summary>
    internal string[]? _lastMatchDetails;

    /// <summary>
    ///     <inheritdoc cref="SensitiveWordValidator" />
    /// </summary>
    public SensitiveWordValidator() => UseResourceKey(GetResourceKey);

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
    ///     <inheritdoc cref="SensitiveWordValidator" />
    /// </summary>
    /// <param name="stream">输入流</param>
    /// <param name="dictionaryName">
    ///     字典名称，默认值为：<c>null</c>。若提供，则利用 <see cref="SensitiveWordSanitizerFactory" /> 进行缓存；否则直接读取流构建实例。
    /// </param>
    public SensitiveWordValidator(Stream stream, string? dictionaryName = null) : this()
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(stream);

        DictionaryName = dictionaryName;
        Sanitizer = !string.IsNullOrWhiteSpace(dictionaryName)
            ? SensitiveWordSanitizerFactory.GetOrCreateFromStream(dictionaryName, stream)
            : SensitiveWordSanitizer.CreateFromStream(stream);
    }

    /// <summary>
    ///     敏感词清理器实例
    /// </summary>
    /// <remarks>优先级最高。</remarks>
    public SensitiveWordSanitizer? Sanitizer { get; set; }

    /// <summary>
    ///     敏感词字典名称
    /// </summary>
    /// <remarks>
    ///     对应 <see cref="SensitiveWordSanitizerFactory" /> 中缓存的实例名称。当 <see cref="Sanitizer" /> 为空时，尝试通过此名称从工厂获取实例。
    /// </remarks>
    public string? DictionaryName { get; set; }

    /// <summary>
    ///     敏感词文件路径
    /// </summary>
    /// <remarks>
    ///     当 <see cref="Sanitizer" /> 和 <see cref="DictionaryName" /> 均为空时，使用此路径加载词库。
    /// </remarks>
    public string? FilePath { get; set; }

    /// <summary>
    ///     是否在错误信息中显示命中的敏感词详情
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool ShowMatchedWords { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 重置所有命中词及精确位置字符串
        _lastMatchDetails = null;

        // 检查值是否为字符串类型，且字符串不是由空白字符组成
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        // 获取敏感词清理器实例
        var sanitizer = GetSanitizer();

        // 检查是否在错误信息中显示命中的敏感词详情
        if (!ShowMatchedWords)
        {
            return !sanitizer.Contains(text);
        }

        // 检查文本并返回所有命中词及精确位置
        var matches = sanitizer.FindMatches(text);

        // 空检查
        if (matches.Length == 0)
        {
            return true;
        }

        // 存储所有命中词及精确位置字符串
        _lastMatchDetails = matches.Select(m => m.ToString()).ToArray();

        return false;
    }

    /// <inheritdoc />
    public override string? FormatErrorMessage(string name)
    {
        // 检查是否在错误信息中显示命中的敏感词详情
        if (ShowMatchedWords && _lastMatchDetails is { Length: > 0 })
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name,
                string.Join(", ", _lastMatchDetails));
        }

        return base.FormatErrorMessage(name);
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
        // 空检查
        if (Sanitizer is not null)
        {
            return Sanitizer;
        }

        // 从工厂缓存中获取
        if (!string.IsNullOrWhiteSpace(DictionaryName))
        {
            return SensitiveWordSanitizerFactory.Get(DictionaryName);
        }

        // 通过文件路径加载
        // ReSharper disable once InvertIf
        if (!string.IsNullOrWhiteSpace(FilePath))
        {
            return SensitiveWordSanitizerFactory.GetOrCreateFromPath(FilePath, FilePath);
        }

        throw new InvalidOperationException(
            $"No dictionary source is configured for the {nameof(SensitiveWordValidator)}. Please set the '{nameof(Sanitizer)}', '{nameof(DictionaryName)}', or '{nameof(FilePath)}' property, or provide a Stream or Sanitizer via the constructor.");
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