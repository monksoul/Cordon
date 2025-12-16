// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.AspNetCore.Tests;

public class ValidationOptionsModelValidatorProviderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var provider = new ValidationOptionsModelValidatorProvider();
        Assert.NotNull(provider);
    }

    [Fact]
    public void CreateValidators_ReturnOK()
    {
    }
}