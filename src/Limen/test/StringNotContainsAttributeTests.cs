// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class StringNotContainsAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(StringNotContainsAttribute);
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
        var attribute = new StringNotContainsAttribute("ur");
        Assert.Equal("ur", attribute.SearchValue);
        Assert.Equal(StringComparison.Ordinal, attribute.Comparison);
        Assert.Null(attribute.ErrorMessage);
        var validator = Helpers.GetValidator(attribute) as StringNotContainsValidator;
        Assert.NotNull(validator);
        Assert.Equal("ur", validator.SearchValue);
        Assert.Equal(StringComparison.Ordinal, validator.Comparison);

        var attribute2 = new StringNotContainsAttribute('i') { Comparison = StringComparison.InvariantCulture };
        Assert.Equal("i", attribute2.SearchValue);
        Assert.Equal(StringComparison.InvariantCulture, attribute2.Comparison);
        Assert.Null(attribute2.ErrorMessage);
        var validator2 = Helpers.GetValidator(attribute2) as StringNotContainsValidator;
        Assert.NotNull(validator2);
        Assert.Equal("i", validator2.SearchValue);
        Assert.Equal(StringComparison.InvariantCulture, validator2.Comparison);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "free", Data2 = "fly" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "fur", Data2 = "fly" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = "free", Data2 = "FUR" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "furion", Data2 = "FUR" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "free", Data2 = "fly" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "fur", Data2 = "fly" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data must not contain the string 'ur'.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "free", Data2 = "FUR" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 must not contain the string 'ur'.", validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "furion", Data2 = "FUR" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data must not contain the string 'ur'.", validationResults4[0].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "free", Data2 = "fly" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "fur", Data2 = "fly" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data must not contain the string 'ur'.", exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "free", Data2 = "FUR" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 must not contain the string 'ur'.", exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "furion", Data2 = "FUR" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data must not contain the string 'ur'.", exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new StringNotContainsAttribute("ur");
        Assert.Equal("The field data must not contain the string 'ur'.", attribute.FormatErrorMessage("data"));
    }

    public class TestModel
    {
        [StringNotContains("ur")] public string? Data { get; set; }

        [StringNotContains("ur", Comparison = StringComparison.OrdinalIgnoreCase)]
        public string? Data2 { get; set; }
    }
}