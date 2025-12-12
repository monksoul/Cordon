// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class IpAddressAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(IpAddressAttribute);
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
        var attribute = new IpAddressAttribute();
        Assert.False(attribute.AllowIPv6);
        Assert.Null(attribute.ErrorMessage);
        var validator = Helpers.GetValidator(attribute) as IpAddressValidator;
        Assert.NotNull(validator);
        Assert.False(validator.AllowIPv6);

        var attribute2 = new IpAddressAttribute { AllowIPv6 = true };
        Assert.True(attribute2.AllowIPv6);
        Assert.Null(attribute2.ErrorMessage);
        var validator2 = Helpers.GetValidator(attribute2) as IpAddressValidator;
        Assert.NotNull(validator2);
        Assert.True(validator2.AllowIPv6);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "192.168.1.1", Data2 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "192.168.1", Data2 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel
        {
            Data = "2001:0db8:85a3:0000:0000:8a2e:0370:7334", Data2 = "2001:0db8::85a3::7334"
        };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "192.168.1", Data2 = "2001:0db8::85a3::7334" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "192.168.1.1", Data2 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "192.168.1", Data2 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data is not a valid IPv4 address.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "192.168.1.1", Data2 = "2001:0db8::85a3::7334" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 is not a valid IP address (IPv4 or IPv6).", validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "192.168.1", Data2 = "2001:0db8::85a3::7334" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data is not a valid IPv4 address.", validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 is not a valid IP address (IPv4 or IPv6).", validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "192.168.1.1", Data2 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "192.168.1", Data2 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data is not a valid IPv4 address.", exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "192.168.1.1", Data2 = "2001:0db8::85a3::7334" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 is not a valid IP address (IPv4 or IPv6).",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "192.168.1", Data2 = "2001:0db8::85a3::7334" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data is not a valid IPv4 address.", exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new IpAddressAttribute();
        Assert.Equal("The field data is not a valid IPv4 address.", attribute.FormatErrorMessage("data"));

        var attribute2 = new IpAddressAttribute { AllowIPv6 = true };
        Assert.Equal("The field data is not a valid IP address (IPv4 or IPv6).", attribute2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new IpAddressAttribute();
        Assert.Equal("IpAddressValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new IpAddressAttribute { AllowIPv6 = true };
        Assert.Equal("IpAddressValidator_ValidationError_AllowIPv6", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [IpAddress] public string? Data { get; set; }

        [IpAddress(AllowIPv6 = true)] public string? Data2 { get; set; }
    }
}