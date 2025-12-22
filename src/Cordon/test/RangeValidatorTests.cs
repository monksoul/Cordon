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

        Assert.NotNull(validator._validator);
        Assert.True(validator._validator.Attributes[0] is RangeAttribute);
        Assert.Equal(
        [
            "MinimumIsExclusive", "MaximumIsExclusive", "ParseLimitsInInvariantCulture",
            "ConvertValueInInvariantCulture"
        ], validator._observedPropertyNames);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be between '{1}' and '{2}'.", validator._errorMessageResourceAccessor());

        var validator2 = new RangeValidator(11.1, 20.1);
        Assert.Equal(11.1, validator2.Minimum);
        Assert.Equal(20.1, validator2.Maximum);
        Assert.Equal(typeof(double), validator2.OperandType);
        Assert.NotNull(validator2._validator);
        Assert.True(validator2._validator.Attributes[0] is RangeAttribute);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be between '{1}' and '{2}'.", validator2._errorMessageResourceAccessor());

        var validator3 = new RangeValidator(typeof(int), "10", "20");
        Assert.Equal("10", validator3.Minimum);
        Assert.Equal("20", validator3.Maximum);
        Assert.Equal(typeof(int), validator3.OperandType);
        Assert.NotNull(validator3._validator);
        Assert.True(validator3._validator.Attributes[0] is RangeAttribute);
        Assert.NotNull(validator3._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be between '{1}' and '{2}'.", validator3._errorMessageResourceAccessor());
    }

    [Fact]
    public void Set_MinimumIsExclusive_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);

        var i = 0;
        var propertyChangedEventMethod =
            typeof(RangeValidator).GetMethod("add_PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, eventArgs) =>
            {
                Assert.Equal("MinimumIsExclusive", eventArgs.PropertyName);
                Assert.True((bool)eventArgs.PropertyValue!);
                i++;
            })
        ]);

        validator.MinimumIsExclusive = true;
        Assert.Equal(1, i);
        Assert.True((validator._validator.Attributes[0] as RangeAttribute)!.MinimumIsExclusive);
    }

    [Fact]
    public void Set_MaximumIsExclusive_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);

        var i = 0;
        var propertyChangedEventMethod =
            typeof(RangeValidator).GetMethod("add_PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, eventArgs) =>
            {
                Assert.Equal("MaximumIsExclusive", eventArgs.PropertyName);
                Assert.True((bool)eventArgs.PropertyValue!);
                i++;
            })
        ]);

        validator.MaximumIsExclusive = true;
        Assert.Equal(1, i);
        Assert.True((validator._validator.Attributes[0] as RangeAttribute)!.MaximumIsExclusive);
    }

    [Fact]
    public void Set_ParseLimitsInInvariantCulture_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);

        var i = 0;
        var propertyChangedEventMethod =
            typeof(RangeValidator).GetMethod("add_PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, eventArgs) =>
            {
                Assert.Equal("ParseLimitsInInvariantCulture", eventArgs.PropertyName);
                Assert.True((bool)eventArgs.PropertyValue!);
                i++;
            })
        ]);

        validator.ParseLimitsInInvariantCulture = true;
        Assert.Equal(1, i);
        Assert.True((validator._validator.Attributes[0] as RangeAttribute)!.ParseLimitsInInvariantCulture);
    }

    [Fact]
    public void Set_ConvertValueInInvariantCulture_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);

        var i = 0;
        var propertyChangedEventMethod =
            typeof(RangeValidator).GetMethod("add_PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, eventArgs) =>
            {
                Assert.Equal("ConvertValueInInvariantCulture", eventArgs.PropertyName);
                Assert.True((bool)eventArgs.PropertyValue!);
                i++;
            })
        ]);

        validator.ConvertValueInInvariantCulture = true;
        Assert.Equal(1, i);
        Assert.True((validator._validator.Attributes[0] as RangeAttribute)!.ConvertValueInInvariantCulture);
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
    public void OnPropertyChanged_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        var attribute = validator._validator.Attributes[0] as RangeAttribute;
        Assert.NotNull(attribute);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("Minimum", 15));
        Assert.Equal(10, attribute.Minimum);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("Maximum", 30));
        Assert.Equal(20, attribute.Maximum);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("OperandType", typeof(double)));
        Assert.Equal(typeof(int), attribute.OperandType);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("MinimumIsExclusive", true));
        Assert.True(attribute.MinimumIsExclusive);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("MaximumIsExclusive", true));
        Assert.True(attribute.MaximumIsExclusive);

        validator.OnPropertyChanged(validator,
            new ValidationPropertyChangedEventArgs("ParseLimitsInInvariantCulture", true));
        Assert.True(attribute.ParseLimitsInInvariantCulture);

        validator.OnPropertyChanged(validator,
            new ValidationPropertyChangedEventArgs("ConvertValueInInvariantCulture", true));
        Assert.True(attribute.ConvertValueInInvariantCulture);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new RangeValidator(10, 20);
        var attribute = validator._validator.Attributes[0] as RangeAttribute;
        Assert.NotNull(attribute);

        validator.Dispose();

        validator.MinimumIsExclusive = true;
        Assert.False(attribute.MinimumIsExclusive);
    }
}