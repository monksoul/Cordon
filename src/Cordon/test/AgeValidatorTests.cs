// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class AgeValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new AgeValidator();
        Assert.False(validator.IsAdultOnly);
        Assert.False(validator.AllowStringValues);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid age.", validator._errorMessageResourceAccessor());

        var validator2 = new AgeValidator { IsAdultOnly = true, AllowStringValues = true };
        Assert.True(validator2.IsAdultOnly);
        Assert.True(validator2.AllowStringValues);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be at least 18 years old.", validator2._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(0, true)]
    [InlineData(10, true)]
    [InlineData(17, true)]
    [InlineData(18, true)]
    [InlineData(30, true)]
    [InlineData(100, true)]
    [InlineData(120, true)]
    [InlineData(121, false)]
    [InlineData(30.00, false)]
    [InlineData(-1, false)]
    [InlineData("0", false)]
    [InlineData("30", false)]
    [InlineData("100", false)]
    [InlineData("120", false)]
    [InlineData("121", false)]
    [InlineData("30.00", false)]
    [InlineData("-1", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new AgeValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(0, false)]
    [InlineData(10, false)]
    [InlineData(17, false)]
    [InlineData(18, true)]
    [InlineData(30, true)]
    [InlineData(100, true)]
    [InlineData(120, true)]
    [InlineData(121, false)]
    public void IsValid_WithIsAdultOnly_ReturnOK(object? value, bool result)
    {
        var validator = new AgeValidator { IsAdultOnly = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("0", true)]
    [InlineData("30", true)]
    [InlineData("100", true)]
    [InlineData("120", true)]
    [InlineData("121", false)]
    [InlineData("30.00", false)]
    [InlineData("-1", false)]
    public void IsValid_WithAllowStringValues_ReturnOK(object? value, bool result)
    {
        var validator = new AgeValidator { AllowStringValues = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new AgeValidator();
        Assert.Null(validator.GetValidationResults(30, "data"));

        var validationResults = validator.GetValidationResults(121, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid age.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(121, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithIsAdultOnly_ReturnOK()
    {
        var validator = new AgeValidator { IsAdultOnly = true };
        Assert.Null(validator.GetValidationResults(30, "data"));

        var validationResults = validator.GetValidationResults(16, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be at least 18 years old.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(16, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithAllowStringValues_ReturnOK()
    {
        var validator = new AgeValidator { AllowStringValues = true };
        Assert.Null(validator.GetValidationResults("30", "data"));

        var validationResults = validator.GetValidationResults("121", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid age.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("121", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new AgeValidator();
        validator.Validate(30, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(121, "data"));
        Assert.Equal("The field data is not a valid age.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(121, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithIsAdultOnly_ReturnOK()
    {
        var validator = new AgeValidator { IsAdultOnly = true };
        validator.Validate(30, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(16, "data"));
        Assert.Equal("The field data must be at least 18 years old.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(16, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithAllowStringValues_ReturnOK()
    {
        var validator = new AgeValidator { AllowStringValues = true };
        validator.Validate("30", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("121", "data"));
        Assert.Equal("The field data is not a valid age.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("121", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new AgeValidator();
        Assert.Equal("The field data is not a valid age.", validator.FormatErrorMessage("data"));

        var validator2 = new AgeValidator { IsAdultOnly = true };
        Assert.Equal("The field data must be at least 18 years old.", validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new AgeValidator();
        Assert.Equal("AgeValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new AgeValidator { IsAdultOnly = true };
        Assert.Equal("AgeValidator_ValidationError_IsAdultOnly", validator2.GetResourceKey());
    }
}