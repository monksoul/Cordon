// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class IDCardValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new IDCardValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid Id card number format.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(622223199912051311, false)]
    [InlineData("622223199912051311", true)]
    [InlineData("12345619991205131x", true)]
    [InlineData(123456991010193, false)]
    [InlineData("123456991010193", true)]
    [InlineData(1234569910101933, false)]
    [InlineData("1234569910101933", false)]
    [InlineData("12345619991205131a", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new IDCardValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new IDCardValidator();
        Assert.Null(validator.GetValidationResults("622223199912051311", "data"));

        var validationResults = validator.GetValidationResults("1234569910101933", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid Id card number format.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("1234569910101933", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new IDCardValidator();
        validator.Validate("622223199912051311", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("1234569910101933", "data"));
        Assert.Equal("The field data is not a valid Id card number format.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("1234569910101933", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}