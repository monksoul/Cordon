// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     敏感词清理器（基于 Aho‑Corasick 自动机）
/// </summary>
/// <remarks>
///     <para>参考文献：https://zhuanlan.zhihu.com/p/368184958</para>
///     <para>分隔符：支持 <c>|</c>、<c>,</c>、<c>\t</c>、<c>;</c> 任意混用，连续分隔符自动跳过。同时兼容一行一个词。</para>
///     <para>注释：以 <c>#</c> 开头的整行将被忽略；行内 <c>#</c> 之后的内容将被截断忽略。</para>
/// </remarks>
public sealed class SensitiveWordSanitizer
{
    /// <summary>
    ///     是否忽略大小写
    /// </summary>
    internal readonly bool _ignoreCase;

    /// <summary>
    ///     是否跳过标点/空格/符号进行匹配
    /// </summary>
    internal readonly bool _ignoreSymbol;

    /// <summary>
    ///     根节点
    /// </summary>
    internal readonly TrieNode _root;

    /// <summary>
    ///     <inheritdoc cref="SensitiveWordSanitizer" />
    /// </summary>
    /// <param name="root">根节点</param>
    /// <param name="ignoreCase">是否忽略大小写</param>
    /// <param name="ignoreSymbol">是否跳过标点/空格/符号进行匹配</param>
    /// <exception cref="ArgumentNullException"></exception>
    private SensitiveWordSanitizer(TrieNode root, bool ignoreCase, bool ignoreSymbol)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(root);

