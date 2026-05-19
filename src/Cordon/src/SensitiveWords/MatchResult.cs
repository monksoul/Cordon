// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     匹配结果结构体
/// </summary>
public readonly struct MatchResult
{
    /// <summary>
    ///     <inheritdoc cref="MatchResult" />
    /// </summary>
    /// <param name="word">命中的敏感词原文</param>
    /// <param name="startIndex">在原始文本中的起始索引（包含）</param>
    /// <param name="endIndex">在原始文本中的结束索引（不包含）</param>
    internal MatchResult(string word, int startIndex, int endIndex)
    {
        Word = word;
        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    /// <summary>
    ///     命中的敏感词原文
    /// </summary>
    public string Word { get; }

    /// <summary>
    ///     在原始文本中的起始索引（包含）
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    ///     在原始文本中的结束索引（不包含）
    /// </summary>
    public int EndIndex { get; }

    /// <inheritdoc />
    public override string ToString() => $"[{Word}] @ {StartIndex}..{EndIndex}";
}