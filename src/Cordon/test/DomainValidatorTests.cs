// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class DomainValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new DomainValidator();
        Assert.NotNull(validator._idnMapping);
        Assert.True(validator._idnMapping.AllowUnassigned);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid domain name.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("furion.net", true)]
    [InlineData("www.furion.net", true)]
    [InlineData("docs.furion.net", true)]
    [InlineData("furion.cn", true)]
    [InlineData("furion.com", true)]
    [InlineData("furion.org", true)]
    [InlineData("furion.pro", true)]
    [InlineData("https://furion.net", false)]
    [InlineData("https://www.furion.net", false)]
    [InlineData("http://www.furion.net", false)]
    [InlineData("http://docs.furion.net", false)]
    [InlineData("furion.net/docs", false)]
    [InlineData("百签.com", true)]
    [InlineData("百签.公司", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new DomainValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new DomainValidator();
        Assert.Null(validator.GetValidationResults("furion.net", "data"));

        var validationResults = validator.GetValidationResults("https://furion.net", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid domain name.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("https://furion.net", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new DomainValidator();
        validator.Validate("furion.net", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("https://furion.net", "data"));
        Assert.Equal("The field data is not a valid domain name.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("https://furion.net", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void ValidateDomain_Invalid_Parameters()
    {
        var validator = new DomainValidator();
        Assert.Throws<ArgumentNullException>(() => validator.ValidateDomain(null!));
        Assert.Throws<ArgumentException>(() => validator.ValidateDomain(string.Empty));
        Assert.Throws<ArgumentException>(() => validator.ValidateDomain(" "));
    }

    [Theory]
    [InlineData("furion.net", true)]
    [InlineData("www.furion.net", true)]
    [InlineData("docs.furion.net", true)]
    [InlineData("furion.cn", true)]
    [InlineData("furion.com", true)]
    [InlineData("furion.org", true)]
    [InlineData("furion.pro", true)]
    [InlineData("https://furion.net", false)]
    [InlineData("https://www.furion.net", false)]
    [InlineData("http://www.furion.net", false)]
    [InlineData("http://docs.furion.net", false)]
    [InlineData("furion.net/docs", false)]
    [InlineData("百签.com", true)]
    [InlineData("百签.公司", false)]
    public void ValidateDomain_ReturnOK(string domain, bool result)
    {
        var validator = new DomainValidator();
        Assert.Equal(result, validator.ValidateDomain(domain));
    }
}