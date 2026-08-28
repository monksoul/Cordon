// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class UnifiedSocialCreditCodeValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new UnifiedSocialCreditCodeValidator();
        Assert.False(validator.AllowLooseMatch);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid unified social credit code.",
            validator._errorMessageResourceAccessor());

        var validator2 = new UnifiedSocialCreditCodeValidator { AllowLooseMatch = true };
        Assert.True(validator2.AllowLooseMatch);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid unified social credit code (loose match).",
            validator2._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("91350100M000100Y43", true)]
    [InlineData("123456789012345678", true)]
    [InlineData("ABC123456789012", false)]
    [InlineData("ABC1234567890123456789", false)]
    [InlineData("91350100S000100Y43", false)]
    [InlineData("91350100V000100Y43", false)]
    [InlineData("91350100I000100Y43", false)]
    [InlineData("91350100O000100Y43", false)]
    [InlineData("91350100Z000100Y43", false)]
    [InlineData("91350100M000100Y4", false)]
    [InlineData("91350100M000100Y433", false)]
    [InlineData(123456789012345678, false)]
    [InlineData("", false)]
    [InlineData("91350100M000100y43", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new UnifiedSocialCreditCodeValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("ABC123456789012", true)]
    [InlineData("ABC123456789012345", true)]
    [InlineData("ABC12345678901234567", true)]
    [InlineData("123456789012345", true)]
    [InlineData("123456789012345678", true)]
    [InlineData("12345678901234567890", true)]
    [InlineData("ABC12345678901", false)]
    [InlineData("ABC1234567890123456", false)]
    [InlineData("ABC12345678901234567890", false)]
    [InlineData("ABC-123456789012", false)]
    [InlineData(123456789012345, false)]
    [InlineData("", false)]
    public void IsValid_WithAllowLooseMatch_ReturnOK(object? value, bool result)
    {
        var validator = new UnifiedSocialCreditCodeValidator { AllowLooseMatch = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new UnifiedSocialCreditCodeValidator();
        Assert.Null(validator.GetValidationResults("91350100M000100Y43", "data"));

        var validationResults = validator.GetValidationResults("invalid", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid unified social credit code.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("invalid", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithAllowLooseMatch_ReturnOK()
    {
        var validator = new UnifiedSocialCreditCodeValidator { AllowLooseMatch = true };
        Assert.Null(validator.GetValidationResults("ABC123456789012", "data"));

        var validationResults = validator.GetValidationResults("invalid", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid unified social credit code (loose match).",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("invalid", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new UnifiedSocialCreditCodeValidator();
        validator.Validate("91350100M000100Y43", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("invalid", "data"));
        Assert.Equal("The field data is not a valid unified social credit code.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("invalid", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithAllowLooseMatch_ReturnOK()
    {
        var validator = new UnifiedSocialCreditCodeValidator { AllowLooseMatch = true };
        validator.Validate("ABC123456789012", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("invalid", "data"));
        Assert.Equal("The field data is not a valid unified social credit code (loose match).", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("invalid", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new UnifiedSocialCreditCodeValidator();
        Assert.Equal("The field data is not a valid unified social credit code.", validator.FormatErrorMessage("data"));

        var validator2 = new UnifiedSocialCreditCodeValidator { AllowLooseMatch = true };
        Assert.Equal("The field data is not a valid unified social credit code (loose match).",
            validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new UnifiedSocialCreditCodeValidator();
        Assert.Equal("UnifiedSocialCreditCodeValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new UnifiedSocialCreditCodeValidator { AllowLooseMatch = true };
        Assert.Equal("UnifiedSocialCreditCodeValidator_ValidationError_AllowLooseMatch", validator2.GetResourceKey());
    }
}