// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class SensitiveWordOptionsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var options = new SensitiveWordOptions();
        Assert.True(options.IgnoreCase);
        Assert.True(options.IgnoreSymbol);

        var defaultOptions = SensitiveWordOptions.Default;
        Assert.NotNull(defaultOptions);
        Assert.Same(defaultOptions, SensitiveWordOptions.Default);
    }
}