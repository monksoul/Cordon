// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class JsonAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(JsonAttribute);
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
        var attribute = new JsonAttribute();
        Assert.False(attribute.AllowTrailingCommas);
        Assert.Null(attribute.ErrorMessage);
        var validator = Helpers.GetValidator(attribute) as JsonValidator;
        Assert.NotNull(validator);
        Assert.False(validator.AllowTrailingCommas);

        var attribute2 = new JsonAttribute { AllowTrailingCommas = true };
        Assert.True(attribute2.AllowTrailingCommas);
        Assert.Null(attribute2.ErrorMessage);
        var validator2 = Helpers.GetValidator(attribute2) as JsonValidator;
        Assert.NotNull(validator2);
        Assert.True(validator2.AllowTrailingCommas);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel
        {
            Data = "{\"id\":1,\"name\":\"furion\"}", Data2 = "{\"id\":1,\"name\":\"furion\",}"
        };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "\"furion\"", Data2 = "{\"id\":1,\"name\":\"furion\",}" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = "{\"id\":1,\"name\":\"furion\"}", Data2 = "\"furion\"" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "\"furion\"", Data2 = "\"furion\"" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel
        {
            Data = "{\"id\":1,\"name\":\"furion\"}", Data2 = "{\"id\":1,\"name\":\"furion\",}"
        };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "\"furion\"", Data2 = "{\"id\":1,\"name\":\"furion\",}" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The Data field must be a valid JSON object or array.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "{\"id\":1,\"name\":\"furion\"}", Data2 = "\"furion\"" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The Data2 field must be a valid JSON object or array.", validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "\"furion\"", Data2 = "\"furion\"" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The Data field must be a valid JSON object or array.", validationResults4[0].ErrorMessage);
        Assert.Equal("The Data2 field must be a valid JSON object or array.", validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel
        {
            Data = "{\"id\":1,\"name\":\"furion\"}", Data2 = "{\"id\":1,\"name\":\"furion\",}"
        };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "\"furion\"", Data2 = "{\"id\":1,\"name\":\"furion\",}" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The Data field must be a valid JSON object or array.", exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "{\"id\":1,\"name\":\"furion\"}", Data2 = "\"furion\"" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The Data2 field must be a valid JSON object or array.", exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "\"furion\"", Data2 = "\"furion\"" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The Data field must be a valid JSON object or array.", exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new JsonAttribute();
        Assert.Equal("The data field must be a valid JSON object or array.", attribute.FormatErrorMessage("data"));
    }

    public class TestModel
    {
        [Json] public string? Data { get; set; }

        [Json(AllowTrailingCommas = true)] public string? Data2 { get; set; }
    }
}