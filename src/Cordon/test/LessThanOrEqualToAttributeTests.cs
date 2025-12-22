// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class LessThanOrEqualToAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(LessThanOrEqualToAttribute);
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
        var attribute = new LessThanOrEqualToAttribute(10);
        Assert.Equal(10, attribute.CompareValue);
        Assert.Null(attribute.ErrorMessage);
        var validator = Helpers.GetValidator(attribute) as LessThanOrEqualToValidator;
        Assert.NotNull(validator);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = 9, Data2 = 10.0 };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = 11, Data2 = 10.0 };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = 9, Data2 = 10.2 };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = 11, Data2 = 10.2 };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = 9, Data2 = 10.0 };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = 11, Data2 = 10.0 };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data must be less than or equal to '10'.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = 9, Data2 = 10.2 };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 must be less than or equal to '10.1'.", validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = 11, Data2 = 10.2 };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data must be less than or equal to '10'.", validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 must be less than or equal to '10.1'.", validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = 9, Data2 = 10.0 };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = 11, Data2 = 10.0 };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data must be less than or equal to '10'.", exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = 9, Data2 = 10.2 };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 must be less than or equal to '10.1'.", exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = 11, Data2 = 10.2 };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data must be less than or equal to '10'.", exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new LessThanOrEqualToAttribute(10);
        Assert.Equal("The field data must be less than or equal to '10'.", attribute.FormatErrorMessage("data"));
    }

    public class TestModel
    {
        [LessThanOrEqualTo(10)] public int Data { get; set; }

        [LessThanOrEqualTo(10.1)] public double Data2 { get; set; }
    }
}