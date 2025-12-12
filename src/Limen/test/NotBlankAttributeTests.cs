// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class NotBlankAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(NotBlankAttribute);
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
        var attribute = new NotBlankAttribute();
        Assert.Null(attribute.ErrorMessage);
        var validator = Helpers.GetValidator(attribute) as NotBlankValidator;
        Assert.NotNull(validator);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "Furion" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "  " };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "Furion" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "  " };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data cannot be empty or whitespace.", validationResults2[0].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "Furion" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "  " };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data cannot be empty or whitespace.", exception.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new NotBlankAttribute();
        Assert.Equal("The field data cannot be empty or whitespace.", attribute.FormatErrorMessage("data"));
    }

    public class TestModel
    {
        [NotBlank] public string? Data { get; set; }
    }
}