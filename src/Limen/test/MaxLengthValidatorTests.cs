// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class MaxLengthValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new MaxLengthValidator(5);
        Assert.NotNull(validator._validator);
        Assert.True(validator._validator.Attributes.First() is MaxLengthAttribute);
        Assert.Equal(5, validator.Length);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal(
            "The field {0} must be a string or array type with a maximum length of '{1}'.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("fur", true)]
    [InlineData("Furion", false)]
    [InlineData("free", true)]
    [InlineData("monk", true)]
    [InlineData("dotnetchina", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new MaxLengthValidator(5);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WithCollectionType_ReturnOK()
    {
        var validator = new MaxLengthValidator(2);

        var list = new List<string>();
        Assert.True(validator.IsValid(list));

        list.Add("furion");
        Assert.True(validator.IsValid(list));

        list.Add("fur");
        Assert.True(validator.IsValid(list));

        list.Add("monksoul");
        Assert.False(validator.IsValid(list));

        list.Add("dotnetchina");
        Assert.False(validator.IsValid(list));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new MaxLengthValidator(5);
        Assert.Null(validator.GetValidationResults("monk", "data"));

        var validationResults = validator.GetValidationResults("dotnetchina", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(
            "The field data must be a string or array type with a maximum length of '5'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("dotnetchina", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new MaxLengthValidator(5);
        validator.Validate("monk", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("dotnetchina", "data"));
        Assert.Equal(
            "The field data must be a string or array type with a maximum length of '5'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("dotnetchina", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new MaxLengthValidator(5);
        Assert.Equal(
            "The field data must be a string or array type with a maximum length of '5'.",
            validator.FormatErrorMessage("data"));
    }
}