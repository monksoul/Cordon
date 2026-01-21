// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class DeniedValuesValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new DeniedValuesValidator(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new DeniedValuesValidator("Furion", "Fur", "百小僧");
        Assert.Equal(["Furion", "Fur", "百小僧"], validator.Values);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The {0} field equals one of the values specified in DeniedValuesValidator.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("百小僧", false)]
    [InlineData("Furion", false)]
    [InlineData("Fur", false)]
    [InlineData("MonkSoul", true)]
    [InlineData("先知", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new DeniedValuesValidator("Furion", "Fur", "百小僧");
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new DeniedValuesValidator("Furion", "Fur", "百小僧");
        Assert.Null(validator.GetValidationResults("MonkSoul", "data"));

        var validationResults = validator.GetValidationResults("Furion", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field equals one of the values specified in DeniedValuesValidator.",
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
        var validator = new DeniedValuesValidator("Furion", "Fur", "百小僧");
        validator.Validate("MonkSoul", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "data"));
        Assert.Equal("The data field equals one of the values specified in DeniedValuesValidator.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}