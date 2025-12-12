// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ChineseValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new ChineseValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} contains invalid Chinese characters.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(666, false)]
    [InlineData("Furion", false)]
    [InlineData("先知", true)]
    [InlineData("百小僧", true)]
    [InlineData("亚历山大", true)]
    [InlineData("贼6", false)]
    [InlineData("老铁666", false)]
    [InlineData("React", false)]
    [InlineData("框架", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new ChineseValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new ChineseValidator();
        Assert.Null(validator.GetValidationResults("百小僧", "data"));

        var validationResults = validator.GetValidationResults("陈老6", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data contains invalid Chinese characters.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("陈老6", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new ChineseValidator();
        validator.Validate("百小僧", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("陈老6", "data"));
        Assert.Equal("The field data contains invalid Chinese characters.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("陈老6", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}