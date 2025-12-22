// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class EndsWithValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new EndsWithValidator(null!));
        Assert.Throws<ArgumentException>(() => new EndsWithValidator(string.Empty));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new EndsWithValidator("ion");
        Assert.Equal("ion", validator.SearchValue);
        Assert.Equal(StringComparison.Ordinal, validator.Comparison);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} does not end with the string '{1}'.",
            validator._errorMessageResourceAccessor());

        var validator2 = new EndsWithValidator('n');
        Assert.Equal("n", validator2.SearchValue);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("furion", true)]
    [InlineData("lion", true)]
    [InlineData("cation", true)]
    [InlineData("furioN", false)]
    [InlineData("furionN", false)]
    [InlineData("dotnetchina", false)]
    [InlineData("github", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new EndsWithValidator("ion");
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("furion", true)]
    [InlineData("furioN", true)]
    [InlineData("furiOn", true)]
    [InlineData("furION", true)]
    public void IsValid_WithComparison_ReturnOK(object? value, bool result)
    {
        var validator = new EndsWithValidator("ion") { Comparison = StringComparison.OrdinalIgnoreCase };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new EndsWithValidator("ion");
        Assert.Null(validator.GetValidationResults("furion", "data"));

        var validationResults = validator.GetValidationResults("dotnetchina", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data does not end with the string 'ion'.",
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
        var validator = new EndsWithValidator("ion");
        validator.Validate("furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("dotnetchina", "data"));
        Assert.Equal("The field data does not end with the string 'ion'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("dotnetchina", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new EndsWithValidator("ion");
        Assert.Equal("The field data does not end with the string 'ion'.",
            validator.FormatErrorMessage("data"));
    }
}