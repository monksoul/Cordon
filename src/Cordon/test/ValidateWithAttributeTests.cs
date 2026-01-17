// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidateWithAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeUsageAttribute = typeof(ValidateWithAttribute<StringValueValidator>)
            .GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsageAttribute);
        Assert.Equal(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
            attributeUsageAttribute.ValidOn);
        Assert.False(attributeUsageAttribute.AllowMultiple);
        Assert.True(attributeUsageAttribute.Inherited);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var attribute = new ValidateWithAttribute<StringValueValidator>();
        Assert.Null(attribute.RuleSets);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var attribute = new ValidateWithAttribute<StringValueValidator>();
        var validationResult = attribute.GetValidationResult(null, new ValidationContext(new object(), null, null));
        Assert.NotNull(validationResult);
        Assert.Equal("The Object field is required.", validationResult.ErrorMessage);

        var validationResult2 = attribute.GetValidationResult("fu", new ValidationContext("fu", null, null));
        Assert.NotNull(validationResult2);
        Assert.Equal("The field String must be a string or array type with a minimum length of '3'.",
            validationResult2.ErrorMessage);

        attribute.RuleSets = ["login"];
        var validationContext = new ValidationContext("fu", null, null);
        var validationResult3 = attribute.GetValidationResult("fu", validationContext);
        Assert.Null(validationResult3);
        Assert.Single(validationContext.Items);
        var metadata = validationContext.Items[Constants.ValidationOptionsKey] as ValidationOptionsMetadata;
        Assert.NotNull(metadata);
        Assert.Equal(["login"], (string[]?)metadata.RuleSets!);
    }

    public class StringValueValidator : AbstractValueValidator<string>
    {
        public StringValueValidator() => Rule().Required().MinLength(3);
    }
}