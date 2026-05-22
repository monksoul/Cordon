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
    ///     <inheritdoc cref="SensitiveWordValidator" />
    /// </summary>
    public SensitiveWordValidator() =>
        UseResourceKey(() => nameof(ValidationMessages.SensitiveWordValidator_ValidationError));

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
}