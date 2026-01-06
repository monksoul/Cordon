// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class CompositeModeTests
{
    [Fact]
    public void Definition_ReturnOK()
    {
        var names = Enum.GetNames(typeof(CompositeMode));
        Assert.Equal(3, names.Length);

        var strings = new[] { nameof(CompositeMode.FailFast), nameof(CompositeMode.All), nameof(CompositeMode.Any) };
        Assert.True(strings.SequenceEqual(names));
    }
}