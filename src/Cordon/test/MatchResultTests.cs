// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class MatchResultTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var result = new MatchResult("敏感词", 1, 5);
        Assert.Equal("敏感词", result.Word);
        Assert.Equal(1, result.StartIndex);
        Assert.Equal(5, result.EndIndex);
    }

    [Fact]
    public void ToString_ReturnOK()
    {
        var result = new MatchResult("敏感词", 1, 5);
        Assert.Equal("[敏感词] @ 1..5", result.ToString());
    }
}