// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class RangeValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        Assert.Equal(10, validator.Minimum);
        Assert.Equal(20, validator.Maximum);
        Assert.False(validator.MinimumIsExclusive);
        Assert.False(validator.MaximumIsExclusive);
        Assert.Equal(typeof(int), validator.OperandType);
        Assert.False(validator.ParseLimitsInInvariantCulture);
        Assert.False(validator.ConvertValueInInvariantCulture);
        Assert.Null(validator.Conversion);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be between '{1}' and '{2}'.", validator._errorMessageResourceAccessor());

        var validator2 = new RangeValidator(11.1, 20.1);
        Assert.Equal(11.1, validator2.Minimum);
        Assert.Equal(20.1, validator2.Maximum);
        Assert.Equal(typeof(double), validator2.OperandType);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be between '{1}' and '{2}'.", validator2._errorMessageResourceAccessor());

        var validator3 = new RangeValidator(typeof(int), "10", "20");
        Assert.Equal("10", validator3.Minimum);
        Assert.Equal("20", validator3.Maximum);
        Assert.Equal(typeof(int), validator3.OperandType);
        Assert.NotNull(validator3._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be between '{1}' and '{2}'.", validator3._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(1, false)]
    [InlineData(10, true)]
    [InlineData(15, true)]
    [InlineData(20, true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new RangeValidator(10, 20);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(1, false)]
    [InlineData(10, false)]
    [InlineData(15, true)]
    [InlineData(20, true)]
    public void IsValid_WithMinimumIsExclusive_ReturnOK(object? value, bool result)
    {
        var validator = new RangeValidator(10, 20) { MinimumIsExclusive = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(1, false)]
    [InlineData(10, true)]
    [InlineData(15, true)]
    [InlineData(20, false)]
    public void IsValid_WithMaximumIsExclusive_ReturnOK(object? value, bool result)
    {
        var validator = new RangeValidator(10, 20) { MaximumIsExclusive = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(1, false)]
    [InlineData(10, false)]
    [InlineData(15, true)]
    [InlineData(20, false)]
    public void IsValid_WithMinimumIsExclusiveAndMaximumIsExclusive_ReturnOK(object? value, bool result)
    {
        var validator = new RangeValidator(10, 20) { MinimumIsExclusive = true, MaximumIsExclusive = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        Assert.Null(validator.GetValidationResults(15, "data"));

        var validationResults = validator.GetValidationResults(5, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be between '10' and '20'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(5, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithMinimumIsExclusive_ReturnOK()
    {
        var validator = new RangeValidator(10, 20) { MinimumIsExclusive = true };
        Assert.Null(validator.GetValidationResults(15, "data"));

        var validationResults = validator.GetValidationResults(10, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be between '10' exclusive and '20'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(10, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithMaximumIsExclusive_ReturnOK()
    {
        var validator = new RangeValidator(10, 20) { MaximumIsExclusive = true };
        Assert.Null(validator.GetValidationResults(15, "data"));

        var validationResults = validator.GetValidationResults(20, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be between '10' and '20' exclusive.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(20, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithMinimumIsExclusiveAndMaximumIsExclusive_ReturnOK()
    {
        var validator = new RangeValidator(10, 20) { MinimumIsExclusive = true, MaximumIsExclusive = true };
        Assert.Null(validator.GetValidationResults(15, "data"));

        var validationResults = validator.GetValidationResults(10, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be between '10' exclusive and '20' exclusive.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(20, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        validator.Validate(15, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(5, "data"));
        Assert.Equal("The field data must be between '10' and '20'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(5, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithMinimumIsExclusive_ReturnOK()
    {
        var validator = new RangeValidator(10, 20) { MinimumIsExclusive = true };
        validator.Validate(15, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(10, "data"));
        Assert.Equal("The field data must be between '10' exclusive and '20'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(10, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithMaximumIsExclusive_ReturnOK()
    {
        var validator = new RangeValidator(10, 20) { MaximumIsExclusive = true };
        validator.Validate(15, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(20, "data"));
        Assert.Equal("The field data must be between '10' and '20' exclusive.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(20, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithMinimumIsExclusiveAndMaximumIsExclusive_ReturnOK()
    {
        var validator = new RangeValidator(10, 20) { MinimumIsExclusive = true, MaximumIsExclusive = true };
        validator.Validate(15, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(10, "data"));
        Assert.Equal("The field data must be between '10' exclusive and '20' exclusive.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(20, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        Assert.Equal("The field data must be between '10' and '20'.", validator.FormatErrorMessage("data"));

        var validator2 = new RangeValidator(10, 20) { MinimumIsExclusive = true };
        Assert.Equal("The field data must be between '10' exclusive and '20'.", validator2.FormatErrorMessage("data"));

        var validator3 = new RangeValidator(10, 20) { MaximumIsExclusive = true };
        Assert.Equal("The field data must be between '10' and '20' exclusive.", validator3.FormatErrorMessage("data"));

        var validator4 = new RangeValidator(10, 20) { MinimumIsExclusive = true, MaximumIsExclusive = true };
        Assert.Equal("The field data must be between '10' exclusive and '20' exclusive.",
            validator4.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        Assert.Equal("RangeValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new RangeValidator(10, 20) { MinimumIsExclusive = true };
        Assert.Equal("RangeValidator_ValidationError_MinExclusive", validator2.GetResourceKey());

        var validator3 = new RangeValidator(10, 20) { MaximumIsExclusive = true };
        Assert.Equal("RangeValidator_ValidationError_MaxExclusive", validator3.GetResourceKey());

        var validator4 = new RangeValidator(10, 20) { MinimumIsExclusive = true, MaximumIsExclusive = true };
        Assert.Equal("RangeValidator_ValidationError_MinExclusive_MaxExclusive", validator4.GetResourceKey());
    }

    [Fact]
    public void Initialize_Invalid_Parameters()
    {
        var validator = new RangeValidator(10, 9);
        var exception = Assert.Throws<InvalidOperationException>(() => validator.Initialize(10, 9, _ => 15));
        Assert.Equal("The maximum value '9' must be greater than or equal to the minimum value '10'.",
            exception.Message);

        var validator2 = new RangeValidator(10, 10) { MinimumIsExclusive = true };
        var exception2 = Assert.Throws<InvalidOperationException>(() => validator2.Initialize(10, 10, _ => 15));
        Assert.Equal("Cannot use exclusive bounds when the maximum value is equal to the minimum value.",
            exception2.Message);

        var validator3 = new RangeValidator(10, 10) { MaximumIsExclusive = true };
        var exception3 = Assert.Throws<InvalidOperationException>(() => validator3.Initialize(10, 10, _ => 15));
        Assert.Equal("Cannot use exclusive bounds when the maximum value is equal to the minimum value.",
            exception3.Message);
    }

    [Fact]
    public void Initialize_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        Assert.Null(validator.Conversion);
        validator.Initialize(10, 20, _ => 15);
        Assert.Equal(10, validator.Minimum);
        Assert.Equal(20, validator.Maximum);
        Assert.NotNull(validator.Conversion);
    }

    [Fact]
    public void SetupConversion_Invalid_Parameters()
    {
        var validator = new RangeValidator(null!, null!, null!);
        var exception = Assert.Throws<InvalidOperationException>(() => validator.SetupConversion());
        Assert.Equal("The minimum and maximum values must be set.", exception.Message);

        var validator2 = new RangeValidator(null!, "10", "20");
        var exception2 = Assert.Throws<InvalidOperationException>(() => validator2.SetupConversion());
        Assert.Equal("The OperandType must be set when strings are used for minimum and maximum values.",
            exception2.Message);

        var validator3 = new RangeValidator(typeof(NoComparableClass), "10", "20");
        var exception3 = Assert.Throws<InvalidOperationException>(() => validator3.SetupConversion());
        Assert.Equal("The type Cordon.Tests.RangeValidatorTests+NoComparableClass must implement System.IComparable.",
            exception3.Message);
    }

    [Fact]
    public void SetupConversion_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        Assert.Null(validator.Conversion);
        validator.SetupConversion();
        Assert.NotNull(validator.Conversion);
        Assert.Equal(10, validator.Minimum);
        Assert.Equal(20, validator.Maximum);

        var validator2 = new RangeValidator(typeof(int), "10", "20");
        Assert.Null(validator2.Conversion);
        validator2.SetupConversion();
        Assert.NotNull(validator2.Conversion);
        Assert.Equal(10, validator2.Minimum);
        Assert.Equal(20, validator2.Maximum);
    }

    [Fact]
    public void GetOperandTypeConverter_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        Assert.NotNull(validator.GetOperandTypeConverter());
    }

    public class NoComparableClass;
}