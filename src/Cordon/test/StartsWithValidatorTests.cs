// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class StartsWithValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new StartsWithValidator(null!));
        Assert.Throws<ArgumentException>(() => new StartsWithValidator(string.Empty));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new StartsWithValidator("fu");
        Assert.Equal("fu", validator.SearchValue);
        Assert.Equal(StringComparison.Ordinal, validator.Comparison);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} does not start with the string '{1}'.",
            validator._errorMessageResourceAccessor());

        var validator2 = new StartsWithValidator('n');
        Assert.Equal("n", validator2.SearchValue);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("furion", true)]
    [InlineData("fur", true)]
    [InlineData("free", false)]
    [InlineData("Fur", false)]
    [InlineData("Furion", false)]
    [InlineData("FUR", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new StartsWithValidator("fu");
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("furion", true)]
    [InlineData("Fur", true)]
    [InlineData("Furion", true)]
    [InlineData("FUR", true)]
    public void IsValid_WithComparison_ReturnOK(object? value, bool result)
    {
        var validator = new StartsWithValidator("fu") { Comparison = StringComparison.OrdinalIgnoreCase };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new StartsWithValidator("fu");
        Assert.Null(validator.GetValidationResults("furion", "data"));

        var validationResults = validator.GetValidationResults("free", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data does not start with the string 'fu'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("free", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new StartsWithValidator("fu");
        validator.Validate("furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("free", "data"));
        Assert.Equal("The field data does not start with the string 'fu'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("free", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new StartsWithValidator("fu");
        Assert.Equal("The field data does not start with the string 'fu'.",
            validator.FormatErrorMessage("data"));
    }
}