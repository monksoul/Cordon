// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class RuleSetMatcherTests
{
    [Fact]
    public void MatchesRuleSet_ReturnOK()
    {
        Assert.True(RuleSetMatcher.Matches(null, null));
        Assert.True(RuleSetMatcher.Matches(null, ["*"]));
        Assert.False(RuleSetMatcher.Matches(null, ["login"]));

        Assert.False(RuleSetMatcher.Matches(["login", "register"], null));
        Assert.True(RuleSetMatcher.Matches(["login", "register"], ["*"]));
        Assert.True(RuleSetMatcher.Matches(["login", "register"], ["login"]));
        Assert.True(RuleSetMatcher.Matches(["login", "register"], ["register"]));
        Assert.False(RuleSetMatcher.Matches(["login", "register"], ["other"]));
        Assert.True(RuleSetMatcher.Matches(["login", "register"], ["other", "login"]));

        Assert.True(RuleSetMatcher.Matches([], null));
        Assert.True(RuleSetMatcher.Matches([], ["*"]));
        Assert.False(RuleSetMatcher.Matches([], ["login"]));
    }
}