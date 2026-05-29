// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     敏感词清理器构建器
/// </summary>
public sealed class SensitiveWordSanitizerBuilder
{
    /// <summary>
    ///     敏感词词库
    /// </summary>
    internal readonly HashSet<string> _words = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     <see cref="SensitiveWordOptions" />
    /// </summary>
    internal SensitiveWordOptions _options = SensitiveWordOptions.Default;

    /// <summary>
    ///     添加一个已解析的敏感词
    /// </summary>
    /// <param name="word">敏感词字符串</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddWord(string word)
    {
        // 空检查
        if (!string.IsNullOrWhiteSpace(word))
        {
            _words.Add(word);
        }

        return this;
    }

    /// <summary>
    ///     批量添加一个已解析的敏感词集合
    /// </summary>
    /// <param name="words">敏感词集合</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddWords(IEnumerable<string> words)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(words);

        // 遍历敏感词集合
        foreach (var word in words)
        {
            // 空检查
            if (!string.IsNullOrWhiteSpace(word))
            {
                _words.Add(word);
            }
        }

        return this;
    }

    /// <summary>
    ///     解析单行文本并添加其中的敏感词
    /// </summary>
    /// <param name="line">待解析的一行文本</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddLine(string line)
    {
        SensitiveWordSanitizer.ParseLine(line, _words);

        return this;
    }

    /// <summary>
    ///     解析多行文本并添加其中的敏感词
    /// </summary>
    /// <param name="lines">待解析的行集合</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddLines(IEnumerable<string> lines)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(lines);

        // 遍历待解析的行集合
        foreach (var line in lines)
        {
            SensitiveWordSanitizer.ParseLine(line, _words);
        }

        return this;
    }

    /// <summary>
    ///     从流中读取并添加敏感词
    /// </summary>
    /// <param name="stream">可读的流</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddStream(Stream stream)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(stream);

        // 检查流是否可读
        if (!stream.CanRead)
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentException("Stream must be readable.", nameof(stream));
        }

        // 检查流是否支持查找
        if (stream.CanSeek)
        {
            // 重置到起始位置
            stream.Position = 0;
        }

        // 初始化 StreamReader 实例
        using var streamReader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true, bufferSize: 81920);

        // 循环读取流的每一行
        while (streamReader.ReadLine() is { } line)
        {
            SensitiveWordSanitizer.ParseLine(line, _words);
        }

        return this;
    }

    /// <summary>
    ///     从文件路径加载并添加敏感词
    /// </summary>
    /// <param name="filePath">词库文件路径</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddPath(string filePath)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Sensitive word file not found.", filePath);
        }

        // 打开并读取文件流
        using var fileStream = File.OpenRead(filePath);

        return AddStream(fileStream);
    }

    /// <summary>
    ///     配置 <see cref="SensitiveWordOptions" />
    /// </summary>
    /// <param name="options">
    ///     <see cref="SensitiveWordOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder ConfigureOptions(SensitiveWordOptions? options)
    {
        _options = options ?? SensitiveWordOptions.Default;

        return this;
    }

    /// <summary>
    ///     配置 <see cref="SensitiveWordOptions" />
    /// </summary>
    /// <param name="configurator">自定义配置委托</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder ConfigureOptions(Func<SensitiveWordOptions, SensitiveWordOptions> configurator)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configurator);

        // 调用自定义配置委托
        _options = configurator(_options);

        return this;
    }

    /// <summary>
    ///     清空已添加的所有敏感词，并将选项恢复为默认值
    /// </summary>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder Clear()
    {
        _words.Clear();
        _options = SensitiveWordOptions.Default;

        return this;
    }

    /// <summary>
    ///     构建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public SensitiveWordSanitizer Build() => SensitiveWordSanitizer.Build(_words, _options);
}