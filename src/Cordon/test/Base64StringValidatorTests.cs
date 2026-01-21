// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class Base64StringValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new Base64StringValidator();

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The {0} field is not a valid Base64 encoding.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("ZnVyaW9u", true)]
    [InlineData("Furion", false)]
    [InlineData("Fur", false)]
    [InlineData("ZnVdyaW9u", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new Base64StringValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new Base64StringValidator();
        Assert.Null(validator.GetValidationResults("ZnVyaW9u", "data"));

        var validationResults = validator.GetValidationResults("Furion", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field is not a valid Base64 encoding.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("Furion", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new Base64StringValidator();
        validator.Validate("ZnVyaW9u", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "data"));
        Assert.Equal("The data field is not a valid Base64 encoding.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}