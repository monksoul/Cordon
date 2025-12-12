// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class PostalCodeValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new PostalCodeValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid postal code.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("734500", true)]
    [InlineData("100101", true)]
    [InlineData("528400", true)]
    [InlineData("528403", true)]
    [InlineData("061000", true)]
    [InlineData("1001001", false)]
    [InlineData(100101, false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new PostalCodeValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new PostalCodeValidator();
        Assert.Null(validator.GetValidationResults("100101", "data"));

        var validationResults = validator.GetValidationResults("1001001", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid postal code.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("1001001", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new PostalCodeValidator();
        validator.Validate("100101", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("1001001", "data"));
        Assert.Equal("The field data is not a valid postal code.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("1001001", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}