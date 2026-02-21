// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class DateOnlyAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(DateOnlyAttribute);
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
        var attribute = new DateOnlyAttribute();
        Assert.Empty(attribute.Formats);
        Assert.Null(attribute.Culture);
        Assert.Equal(CultureInfo.InvariantCulture, attribute.Provider);
        Assert.Equal(DateTimeStyles.None, attribute.Style);
        Assert.Null(attribute.ErrorMessage);
        Assert.NotNull(attribute._validator);
        Assert.Empty(attribute._validator.Formats);
        Assert.Equal(CultureInfo.InvariantCulture, attribute._validator.Provider);
        Assert.Equal(DateTimeStyles.None, attribute._validator.Style);
        Assert.Empty(attribute.FormatsFormatted);

        var attribute2 = new DateOnlyAttribute("yyyy-MM-dd", "yyyy/MM/dd")
        {
            Culture = "en-GB", Style = DateTimeStyles.AllowTrailingWhite
        };
        var enGbCulture = CultureInfo.GetCultureInfo("en-GB");
        Assert.Equal(["yyyy-MM-dd", "yyyy/MM/dd"], attribute2.Formats);
        Assert.Equal("en-GB", attribute2.Culture);
        Assert.Equal(enGbCulture, attribute2.Provider);
        Assert.Equal(DateTimeStyles.AllowTrailingWhite, attribute2.Style);
        Assert.Null(attribute2.ErrorMessage);
        Assert.NotNull(attribute2._validator);
        Assert.Equal(["yyyy-MM-dd", "yyyy/MM/dd"], attribute2._validator.Formats);
        Assert.Equal(enGbCulture, attribute2._validator.Provider);
        Assert.Equal(DateTimeStyles.AllowTrailingWhite, attribute2._validator.Style);
        Assert.Equal("'yyyy-MM-dd', 'yyyy/MM/dd'", attribute2.FormatsFormatted);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "2023-09-26", Data2 = "2023/09/26" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "26/09/2023", Data2 = "2023/09/26" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = "2023-09-26", Data2 = "2033-01/10" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "26/09/2023", Data2 = "2033-01/10" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "2023-09-26", Data2 = "2023/09/26" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "26/09/2023", Data2 = "2023/09/26" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data must be a valid date.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "2023-09-26", Data2 = "2033-01/10" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 must be a valid date in the following format(s): 'yyyy-MM-dd', 'yyyy/MM/dd'.",
            validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "26/09/2023", Data2 = "2033-01/10" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data must be a valid date.", validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 must be a valid date in the following format(s): 'yyyy-MM-dd', 'yyyy/MM/dd'.",
            validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "2023-09-26", Data2 = "2023/09/26" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "26/09/2023", Data2 = "2023/09/26" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data must be a valid date.", exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "2023-09-26", Data2 = "2033-01/10" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 must be a valid date in the following format(s): 'yyyy-MM-dd', 'yyyy/MM/dd'.",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "26/09/2023", Data2 = "2033-01/10" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data must be a valid date.", exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new DateOnlyAttribute();
        Assert.Equal("The field data must be a valid date.",
            attribute.FormatErrorMessage("data"));

        var attribute2 = new DateOnlyAttribute("yyyy-MM-dd");
        Assert.Equal("The field data must be a valid date in the following format(s): 'yyyy-MM-dd'.",
            attribute2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new DateOnlyAttribute();
        Assert.Equal("DateOnlyValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new DateOnlyAttribute("yyyy-MM-dd");
        Assert.Equal("DateOnlyValidator_ValidationError_Formats", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [DateOnly] public string? Data { get; set; }

        [DateOnly("yyyy-MM-dd", "yyyy/MM/dd")] public string? Data2 { get; set; }
    }
}