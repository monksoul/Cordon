// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class DateTimeValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() => Assert.Throws<ArgumentNullException>(() => new DateTimeValidator(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new DateTimeValidator();
        Assert.NotNull(validator);
        Assert.Empty(validator.Formats);
        Assert.NotNull(validator.Provider);
        Assert.Equal(CultureInfo.InvariantCulture, validator.Provider);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a valid datetime.", validator._errorMessageResourceAccessor());

        var validator2 = new DateTimeValidator("yyyy-MM-dd", "yyyy/MM/dd");
        Assert.Equal(2, validator2.Formats.Length);
        Assert.Equal(["yyyy-MM-dd", "yyyy/MM/dd"], validator2.Formats);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a valid datetime in the following format(s): {1}.",
            validator2._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("yyyy/MM/dd", false)]
    [InlineData("yyyy/MM/dd HH:mm:ss", false)]
    [InlineData(true, false)]
    [InlineData("100", false)]
    [InlineData("2023-09-26 16:41:56", true)]
    [InlineData("2023/09/26 16:41:56", true)]
    [InlineData("2023年09月26日 16时41分56秒", true)]
    [InlineData("Sep 26, 2023 4:41:56 PM", true)]
    [InlineData("September 26, 2023 4:41:56 PM", true)]
    [InlineData("26 Sep 2023 16:41:56", true)]
    [InlineData("Sep 26, 2023 4:41 PM", true)]
    [InlineData("September 26, 2023 4:41 PM", true)]
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
        var validator = new DateTimeValidator();
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
        var validator = new DateTimeValidator("yyyy/MM/dd", "yyyy-MM-dd", "dd/MM/yyyy");
        Assert.Equal(expected, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WhenDateTimeType_ReturnOK()
    {
        var validator = new DateTimeValidator();
        var dateTime = new DateTime(2023, 9, 26, 12, 0, 0, 0);
        Assert.True(validator.IsValid(dateTime));
    }

    [Fact]
    public void IsValid_WhenDateTimeOffsetType_ReturnOK()
    {
        var validator = new DateTimeValidator();
        var dateTime = new DateTime(2023, 9, 26, 12, 0, 0, 0);
        var dateTimeOffset = new DateTimeOffset(dateTime);
        Assert.True(validator.IsValid(dateTimeOffset));
    }

    [Fact]
    public void IsValid_WhenDateOnlyType_ReturnOK()
    {
        var validator = new DateTimeValidator();
        var dateOnly = new DateOnly(2025, 1, 1);
        Assert.True(validator.IsValid(dateOnly));
    }

    [Fact]
    public void IsValid_WithProvider_ReturnOK()
    {
        var validator = new DateTimeValidator();
        Assert.False(validator.IsValid("26/09/2023 12:13:14"));

        validator.Provider = CultureInfo.GetCultureInfo("en-GB");
        Assert.True(validator.IsValid("26/09/2023 12:13:14"));
    }

    [Fact]
    public void IsValid_WithStyle_ReturnOK()
    {
        var validator = new DateTimeValidator("yyyy-MM-dd HH:mm:ss");
        Assert.False(validator.IsValid(" 2023-09-26 12:13:14 "));

        validator.Style = DateTimeStyles.AllowWhiteSpaces;
        Assert.True(validator.IsValid(" 2023-09-26 12:13:14 "));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new DateTimeValidator();
        Assert.Null(validator.GetValidationResults("2023-09-26 12:13:14", "data"));

        var validationResults = validator.GetValidationResults("26/09/2023 12:13:14", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a valid datetime.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("26/09/2023 12:13:14", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithFormats_ReturnOK()
    {
        var validator = new DateTimeValidator("yyyy-MM-dd HH:mm:ss");
        Assert.Null(validator.GetValidationResults("2023-09-26 12:13:14", "data"));

        var validationResults = validator.GetValidationResults("26/09/2023 12:13:14", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a valid datetime in the following format(s): 'yyyy-MM-dd HH:mm:ss'.",
            validationResults.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new DateTimeValidator();
        validator.Validate("2023-09-26 12:13:14", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("26/09/2023 12:13:14", "data"));
        Assert.Equal("The field data must be a valid datetime.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("26/09/2023 12:13:14", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithFormats_ReturnOK()
    {
        var validator = new DateTimeValidator("yyyy-MM-dd HH:mm:ss");
        validator.Validate("2023-09-26 12:13:14", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("26/09/2023 12：13：14", "data"));
        Assert.Equal("The field data must be a valid datetime in the following format(s): 'yyyy-MM-dd HH:mm:ss'.",
            exception.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new DateTimeValidator();
        Assert.Equal("The field data must be a valid datetime.",
            validator.FormatErrorMessage("data"));

        var validator2 = new DateTimeValidator("yyyy-MM-dd HH:mm:ss");
        Assert.Equal("The field data must be a valid datetime in the following format(s): 'yyyy-MM-dd HH:mm:ss'.",
            validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void ValidateDateTime_ReturnOK()
    {
        var validator = new DateTimeValidator();
        Assert.True(validator.ValidateDateTime("2023-09-26 12:13:14"));

        var validator2 = new DateTimeValidator("yyyy/MM/dd Hh:mm:ss");
        Assert.False(validator2.ValidateDateTime("2023-09-26 12:13:14"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new DateTimeValidator();
        Assert.Equal("DateTimeValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new DateTimeValidator("yyyy-MM-dd");
        Assert.Equal("DateTimeValidator_ValidationError_Formats", validator2.GetResourceKey());
    }
}