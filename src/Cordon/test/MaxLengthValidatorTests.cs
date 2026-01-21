// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class MaxLengthValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new MaxLengthValidator(5);
        Assert.Equal(5, validator.Length);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal(
            "The field {0} must be a string or array type with a maximum length of '{1}'.",
            validator._errorMessageResourceAccessor());

        Assert.Equal(-1, MaxLengthValidator.MaxAllowableLength);

        var validator2 = new MaxLengthValidator();
        Assert.Equal(-1, validator2.Length);
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        var validator = new MaxLengthValidator(5);
        var exception = Assert.Throws<InvalidCastException>(() => validator.IsValid(CompositeMode.All));
        Assert.Equal("The field of type Cordon.CompositeMode must be a string, array or ICollection type.",
            exception.Message);
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

    [Fact]
    public void EnsureLegalLengths_ReturnOK()
    {
        var validator = new MaxLengthValidator(0);
        var exception = Assert.Throws<InvalidOperationException>(() => validator.EnsureLegalLengths());
        Assert.Equal(
            "MaxLengthValidator must have a Length value that is greater than zero. Use MaxLength() without parameters to indicate that the string or array can have the maximum allowable length.",
            exception.Message);

        var validator2 = new MaxLengthValidator(-2);
        var exception2 = Assert.Throws<InvalidOperationException>(() => validator2.EnsureLegalLengths());
        Assert.Equal(
            "MaxLengthValidator must have a Length value that is greater than zero. Use MaxLength() without parameters to indicate that the string or array can have the maximum allowable length.",
            exception2.Message);

        var validator3 = new MaxLengthValidator(-1);
        validator3.EnsureLegalLengths();
    }
}