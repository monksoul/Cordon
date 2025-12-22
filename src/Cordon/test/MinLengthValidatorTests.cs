// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class MinLengthValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new MinLengthValidator(5);
        Assert.NotNull(validator._validator);
        Assert.True(validator._validator.Attributes.First() is MinLengthAttribute);
        Assert.Equal(5, validator.Length);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal(
            "The field {0} must be a string or array type with a minimum length of '{1}'.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("fur", false)]
    [InlineData("Furion", true)]
    [InlineData("free", false)]
    [InlineData("monksoul", true)]
    [InlineData("dotnetchina", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new MinLengthValidator(5);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WithCollectionType_ReturnOK()
    {
        var validator = new MinLengthValidator(2);

        var list = new List<string>();
        Assert.False(validator.IsValid(list));

        list.Add("furion");
        Assert.False(validator.IsValid(list));

        list.Add("fur");
        Assert.True(validator.IsValid(list));

        list.Add("monksoul");
        Assert.True(validator.IsValid(list));

        list.Add("dotnetchina");
        Assert.True(validator.IsValid(list));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new MinLengthValidator(5);
        Assert.Null(validator.GetValidationResults("monksoul", "data"));

        var validationResults = validator.GetValidationResults("fur", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(
            "The field data must be a string or array type with a minimum length of '5'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("fur", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new MinLengthValidator(5);
        validator.Validate("monksoul", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("fur", "data"));
        Assert.Equal(
            "The field data must be a string or array type with a minimum length of '5'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("fur", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new MinLengthValidator(5);
        Assert.Equal(
            "The field data must be a string or array type with a minimum length of '5'.",
            validator.FormatErrorMessage("data"));
    }
}