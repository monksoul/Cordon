// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class DecimalValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new DecimalValidator(0));
        Assert.Equal("precision must be a positive number. (Parameter 'precision')", exception.Message);

        var exception2 = Assert.Throws<ArgumentOutOfRangeException>(() => new DecimalValidator(-1));
        Assert.Equal("precision must be a positive number. (Parameter 'precision')", exception2.Message);

        var exception3 = Assert.Throws<ArgumentOutOfRangeException>(() => new DecimalValidator(10, -1));
        Assert.Equal("scale must be between 0 and precision. (Parameter 'scale')", exception3.Message);

        var exception4 = Assert.Throws<ArgumentOutOfRangeException>(() => new DecimalValidator(10, 12));
        Assert.Equal("scale must be between 0 and precision. (Parameter 'scale')", exception4.Message);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new DecimalValidator();
        Assert.Equal(18, validator.Precision);
        Assert.Equal(2, validator.Scale);
        Assert.False(validator.AllowNegative);
        Assert.False(validator.AllowStringValues);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal(
            "The field {0} must be a non-negative decimal within the DECIMAL({1},{2}) precision or scale limit.",
            validator._errorMessageResourceAccessor());

        var validator2 = new DecimalValidator { AllowNegative = true };
        Assert.True(validator2.AllowNegative);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal(
            "The field {0} must be a valid decimal within the DECIMAL({1},{2}) precision or scale limit.",
            validator2._errorMessageResourceAccessor());

        var validator3 = new DecimalValidator(10, 3);
        Assert.Equal(10, validator3.Precision);
        Assert.Equal(3, validator3.Scale);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(2, true)]
    [InlineData(2.0, true)]
    [InlineData(2.1D, true)]
    [InlineData(2.2F, true)]
    [InlineData(2.33, true)]
    [InlineData(-20, false)]
    [InlineData(-20.00, false)]
    [InlineData(2.333, false)]
    [InlineData("12", false)]
    [InlineData("12.30", false)]
    [InlineData("-12.30", false)]
    [InlineData("1,000.00", false)]
    [InlineData("1,000", false)]
    [InlineData("Furion", false)]
    [InlineData(20.333, false)]
    [InlineData(-20.333, false)]
    [InlineData("-2.333", false)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new DecimalValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(2, true)]
    [InlineData(2.0, true)]
    [InlineData(2.1D, true)]
    [InlineData(2.2F, true)]
    [InlineData(2.33, true)]
    [InlineData(-20, false)]
    [InlineData(-20.00, false)]
    [InlineData(2.333, false)]
    [InlineData("12", true)]
    [InlineData("12.30", true)]
    [InlineData("-12.30", false)]
    [InlineData("1,000.00", true)]
    [InlineData("1,000", true)]
    [InlineData("Furion", false)]
    [InlineData(20.333, false)]
    [InlineData(-20.333, false)]
    [InlineData("-2.333", false)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void IsValid_WithAllowStringValues_ReturnOK(object? value, bool result)
    {
        var validator = new DecimalValidator { AllowStringValues = true };
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
    [InlineData(2.333, false)]
    [InlineData("12", false)]
    [InlineData("12.30", false)]
    [InlineData("-12.30", false)]
    [InlineData("1,000.00", false)]
    [InlineData("1,000", false)]
    [InlineData("Furion", false)]
    [InlineData(20.333, false)]
    [InlineData(-20.333, false)]
    [InlineData("-2.333", false)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void IsValid_WithAllowNegative_ReturnOK(object? value, bool result)
    {
        var validator = new DecimalValidator { AllowNegative = true };
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
    [InlineData(2.333, false)]
    [InlineData("12", true)]
    [InlineData("12.30", true)]
    [InlineData("-12.30", true)]
    [InlineData("1,000.00", true)]
    [InlineData("1,000", true)]
    [InlineData("Furion", false)]
    [InlineData(20.333, false)]
    [InlineData(-20.333, false)]
    [InlineData("-2.333", false)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void IsValid_WithAllowNegative_WithAllowStringValues_ReturnOK(object? value, bool result)
    {
        var validator = new DecimalValidator { AllowNegative = true, AllowStringValues = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new DecimalValidator();
        Assert.Null(validator.GetValidationResults(2.33, "data"));

        var validationResults = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);

        var validator2 = new DecimalValidator();
        var validationResults3 = validator2.GetValidationResults(-2.33, "data");
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithAllowStringValues_ReturnOK()
    {
        var validator = new DecimalValidator { AllowStringValues = true };
        Assert.Null(validator.GetValidationResults("12.30", "data"));

        var validationResults = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);

        var validator2 = new DecimalValidator { AllowStringValues = true };
        var validationResults3 = validator2.GetValidationResults(-2.33, "data");
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithAllowNegative_ReturnOK()
    {
        var validator = new DecimalValidator { AllowNegative = true };
        Assert.Null(validator.GetValidationResults(2.33, "data"));

        var validationResults = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a valid decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);

        var validator2 = new DecimalValidator { AllowNegative = true };
        Assert.Null(validator2.GetValidationResults(-2.33, "data"));
    }

    [Fact]
    public void GetValidationResults_WithAllowNegative_WithAllowStringValues_ReturnOK()
    {
        var validator = new DecimalValidator { AllowNegative = true, AllowStringValues = true };
        Assert.Null(validator.GetValidationResults("12.30", "data"));

        var validationResults = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a valid decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(20.333, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);

        var validator2 = new DecimalValidator { AllowNegative = true, AllowStringValues = true };
        Assert.Null(validator2.GetValidationResults(-2.33, "data"));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new DecimalValidator();
        validator.Validate(2.33, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("数据无效", exception2.Message);

        var validator2 = new DecimalValidator();
        var exception3 = Assert.Throws<ValidationException>(() => validator2.Validate(-2.33, "data"));
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            exception3.Message);
    }

    [Fact]
    public void Validate_WithAllowStringValues_ReturnOK()
    {
        var validator = new DecimalValidator { AllowStringValues = true };
        validator.Validate("12.30", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("数据无效", exception2.Message);

        var validator2 = new DecimalValidator { AllowStringValues = true };
        var exception3 = Assert.Throws<ValidationException>(() => validator2.Validate(-2.33, "data"));
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            exception3.Message);
    }

    [Fact]
    public void Validate_WithAllowNegative_ReturnOK()
    {
        var validator = new DecimalValidator { AllowNegative = true };
        validator.Validate(2.33, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("The field data must be a valid decimal within the DECIMAL(18,2) precision or scale limit.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("数据无效", exception2.Message);

        var validator2 = new DecimalValidator { AllowNegative = true };
        validator2.Validate(-2.33, "data");
    }

    [Fact]
    public void Validate_WithAllowNegative_WithAllowStringValues_ReturnOK()
    {
        var validator = new DecimalValidator { AllowNegative = true, AllowStringValues = true };
        validator.Validate("12.30", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("The field data must be a valid decimal within the DECIMAL(18,2) precision or scale limit.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(20.333, "data"));
        Assert.Equal("数据无效", exception2.Message);

        var validator2 = new DecimalValidator { AllowNegative = true, AllowStringValues = true };
        validator2.Validate(-2.33, "data");
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new DecimalValidator();
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            validator.FormatErrorMessage("data"));

        var validator2 = new DecimalValidator { AllowNegative = true };
        Assert.Equal("The field data must be a valid decimal within the DECIMAL(18,2) precision or scale limit.",
            validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new DecimalValidator();
        Assert.Equal("DecimalValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new DecimalValidator { AllowNegative = true };
        Assert.Equal("DecimalValidator_ValidationError_AllowNegative", validator2.GetResourceKey());
    }

    [Theory]
    [InlineData(2, 0)]
    [InlineData(-2, 0)]
    [InlineData(2.0, 0)]
    [InlineData(2.1, 1)]
    [InlineData(2.2, 1)]
    [InlineData(2.23, 2)]
    [InlineData(2.234, 3)]
    public void GetActualScale_ReturnOK(decimal value, int places) =>
        Assert.Equal(places, DecimalValidator.GetActualScale(value));
}