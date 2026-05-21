// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class TrieNodeTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var node = new TrieNode { IsEnd = true };
        node.Fail = node;
        node.Children.Add('敏', new TrieNode { IsEnd = true });
        node.MatchedWords.Add(("敏感词", 3));

        Assert.NotNull(node.Fail);
        Assert.True(node.IsEnd);
        Assert.Single(node.Children);
        Assert.Single(node.MatchedWords);
    }
}