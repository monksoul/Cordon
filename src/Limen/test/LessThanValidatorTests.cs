// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class LessThanValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new LessThanValidator(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        Assert.Equal(10, validator.CompareValue);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be less than '{1}'.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(0, true)]
    [InlineData(8, true)]
    [InlineData(9, true)]
    [InlineData(10, false)]
    [InlineData(11, false)]
    [InlineData(30, false)]
    [InlineData(10.1, false)]
    [InlineData(9.99, false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new LessThanValidator(10);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WithDateTimeType_ReturnOK()
    {
        var validator = new LessThanValidator(new DateTime(2020, 1, 1));
        Assert.False(validator.IsValid(new DateTime(2020, 1, 1)));
        Assert.False(validator.IsValid(new DateTime(2020, 1, 2)));
        Assert.True(validator.IsValid(new DateTime(2019, 12, 31)));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        Assert.Null(validator.GetValidationResults(9, "data"));

        var validationResults = validator.GetValidationResults(30, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be less than '10'.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(30, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        validator.Validate(9, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(30, "data"));
        Assert.Equal("The field data must be less than '10'.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(30, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        Assert.Equal("The field data must be less than '10'.", validator.FormatErrorMessage("data"));
    }
}