        _root = root;
        _ignoreCase = ignoreCase;
        _ignoreSymbol = ignoreSymbol;
    }

    /// <summary>
    ///     从 <see cref="Stream" /> 加载词库并构建敏感词清理器
    /// </summary>
    /// <param name="stream">输入流</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static SensitiveWordSanitizer CreateFromStream(Stream stream, bool ignoreCase = true,
        bool ignoreSymbol = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(stream);

        // 检查流是否可读
        if (!stream.CanRead)
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentException("Stream must be readable.", nameof(stream));
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

        return Build(words, ignoreCase, ignoreSymbol);
    }

    /// <summary>
    ///     从文件路径加载词库并构建敏感词清理器
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    public static SensitiveWordSanitizer CreateFromPath(string filePath, bool ignoreCase = true,
        bool ignoreSymbol = true)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Sensitive word file not found.", filePath);
        }

        // 打开并读取文件流
        using var stream = File.OpenRead(filePath);

        return CreateFromStream(stream, ignoreCase, ignoreSymbol);
    }

    /// <summary>
    ///     从内存词表构建敏感词清理器
    /// </summary>
    /// <param name="words">敏感词集合</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer Build(IEnumerable<string> words, bool ignoreCase = true,
        bool ignoreSymbol = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(words);

        // 初始化 TrieNode 实例
        var root = new TrieNode();
        root.Fail = root;

        // 构建 Trie 树
        foreach (var word in words)
        {
            // 空检查
            if (string.IsNullOrWhiteSpace(word))
            {
                continue;
            }

            // 初始化根节点
            var node = root;
            foreach (var c in word.Select(ch => ignoreCase ? char.ToLowerInvariant(ch) : ch))
            {
                if (!node.Children.TryGetValue(c, out var next))
                {
                    next = new TrieNode();
                    node.Children[c] = next;
                }

                node = next;
            }

            node.IsEnd = true;
            node.MatchedWords.Add(word);
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
            if (currentNode.Fail?.IsEnd == true)
            {
                currentNode.IsEnd = true;

                // 合并 fail 链上的匹配词
                foreach (var w in currentNode.Fail.MatchedWords.Where(w => !currentNode.MatchedWords.Contains(w)))
                {
                    currentNode.MatchedWords.Add(w);
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

                // 提前合并 fail 节点的匹配词
                if (child.Fail.IsEnd)
                {
                    foreach (var w in child.Fail.MatchedWords.Where(w => !child.MatchedWords.Contains(w)))
                    {
                        child.MatchedWords.Add(w);
                    }
                }

                queue.Enqueue(child);
            }
        }

        return new SensitiveWordSanitizer(root, ignoreCase, ignoreSymbol);
    }

    /// <summary>
    ///     检查文本并返回所有命中词及精确位置
    /// </summary>
    /// <param name="text">待检测文本</param>
    /// <returns><see cref="MatchResult" />[]</returns>
    public MatchResult[] FindMatches(string text)
    {
        // 空检查
        if (string.IsNullOrEmpty(text))
        {
            return [];
        }

        var matches = new List<MatchResult>(32);
        var node = _root;
        var virtualIndex = 0;

        // 记录 虚拟索引 -> 真实索引 的映射
        var realIndexMap = ArrayPool<int>.Shared.Rent(text.Length);
        try
        {
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                // 符号过滤：跳过但不增加虚拟索引
                if (_ignoreSymbol && ShouldSkip(c))
                {
                    continue;
                }

                // 记录映射关系
                realIndexMap[virtualIndex] = i;
                var matchChar = _ignoreCase ? char.ToLowerInvariant(c) : c;

                // AC 状态跳转：当前节点无匹配时，沿 Fail 指针回溯
                while (node != _root && !node.Children.ContainsKey(matchChar))
                {
                    node = node.Fail ?? _root;
                }

                if (node.Children.TryGetValue(matchChar, out var child))
                {
                    node = child;

                    // 收集匹配结果
                    if (node is { IsEnd: true, MatchedWords.Count: > 0 })
                    {
                        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                        foreach (var word in node.MatchedWords)
                        {
                            var startVirtual = virtualIndex - word.Length + 1;
                            if (startVirtual >= 0 && startVirtual < realIndexMap.Length)
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
    ///     快速检查是否包含敏感词
    /// </summary>
    /// <param name="text">待检测文本</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool Contains(string text)
    {
        // 空检查
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        var node = _root;

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var c in text)
        {
            if (_ignoreSymbol && ShouldSkip(c))
            {
                continue;
            }

            var matchChar = _ignoreCase ? char.ToLowerInvariant(c) : c;

            while (node != _root && !node.Children.ContainsKey(matchChar))
            {
                node = node.Fail ?? _root;
            }

            // ReSharper disable once InvertIf
            if (node.Children.TryGetValue(matchChar, out var child))
            {
                node = child;
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
    /// <param name="replaceChar">替换字符（默认 <c>'*'</c>）</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public string Replace(string text, char replaceChar = '*')
    {
        // 空检查
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var matches = FindMatches(text);
        if (matches.Length == 0)
        {
            return text;
        }

        var stringBuilder = new StringBuilder(text.Length);
        var lastEnd = 0;

        // 按起始位置排序，若起始位置相同则按长度降序
        Array.Sort(matches, (a, b) =>
        {
            var cmp = a.StartIndex.CompareTo(b.StartIndex);
            return cmp != 0 ? cmp : b.EndIndex.CompareTo(a.EndIndex);
        });

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
    /// </remarks>
    /// <param name="line">词库中的一行原始文本</param>
    /// <param name="words">用于收集解析结果的 <see cref="HashSet{T}" /> 集合</param>
    internal static void ParseLine(string line, HashSet<string> words)
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

        // 处理 | , \t ; 分隔符
        var span = trimmed.AsSpan();

        var start = 0;
        for (var i = 0; i < span.Length; i++)
        {
            var c = span[i];

            // ReSharper disable once InvertIf
            if (c is '|' or ',' or '\t' or ';')
            {
                if (i > start)
                {
                    AddWord(span.Slice(start, i - start), words);
                }

                start = i + 1;
            }
        }

        // 处理最后一个词
        if (start < span.Length)
        {
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
    /// <param name="c">待检测字符</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool ShouldSkip(char c) => char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsSymbol(c);
}