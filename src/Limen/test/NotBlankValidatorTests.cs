// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class NotBlankValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new NotBlankValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} cannot be empty or whitespace.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("  ", false)]
    [InlineData("a", true)]
    [InlineData('A', true)]
    [InlineData("Furion", true)]
    [InlineData(1, false)]
    [InlineData(false, false)]
    [InlineData("\u3000", false)] // 特殊 Unicode 空白字符
    [InlineData('\0', false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new NotBlankValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new NotBlankValidator();
        Assert.Null(validator.GetValidationResults("Furion", "data"));

        var validationResults = validator.GetValidationResults(string.Empty, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data cannot be empty or whitespace.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(string.Empty, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new NotBlankValidator();
        validator.Validate("Furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(string.Empty, "data"));
        Assert.Equal("The field data cannot be empty or whitespace.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(string.Empty, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}