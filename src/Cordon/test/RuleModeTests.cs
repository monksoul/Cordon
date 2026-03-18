// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class RuleModeTests
{
    [Fact]
    public void Definition_ReturnOK()
    {
        var names = Enum.GetNames(typeof(RuleMode));
        Assert.Equal(3, names.Length);

        var strings = new[] { nameof(RuleMode.All), nameof(RuleMode.FailFast), nameof(RuleMode.Any) };
        Assert.True(strings.SequenceEqual(names));
    }
}