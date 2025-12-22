// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class MD5StringValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new MD5StringValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid MD5 string.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(666, false)]
    [InlineData("Furion", false)]
    [InlineData("3f2d0ea0ef4df562719e70e41413658e", true)]
    [InlineData("3F2D0EA0EF4DF562719E70E41413658E", true)]
    [InlineData("ef4df562719e70e4", false)]
    [InlineData("EF4DF562719E70E4", false)]
    [InlineData("3F2D0EA0EF4DF562719E70E41413658", false)]
    [InlineData("3f2d0ea0ef4df562719e70e41413658ea", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new MD5StringValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData("3f2d0ea0ef4df562719e70e41413658e", true)]
    [InlineData("3F2D0EA0EF4DF562719E70E41413658E", true)]
    [InlineData("ef4df562719e70e4", true)]
    [InlineData("EF4DF562719E70E4", true)]
    public void IsValid_WithAllowShortFormat_ReturnOK(object? value, bool result)
    {
        var validator = new MD5StringValidator { AllowShortFormat = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new MD5StringValidator();
        Assert.Null(validator.GetValidationResults("3f2d0ea0ef4df562719e70e41413658e", "data"));

        var validationResults = validator.GetValidationResults("Furion", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid MD5 string.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("Furion", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new MD5StringValidator();
        validator.Validate("3f2d0ea0ef4df562719e70e41413658e", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "data"));
        Assert.Equal("The field data is not a valid MD5 string.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}