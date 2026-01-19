// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class UrlStrictAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(UrlStrictAttribute);
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
        var attribute = new UrlStrictAttribute();
        Assert.False(attribute.SupportsFtp);
        Assert.Null(attribute.ErrorMessage);
        Assert.NotNull(attribute._validator);
        Assert.False(attribute._validator.SupportsFtp);

        var attribute2 = new UrlStrictAttribute { SupportsFtp = true };
        Assert.True(attribute2.SupportsFtp);
        Assert.Null(attribute2.ErrorMessage);
        Assert.NotNull(attribute2._validator);
        Assert.True(attribute2._validator.SupportsFtp);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "https://furion.net/", Data2 = "ftp://furion.net/" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "furion.net", Data2 = "ftp://furion.net/" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = "https://furion.net/", Data2 = "https://furion.net:65536" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "furion.net", Data2 = "https://furion.net:65536" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "https://furion.net/", Data2 = "ftp://furion.net/" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "furion.net", Data2 = "ftp://furion.net/" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The Data field is not a valid fully-qualified http, https URL.",
            validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "https://furion.net/", Data2 = "https://furion.net:65536" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The Data2 field is not a valid fully-qualified http, https, or ftp URL.",
            validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "furion.net", Data2 = "https://furion.net:65536" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The Data field is not a valid fully-qualified http, https URL.",
            validationResults4[0].ErrorMessage);
        Assert.Equal("The Data2 field is not a valid fully-qualified http, https, or ftp URL.",
            validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "https://furion.net/", Data2 = "ftp://furion.net/" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "furion.net", Data2 = "ftp://furion.net/" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The Data field is not a valid fully-qualified http, https URL.",
            exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "https://furion.net/", Data2 = "https://furion.net:65536" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The Data2 field is not a valid fully-qualified http, https, or ftp URL.",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "furion.net", Data2 = "https://furion.net:65536" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The Data field is not a valid fully-qualified http, https URL.",
            exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new UrlStrictAttribute();
        Assert.Equal("The data field is not a valid fully-qualified http, https URL.",
            attribute.FormatErrorMessage("data"));

        var attribute2 = new UrlStrictAttribute { SupportsFtp = true };
        Assert.Equal("The data field is not a valid fully-qualified http, https, or ftp URL.",
            attribute2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new UrlStrictAttribute();
        Assert.Equal("UrlValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new UrlStrictAttribute { SupportsFtp = true };
        Assert.Equal("UrlValidator_ValidationError_SupportsFtp", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [UrlStrict] public string? Data { get; set; }

        [UrlStrict(SupportsFtp = true)] public string? Data2 { get; set; }
    }
}