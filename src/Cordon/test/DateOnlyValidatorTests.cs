// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class DateOnlyValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() => Assert.Throws<ArgumentNullException>(() => new DateOnlyValidator(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new DateOnlyValidator();
        Assert.NotNull(validator);
        Assert.Empty(validator.Formats);
        Assert.NotNull(validator.Provider);
        Assert.Equal(CultureInfo.InvariantCulture, validator.Provider);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a valid date.", validator._errorMessageResourceAccessor());

        var validator2 = new DateOnlyValidator("yyyy-MM-dd", "yyyy/MM/dd");
        Assert.Equal(2, validator2.Formats.Length);
        Assert.Equal(["yyyy-MM-dd", "yyyy/MM/dd"], validator2.Formats);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a valid date in the following format(s): {1}.",
            validator2._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("yyyy/MM/dd", false)]
    [InlineData("yyyy/MM/dd HH:mm:ss", false)]
    [InlineData(true, false)]
    [InlineData("100", false)]
    [InlineData("2023-09-26 16:41:56", false)]
    [InlineData("2023/09/26 16:41:56", false)]
    [InlineData("2023年09月26日 16时41分56秒", false)]
    [InlineData("Sep 26, 2023 4:41:56 PM", false)]
    [InlineData("September 26, 2023 4:41:56 PM", false)]
    [InlineData("26 Sep 2023 16:41:56", false)]
    [InlineData("Sep 26, 2023 4:41 PM", false)]
    [InlineData("September 26, 2023 4:41 PM", false)]
    [InlineData("2023-09-26T16:41:56", true)]
    [InlineData("2023/09/26T16:41:56", true)]
    [InlineData("2023-09-26", true)]
    [InlineData("2023/09/26", true)]
    [InlineData("09/26/2023", true)]
    [InlineData("26/09/2023", false)]
    [InlineData("2023年09月26日", true)]
    [InlineData("Sep 26, 2023", true)]
    [InlineData("September 26, 2023", true)]
    [InlineData("26 Sep 2023", true)]
    [InlineData("2033-01/10 14:32-30", false)]
    [InlineData("2033-01/10", true)]
    public void IsValid_ReturnOK(object? value, bool expected)
    {
        var validator = new DateOnlyValidator();
        Assert.Equal(expected, validator.IsValid(value));
    }

    [Theory]
    [InlineData("2023-09-26", true)]
    [InlineData("2023/09/26", true)]
    [InlineData("09/26/2023", false)]
    [InlineData("26/09/2023", true)]
    [InlineData("2023年09月26日", false)]
    [InlineData("Sep 26, 2023", false)]
    [InlineData("September 26, 2023", false)]
    [InlineData("26 Sep 2023", false)]
    [InlineData("2033-01/10", false)]
    public void IsValid_WithFormats_ReturnOK(object? value, bool expected)
    {
        var validator = new DateOnlyValidator("yyyy/MM/dd", "yyyy-MM-dd", "dd/MM/yyyy");
        Assert.Equal(expected, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WhenDateOnlyType_ReturnOK()
    {
        var validator = new DateOnlyValidator();
        var dateOnly = new DateOnly(2025, 1, 1);
        Assert.True(validator.IsValid(dateOnly));
    }

    [Fact]
    public void IsValid_WithProvider_ReturnOK()
    {
        var validator = new DateOnlyValidator();
        Assert.False(validator.IsValid("26/09/2023"));

        validator.Provider = CultureInfo.GetCultureInfo("en-GB");
        Assert.True(validator.IsValid("26/09/2023"));
    }

    [Fact]
    public void IsValid_WithStyle_ReturnOK()
    {
        var validator = new DateOnlyValidator("yyyy-MM-dd");
        Assert.False(validator.IsValid(" 2023-09-26 "));

        validator.Style = DateTimeStyles.AllowWhiteSpaces;
        Assert.True(validator.IsValid(" 2023-09-26 "));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new DateOnlyValidator();
        Assert.Null(validator.GetValidationResults("2023-09-26", "data"));

        var validationResults = validator.GetValidationResults("26/09/2023", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a valid date.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("26/09/2023", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithFormats_ReturnOK()
    {
        var validator = new DateOnlyValidator("yyyy-MM-dd");
        Assert.Null(validator.GetValidationResults("2023-09-26", "data"));

        var validationResults = validator.GetValidationResults("26/09/2023", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a valid date in the following format(s): 'yyyy-MM-dd'.",
            validationResults.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new DateOnlyValidator();
        validator.Validate("2023-09-26", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("26/09/2023", "data"));
        Assert.Equal("The field data must be a valid date.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("26/09/2023", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithFormats_ReturnOK()
    {
        var validator = new DateOnlyValidator("yyyy-MM-dd");
        validator.Validate("2023-09-26", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("26/09/2023", "data"));
        Assert.Equal("The field data must be a valid date in the following format(s): 'yyyy-MM-dd'.",
            exception.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new DateOnlyValidator();
        Assert.Equal("The field data must be a valid date.",
            validator.FormatErrorMessage("data"));

        var validator2 = new DateOnlyValidator("yyyy-MM-dd");
        Assert.Equal("The field data must be a valid date in the following format(s): 'yyyy-MM-dd'.",
            validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void ValidateDate_ReturnOK()
    {
        var validator = new DateOnlyValidator();
        Assert.True(validator.ValidateDate("2023-09-26"));

        var validator2 = new DateOnlyValidator("yyyy/MM/dd");
        Assert.False(validator2.ValidateDate("2023-09-26"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new DateOnlyValidator();
        Assert.Equal("DateOnlyValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new DateOnlyValidator("yyyy-MM-dd");
        Assert.Equal("DateOnlyValidator_ValidationError_Formats", validator2.GetResourceKey());
    }
}