// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class IpAddressValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new IpAddressValidator();
        Assert.False(validator.AllowIPv6);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid IPv4 address.", validator._errorMessageResourceAccessor());

        var validator2 = new IpAddressValidator { AllowIPv6 = true };
        Assert.True(validator2.AllowIPv6);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid IP address (IPv4 or IPv6).",
            validator2._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData(1, false)]
    [InlineData(true, false)]
    [InlineData("0.0.0.0", true)]
    [InlineData("::1", false)]
    [InlineData("192.168.1.1", true)]
    [InlineData("255.255.255.0", true)]
    [InlineData("192.168.1", false)] // 192.168.0.1
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334", false)]
    [InlineData("2001:0db8::85a3::7334", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new IpAddressValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData(1, false)]
    [InlineData(true, false)]
    [InlineData("0.0.0.0", true)]
    [InlineData("::1", true)]
    [InlineData("192.168.1.1", true)]
    [InlineData("255.255.255.0", true)]
    [InlineData("192.168.1", false)] // 192.168.0.1
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334", true)]
    [InlineData("2001:0db8::85a3::7334", false)]
    public void IsValid_WithAllowIPv6_ReturnOK(object? value, bool result)
    {
        var validator = new IpAddressValidator { AllowIPv6 = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new IpAddressValidator();
        Assert.Null(validator.GetValidationResults("192.168.1.1", "data"));

        var validationResults = validator.GetValidationResults("192.168.1", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid IPv4 address.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("192.168.1", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithAllowIPv6_ReturnOK()
    {
        var validator = new IpAddressValidator { AllowIPv6 = true };
        Assert.Null(validator.GetValidationResults("192.168.1.1", "data"));

        var validationResults = validator.GetValidationResults(16, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid IP address (IPv4 or IPv6).",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(16, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new IpAddressValidator();
        validator.Validate("192.168.1.1", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("192.168.1", "data"));
        Assert.Equal("The field data is not a valid IPv4 address.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("192.168.1", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithAllowIPv6_ReturnOK()
    {
        var validator = new IpAddressValidator { AllowIPv6 = true };
        validator.Validate("192.168.1.1", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(16, "data"));
        Assert.Equal("The field data is not a valid IP address (IPv4 or IPv6).", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(16, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new IpAddressValidator();
        Assert.Equal("The field data is not a valid IPv4 address.", validator.FormatErrorMessage("data"));

        var validator2 = new IpAddressValidator { AllowIPv6 = true };
        Assert.Equal("The field data is not a valid IP address (IPv4 or IPv6).", validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new IpAddressValidator();
        Assert.Equal("IpAddressValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new IpAddressValidator { AllowIPv6 = true };
        Assert.Equal("IpAddressValidator_ValidationError_AllowIPv6", validator2.GetResourceKey());
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("0.0.0.0", true)]
    [InlineData("::1", false)]
    [InlineData("192.168.1.1", true)]
    [InlineData("255.255.255.0", true)]
    [InlineData("192.168.1", false)] // 192.168.0.1
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334", false)]
    [InlineData("2001:0db8::85a3::7334", false)]
    public void CheckIpAddress_ReturnOK(string? value, bool result) =>
        Assert.Equal(result, IpAddressValidator.CheckIpAddress(value!, false));

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("0.0.0.0", true)]
    [InlineData("::1", true)]
    [InlineData("192.168.1.1", true)]
    [InlineData("255.255.255.0", true)]
    [InlineData("192.168.1", false)] // 192.168.0.1
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334", true)]
    [InlineData("2001:0db8::85a3::7334", false)]
    public void CheckIpAddress_WithAllowIPv6_ReturnOK(string? value, bool result) =>
        Assert.Equal(result, IpAddressValidator.CheckIpAddress(value!, true));
}