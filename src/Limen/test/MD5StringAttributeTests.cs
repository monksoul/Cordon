// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class MD5StringAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(MD5StringAttribute);
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
        var attribute = new MD5StringAttribute();
        Assert.False(attribute.AllowShortFormat);
        Assert.Null(attribute.ErrorMessage);
        var validator = Helpers.GetValidator(attribute) as MD5StringValidator;
        Assert.NotNull(validator);
        Assert.False(validator.AllowShortFormat);

        var attribute2 = new MD5StringAttribute { AllowShortFormat = true };
        Assert.True(attribute2.AllowShortFormat);
        Assert.Null(attribute2.ErrorMessage);
        var validator2 = Helpers.GetValidator(attribute) as MD5StringValidator;
        Assert.NotNull(validator2);
        Assert.False(validator2.AllowShortFormat);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "3f2d0ea0ef4df562719e70e41413658e", Data2 = "EF4DF562719E70E4" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "ef4df562719e70e4", Data2 = "EF4DF562719E70E4" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel
        {
            Data = "3f2d0ea0ef4df562719e70e41413658e", Data2 = "3F2D0EA0EF4DF562719E70E41413658"
        };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "ef4df562719e70e4", Data2 = "3F2D0EA0EF4DF562719E70E41413658" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "3f2d0ea0ef4df562719e70e41413658e", Data2 = "EF4DF562719E70E4" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "ef4df562719e70e4", Data2 = "EF4DF562719E70E4" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data is not a valid MD5 string.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel
        {
            Data = "3f2d0ea0ef4df562719e70e41413658e", Data2 = "3F2D0EA0EF4DF562719E70E41413658"
        };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 is not a valid MD5 string.", validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "ef4df562719e70e4", Data2 = "3F2D0EA0EF4DF562719E70E41413658" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data is not a valid MD5 string.", validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 is not a valid MD5 string.", validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "3f2d0ea0ef4df562719e70e41413658e", Data2 = "EF4DF562719E70E4" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "ef4df562719e70e4", Data2 = "EF4DF562719E70E4" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data is not a valid MD5 string.", exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel
        {
            Data = "3f2d0ea0ef4df562719e70e41413658e", Data2 = "3F2D0EA0EF4DF562719E70E41413658"
        };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 is not a valid MD5 string.", exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "ef4df562719e70e4", Data2 = "3F2D0EA0EF4DF562719E70E41413658" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data is not a valid MD5 string.", exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new MD5StringAttribute();
        Assert.Equal("The field data is not a valid MD5 string.", attribute.FormatErrorMessage("data"));
    }

    public class TestModel
    {
        [MD5String] public string? Data { get; set; }

        [MD5String(AllowShortFormat = true)] public string? Data2 { get; set; }
    }
}