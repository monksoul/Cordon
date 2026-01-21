// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class RegularExpressionValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new RegularExpressionValidator("^[1-9]{2,5}$");
        Assert.Equal("^[1-9]{2,5}$", validator.Pattern);
        Assert.Equal(2000, validator.MatchTimeoutInMilliseconds);
        Assert.Equal(TimeSpan.FromMilliseconds(2000), validator.MatchTimeout);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must match the regular expression '{1}'.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(1, false)]
    [InlineData(1234, true)]
    [InlineData("1234", true)]
    [InlineData("12340", false)]
    [InlineData("12345", true)]
    [InlineData("123456", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new RegularExpressionValidator("^[1-9]{2,5}$");
        Assert.Equal(result, validator.IsValid(value));
        Assert.NotNull(validator.Regex);
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new RegularExpressionValidator("^[1-9]{2,5}$");
        Assert.Null(validator.GetValidationResults("12345", "data"));

        var validationResults = validator.GetValidationResults("123456", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must match the regular expression '^[1-9]{2,5}$'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("123456", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new RegularExpressionValidator("^[1-9]{2,5}$");
        validator.Validate("12345", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("123456", "data"));
        Assert.Equal("The field data must match the regular expression '^[1-9]{2,5}$'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("123456", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new RegularExpressionValidator("^[1-9]{2,5}$");
        Assert.Equal("The field data must match the regular expression '^[1-9]{2,5}$'.",
            validator.FormatErrorMessage("data"));
    }

    [Fact]
    public void SetupRegex_Invalid_Parameters()
    {
        var validator = new RegularExpressionValidator(null!);
        var exception = Assert.Throws<InvalidOperationException>(() => validator.SetupRegex());
        Assert.Equal("The pattern must be set to a valid regular expression.", exception.Message);

        var validator2 = new RegularExpressionValidator(string.Empty);
        var exception2 = Assert.Throws<InvalidOperationException>(() => validator2.SetupRegex());
        Assert.Equal("The pattern must be set to a valid regular expression.", exception2.Message);

        var validator3 = new RegularExpressionValidator("^[1-9]{2,5}$");
        validator3.SetupRegex();
        Assert.NotNull(validator3.Regex);
    }
}