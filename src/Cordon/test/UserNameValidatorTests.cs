// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class UserNameValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new UserNameValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid username.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(666, false)]
    [InlineData("Furion", true)]
    [InlineData("贼6", false)]
    [InlineData("百小僧", false)]
    [InlineData("React", true)]
    [InlineData("monksoul", true)]
    [InlineData("monk-soul", true)]
    [InlineData("monk.soul", false)]
    [InlineData("monk_soul", true)]
    [InlineData("monk__soul", false)]
    [InlineData("monk soul", false)]
    [InlineData("monk*soul", false)]
    [InlineData("_monksoul", false)]
    [InlineData("lzy323", true)]
    [InlineData("monksoul_", false)]
    [InlineData("2023furion", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new UserNameValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new UserNameValidator();
        Assert.Null(validator.GetValidationResults("monksoul", "data"));

        var validationResults = validator.GetValidationResults("monk.soul", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid username.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("monk.soul", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new UserNameValidator();
        validator.Validate("monksoul", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("monk.soul", "data"));
        Assert.Equal("The field data is not a valid username.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("monk.soul", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}