// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class DecimalPlacesAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(DecimalPlacesAttribute);
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
        var attribute = new DecimalPlacesAttribute(1);
        Assert.Equal(1, attribute.MaxDecimalPlaces);
        Assert.False(attribute.AllowStringValues);
        Assert.Null(attribute.ErrorMessage);
        Assert.NotNull(attribute._validator);
        Assert.Equal(1, attribute._validator.MaxDecimalPlaces);
        Assert.False(attribute._validator.AllowStringValues);

        var attribute2 = new DecimalPlacesAttribute(1) { AllowStringValues = true };
        Assert.Equal(1, attribute2.MaxDecimalPlaces);
        Assert.True(attribute2.AllowStringValues);
        Assert.Null(attribute2.ErrorMessage);
        Assert.NotNull(attribute2._validator);
        Assert.Equal(1, attribute2._validator.MaxDecimalPlaces);
        Assert.True(attribute2._validator.AllowStringValues);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = 10.1, Data2 = "10.1" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = 10.12, Data2 = "10.1" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = 10.1, Data2 = "10.12" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = 10.12, Data2 = "10.12" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = 10.1, Data2 = "10.1" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = 10.12, Data2 = "10.1" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data must not have more than '1' decimal places.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = 10.1, Data2 = "10.12" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 must not have more than '1' decimal places.", validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = 10.12, Data2 = "10.12" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data must not have more than '1' decimal places.", validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 must not have more than '1' decimal places.", validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = 10.1, Data2 = "10.1" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = 10.12, Data2 = "10.1" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data must not have more than '1' decimal places.",
            exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = 10.1, Data2 = "10.12" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 must not have more than '1' decimal places.",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = 10.12, Data2 = "10.12" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data must not have more than '1' decimal places.",
            exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new DecimalPlacesAttribute(1);
        Assert.Equal("The field data must not have more than '1' decimal places.",
            attribute.FormatErrorMessage("data"));
    }

    public class TestModel
    {
        [DecimalPlaces(1)] public double Data { get; set; }

        [DecimalPlaces(1, AllowStringValues = true)]
        public string? Data2 { get; set; }
    }
}