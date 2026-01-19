// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class StartsWithAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(StartsWithAttribute);
        Assert.True(typeof(ValidationAttribute).IsAssignableFrom(attributeType));

        var attributeUsageAttribute = attributeType.GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsageAttribute);
        Assert.Equal(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
            attributeUsageAttribute.ValidOn);
        Assert.False(attributeUsageAttribute.AllowMultiple);
        Assert.True(attributeUsageAttribute.Inherited);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var attribute = new StartsWithAttribute("fu");
        Assert.Equal("fu", attribute.SearchValue);
        Assert.Equal(StringComparison.Ordinal, attribute.Comparison);
        Assert.Null(attribute.ErrorMessage);
        Assert.NotNull(attribute._validator);
        Assert.Equal("fu", attribute._validator.SearchValue);
        Assert.Equal(StringComparison.Ordinal, attribute._validator.Comparison);

        var attribute2 = new StartsWithAttribute('i') { Comparison = StringComparison.InvariantCulture };
        Assert.Equal("i", attribute2.SearchValue);
        Assert.Equal(StringComparison.InvariantCulture, attribute2.Comparison);
        Assert.Null(attribute2.ErrorMessage);
        Assert.NotNull(attribute2._validator);
        Assert.Equal("i", attribute2._validator.SearchValue);
        Assert.Equal(StringComparison.InvariantCulture, attribute2._validator.Comparison);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "furion", Data2 = "fur" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "Furion", Data2 = "fur" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = "furion", Data2 = "free" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "Furion", Data2 = "free" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "furion", Data2 = "fur" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "Furion", Data2 = "fur" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data does not start with the string 'fu'.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "furion", Data2 = "free" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 does not start with the string 'fu'.", validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "Furion", Data2 = "free" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data does not start with the string 'fu'.", validationResults4[0].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "furion", Data2 = "fur" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "Furion", Data2 = "fur" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data does not start with the string 'fu'.", exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "furion", Data2 = "free" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 does not start with the string 'fu'.", exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "Furion", Data2 = "fur" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data does not start with the string 'fu'.", exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new StartsWithAttribute("fu");
        Assert.Equal("The field data does not start with the string 'fu'.", attribute.FormatErrorMessage("data"));
    }

    public class TestModel
    {
        [StartsWith("fu")] public string? Data { get; set; }

        [StartsWith("fu", Comparison = StringComparison.OrdinalIgnoreCase)]
        public string? Data2 { get; set; }
    }
}