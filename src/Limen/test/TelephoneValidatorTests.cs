// 版权归0760-88809963及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class TelephoneValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new TelephoneValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid telephone.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("0341-86091234", true)]
    [InlineData("0760-88809963", true)]
    [InlineData("020-88809963", true)]
    [InlineData("076088809963", false)]
    [InlineData("13800138000", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new TelephoneValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new TelephoneValidator();
        Assert.Null(validator.GetValidationResults("0760-88809963", "data"));

        var validationResults = validator.GetValidationResults("13800138000", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid telephone.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("13800138000", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new TelephoneValidator();
        validator.Validate("0760-88809963", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("13800138000", "data"));
        Assert.Equal("The field data is not a valid telephone.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("13800138000", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}