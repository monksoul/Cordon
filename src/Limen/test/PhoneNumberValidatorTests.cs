// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class PhoneNumberValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new PhoneNumberValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid phone number.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(13800138000, false)]
    [InlineData("13800138000", true)]
    [InlineData("+8613800138000", true)]
    [InlineData("1887654398", false)]
    [InlineData("18876543981", true)]
    [InlineData("16606603420", true)]
    [InlineData("14000000000", false)]
    [InlineData("008615912345678", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new PhoneNumberValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new PhoneNumberValidator();
        Assert.Null(validator.GetValidationResults("13800138000", "data"));

        var validationResults = validator.GetValidationResults("14000000000", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid phone number.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("14000000000", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new PhoneNumberValidator();
        validator.Validate("13800138000", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("14000000000", "data"));
        Assert.Equal("The field data is not a valid phone number.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("14000000000", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}