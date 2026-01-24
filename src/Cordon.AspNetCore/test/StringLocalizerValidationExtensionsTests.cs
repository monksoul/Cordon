// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.AspNetCore.Tests;

public class StringLocalizerValidationExtensionsTests
{
    [Fact]
    public void GetString_ReturnOK()
    {
        Assert.Equal("错误信息", StringLocalizerValidationExtensions.GetString(null, "错误信息"));
        Assert.Equal("Name 错误信息", StringLocalizerValidationExtensions.GetString(null, "{0} 错误信息", "Name"));
    }
}