// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     Aho‑Corasick 自动机的字典树节点
/// </summary>
internal sealed class TrieNode
{
    /// <summary>
    ///     <see cref="Fail" /> 指针：匹配失败时跳转的目标节点
    /// </summary>
    internal TrieNode? Fail { get; set; }

    /// <summary>
    ///     是否为某个敏感词的结尾
    /// </summary>
    internal bool IsEnd { get; set; }

    /// <summary>
    ///     子节点映射：字符 -> <see cref="TrieNode" />
    /// </summary>
    internal Dictionary<char, TrieNode> Children { get; set; } = new(64);

    /// <summary>
    ///     在此节点结束的所有敏感词及其核心长度
    /// </summary>
    internal List<(string Word, int CoreLength)> MatchedWords { get; set; } = new(2);
}