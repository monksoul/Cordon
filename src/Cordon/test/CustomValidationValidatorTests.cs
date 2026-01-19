// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class CustomValidationValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new CustomValidationValidator(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new CustomValidationValidator(typeof(CustomValidators), null!));
        Assert.Throws<ArgumentException>(() => new CustomValidationValidator(typeof(CustomValidators), string.Empty));
        Assert.Throws<ArgumentException>(() => new CustomValidationValidator(typeof(CustomValidators), " "));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue));
        Assert.Equal(typeof(CustomValidators), validator.ValidatorType);
        Assert.Equal("ValidateValue", validator.Method);
        Assert.Equal(["ErrorMessage", "ErrorMessageResourceType", "ErrorMessageResourceName"],
            validator._observedPropertyNames);

        Assert.NotNull(validator._validator);
        Assert.True(validator._validator.Attributes[0] is CustomValidationAttribute);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("{0} is not valid.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("f", false)]
    [InlineData("fu", false)]
    [InlineData("Furion", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue));
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("f", false)]
    [InlineData("fu", false)]
    [InlineData("Furion", true)]
    public void IsValid_SingleParameter_ReturnOK(object? value, bool result)
    {
        var validator =
            new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue2));
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue));
        Assert.Null(validator.GetValidationResults("Furion", "data"));

        var validationResults = validator.GetValidationResults("fu", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("不能小于或等于 3", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效"; // 除非明确返回 null，否则不支持修改
        var validationResults2 = validator.GetValidationResults("fu", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("不能小于或等于 3", validationResults2.First().ErrorMessage);

        var validator2 =
            new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue3));
        validator2.ErrorMessage = "数据无效";
        var validationResults3 = validator2.GetValidationResults("fu", "data");
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal("数据无效", validationResults3.First().ErrorMessage);

        var validator3 =
            new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue3));
        var validationResults4 = validator3.GetValidationResults("fu", "data");
        Assert.NotNull(validationResults4);
        Assert.Single(validationResults4);
        Assert.Equal("data is not valid.", validationResults4.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue));
        validator.Validate("Furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("fu", "data"));
        Assert.Equal("不能小于或等于 3",
            exception.Message);

        var validator2 =
            new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue3));
        validator2.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator2.Validate("fu", "data"));
        Assert.Equal("数据无效", exception2.Message);

        var validator3 =
            new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue3));
        var exception3 = Assert.Throws<ValidationException>(() => validator3.Validate("fu", "data"));
        Assert.Equal("data is not valid.", exception3.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue));
        Assert.Null(validator.FormatErrorMessage("data"));
    }

    [Fact]
    public void OnPropertyChanged_ReturnOK()
    {
        var validator = new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue));
        var attribute = validator._validator.Attributes[0] as CustomValidationAttribute;
        Assert.NotNull(attribute);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("ErrorMessage", "错误消息"));
        Assert.Equal("错误消息", attribute.ErrorMessage);

        validator.OnPropertyChanged(validator,
            new ValidationPropertyChangedEventArgs("ErrorMessageResourceType", typeof(TestValidationMessages)));
        Assert.Equal(typeof(TestValidationMessages), attribute.ErrorMessageResourceType);

        validator.OnPropertyChanged(validator,
            new ValidationPropertyChangedEventArgs("ErrorMessageResourceName", "TestValidator_ValidationError"));
        Assert.Equal("TestValidator_ValidationError", attribute.ErrorMessageResourceName);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new CustomValidationValidator(typeof(CustomValidators), nameof(CustomValidators.ValidateValue));
        var attribute = validator._validator.Attributes[0] as CustomValidationAttribute;
        Assert.NotNull(attribute);

        validator.Dispose();

        validator.ErrorMessage = "错误消息";
        Assert.Null(attribute.ErrorMessage);
    }

    public static class CustomValidators
    {
        public static ValidationResult? ValidateValue(object? value, ValidationContext context) =>
            value switch
            {
                null => ValidationResult.Success,
                string { Length: < 3 } => new ValidationResult("不能小于或等于 3"),
                _ => ValidationResult.Success
            };

        public static ValidationResult? ValidateValue2(object? value) =>
            value switch
            {
                null => ValidationResult.Success,
                string { Length: < 3 } => new ValidationResult("不能小于或等于 3"),
                _ => ValidationResult.Success
            };

        public static ValidationResult? ValidateValue3(object? value, ValidationContext context) =>
            value switch
            {
                null => ValidationResult.Success,
                string { Length: < 3 } => new ValidationResult(null),
                _ => ValidationResult.Success
            };
    }
}