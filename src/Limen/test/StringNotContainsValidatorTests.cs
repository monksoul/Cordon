// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class StringNotContainsValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new StringNotContainsValidator(null!));
        Assert.Throws<ArgumentException>(() => new StringNotContainsValidator(string.Empty));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new StringNotContainsValidator("ur");
        Assert.Equal("ur", validator.SearchValue);
        Assert.Equal(StringComparison.Ordinal, validator.Comparison);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must not contain the string '{1}'.",
            validator._errorMessageResourceAccessor());

        var validator2 = new StringNotContainsValidator('n');
        Assert.Equal("n", validator2.SearchValue);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("furion", false)]
    [InlineData("fur", false)]
    [InlineData("free", true)]
    [InlineData("Fur", false)]
    [InlineData("Furion", false)]
    [InlineData("FUR", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new StringNotContainsValidator("ur");
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("furion", false)]
    [InlineData("Fur", false)]
    [InlineData("Furion", false)]
    [InlineData("FUR", false)]
    public void IsValid_WithComparison_ReturnOK(object? value, bool result)
    {
        var validator = new StringNotContainsValidator("ur") { Comparison = StringComparison.OrdinalIgnoreCase };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new StringNotContainsValidator("ur");
        Assert.Null(validator.GetValidationResults("free", "data"));

        var validationResults = validator.GetValidationResults("furion", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must not contain the string 'ur'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("furion", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new StringNotContainsValidator("ur");
        validator.Validate("free", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("furion", "data"));
        Assert.Equal("The field data must not contain the string 'ur'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("furion", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new StringNotContainsValidator("ur");
        Assert.Equal("The field data must not contain the string 'ur'.",
            validator.FormatErrorMessage("data"));
    }
}