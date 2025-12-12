// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class TimeOnlyValidatorTest
{
    [Fact]
    public void New_Invalid_Parameters() => Assert.Throws<ArgumentNullException>(() => new TimeOnlyValidator(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new TimeOnlyValidator();
        Assert.NotNull(validator);
        Assert.Empty(validator.Formats);
        Assert.NotNull(validator.Provider);
        Assert.Equal(CultureInfo.InvariantCulture, validator.Provider);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a valid time.", validator._errorMessageResourceAccessor());

        var validator2 = new TimeOnlyValidator("HH:mm:ss", "HH时mm分ss秒");
        Assert.Equal(2, validator2.Formats.Length);
        Assert.Equal(["HH:mm:ss", "HH时mm分ss秒"], validator2.Formats);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a valid time in the following format(s): {1}.",
            validator2._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("HH:mm:ss", false)]
    [InlineData("HH时mm分ss秒", false)]
    [InlineData(true, false)]
    [InlineData("100", false)]
    [InlineData("2023-09-26 16:41:56", false)]
    [InlineData("2023/09/26 16:41:56", false)]
    [InlineData("16:41:56", true)]
    [InlineData("16时41分56秒", true)]
    [InlineData("4:41:56 PM", true)]
    [InlineData("14:32-30", false)]
    public void IsValid_ReturnOK(object? value, bool expected)
    {
        var validator = new TimeOnlyValidator();
        Assert.Equal(expected, validator.IsValid(value));
    }

    [Theory]
    [InlineData("16:41:56", true)]
    [InlineData("16时41分56秒", true)]
    [InlineData("14:32-30", true)]
    public void IsValid_WithFormats_ReturnOK(object? value, bool expected)
    {
        var validator = new TimeOnlyValidator("HH:mm:ss", "HH时mm分ss秒", "HH:mm-ss");
        Assert.Equal(expected, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WhenTimeOnlyType_ReturnOK()
    {
        var validator = new TimeOnlyValidator();
        var timeOnly = new TimeOnly(12, 13, 14);
        Assert.True(validator.IsValid(timeOnly));
    }

    [Fact]
    public void IsValid_WithProvider_ReturnOK()
    {
        var validator = new TimeOnlyValidator();
        Assert.True(validator.IsValid("12:13:14"));

        validator.Provider = CultureInfo.GetCultureInfo("en-GB");
        Assert.True(validator.IsValid("12:13:14"));
    }

    [Fact]
    public void IsValid_WithStyle_ReturnOK()
    {
        var validator = new TimeOnlyValidator("HH:mm:ss");
        Assert.False(validator.IsValid(" 12:13:14 "));

        validator.Style = DateTimeStyles.AllowWhiteSpaces;
        Assert.True(validator.IsValid(" 12:13:14 "));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new TimeOnlyValidator();
        Assert.Null(validator.GetValidationResults("12:13:14", "data"));

        var validationResults = validator.GetValidationResults("12小时13分钟14秒数", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a valid time.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("12小时13分钟14秒数", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithFormats_ReturnOK()
    {
        var validator = new TimeOnlyValidator("HH:mm:ss");
        Assert.Null(validator.GetValidationResults("12:13:14", "data"));

        var validationResults = validator.GetValidationResults("12小时13分钟14秒数", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a valid time in the following format(s): 'HH:mm:ss'.",
            validationResults.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new TimeOnlyValidator();
        validator.Validate("12:13:14", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("12小时13分钟14秒数", "data"));
        Assert.Equal("The field data must be a valid time.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("12小时13分钟14秒数", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithFormats_ReturnOK()
    {
        var validator = new TimeOnlyValidator("HH:mm:ss");
        validator.Validate("12:13:14", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("12小时13分钟14秒数", "data"));
        Assert.Equal("The field data must be a valid time in the following format(s): 'HH:mm:ss'.",
            exception.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new TimeOnlyValidator();
        Assert.Equal("The field data must be a valid time.",
            validator.FormatErrorMessage("data"));

        var validator2 = new TimeOnlyValidator("HH:mm:ss");
        Assert.Equal("The field data must be a valid time in the following format(s): 'HH:mm:ss'.",
            validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void ValidateTime_ReturnOK()
    {
        var validator = new TimeOnlyValidator();
        Assert.True(validator.ValidateTime("12:13:14"));

        var validator2 = new TimeOnlyValidator("HH时mm分ss秒");
        Assert.False(validator2.ValidateTime("12:13:14"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new TimeOnlyValidator();
        Assert.Equal("TimeOnlyValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new TimeOnlyValidator("HH:mm:ss");
        Assert.Equal("TimeOnlyValidator_ValidationError_Formats", validator2.GetResourceKey());
    }
}