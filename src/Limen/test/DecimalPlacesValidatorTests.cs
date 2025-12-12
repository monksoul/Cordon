// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class DecimalPlacesValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new DecimalPlacesValidator(-1));
        Assert.Equal("maxDecimalPlaces must be a non-negative number. (Parameter 'maxDecimalPlaces')",
            exception.Message);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new DecimalPlacesValidator(1);
        Assert.Equal(1, validator.MaxDecimalPlaces);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must not have more than '{1}' decimal places.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(2, true)]
    [InlineData(2.0, true)]
    [InlineData(2.1D, true)]
    [InlineData(2.2F, true)]
    [InlineData(2.33, true)]
    [InlineData(-20, true)]
    [InlineData(-20.00, true)]
    [InlineData("12", false)]
    [InlineData("12.30", false)]
    [InlineData("1,000.00", false)]
    [InlineData("1,000", false)]
    [InlineData("Furion", false)]
    [InlineData(20.333, false)]
    [InlineData(-20.333, false)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new DecimalPlacesValidator(2);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(2, true)]
    [InlineData(2.0, true)]
    [InlineData(2.1D, true)]
    [InlineData(2.2F, true)]
    [InlineData(2.33, true)]
    [InlineData(-20, true)]
    [InlineData(-20.00, true)]
    [InlineData("12", true)]
    [InlineData("12.30", true)]
    [InlineData("1,000.00", true)]
    [InlineData("1,000", true)]
    [InlineData("Furion", false)]
    [InlineData(20.333, false)]
    [InlineData(-20.333, false)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void IsValid_WithAllowStringValues_ReturnOK(object? value, bool result)
    {
        var validator = new DecimalPlacesValidator(2) { AllowStringValues = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new DecimalPlacesValidator(2);
        Assert.Null(validator.GetValidationResults(2.33, "data"));

        var validationResults = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must not have more than '2' decimal places.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithAllowStringValues_ReturnOK()
    {
        var validator = new DecimalPlacesValidator(2) { AllowStringValues = true };
        Assert.Null(validator.GetValidationResults("12.30", "data"));

        var validationResults = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must not have more than '2' decimal places.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new DecimalPlacesValidator(2);
        validator.Validate(2.33, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("The field data must not have more than '2' decimal places.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithAllowStringValues_ReturnOK()
    {
        var validator = new DecimalPlacesValidator(2) { AllowStringValues = true };
        validator.Validate("12.30", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("The field data must not have more than '2' decimal places.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new DecimalPlacesValidator(2);
        Assert.Equal("The field data must not have more than '2' decimal places.",
            validator.FormatErrorMessage("data"));
    }

    [Theory]
    [InlineData(2, 0)]
    [InlineData(-2, 0)]
    [InlineData(2.0, 0)]
    [InlineData(2.1, 1)]
    [InlineData(2.2, 1)]
    [InlineData(2.23, 2)]
    [InlineData(2.234, 3)]
    public void GetDecimalPlaces_ReturnOK(decimal value, int places) =>
        Assert.Equal(places, DecimalPlacesValidator.GetDecimalPlaces(value));
}