// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class UnifiedSocialCreditCodeAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(UnifiedSocialCreditCodeAttribute);
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
        var attribute = new UnifiedSocialCreditCodeAttribute();
        Assert.False(attribute.AllowLooseMatch);
        Assert.Null(attribute.ErrorMessage);
        Assert.NotNull(attribute._validator);
        Assert.False(attribute._validator.AllowLooseMatch);

        var attribute2 = new UnifiedSocialCreditCodeAttribute { AllowLooseMatch = true };
        Assert.True(attribute2.AllowLooseMatch);
        Assert.Null(attribute2.ErrorMessage);
        Assert.NotNull(attribute2._validator);
        Assert.True(attribute2._validator.AllowLooseMatch);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "91350100M000100Y43", Data2 = "ABC123456789012" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "ABC123456789012", Data2 = "ABC123456789012" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = "91350100M000100Y43", Data2 = "ABC12345678901" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "invalid", Data2 = "invalid" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "91350100M000100Y43", Data2 = "ABC123456789012" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "ABC123456789012", Data2 = "ABC123456789012" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data is not a valid unified social credit code.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "91350100M000100Y43", Data2 = "ABC12345678901" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 is not a valid unified social credit code (loose match).",
            validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "invalid", Data2 = "invalid" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data is not a valid unified social credit code.", validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 is not a valid unified social credit code (loose match).",
            validationResults4[1].ErrorMessage);

        var model5 = new TestModel { Data = "invalid", Data2 = "invalid", Data3 = "invalid" };
        var validationResults5 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model5, new ValidationContext(model5), validationResults5, true));
        Assert.Equal(3, validationResults5.Count);
        Assert.Equal("The field Data is not a valid unified social credit code.", validationResults5[0].ErrorMessage);
        Assert.Equal("The field Data2 is not a valid unified social credit code (loose match).",
            validationResults5[1].ErrorMessage);
        Assert.Equal("数据无效", validationResults5[2].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "91350100M000100Y43", Data2 = "ABC123456789012" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "ABC123456789012", Data2 = "ABC123456789012" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data is not a valid unified social credit code.",
            exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "91350100M000100Y43", Data2 = "ABC12345678901" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 is not a valid unified social credit code (loose match).",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "invalid", Data2 = "invalid" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data is not a valid unified social credit code.",
            exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new UnifiedSocialCreditCodeAttribute();
        Assert.Equal("The field data is not a valid unified social credit code.", attribute.FormatErrorMessage("data"));

        var attribute2 = new UnifiedSocialCreditCodeAttribute { AllowLooseMatch = true };
        Assert.Equal("The field data is not a valid unified social credit code (loose match).",
            attribute2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new UnifiedSocialCreditCodeAttribute();
        Assert.Equal("UnifiedSocialCreditCodeValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new UnifiedSocialCreditCodeAttribute { AllowLooseMatch = true };
        Assert.Equal("UnifiedSocialCreditCodeValidator_ValidationError_AllowLooseMatch", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [UnifiedSocialCreditCode] public string? Data { get; set; }

        [UnifiedSocialCreditCode(AllowLooseMatch = true)]
        public string? Data2 { get; set; }

        [UnifiedSocialCreditCode(ErrorMessage = "数据无效")]
        public string? Data3 { get; set; }
    }
}