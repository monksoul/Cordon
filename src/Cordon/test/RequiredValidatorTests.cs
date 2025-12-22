// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class RequiredValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new RequiredValidator();
        Assert.False(validator.AllowEmptyStrings);

        Assert.NotNull(validator._validator);
        Assert.True(validator._validator.Attributes[0] is RequiredAttribute);
        Assert.Equal(["AllowEmptyStrings"], validator._observedPropertyNames);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The {0} field is required.", validator._errorMessageResourceAccessor());

        Assert.True(typeof(IHighPriorityValidator).IsAssignableFrom(typeof(RequiredValidator)));
        Assert.Equal(10, validator.Priority);
    }

    [Fact]
    public void Set_AllowEmptyStrings_ReturnOK()
    {
        var validator = new RequiredValidator();

        var i = 0;
        var propertyChangedEventMethod =
            typeof(RequiredValidator).GetMethod("add_PropertyChanged",
                BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, eventArgs) =>
            {
                Assert.Equal("AllowEmptyStrings", eventArgs.PropertyName);
                Assert.True((bool)eventArgs.PropertyValue!);
                i++;
            })
        ]);

        validator.AllowEmptyStrings = true;
        Assert.Equal(1, i);
        Assert.True((validator._validator.Attributes[0] as RequiredAttribute)!.AllowEmptyStrings);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("Furion", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new RequiredValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", true)]
    [InlineData("Furion", true)]
    public void IsValid_WithAllowEmptyStrings_ReturnOK(object? value, bool result)
    {
        var validator = new RequiredValidator { AllowEmptyStrings = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new RequiredValidator();
        Assert.Null(validator.GetValidationResults("Furion", "data"));

        var validationResults = validator.GetValidationResults(null, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field is required.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(string.Empty, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithAllowEmptyStrings_ReturnOK()
    {
        var validator = new RequiredValidator { AllowEmptyStrings = true };
        Assert.Null(validator.GetValidationResults("Furion", "data"));
        Assert.Null(validator.GetValidationResults(string.Empty, "data"));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new RequiredValidator();
        validator.Validate("Furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(null, "data"));
        Assert.Equal("The data field is required.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(string.Empty, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithAllowEmptyStrings_ReturnOK()
    {
        var validator = new RequiredValidator { AllowEmptyStrings = true };
        validator.Validate("Furion", "data");
        validator.Validate(string.Empty, "data");
    }

    [Fact]
    public void OnPropertyChanged_ReturnOK()
    {
        var validator = new RequiredValidator();
        var attribute = validator._validator.Attributes[0] as RequiredAttribute;
        Assert.NotNull(attribute);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("AllowEmptyStrings", true));
        Assert.True(attribute.AllowEmptyStrings);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new RequiredValidator();
        var attribute = validator._validator.Attributes[0] as RequiredAttribute;
        Assert.NotNull(attribute);

        validator.Dispose();

        validator.AllowEmptyStrings = true;
        Assert.False(attribute.AllowEmptyStrings);
    }
}