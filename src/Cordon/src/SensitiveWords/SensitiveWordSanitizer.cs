// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     敏感词清理器（基于 Aho‑Corasick 自动机）
/// </summary>
/// <remarks>
///     <para>分隔符：支持 <c>|</c>、<c>,</c>、<c>\t</c>、<c>;</c> 任意混用，连续分隔符自动跳过。同时兼容一行一个词。</para>
///     <para>注释：以 <c>#</c> 开头的整行将被忽略；行内 <c>#</c> 之后的内容将被截断忽略。</para>
///     <para>词库：https://github.com/konsheng/Sensitive-lexicon</para>
/// </remarks>
public sealed class SensitiveWordSanitizer
{
    /// <summary>
    ///     预计算的符号跳过映射表
    /// </summary>
    internal static readonly bool[] SkipMap = InitSkipMap();

    /// <summary>
    ///     以下字符在匹配时一律视为"隐形分隔符"
    /// </summary>
    /// <remarks>
    ///     <para>包含：_ / - / 空格 / 全角空格 / 制表符 / 回车 / 换行</para>
    ///     <para>匹配时始终跳过的"隐形分隔符"（例如 <c>敏_感_词</c> 视同于 <c>敏感词</c>）</para>
    /// </remarks>
    internal static readonly SearchValues<char> IgnoredSeparators =
        SearchValues.Create(['_', '-', ' ', '\t', '　', '\r', '\n']);

    /// <inheritdoc cref="SensitiveWordOptions" />
    internal readonly SensitiveWordOptions _options;

    /// <summary>
    ///     根节点
    /// </summary>
    internal readonly TrieNode _root;

    /// <summary>
    ///     <inheritdoc cref="SensitiveWordSanitizer" />
    /// </summary>
    /// <param name="root">根节点</param>
    /// <param name="options">
    ///     <see cref="SensitiveWordOptions" />
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    private SensitiveWordSanitizer(TrieNode root, SensitiveWordOptions options)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(options);

