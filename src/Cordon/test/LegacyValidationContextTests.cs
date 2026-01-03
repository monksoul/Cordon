// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class LegacyValidationContextTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validationContext = new LegacyValidationContext();
        Assert.Null(validationContext.Instance);
        Assert.Null(validationContext.DisplayName);
        Assert.Null(validationContext.MemberNames);
        Assert.Null(validationContext.RuleSets);
        Assert.Empty(validationContext.Items);

        var validationContext2 = new LegacyValidationContext("Furion", "Name", ["Name"]);
        Assert.NotNull(validationContext2.Instance);
        Assert.Equal("Furion", validationContext2.Instance);
        Assert.Equal("Name", validationContext2.DisplayName);
        Assert.Equal(["Name"], validationContext2.MemberNames);
        Assert.Null(validationContext2.RuleSets);
        Assert.Empty(validationContext2.Items);
    }

    [Fact]
    public void GetService_ReturnOK()
    {
        var validationContext = new LegacyValidationContext();
        Assert.Null(validationContext.GetService<IServiceProvider>());
    }
}