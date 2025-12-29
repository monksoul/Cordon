// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class HaveLengthValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new HaveLengthValidator(2);
        Assert.Equal(2, validator.Length);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a string or collection type with a length of exactly '{1}'.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("f", false)]
    [InlineData("fu", true)]
    [InlineData("fur", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new HaveLengthValidator(2);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WithCollection_ReturnOK()
    {
        var validator = new HaveLengthValidator(2);
        Assert.False(validator.IsValid(Array.Empty<string>()));
        Assert.True(validator.IsValid(new[] { "fur", "furion" }));
        Assert.False(validator.IsValid(new[] { "fur", "furion", "百小僧" }));
        Assert.True(validator.IsValid(new List<string> { "fur", "furion" }));
        Assert.False(validator.IsValid(new List<string> { "fur", "furion", "百小僧" }));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new HaveLengthValidator(2);
        Assert.Null(validator.GetValidationResults(new[] { "fur", "furion" }, "data"));

        var validationResults = validator.GetValidationResults(new List<string> { "fur", "furion", "百小僧" }, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a string or collection type with a length of exactly '2'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(new List<string> { "fur", "furion", "百小僧" }, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new HaveLengthValidator(2);
        validator.Validate(new[] { "fur", "furion" }, "data");

        var exception = Assert.Throws<ValidationException>(() =>
            validator.Validate(new List<string> { "fur", "furion", "百小僧" }, "data"));
        Assert.Equal("The field data must be a string or collection type with a length of exactly '2'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() =>
            validator.Validate(new List<string> { "fur", "furion", "百小僧" }, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new HaveLengthValidator(2);
        Assert.Equal("The field data must be a string or collection type with a length of exactly '2'.",
            validator.FormatErrorMessage("data"));
    }
}