        _root = root;
        _options = options;
    }

    /// <summary>
    ///     从 <see cref="Stream" /> 加载词库并构建敏感词清理器
    /// </summary>
    /// <param name="stream">输入流</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static SensitiveWordSanitizer CreateFromStream(Stream stream, SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(stream);
        options ??= SensitiveWordOptions.Default;

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

        // 初始化 HashSet 实例
        var words = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 初始化 StreamReader 实例
        using var streamReader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true, bufferSize: 81920);

        // 循环读取流的每一行
        while (streamReader.ReadLine() is { } line)
        {
            ParseLine(line, words);
        }

        return Build(words, options);
    }

    /// <summary>
    ///     从文件路径加载词库并构建敏感词清理器
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static SensitiveWordSanitizer CreateFromPath(string filePath, SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        options ??= SensitiveWordOptions.Default;

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Sensitive word file not found.", filePath);
        }

        // 打开并读取文件流
        using var fileStream = File.OpenRead(filePath);

        return CreateFromStream(fileStream, options);
    }

    /// <summary>
    ///     从内存词表构建敏感词清理器
    /// </summary>
    /// <param name="words">敏感词集合</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer Build(IEnumerable<string> words, SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(words);
        options ??= SensitiveWordOptions.Default;

        // 初始化 TrieNode 实例
        var root = new TrieNode();
        root.Fail = root;

        // 初始化并复用 StringBuilder 实例
        var coreBuilder = new StringBuilder();

        // 构建 Trie 树
        foreach (var originalWord in words)
        {
            // 空检查
            if (string.IsNullOrWhiteSpace(originalWord))
            {
                continue;
            }

            // 清空 StringBuilder
            coreBuilder.Clear();
            var coreLength = 0;

            // 移除分隔符生成纯净匹配键，同时计算核心长度
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var ch in originalWord)
            {
                // 始终跳过分隔符
                if (IgnoredSeparators.Contains(ch))
                {
                    continue;
                }

                // 符号过滤（仅当选项启用时）
                if (options.IgnoreSymbol && ShouldSkip(ch))
                {
                    continue;
                }

                coreBuilder.Append(ch);
                coreLength++;
            }

            var normalizedKey = coreBuilder.ToString();

            // 空检查
            if (string.IsNullOrEmpty(normalizedKey))
            {
                continue;
            }

            // 处理大小写
            if (options.IgnoreCase)
            {
                normalizedKey = normalizedKey.ToLowerInvariant();
            }

            // 初始化根节点
            var node = root;

            foreach (var c in normalizedKey)
            {
                if (!node.Children.TryGetValue(c, out var next))
                {
                    next = new TrieNode();
                    node.Children[c] = next;
                }

                node = next;
            }

            node.IsEnd = true;

            // 保留原始词及其核心长度（含分隔符）
            if (node.MatchedWordSet.Add(originalWord))
            {
                node.MatchedWords.Add((originalWord, coreLength));
            }
        }

        // 构建 Fail 指针
        var queue = new Queue<TrieNode>(256);

        // 根的直接子节点 fail 指向 root
        foreach (var child in root.Children.Values)
        {
            child.Fail = root;
            queue.Enqueue(child);
        }

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            // 继承 fail 节点的匹配状态
            if (currentNode.Fail is { IsEnd: true })
            {
                currentNode.IsEnd = true;

                // 合并 fail 链上的匹配词
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var w in currentNode.Fail.MatchedWords)
                {
                    if (currentNode.MatchedWordSet.Add(w.Word))
                    {
                        currentNode.MatchedWords.Add(w);
                    }
                }
            }

            foreach (var (c, child) in currentNode.Children)
            {
                // 查找 fail 路径：从父节点的 fail 开始回溯
                var failNode = currentNode.Fail ?? root;
                while (failNode != root && !failNode.Children.ContainsKey(c))
                {
                    failNode = failNode.Fail ?? root;
                }

                // 设置子节点的 fail 指针
                child.Fail = failNode.Children.GetValueOrDefault(c, root);

                queue.Enqueue(child);
            }
        }

        return new SensitiveWordSanitizer(root, options);
    }

    /// <summary>
    ///     检查文本并返回所有命中词及精确位置
    /// </summary>
    /// <param name="text">待检测文本</param>
    /// <returns><see cref="MatchResult" />[]</returns>
    public MatchResult[] FindMatches(string? text)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(text))
        {
            return [];
        }

        // 初始化匹配结果集合
        var matches = new List<MatchResult>(32);

        var node = _root;
        var virtualIndex = 0;

        // 记录 虚拟索引 -> 真实索引 的映射
        var realIndexMap = ArrayPool<int>.Shared.Rent(text.Length);

        try
        {
            // 遍历文本每一个字符
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                // 始终跳过分隔符（_/-/空格/全角空格/制表符/回车/换行），使输入流与 Trie 结构对齐
                if (IgnoredSeparators.Contains(c))
                {
                    continue;
                }

                // 符号过滤：跳过但不增加虚拟索引（仅当选项启用时）
                if (_options.IgnoreSymbol && ShouldSkip(c))
                {
                    continue;
                }

                // 记录映射关系
                realIndexMap[virtualIndex] = i;
                var matchChar = _options.IgnoreCase ? char.ToLowerInvariant(c) : c;

                // AC 状态跳转：当前节点无匹配时，沿 Fail 指针回溯
                TrieNode? next;
                while (!node.Children.TryGetValue(matchChar, out next))
                {
                    if (node == _root)
                    {
                        break;
                    }

                    node = node.Fail ?? _root;
                }

                if (next != null)
                {
                    node = next;

                    // 收集匹配结果
                    if (node is { IsEnd: true, MatchedWords.Count: > 0 })
                    {
                        foreach (var (word, coreLength) in node.MatchedWords)
                        {
                            var startVirtual = virtualIndex - coreLength + 1;

                            // 边界检查
                            if (startVirtual >= 0)
                            {
                                matches.Add(new MatchResult(word, realIndexMap[startVirtual],
                                    realIndexMap[virtualIndex] + 1));
                            }
                        }
                    }
                }

                virtualIndex++;
            }

            return matches.ToArray();
        }
        finally
        {
            ArrayPool<int>.Shared.Return(realIndexMap);
        }
    }

    /// <summary>
    ///     检查文本并返回首个命中词及精确位置
    /// </summary>
    /// <param name="text">待检测文本</param>
    /// <returns>
    ///     <see cref="MatchResult" />
    /// </returns>
    public MatchResult? FindFirst(string? text)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var node = _root;
        var virtualIndex = 0;

        // 记录 虚拟索引 -> 真实索引 的映射
        var realIndexMap = ArrayPool<int>.Shared.Rent(text.Length);

        try
        {
            // 遍历文本每一个字符
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                // 始终跳过分隔符（_/-/空格/全角空格/制表符/回车/换行），使输入流与 Trie 结构对齐
                if (IgnoredSeparators.Contains(c))
                {
                    continue;
                }

                // 符号过滤：跳过但不增加虚拟索引（仅当选项启用时）
                if (_options.IgnoreSymbol && ShouldSkip(c))
                {
                    continue;
                }

                // 记录映射关系
                realIndexMap[virtualIndex] = i;
                var matchChar = _options.IgnoreCase ? char.ToLowerInvariant(c) : c;

                // AC 状态跳转：当前节点无匹配时，沿 Fail 指针回溯
                TrieNode? next;
                while (!node.Children.TryGetValue(matchChar, out next))
                {
                    if (node == _root)
                    {
                        break;
                    }

                    node = node.Fail ?? _root;
                }

                if (next != null)
                {
                    node = next;

                    // 收集匹配结果
                    if (node is { IsEnd: true, MatchedWords.Count: > 0 })
                    {
                        foreach (var (word, coreLength) in node.MatchedWords)
                        {
                            var startVirtual = virtualIndex - coreLength + 1;

                            // 边界检查
                            if (startVirtual >= 0)
                            {
                                return new MatchResult(word, realIndexMap[startVirtual],
                                    realIndexMap[virtualIndex] + 1);
                            }
                        }
                    }
                }

                virtualIndex++;
            }

            return null;
        }
        finally
        {
            ArrayPool<int>.Shared.Return(realIndexMap);
        }
    }

    /// <summary>
    ///     快速检查是否包含敏感词
    /// </summary>
    /// <param name="text">待检测文本</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool Contains(string? text)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var node = _root;

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var c in text)
        {
            // 始终跳过分隔符
            if (IgnoredSeparators.Contains(c))
            {
                continue;
            }

            // 符号过滤（仅当选项启用时）
            if (_options.IgnoreSymbol && ShouldSkip(c))
            {
                continue;
            }

            var matchChar = _options.IgnoreCase ? char.ToLowerInvariant(c) : c;

            // AC 状态跳转：当前节点无匹配时，沿 Fail 指针回溯
            TrieNode? next;
            while (!node.Children.TryGetValue(matchChar, out next))
            {
                if (node == _root)
                {
                    break;
                }

                node = node.Fail ?? _root;
            }

            // ReSharper disable once InvertIf
            if (next != null)
            {
                node = next;
                if (node.IsEnd)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    ///     替换敏感词为指定字符
    /// </summary>
    /// <remarks>保留原文本符号与空格结构。</remarks>
    /// <param name="text">原始文本</param>
    /// <param name="replaceChar">替换字符，默认值为：<c>*</c>）</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public string Replace(string text, char replaceChar = '*')
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        // 检查文本并返回所有命中词及精确位置
        var matches = FindMatches(text);

        // 空检查
        if (matches.Length == 0)
        {
            return text;
        }

        // 初始化 StringBuilder 实例
        var stringBuilder = new StringBuilder(text.Length);
        var lastEnd = 0;

        // 按起始位置排序，若起始位置相同则按长度降序
        Array.Sort(matches, (a, b) =>
        {
            var cmp = a.StartIndex.CompareTo(b.StartIndex);
            return cmp != 0 ? cmp : b.EndIndex.CompareTo(a.EndIndex);
        });

        // 遍历所有匹配结果
        foreach (var m in matches)
        {
            // 跳过被前面匹配覆盖的重叠部分
            if (m.StartIndex < lastEnd)
            {
                continue;
            }

            // 追加未匹配部分
            stringBuilder.Append(text, lastEnd, m.StartIndex - lastEnd);
            // 替换敏感词
            stringBuilder.Append(replaceChar, m.EndIndex - m.StartIndex);

            lastEnd = m.EndIndex;
        }

        // 追加剩余部分
        stringBuilder.Append(text, lastEnd, text.Length - lastEnd);

        return stringBuilder.ToString();
    }

    /// <summary>
    ///     解析单行词库
    /// </summary>
    /// <remarks>
    ///     <para>分隔符：支持 <c>|</c>、<c>,</c>、<c>\t</c>、<c>;</c> 任意混用，连续分隔符自动跳过。同时兼容一行一个词。</para>
    ///     <para>注释：以 <c>#</c> 开头的整行将被忽略；行内 <c>#</c> 之后的内容将被截断忽略。</para>
    ///     <para>公开此方法，方便后续用户解析并获取敏感词词库列表。</para>
    /// </remarks>
    /// <param name="line">词库中的一行原始文本</param>
    /// <param name="words">用于收集解析结果的 <see cref="HashSet{T}" /> 集合</param>
    public static void ParseLine(string line, HashSet<string> words)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(line))
        {
            return;
        }

        // 移除前后空格
        var trimmed = line.Trim();

        // 处理 # 字符开头的注释
        if (trimmed.Length == 0 || trimmed[0] == '#')
        {
            return;
        }

        // 处理行内注释
        var commentIdx = trimmed.IndexOf('#');
        if (commentIdx > 0)
        {
            trimmed = trimmed[..commentIdx].TrimEnd();
        }

        // 空检查
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return;
        }

        var span = trimmed.AsSpan();
        var start = 0;

        // 处理 | , \t ; 分隔符
        for (var i = 0; i < span.Length; i++)
        {
            var c = span[i];

            // ReSharper disable once InvertIf
            if (c is '|' or ',' or '\t' or ';')
            {
                if (i > start)
                {
                    // 将单词片段添加到词集
                    AddWord(span.Slice(start, i - start), words);
                }

                start = i + 1;
            }
        }

        // 处理最后一个词
        if (start < span.Length)
        {
            // 将单词片段添加到词集
            AddWord(span[start..], words);
        }
    }

    /// <summary>
    ///     将单词片段添加到词集
    /// </summary>
    /// <param name="span">单词片段</param>
    /// <param name="words">目标词集</param>
    internal static void AddWord(ReadOnlySpan<char> span, HashSet<string> words)
    {
        // 移除前后空格
        var word = span.Trim().ToString();

        // 检查非空白字符串
        if (word.Length > 0)
        {
            words.Add(word);
        }
    }

    /// <summary>
    ///     判断字符是否应被跳过
    /// </summary>
    /// <remarks>
    ///     <para>此方法仅受 <see cref="SensitiveWordOptions.IgnoreSymbol" /> 参数控制，用于跳过标点/符号/空白</para>
    ///     <para>分隔符（_/-/空格/全角空格/制表符/回车/换行）的跳过由 <see cref="IgnoredSeparators" /> 独立控制，始终生效</para>
    /// </remarks>
    /// <param name="c">待检测字符</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool ShouldSkip(char c) => SkipMap[c];

    /// <summary>
    ///     初始化符号跳过映射表
    /// </summary>
    /// <returns><see cref="bool" />[]</returns>
    internal static bool[] InitSkipMap()
    {
        var map = new bool[65536];
        for (var i = 0; i < 65536; i++)
        {
            var c = (char)i;
            map[i] = char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsSymbol(c);
        }

        return map;
    }

    /// <summary>
    ///     Aho‑Corasick 自动机的字典树节点
    /// </summary>
    internal sealed class TrieNode
    {
        /// <summary>
        ///     <see cref="TrieNode" />
        /// </summary>
        internal TrieNode? Fail { get; set; }

        /// <summary>
        ///     是否为某个敏感词的结尾
        /// </summary>
        internal bool IsEnd { get; set; }

        /// <summary>
        ///     子节点映射字典
        /// </summary>
        internal Dictionary<char, TrieNode> Children { get; set; } = new(64);

        /// <summary>
        ///     在此节点结束的所有敏感词及其核心长度
        /// </summary>
        internal List<(string Word, int CoreLength)> MatchedWords { get; set; } = new(2);

        /// <summary>
        ///     辅助去重集合
        /// </summary>
        internal HashSet<string> MatchedWordSet { get; set; } = new(2);
    }
}