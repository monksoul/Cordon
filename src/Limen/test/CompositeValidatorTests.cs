// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class CompositeValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new CompositeValidator(null!));

        Assert.Throws<ArgumentNullException>(() =>
            new CompositeValidator(new RequiredValidator(), null!, new ChineseValidator()));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new CompositeValidator();
        Assert.NotNull(validator);
        Assert.Equal(0, validator._highPriorityEndIndex);
        Assert.NotNull(validator._validators);
        Assert.Empty(validator._validators);

        var validator2 =
            new CompositeValidator(new ChineseValidator(), new RequiredValidator(), new NotNullValidator());

        Assert.NotNull(validator2._validators);
        Assert.Equal(3, validator2._validators.Count);
        Assert.Equal([typeof(NotNullValidator), typeof(RequiredValidator), typeof(ChineseValidator)],
            validator2._validators.Select(u => u.GetType()));
        Assert.NotNull(validator2.Validators);
        Assert.Equal(3, validator2.Validators.Count);
        Assert.Equal([typeof(NotNullValidator), typeof(RequiredValidator), typeof(ChineseValidator)],
            validator2.Validators.Select(u => u.GetType()));
        Assert.Equal(ValidationMode.ValidateAll, validator2.Mode);

        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Null(validator2._errorMessageResourceAccessor());
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator());
        Assert.True(validator.IsValid(null));

        var validator2 = new CompositeValidator(new ChineseValidator(), new RequiredValidator());
        Assert.False(validator2.IsValid(null));

        var validator3 = new CompositeValidator(new ChineseValidator(), new NotNullValidator());
        Assert.False(validator3.IsValid(null));

        var validator4 = new CompositeValidator(new ChineseValidator(), new NotNullValidator());
        Assert.True(validator4.IsValid("百小僧"));

        var validator5 = new CompositeValidator(new ChineseValidator(), new NotNullValidator());
        Assert.False(validator5.IsValid("Furion"));
    }

    [Fact]
    public void IsValid_WithMode_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator(), new ChineseNameValidator());
        Assert.False(validator.IsValid("凯文·杜兰特"));

        validator.Mode = ValidationMode.BreakOnFirstError;
        Assert.False(validator.IsValid("凯文·杜兰特"));

        validator.Mode = ValidationMode.BreakOnFirstSuccess;
        Assert.True(validator.IsValid("凯文·杜兰特"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator());
        Assert.Null(validator.GetValidationResults(null, "Value"));

        var validator2 = new CompositeValidator(new ChineseValidator(), new RequiredValidator());
        Assert.Null(validator2.GetValidationResults("百小僧", "Value"));
    }

    [Fact]
    public void GetValidationResults_WithNullValue_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator(), new RequiredValidator());
        var validationResults = validator.GetValidationResults(null, "Value");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The Value field is required.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "自定义错误信息";
        var validationResults2 = validator.GetValidationResults(null, "Value");
        Assert.NotNull(validationResults2);
        Assert.Equal(2, validationResults2.Count);
        Assert.Equal("自定义错误信息", validationResults2.First().ErrorMessage);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var validationResults3 = validator.GetValidationResults(null, "Value");
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal("单元测试Value错误信息2", validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithNoNullValue_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator(), new RequiredValidator());
        var validationResults = validator.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field Value contains invalid Chinese characters.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "自定义错误信息";
        var validationResults2 = validator.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults2);
        Assert.Equal(2, validationResults2.Count);
        Assert.Equal("自定义错误信息", validationResults2.First().ErrorMessage);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var validationResults3 = validator.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal("单元测试Value错误信息2", validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithMode_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator(), new ChineseNameValidator());
        Assert.Null(validator.GetValidationResults(null, "Value"));

        validator.Mode = ValidationMode.BreakOnFirstError;
        Assert.Null(validator.GetValidationResults(null, "Value"));

        validator.Mode = ValidationMode.BreakOnFirstSuccess;
        Assert.Null(validator.GetValidationResults(null, "Value"));

        var validator2 = new CompositeValidator(new ChineseValidator(), new ChineseNameValidator());
        var validationResults = validator2.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults);
        Assert.Equal(2, validationResults.Count);

        validator2.Mode = ValidationMode.BreakOnFirstError;
        validationResults = validator2.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);

        validator2.Mode = ValidationMode.BreakOnFirstSuccess;
        validationResults = validator2.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults);
        Assert.Equal(2, validationResults.Count);

        var validator3 = new CompositeValidator(new ChineseValidator(), new ChineseNameValidator());
        var validationResults2 = validator3.GetValidationResults("凯文·杜兰特", "Value");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);

        validator3.Mode = ValidationMode.BreakOnFirstError;
        validationResults2 = validator3.GetValidationResults("凯文·杜兰特", "Value");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);

        validator3.Mode = ValidationMode.BreakOnFirstSuccess;
        validationResults2 = validator3.GetValidationResults("凯文·杜兰特", "Value");
        Assert.Null(validationResults2);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator());
        validator.Validate(null, "Value");

        var validator2 = new CompositeValidator(new ChineseValidator(), new RequiredValidator());
        validator2.Validate("百小僧", "Value");
    }

    [Fact]
    public void Validate_WithNullValue_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator(), new RequiredValidator());
        var exception = Assert.Throws<ValidationException>(() => validator.Validate(null, "Value"));
        Assert.Equal("The Value field is required.", exception.Message);

        validator.ErrorMessage = "自定义错误信息";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(null, "Value"));
        Assert.Equal("自定义错误信息", exception2.Message);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var exception3 = Assert.Throws<ValidationException>(() => validator.Validate(null, "Value"));
        Assert.Equal("单元测试Value错误信息2", exception3.Message);
    }

    [Fact]
    public void Validate_WithNoNullValue_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator(), new RequiredValidator());
        var exception = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "Value"));
        Assert.Equal("The field Value contains invalid Chinese characters.", exception.Message);

        validator.ErrorMessage = "自定义错误信息";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "Value"));
        Assert.Equal("自定义错误信息", exception2.Message);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var exception3 = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "Value"));
        Assert.Equal("单元测试Value错误信息2", exception3.Message);
    }

    [Fact]
    public void Validate_WithMode_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator(), new ChineseNameValidator());
        validator.Validate(null, "Value");

        validator.Mode = ValidationMode.BreakOnFirstError;
        validator.Validate(null, "Value");

        validator.Mode = ValidationMode.BreakOnFirstSuccess;
        validator.Validate(null, "Value");

        var validator2 = new CompositeValidator(new ChineseValidator(), new ChineseNameValidator());
        Assert.Throws<ValidationException>(() => validator2.Validate("Furion", "Value"));

        validator2.Mode = ValidationMode.BreakOnFirstError;
        Assert.Throws<ValidationException>(() => validator2.Validate("Furion", "Value"));

        validator2.Mode = ValidationMode.BreakOnFirstSuccess;
        Assert.Throws<ValidationException>(() => validator2.Validate("Furion", "Value"));

        var validator3 = new CompositeValidator(new ChineseValidator(), new ChineseNameValidator());
        Assert.Throws<ValidationException>(() => validator3.Validate("凯文·杜兰特", "Value"));

        validator3.Mode = ValidationMode.BreakOnFirstError;
        Assert.Throws<ValidationException>(() => validator3.Validate("凯文·杜兰特", "Value"));

        validator3.Mode = ValidationMode.BreakOnFirstSuccess;
        validator3.Validate("凯文·杜兰特", "Value");
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new CompositeValidator(new ChineseValidator(), new RequiredValidator());
        Assert.Null(validator.FormatErrorMessage(null!));

        validator.ErrorMessage = "自定义错误信息";
        Assert.Equal("自定义错误信息", validator.FormatErrorMessage(null!));
    }

    [Fact]
    public void ThrowValidationException_Invalid_Parameters()
    {
        var validator = new CompositeValidator(new ChineseValidator(), new RequiredValidator());
        Assert.Throws<ArgumentNullException>(() => validator.ThrowValidationException(null, null!, null!));
    }

    [Fact]
    public void ThrowValidationException_ReturnOK()
    {
        var validator = new CompositeValidator(new RequiredValidator(), new ChineseValidator());

        var exception = Assert.Throws<ValidationException>(() =>
            validator.ThrowValidationException(null, "Value", validator.Validators.First()));
        Assert.Equal("The Value field is required.", exception.Message);

        var exception2 = Assert.Throws<ValidationException>(() =>
            validator.ThrowValidationException("Furion", "Value", validator.Validators.Last()));
        Assert.Equal("The field Value contains invalid Chinese characters.", exception2.Message);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var validator = new CompositeValidator(new ValueAnnotationValidator(new RequiredAttribute()));
        var valueAnnotationValidator = validator.Validators[0] as ValueAnnotationValidator;
        Assert.NotNull(valueAnnotationValidator);
        Assert.Null(valueAnnotationValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(valueAnnotationValidator._serviceProvider);
    }

    [Fact]
    public void AddValidator_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new CompositeValidator().AddValidator(null!));

    [Fact]
    public void AddValidator_ReturnOK()
    {
        using var validator = new CompositeValidator();

        validator.AddValidator(new AgeValidator());
        Assert.Single(validator.Validators);
        Assert.Equal(0, validator._highPriorityEndIndex);

        validator.AddValidator(new RequiredValidator());
        Assert.Equal(2, validator.Validators.Count);
        Assert.Equal(1, validator._highPriorityEndIndex);
        Assert.True(validator.Validators.First() is RequiredValidator);

        validator.AddValidator(new NotNullValidator());
        Assert.Equal(3, validator.Validators.Count);
        Assert.Equal(2, validator._highPriorityEndIndex);
        Assert.True(validator.Validators.First() is NotNullValidator);

        validator.AddValidator(new EmailAddressValidator());
        Assert.Equal(4, validator.Validators.Count);
        Assert.Equal(2, validator._highPriorityEndIndex);
        Assert.True(validator.Validators.Last() is EmailAddressValidator);

        var newNullValidator = new NotNullValidator();
        validator.AddValidator(newNullValidator);
        Assert.Equal(5, validator.Validators.Count);
        Assert.Equal(3, validator._highPriorityEndIndex);
        Assert.Equal(newNullValidator, validator.Validators[1]);
    }
}