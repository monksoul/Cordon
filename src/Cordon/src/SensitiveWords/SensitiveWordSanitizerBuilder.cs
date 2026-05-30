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
    ///     是否允许构建词集为空的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。启用后，即使未添加任何敏感词，<see cref="Build" /> 方法也不会抛出异常，而是返回一个空清理器（所有检测均返回 <c>false</c>）。</remarks>
    public bool AllowEmptyWords { get; set; }

    /// <summary>
    ///     添加一个已解析的敏感词
    /// </summary>
    /// <param name="word">敏感词</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddWord(string? word)
    {
        // 空检查
        if (!string.IsNullOrWhiteSpace(word))
        {
            _words.Add(word.Trim()); // 移除前后空格
        }

        return this;
    }

    /// <summary>
    ///     批量添加一个已解析的敏感词集合
    /// </summary>
    /// <param name="words">敏感词集合</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddWords(IEnumerable<string?> words)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(words);

        // 遍历敏感词集合
        foreach (var word in words)
        {
            // 空检查
            if (!string.IsNullOrWhiteSpace(word))
            {
                _words.Add(word.Trim()); // 移除前后空格
            }
        }

        return this;
    }

    /// <summary>
    ///     解析单行文本并添加其中的敏感词
    /// </summary>
    /// <param name="line">待解析的一行文本</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddLine(string? line)
    {
        SensitiveWordSanitizer.ParseLine(line, _words);

        return this;
    }

    /// <summary>
    ///     解析多行文本并添加其中的敏感词
    /// </summary>
    /// <param name="lines">待解析的行集合</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder AddLines(IEnumerable<string?> lines)
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

        // 解析文件路径
        var resolvedPath = ResolveFilePath(filePath);

        // 检查文件是否存在
        if (!File.Exists(resolvedPath))
        {
            throw new FileNotFoundException("Sensitive word file not found.", resolvedPath);
        }

        // 打开并读取文件流
        using var fileStream = File.OpenRead(resolvedPath);

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
    /// <remarks>可通过 <c>options => options with { IgnoreCase = false }</c> 方式更改其属性。</remarks>
    /// <param name="configurator">自定义配置委托</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder ConfigureOptions(
        Func<SensitiveWordOptions, SensitiveWordOptions?> configurator)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configurator);

        // 调用自定义配置委托
        _options = configurator(_options) ?? SensitiveWordOptions.Default;

        return this;
    }

    /// <summary>
    ///     清空已添加的所有敏感词
    /// </summary>
    /// <remarks>注意：选项也将恢复为默认值。</remarks>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizerBuilder" />
    /// </returns>
    public SensitiveWordSanitizerBuilder Clear()
    {
        // 重置字典和配置选项
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
    /// <exception cref="InvalidOperationException"></exception>
    public SensitiveWordSanitizer Build()
    {
        // 空词集检查：敏感词清理器的核心用途是匹配敏感词，若词集为空则构建无意义。
        // 提前抛异常可避免用户误以为验证生效，而实际上所有输入都将被放行。
        if (_words.Count == 0 && !AllowEmptyWords)
        {
            throw new InvalidOperationException(
                $"Cannot build a {nameof(SensitiveWordSanitizer)} with an empty word list. Please add at least one sensitive word via {nameof(AddWord)}, {nameof(AddPath)}, {nameof(AddStream)}, etc.");
        }

        return SensitiveWordSanitizer.Build(_words, _options);
    }

    /// <summary>
    ///     解析文件路径
    /// </summary>
    /// <remarks>如果已是绝对路径，直接返回。如果是相对路径，基于 <see cref="AppContext.BaseDirectory" /> 解析。</remarks>
    /// <param name="filePath">文件路径</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string ResolveFilePath(string filePath)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 移除前后空格
        filePath = filePath.Trim();

        // 处理绝对和相对路径问题
        var basePath = Path.IsPathRooted(filePath)
            ? filePath
            : Path.Combine(AppContext.BaseDirectory, filePath);

        return Path.GetFullPath(basePath);
    }
}