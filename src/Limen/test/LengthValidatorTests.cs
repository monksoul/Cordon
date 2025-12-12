// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class LengthValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new LengthValidator(5, 10);
        Assert.NotNull(validator._validator);
        Assert.True(validator._validator.Attributes.First() is LengthAttribute);
        Assert.Equal(5, validator.MinimumLength);
        Assert.Equal(10, validator.MaximumLength);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal(
            "The field {0} must be a string or collection type with a minimum length of '{1}' and maximum length of '{2}'.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("fur", false)]
    [InlineData("Furion", true)]
    [InlineData("free", false)]
    [InlineData("monks", true)]
    [InlineData("dotnetchina", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new LengthValidator(5, 10);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WithCollectionType_ReturnOK()
    {
        var validator = new LengthValidator(2, 3);

        var list = new List<string>();
        Assert.False(validator.IsValid(list));

        list.Add("furion");
        Assert.False(validator.IsValid(list));

        list.Add("fur");
        Assert.True(validator.IsValid(list));

        list.Add("monksoul");
        Assert.True(validator.IsValid(list));

        list.Add("dotnetchina");
        Assert.False(validator.IsValid(list));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new LengthValidator(5, 10);
        Assert.Null(validator.GetValidationResults("Furion", "data"));

        var validationResults = validator.GetValidationResults("dotnetchina", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(
            "The field data must be a string or collection type with a minimum length of '5' and maximum length of '10'.",
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
        var validator = new LengthValidator(5, 10);
        validator.Validate("Furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("dotnetchina", "data"));
        Assert.Equal(
            "The field data must be a string or collection type with a minimum length of '5' and maximum length of '10'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("dotnetchina", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new LengthValidator(5, 10);
        Assert.Equal(
            "The field data must be a string or collection type with a minimum length of '5' and maximum length of '10'.",
            validator.FormatErrorMessage("data"));
    }
}