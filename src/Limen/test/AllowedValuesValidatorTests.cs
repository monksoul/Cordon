// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class AllowedValuesValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new AllowedValuesValidator(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new AllowedValuesValidator("Furion", "Fur", "百小僧");
        Assert.NotNull(validator._validator);
        Assert.True(validator._validator.Attributes.First() is AllowedValuesAttribute);
        Assert.Equal(["Furion", "Fur", "百小僧"], validator.Values);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The {0} field does not equal any of the values specified in AllowedValuesValidator.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("百小僧", true)]
    [InlineData("Furion", true)]
    [InlineData("Fur", true)]
    [InlineData("MonkSoul", false)]
    [InlineData("先知", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new AllowedValuesValidator(null, "Furion", "Fur", "百小僧");
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new AllowedValuesValidator("Furion", "Fur", "百小僧");
        Assert.Null(validator.GetValidationResults("Furion", "data"));

        var validationResults = validator.GetValidationResults("MonkSoul", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field does not equal any of the values specified in AllowedValuesValidator.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("MonkSoul", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new AllowedValuesValidator("Furion", "Fur", "百小僧");
        validator.Validate("Furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("MonkSoul", "data"));
        Assert.Equal("The data field does not equal any of the values specified in AllowedValuesValidator.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("MonkSoul", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}