// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ValueValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Null(valueValidator._serviceProvider);
        Assert.Null(valueValidator._items);
        Assert.NotNull(valueValidator.Validators);
        Assert.Null(valueValidator._lastAddedValidator);
        Assert.Empty(valueValidator.Validators);
        Assert.Equal(0, valueValidator._highPriorityEndIndex);
        Assert.Null(valueValidator._preProcessor);
        Assert.Null(valueValidator.DisplayName);
        Assert.Null(valueValidator.WhenCondition);
        Assert.Null(valueValidator.UnlessCondition);
        Assert.NotNull(valueValidator.This);
        Assert.Equal(valueValidator.This, valueValidator);

        var valueValidator2 = new ValueValidator<string>(new Dictionary<object, object?>());
        Assert.Null(valueValidator2._serviceProvider);
        Assert.NotNull(valueValidator2._items);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var valueValidator3 = new ValueValidator<string>(serviceProvider, new Dictionary<object, object?>());
        Assert.NotNull(valueValidator3._serviceProvider);
        Assert.NotNull(valueValidator3._items);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddValidators(
            new RequiredValidator(), new MinLengthValidator(3));

        Assert.False(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("Furion"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddValidators(
            new RequiredValidator(), new MinLengthValidator(3));

        var validationResults = valueValidator.GetValidationResults(null);
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The Value field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = valueValidator.GetValidationResults("Fu");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field Value must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(valueValidator.GetValidationResults("Furion"));
    }

    [Fact]
    public void GetValidationResults_WithDisplayName_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddValidators(
            new RequiredValidator(), new MinLengthValidator(3)).WithDisplayName("MyFirstName");

        var validationResults = valueValidator.GetValidationResults(null);
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The MyFirstName field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = valueValidator.GetValidationResults("Fu");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field MyFirstName must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddValidators(
            new RequiredValidator(), new MinLengthValidator(3));

        var exception = Assert.Throws<ValidationException>(() => valueValidator.Validate(null));
        Assert.Equal("The Value field is required.", exception.Message);
        Assert.Empty(exception.ValidationResult.MemberNames);

        var exception2 =
            Assert.Throws<ValidationException>(() => valueValidator.Validate("Fu"));
        Assert.Equal("The field Value must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Empty(exception2.ValidationResult.MemberNames);

        valueValidator.Validate("Furion");
    }

    [Fact]
    public void Validate_WithDisplayName_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddValidators(
            new RequiredValidator(), new MinLengthValidator(3)).WithDisplayName("MyFirstName");

        var exception = Assert.Throws<ValidationException>(() => valueValidator.Validate(null));
        Assert.Equal("The MyFirstName field is required.", exception.Message);
        Assert.Empty(exception.ValidationResult.MemberNames);

        var exception2 =
            Assert.Throws<ValidationException>(() => valueValidator.Validate("Fu"));
        Assert.Equal("The field MyFirstName must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Empty(exception2.ValidationResult.MemberNames);
    }

    [Fact]
    public void AddValidator_Invalid_Parameters()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Throws<ArgumentNullException>(() => valueValidator.AddValidator<ValidatorBase>(null!));
    }

    [Fact]
    public void AddValidator_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();

        valueValidator.AddValidator(new AgeValidator());
        Assert.Single(valueValidator.Validators);
        Assert.Equal(0, valueValidator._highPriorityEndIndex);
        Assert.NotNull(valueValidator._lastAddedValidator);
        Assert.True(valueValidator._lastAddedValidator is AgeValidator);

        valueValidator.AddValidator(new RequiredValidator());
        Assert.Equal(2, valueValidator.Validators.Count);
        Assert.Equal(1, valueValidator._highPriorityEndIndex);
        Assert.True(valueValidator.Validators.First() is RequiredValidator);
        Assert.NotNull(valueValidator._lastAddedValidator);
        Assert.True(valueValidator._lastAddedValidator is RequiredValidator);

        valueValidator.AddValidator(new NotNullValidator());
        Assert.Equal(3, valueValidator.Validators.Count);
        Assert.Equal(2, valueValidator._highPriorityEndIndex);
        Assert.True(valueValidator.Validators.First() is NotNullValidator);
        Assert.NotNull(valueValidator._lastAddedValidator);
        Assert.True(valueValidator._lastAddedValidator is NotNullValidator);

        valueValidator.AddValidator(new EmailAddressValidator());
        Assert.Equal(4, valueValidator.Validators.Count);
        Assert.Equal(2, valueValidator._highPriorityEndIndex);
        Assert.True(valueValidator.Validators.Last() is EmailAddressValidator);
        Assert.NotNull(valueValidator._lastAddedValidator);
        Assert.True(valueValidator._lastAddedValidator is EmailAddressValidator);

        var newNullValidator = new NotNullValidator();
        valueValidator.AddValidator(newNullValidator);
        Assert.Equal(5, valueValidator.Validators.Count);
        Assert.Equal(3, valueValidator._highPriorityEndIndex);
        Assert.Equal(newNullValidator, valueValidator.Validators[1]);
        Assert.NotNull(valueValidator._lastAddedValidator);
        Assert.True(valueValidator._lastAddedValidator is NotNullValidator);
    }

    [Fact]
    public void AddValidator_WithConfigure_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();

        valueValidator.AddValidator(new AgeValidator(), v =>
        {
            v.IsAdultOnly = true;
            v.AllowStringValues = true;
        });

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as AgeValidator;
        Assert.NotNull(addedValidator);
        Assert.True(addedValidator.IsAdultOnly);
        Assert.True(addedValidator.AllowStringValues);
    }

    [Fact]
    public void When_Invalid_Parameters()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Throws<ArgumentNullException>(() => valueValidator.When(null!));
    }

    [Fact]
    public void When_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        valueValidator.When(u => u != "Fur");
        Assert.NotNull(valueValidator.WhenCondition);
        Assert.False(valueValidator.WhenCondition("Fur"));
        Assert.True(valueValidator.WhenCondition("Furion"));
    }

    [Fact]
    public void Unless_Invalid_Parameters()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Throws<ArgumentNullException>(() => valueValidator.Unless(null!));
    }

    [Fact]
    public void Unless_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        valueValidator.Unless(u => u != "Fur");
        Assert.NotNull(valueValidator.UnlessCondition);
        Assert.False(valueValidator.UnlessCondition("Fur"));
        Assert.True(valueValidator.UnlessCondition("Furion"));
    }

    [Fact]
    public void PreProcess_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();

        valueValidator.PreProcess(p => p.Trim());
        Assert.NotNull(valueValidator._preProcessor);

        valueValidator.PreProcess(null);
        Assert.Null(valueValidator._preProcessor);
    }

    [Fact]
    public void ShouldValidate_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();

        Assert.True(valueValidator.ShouldValidate(null));
        Assert.True(valueValidator.ShouldValidate("Furion"));
        Assert.True(valueValidator.ShouldValidate("Fur"));
        Assert.True(valueValidator.ShouldValidate("百小僧"));
        Assert.True(valueValidator.ShouldValidate("百签"));
    }

    [Fact]
    public void ShouldValidate_WithCondition_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>()
            .When(u => u != "Fur").Unless(u => u == "新生帝");

        Assert.True(valueValidator.ShouldValidate(null));
        Assert.False(valueValidator.ShouldValidate("Fur"));
        Assert.True(valueValidator.ShouldValidate("Furion"));
        Assert.False(valueValidator.ShouldValidate("新生帝"));
    }

    [Fact]
    public void GetDisplayName_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Equal("Value", valueValidator.GetDisplayName());

        var valueValidator2 = new ValueValidator<string>().WithDisplayName("Field");
        Assert.Equal("Field", valueValidator2.GetDisplayName());
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        valueValidator.AddAnnotations(new RequiredAttribute())
            .Composite(new MinLengthValidator(3));

        Assert.Null(valueValidator._serviceProvider);
        var valueAnnotationValidator = valueValidator.Validators[0] as ValueAnnotationValidator;
        Assert.NotNull(valueAnnotationValidator);
        Assert.Null(valueAnnotationValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        valueValidator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(valueValidator._serviceProvider);
        var valueAnnotationValidator2 = valueValidator.Validators[0] as ValueAnnotationValidator;
        Assert.NotNull(valueAnnotationValidator2);
        Assert.NotNull(valueAnnotationValidator2._serviceProvider);

        valueValidator.AddAnnotations(new UserNameAttribute());
        var valueAnnotationValidator3 = valueValidator.Validators[2] as ValueAnnotationValidator;
        Assert.NotNull(valueAnnotationValidator3);
        Assert.NotNull(valueAnnotationValidator3._serviceProvider);
    }

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        var validationContext = valueValidator.CreateValidationContext("Furion");

        Assert.Null(valueValidator._serviceProvider);
        Assert.Null(validationContext._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        valueValidator.InitializeServiceProvider(serviceProvider.GetService);
        Assert.NotNull(valueValidator._serviceProvider);

        var validationContext2 = valueValidator.CreateValidationContext("百小僧");
        Assert.NotNull(valueValidator._serviceProvider);
        Assert.NotNull(validationContext2._serviceProvider);
    }

    [Fact]
    public void GetValueForValidation_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();

        Assert.Equal(" Furion ", valueValidator.GetValueForValidation(" Furion "));

        valueValidator.PreProcess(u => u.Trim());
        Assert.Equal("Furion", valueValidator.GetValueForValidation(" Furion "));
    }
}