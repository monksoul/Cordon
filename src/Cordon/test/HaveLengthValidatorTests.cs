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
        Assert.False(validator.AllowEmpty);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a string or collection type with a length of exactly '{1}'.",
            validator._errorMessageResourceAccessor());

        var validator2 = new HaveLengthValidator(2) { AllowEmpty = true };
        Assert.True(validator2.AllowEmpty);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be empty or have a length of exactly '{1}'.",
            validator2._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("f", false)]
    [InlineData("fu", true)]
    [InlineData("fur", false)]
    [InlineData("", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new HaveLengthValidator(2);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("f", false)]
    [InlineData("fu", true)]
    [InlineData("fur", false)]
    [InlineData("", true)]
    public void IsValid_WithAllowEmpty_ReturnOK(object? value, bool result)
    {
        var validator = new HaveLengthValidator(2) { AllowEmpty = true };
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
    public void IsValid_WithCollection_WithAllowEmpty_ReturnOK()
    {
        var validator = new HaveLengthValidator(2) { AllowEmpty = true };
        Assert.True(validator.IsValid(Array.Empty<string>()));
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

        var validationResults2 = validator.GetValidationResults(Array.Empty<string>(), "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("The field data must be a string or collection type with a length of exactly '2'.",
            validationResults2.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults3 = validator.GetValidationResults(new List<string> { "fur", "furion", "百小僧" }, "data");
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal("数据无效", validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithAllowEmpty_ReturnOK()
    {
        var validator = new HaveLengthValidator(2) { AllowEmpty = true };
        Assert.Null(validator.GetValidationResults(new[] { "fur", "furion" }, "data"));

        var validationResults = validator.GetValidationResults(new List<string> { "fur", "furion", "百小僧" }, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be empty or have a length of exactly '2'.",
            validationResults.First().ErrorMessage);

        Assert.Null(validator.GetValidationResults(Array.Empty<string>(), "data"));

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

        var exception2 = Assert.Throws<ValidationException>(() =>
            validator.Validate(Array.Empty<string>(), "data"));
        Assert.Equal("The field data must be a string or collection type with a length of exactly '2'.",
            exception2.Message);

        validator.ErrorMessage = "数据无效";
        var exception3 = Assert.Throws<ValidationException>(() =>
            validator.Validate(new List<string> { "fur", "furion", "百小僧" }, "data"));
        Assert.Equal("数据无效", exception3.Message);
    }

    [Fact]
    public void Validate_WithAllowEmpty_ReturnOK()
    {
        var validator = new HaveLengthValidator(2) { AllowEmpty = true };
        validator.Validate(new[] { "fur", "furion" }, "data");

        var exception = Assert.Throws<ValidationException>(() =>
            validator.Validate(new List<string> { "fur", "furion", "百小僧" }, "data"));
        Assert.Equal("The field data must be empty or have a length of exactly '2'.",
            exception.Message);

        validator.Validate(Array.Empty<string>(), "data");

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

        var validator2 = new HaveLengthValidator(2) { AllowEmpty = true };
        Assert.Equal("The field data must be empty or have a length of exactly '2'.",
            validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new HaveLengthValidator(2);
        Assert.Equal("HaveLengthValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new HaveLengthValidator(2) { AllowEmpty = true };
        Assert.Equal("HaveLengthValidator_ValidationError_AllowEmpty", validator2.GetResourceKey());
    }
}