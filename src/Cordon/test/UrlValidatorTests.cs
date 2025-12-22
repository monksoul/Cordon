// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class UrlValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new UrlValidator();
        Assert.False(validator.SupportsFtp);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The {0} field is not a valid fully-qualified http, https URL.",
            validator._errorMessageResourceAccessor());

        var validator2 = new UrlValidator { SupportsFtp = true };
        Assert.True(validator2.SupportsFtp);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The {0} field is not a valid fully-qualified http, https, or ftp URL.",
            validator2._errorMessageResourceAccessor());
    }


    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("https://furion.net/", true)]
    [InlineData("http://furion.net/", true)]
    [InlineData("ftp://furion.net/", false)]
    [InlineData("furion.net", false)]
    [InlineData("https://api.furion.net/", true)]
    [InlineData("https://localhost", true)]
    [InlineData("https://localhost:3000", true)]
    [InlineData("https://127.0.0.1:8080", true)]
    [InlineData("https://furion.net:3000", true)]
    [InlineData("https://furion.net:65535", true)]
    [InlineData("https://furion.net:65536", false)]
    [InlineData("https://百签.com", true)]
    [InlineData("xn--bxyq0i.com", false)]
    [InlineData("https://xn--bxyq0i.com", true)]
    [InlineData("https://furion.net?id=10&name=furion", true)]
    [InlineData("monksoul@outlook.com", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new UrlValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData("ftp://furion.net/", true)]
    public void IsValid_WithSupportsFtp_ReturnOK(object? value, bool result)
    {
        var validator = new UrlValidator { SupportsFtp = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new UrlValidator();
        Assert.Null(validator.GetValidationResults("https://furion.net/", "data"));

        var validationResults = validator.GetValidationResults("furion.net", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field is not a valid fully-qualified http, https URL.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("furion.net", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithSupportsFtp_ReturnOK()
    {
        var validator = new UrlValidator { SupportsFtp = true };
        Assert.Null(validator.GetValidationResults("ftp://furion.net/", "data"));

        var validationResults = validator.GetValidationResults("furion.net", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field is not a valid fully-qualified http, https, or ftp URL.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("furion.net", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new UrlValidator();
        validator.Validate("https://furion.net/", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("furion.net", "data"));
        Assert.Equal("The data field is not a valid fully-qualified http, https URL.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("furion.net", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithSupportsFtp_ReturnOK()
    {
        var validator = new UrlValidator { SupportsFtp = true };
        validator.Validate("ftp://furion.net/", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("furion.net", "data"));
        Assert.Equal("The data field is not a valid fully-qualified http, https, or ftp URL.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("furion.net", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new UrlValidator();
        Assert.Equal("The data field is not a valid fully-qualified http, https URL.",
            validator.FormatErrorMessage("data"));

        var validator2 = new UrlValidator { SupportsFtp = true };
        Assert.Equal("The data field is not a valid fully-qualified http, https, or ftp URL.",
            validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new UrlValidator();
        Assert.Equal("UrlValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new UrlValidator { SupportsFtp = true };
        Assert.Equal("UrlValidator_ValidationError_SupportsFtp", validator2.GetResourceKey());
    }

    [Fact]
    public void ValidateUrl_Invalid_Parameters()
    {
        var validator = new UrlValidator();
        Assert.Throws<ArgumentNullException>(() => validator.ValidateUrl(null!));
        Assert.Throws<ArgumentException>(() => validator.ValidateUrl(string.Empty));
        Assert.Throws<ArgumentException>(() => validator.ValidateUrl(" "));
    }

    [Theory]
    [InlineData("https://furion.net/", true)]
    [InlineData("http://furion.net/", true)]
    [InlineData("ftp://furion.net/", false)]
    [InlineData("furion.net", false)]
    [InlineData("https://api.furion.net/", true)]
    [InlineData("https://localhost", true)]
    [InlineData("https://localhost:3000", true)]
    [InlineData("https://127.0.0.1:8080", true)]
    [InlineData("https://furion.net:3000", true)]
    [InlineData("https://furion.net:65535", true)]
    [InlineData("https://furion.net:65536", false)]
    [InlineData("https://百签.com", true)]
    [InlineData("xn--bxyq0i.com", false)]
    [InlineData("https://xn--bxyq0i.com", true)]
    [InlineData("https://furion.net?id=10&name=furion", true)]
    [InlineData("monksoul@outlook.com", false)]
    public void ValidateUrl_ReturnOK(string domain, bool result)
    {
        var validator = new UrlValidator();
        Assert.Equal(result, validator.ValidateUrl(domain));
    }

    [Theory]
    [InlineData("ftp://furion.net/", true)]
    public void ValidateUrl_WithSupportsFtp_ReturnOK(string domain, bool result)
    {
        var validator = new UrlValidator { SupportsFtp = true };
        Assert.Equal(result, validator.ValidateUrl(domain));
    }
}