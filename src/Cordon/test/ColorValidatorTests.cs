// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ColorValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new ColorValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid color.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("#fff", true)]
    [InlineData("#ffffff", true)]
    [InlineData("#f1f1f9", true)]
    [InlineData("rgb(255, 255, 255)", true)]
    [InlineData("rgba(255, 255, 255, 0.3)", true)]
    [InlineData("#dedede", true)]
    [InlineData("#FFF", true)]
    [InlineData("#FFFFFF", true)]
    [InlineData("rgb(100%, 0%, 50%)", true)]
    [InlineData("rgba(255, 0, 0, 0.5)", true)]
    [InlineData("rgba(100%, 0%, 50%, 0.8)", true)]
    [InlineData("hsl(0, 100%, 50%)", false)]
    [InlineData("hsla(0, 100%, 50%, 0.5)", false)]
    [InlineData("hwb(240, 0%, 0%)", false)]
    [InlineData("lch(50, 30, 120)", false)]
    [InlineData("oklch(50, 30, 120)", false)]
    [InlineData("lab(50, -30, 40)", false)]
    [InlineData("oklab(50, -30, 40)", false)]
    [InlineData("#ff", false)]
    [InlineData("#fffffff", false)]
    [InlineData("#FF0000FF", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new ColorValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("#fff", true)]
    [InlineData("#ffffff", true)]
    [InlineData("#f1f1f9", true)]
    [InlineData("rgb(255, 255, 255)", true)]
    [InlineData("rgba(255, 255, 255, 0.3)", true)]
    [InlineData("#dedede", true)]
    [InlineData("#FFF", true)]
    [InlineData("#FFFFFF", true)]
    [InlineData("rgb(100%, 0%, 50%)", true)]
    [InlineData("rgba(255, 0, 0, 0.5)", true)]
    [InlineData("rgba(100%, 0%, 50%, 0.8)", true)]
    [InlineData("hsl(0, 100%, 50%)", true)]
    [InlineData("hsla(0, 100%, 50%, 0.5)", true)]
    [InlineData("hwb(240, 0%, 0%)", true)]
    [InlineData("lch(50, 30, 120)", true)]
    [InlineData("oklch(50, 30, 120)", true)]
    [InlineData("lab(50, -30, 40)", true)]
    [InlineData("oklab(50, -30, 40)", true)]
    [InlineData("#ff", false)]
    [InlineData("#fffffff", false)]
    [InlineData("#FF0000FF", true)]
    public void IsValid_WithFullMode_ReturnOK(object? value, bool result)
    {
        var validator = new ColorValidator { FullMode = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new ColorValidator();
        Assert.Null(validator.GetValidationResults("#ffffff", "data"));

        var validationResults = validator.GetValidationResults("#ff", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid color.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("#ff", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new ColorValidator();
        validator.Validate("#ffffff", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("#ff", "data"));
        Assert.Equal("The field data is not a valid color.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("#ff", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}