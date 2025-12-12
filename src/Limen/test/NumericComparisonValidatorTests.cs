// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class NumericComparisonValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new TestNumericValidator(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new TestNumericValidator(10);
        Assert.Equal(10, validator.CompareValue);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be greater than or equal to '{1}'.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(0, false)]
    [InlineData(8, false)]
    [InlineData(9, false)]
    [InlineData(10, true)]
    [InlineData(11, true)]
    [InlineData(30, true)]
    [InlineData(10.1, true)]
    [InlineData(10.0, true)]
    [InlineData(11.1, true)]
    [InlineData(9.99, false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new TestNumericValidator(10);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new TestNumericValidator(10);
        Assert.Null(validator.GetValidationResults(30, "data"));

        var validationResults = validator.GetValidationResults(9, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be greater than or equal to '10'.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(9, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new TestNumericValidator(10);
        validator.Validate(30, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(9, "data"));
        Assert.Equal("The field data must be greater than or equal to '10'.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(9, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new TestNumericValidator(10);
        Assert.Equal("The field data must be greater than or equal to '10'.", validator.FormatErrorMessage("data"));
    }
}

public class TestNumericValidator : NumericComparisonValidator
{
    /// <inheritdoc />
    public TestNumericValidator(IComparable compareValue)
        : base(compareValue, () => "The field {0} must be greater than or equal to '{1}'.")
    {
    }

    /// <inheritdoc />
    protected override bool IsValid(decimal value, decimal compareValue) => value >= compareValue;
}