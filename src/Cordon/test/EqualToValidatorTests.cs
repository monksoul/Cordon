// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class EqualToValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new EqualToValidator("furion");
        Assert.Equal("furion", validator.CompareValue);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be equal to '{1}'.", validator._errorMessageResourceAccessor());

        var validator2 = new EqualToValidator(6);
        Assert.Equal(6, validator2.CompareValue);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("furion", true)]
    [InlineData("Furion", false)]
    [InlineData("fur", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new EqualToValidator("furion");
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new EqualToValidator("furion");
        Assert.Null(validator.GetValidationResults("furion", "data"));

        var validationResults = validator.GetValidationResults("fur", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be equal to 'furion'.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("fur", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new EqualToValidator("furion");
        validator.Validate("furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("fur", "data"));
        Assert.Equal("The field data must be equal to 'furion'.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("fur", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new EqualToValidator("furion");
        Assert.Equal("The field data must be equal to 'furion'.", validator.FormatErrorMessage("data"));

        var validator2 = new EqualToValidator(null);
        Assert.Equal("The field data must be equal to 'null'.", validator2.FormatErrorMessage("data"));
    }
}