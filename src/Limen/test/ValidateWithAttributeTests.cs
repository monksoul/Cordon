// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ValidateWithAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeUsageAttribute = typeof(ValidateWithAttribute<StringValueValidator>)
            .GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsageAttribute);
        Assert.Equal(AttributeTargets.Parameter, attributeUsageAttribute.ValidOn);
        Assert.False(attributeUsageAttribute.AllowMultiple);
        Assert.True(attributeUsageAttribute.Inherited);
    }

    public class StringValueValidator : AbstractValueValidator<string>;
}