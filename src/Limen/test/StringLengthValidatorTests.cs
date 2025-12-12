// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class StringLengthValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new StringLengthValidator(5);
        Assert.Equal(5, validator.MaximumLength);
        Assert.Equal(0, validator.MinimumLength);

        Assert.NotNull(validator._validator);
        Assert.True(validator._validator.Attributes[0] is StringLengthAttribute);
        Assert.Equal(["MinimumLength"], validator._observedPropertyNames);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a string with a maximum length of '{1}'.",
            validator._errorMessageResourceAccessor());

        var validator2 = new StringLengthValidator(5) { MinimumLength = 2 };
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a string with a maximum length of '{1}'.",
            validator2._errorMessageResourceAccessor());
    }

    [Fact]
    public void Set_MinimumLength_ReturnOK()
    {
        var validator = new StringLengthValidator(5);

        var i = 0;
        var propertyChangedEventMethod =
            typeof(StringLengthValidator).GetMethod("add_PropertyChanged",
                BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, eventArgs) =>
            {
                Assert.Equal("MinimumLength", eventArgs.PropertyName);
                Assert.Equal(2, eventArgs.PropertyValue!);
                i++;
            })
        ]);

        validator.MinimumLength = 2;
        Assert.Equal(1, i);
        Assert.Equal(2, (validator._validator.Attributes[0] as StringLengthAttribute)!.MinimumLength);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("1", true)]
    [InlineData("1234", true)]
    [InlineData("12345", true)]
    [InlineData("123456", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new StringLengthValidator(5);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("1", false)]
    [InlineData("1234", true)]
    [InlineData("12345", true)]
    [InlineData("123456", false)]
    public void IsValid_WithMinimumLength_ReturnOK(object? value, bool result)
    {
        var validator = new StringLengthValidator(5) { MinimumLength = 2 };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new StringLengthValidator(5);
        Assert.Null(validator.GetValidationResults("12345", "data"));

        var validationResults = validator.GetValidationResults("123456", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a string with a maximum length of '5'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("123456", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithMinimumLength_ReturnOK()
    {
        var validator = new StringLengthValidator(5) { MinimumLength = 2 };
        Assert.Null(validator.GetValidationResults("12345", "data"));

        var validationResults = validator.GetValidationResults("1", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a string with a minimum length of '2' and a maximum length of '5'.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("1", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new StringLengthValidator(5);
        validator.Validate("12345", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("123456", "data"));
        Assert.Equal("The field data must be a string with a maximum length of '5'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("123456", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithMinimumLength_ReturnOK()
    {
        var validator = new StringLengthValidator(5) { MinimumLength = 2 };
        validator.Validate("12345", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("123456", "data"));
        Assert.Equal("The field data must be a string with a minimum length of '2' and a maximum length of '5'.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("123456", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new StringLengthValidator(5);
        Assert.Equal("The field data must be a string with a maximum length of '5'.",
            validator.FormatErrorMessage("data"));

        var validator2 = new StringLengthValidator(5) { MinimumLength = 2 };
        Assert.Equal("The field data must be a string with a minimum length of '2' and a maximum length of '5'.",
            validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void OnPropertyChanged_ReturnOK()
    {
        var validator = new StringLengthValidator(5);
        var attribute = validator._validator.Attributes[0] as StringLengthAttribute;
        Assert.NotNull(attribute);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("MinimumLength", 3000));
        Assert.Equal(3000, attribute.MinimumLength);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new StringLengthValidator(5);
        var attribute = validator._validator.Attributes[0] as StringLengthAttribute;
        Assert.NotNull(attribute);

        validator.Dispose();

        validator.MinimumLength = 3000;
        Assert.Equal(0, attribute.MinimumLength);
    }
}