// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class HaveLengthAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(HaveLengthAttribute);
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
        var attribute = new HaveLengthAttribute(2);
        Assert.Equal(2, attribute.Length);
        Assert.False(attribute.AllowEmpty);
        Assert.Null(attribute.ErrorMessage);
        var validator = Helpers.GetValidator(attribute) as HaveLengthValidator;
        Assert.NotNull(validator);
        Assert.Equal(2, validator.Length);
        Assert.False(validator.AllowEmpty);

        var attribute2 = new HaveLengthAttribute(2) { AllowEmpty = true };
        Assert.Equal(2, attribute2.Length);
        Assert.True(attribute2.AllowEmpty);
        Assert.Null(attribute2.ErrorMessage);
        var validator2 = Helpers.GetValidator(attribute2) as HaveLengthValidator;
        Assert.NotNull(validator2);
        Assert.Equal(2, validator2.Length);
        Assert.True(validator2.AllowEmpty);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "fu", Data2 = new[] { "fur", "furion" } };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "fur", Data2 = new[] { "fur", "furion" } };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = "fu", Data2 = new[] { "fur", "furion", "百小僧" } };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "fur", Data2 = new[] { "fur", "furion", "百小僧" } };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "fu", Data2 = new[] { "fur", "furion" } };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "fur", Data2 = new[] { "fur", "furion" } };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data must be a string or collection type with a length of exactly '2'.",
            validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "fu", Data2 = new[] { "fur", "furion", "百小僧" } };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 must be a string or collection type with a length of exactly '2'.",
            validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "fur", Data2 = new[] { "fur", "furion", "百小僧" } };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data must be a string or collection type with a length of exactly '2'.",
            validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 must be a string or collection type with a length of exactly '2'.",
            validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "fu", Data2 = new[] { "fur", "furion" } };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "fur", Data2 = new[] { "fur", "furion" } };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data must be a string or collection type with a length of exactly '2'.",
            exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "fu", Data2 = new[] { "fur", "furion", "百小僧" } };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 must be a string or collection type with a length of exactly '2'.",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "fur", Data2 = new[] { "fur", "furion", "百小僧" } };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data must be a string or collection type with a length of exactly '2'.",
            exception3.ValidationResult.ErrorMessage);
    }


    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new HaveLengthAttribute(2);
        Assert.Equal("The field data must be a string or collection type with a length of exactly '2'.",
            attribute.FormatErrorMessage("data"));

        var attribute2 = new HaveLengthAttribute(2) { AllowEmpty = true };
        Assert.Equal("The field data must be empty or have a length of exactly '2'.",
            attribute2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new HaveLengthAttribute(2);
        Assert.Equal("HaveLengthValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new HaveLengthAttribute(2) { AllowEmpty = true };
        Assert.Equal("HaveLengthValidator_ValidationError_AllowEmpty", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [HaveLength(2)] public string? Data { get; set; }

        [HaveLength(2)] public string[]? Data2 { get; set; }
    }
}