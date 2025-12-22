// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidationOptionsAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeUsageAttribute = typeof(ValidationOptionsAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsageAttribute);
        Assert.Equal(AttributeTargets.Class | AttributeTargets.Method, attributeUsageAttribute.ValidOn);
        Assert.False(attributeUsageAttribute.AllowMultiple);
        Assert.True(attributeUsageAttribute.Inherited);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var attribute = new ValidationOptionsAttribute();
        Assert.Null(attribute.RuleSets);

        var attribute2 = new ValidationOptionsAttribute([]);
        Assert.NotNull(attribute2.RuleSets);
        Assert.Empty(attribute2.RuleSets);

        var attribute3 = new ValidationOptionsAttribute(["email"]);
        Assert.Equal(["email"], (string[]?)attribute3.RuleSets!);
    }
}