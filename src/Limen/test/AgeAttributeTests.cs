// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class AgeAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(AgeAttribute);
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
        var attribute = new AgeAttribute();
        Assert.False(attribute.IsAdultOnly);
        Assert.False(attribute.AllowStringValues);
        Assert.Null(attribute.ErrorMessage);
        var validator = Helpers.GetValidator(attribute) as AgeValidator;
        Assert.NotNull(validator);
        Assert.False(validator.IsAdultOnly);
        Assert.False(validator.AllowStringValues);

        var attribute2 = new AgeAttribute { IsAdultOnly = true, AllowStringValues = true };
        Assert.True(attribute2.IsAdultOnly);
        Assert.Null(attribute2.ErrorMessage);
        var validator2 = Helpers.GetValidator(attribute2) as AgeValidator;
        Assert.NotNull(validator2);
        Assert.True(validator2.IsAdultOnly);
        Assert.True(validator2.AllowStringValues);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = 10, Data2 = 18 };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = -1, Data2 = 18 };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = 18, Data2 = 16 };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = -1, Data2 = 16 };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = 10, Data2 = 18 };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = -1, Data2 = 18 };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data is not a valid age.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = 18, Data2 = 16 };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 must be at least 18 years old.", validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = -1, Data2 = 16 };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data is not a valid age.", validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 must be at least 18 years old.", validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = 10, Data2 = 18 };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = -1, Data2 = 18 };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data is not a valid age.", exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = 18, Data2 = 16 };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 must be at least 18 years old.", exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = -1, Data2 = 16 };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data is not a valid age.", exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new AgeAttribute();
        Assert.Equal("The field data is not a valid age.", attribute.FormatErrorMessage("data"));

        var attribute2 = new AgeAttribute { IsAdultOnly = true };
        Assert.Equal("The field data must be at least 18 years old.", attribute2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new AgeAttribute();
        Assert.Equal("AgeValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new AgeAttribute { IsAdultOnly = true };
        Assert.Equal("AgeValidator_ValidationError_IsAdultOnly", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [Age] public int Data { get; set; }

        [Age(IsAdultOnly = true)] public int Data2 { get; set; }
    }
}