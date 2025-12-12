// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ChineseNameValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new ChineseNameValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid Chinese name.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("秦", false)]
    [InlineData("张三", true)]
    [InlineData("李四", true)]
    [InlineData("王五", true)]
    [InlineData("葛二蛋", true)]
    [InlineData("易烊千玺", true)]
    [InlineData("百小僧", true)]
    [InlineData("凯文·杜兰特", true)]
    [InlineData("德克·维尔纳·诺维茨基", true)]
    [InlineData("蒙奇·D·路飞", false)]
    [InlineData("罗罗诺亚·索隆", true)]
    [InlineData("陈老6", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new ChineseNameValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new ChineseNameValidator();
        Assert.Null(validator.GetValidationResults("百小僧", "data"));

        var validationResults = validator.GetValidationResults("陈老6", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid Chinese name.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("陈老6", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new ChineseNameValidator();
        validator.Validate("百小僧", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("陈老6", "data"));
        Assert.Equal("The field data is not a valid Chinese name.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("陈老6", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}