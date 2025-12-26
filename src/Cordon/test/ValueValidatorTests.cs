// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValueValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Null(valueValidator._serviceProvider);
        Assert.Null(valueValidator._items);
        Assert.NotNull(valueValidator._ruleSetStack);
        Assert.Empty(valueValidator._ruleSetStack);
        Assert.NotNull(valueValidator.Validators);
        Assert.Null(valueValidator._lastAddedValidator);
        Assert.Null(valueValidator._valueValidator);
        Assert.Empty(valueValidator.Validators);
        Assert.Equal(0, valueValidator._highPriorityEndIndex);
        Assert.Null(valueValidator._preProcessor);
        Assert.Null(valueValidator.DisplayName);
        Assert.Null(valueValidator.WhenCondition);
        Assert.Null(valueValidator.UnlessCondition);
        Assert.NotNull(valueValidator.This);
        Assert.Equal(valueValidator.This, valueValidator);
        Assert.Null(valueValidator._allowEmptyStrings);

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
        Assert.True(valueValidator.IsValid("Fur"));
        Assert.True(valueValidator.IsValid("Furion"));
    }

    [Fact]
    public void IsValid_WithRuleSet_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddValidators(
            new RequiredValidator(), new MinLengthValidator(3)).RuleSet("new", v =>
        {
            v.Required().MinLength(5);
        });

        Assert.False(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("Fur"));
        Assert.True(valueValidator.IsValid("Furion"));

        Assert.False(valueValidator.IsValid(null, ["new"]));
        Assert.False(valueValidator.IsValid("Fu", ["new"]));
        Assert.False(valueValidator.IsValid("Fur", ["new"]));
        Assert.True(valueValidator.IsValid("Furion", ["new"]));
    }

    [Fact]
    public void IsValid_WithValueValidator_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>().Required().MinLength(3)
            .SetValidator(new StringValueValidator());

        Assert.False(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("Fu"));
        Assert.False(valueValidator.IsValid("Fur"));
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
        Assert.Equal(["The Object field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = valueValidator.GetValidationResults("Fu");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field Object must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(valueValidator.GetValidationResults("Fur"));
        Assert.Null(valueValidator.GetValidationResults("Furion"));
    }

    [Fact]
    public void GetValidationResults_WithRuleSet_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddValidators(
            new RequiredValidator(), new MinLengthValidator(3)).RuleSet("new", v =>
        {
            v.Required().MinLength(5);
        });

        var validationResults = valueValidator.GetValidationResults(null);
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The Object field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = valueValidator.GetValidationResults("Fu");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field Object must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(valueValidator.GetValidationResults("Fur"));
        Assert.Null(valueValidator.GetValidationResults("Furion"));

        var validationResults3 = valueValidator.GetValidationResults(null, ["new"]);
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal(["The Object field is required."],
            validationResults3.Select(u => u.ErrorMessage));

        var validationResults4 = valueValidator.GetValidationResults("Fu", ["new"]);
        Assert.NotNull(validationResults4);
        Assert.Single(validationResults4);
        Assert.Equal(["The field Object must be a string or array type with a minimum length of '5'."],
            validationResults4.Select(u => u.ErrorMessage));

        var validationResults5 = valueValidator.GetValidationResults("Fur", ["new"]);
        Assert.NotNull(validationResults5);
        Assert.Single(validationResults5);
        Assert.Equal(["The field Object must be a string or array type with a minimum length of '5'."],
            validationResults5.Select(u => u.ErrorMessage));

        Assert.Null(valueValidator.GetValidationResults("Furion", ["new"]));
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
    public void GetValidationResults_WithValueValidator_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>().Required().MinLength(3)
            .SetValidator(new StringValueValidator());

        var validationResults = valueValidator.GetValidationResults(null);
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The String field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = valueValidator.GetValidationResults("Fu");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field String must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        var validationResults3 = valueValidator.GetValidationResults("Fur");
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal(["The field String cannot be equal to 'Fur'."],
            validationResults3.Select(u => u.ErrorMessage));

        Assert.Null(valueValidator.GetValidationResults("Furion"));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddValidators(
            new RequiredValidator(), new MinLengthValidator(3));

        var exception = Assert.Throws<ValidationException>(() => valueValidator.Validate(null));
        Assert.Equal("The Object field is required.", exception.Message);
        Assert.Empty(exception.ValidationResult.MemberNames);

        var exception2 =
            Assert.Throws<ValidationException>(() => valueValidator.Validate("Fu"));
        Assert.Equal("The field Object must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Empty(exception2.ValidationResult.MemberNames);

        valueValidator.Validate("Fur");
        valueValidator.Validate("Furion");
    }

    [Fact]
    public void Validate_WithRuleSet_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddValidators(
            new RequiredValidator(), new MinLengthValidator(3)).RuleSet("new", v =>
        {
            v.Required().MinLength(5);
        });

        var exception = Assert.Throws<ValidationException>(() => valueValidator.Validate(null));
        Assert.Equal("The Object field is required.", exception.Message);
        Assert.Empty(exception.ValidationResult.MemberNames);

        var exception2 =
            Assert.Throws<ValidationException>(() => valueValidator.Validate("Fu"));
        Assert.Equal("The field Object must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Empty(exception2.ValidationResult.MemberNames);

        valueValidator.Validate("Fur");
        valueValidator.Validate("Furion");

        var exception3 = Assert.Throws<ValidationException>(() => valueValidator.Validate(null, ["new"]));
        Assert.Equal("The Object field is required.", exception3.Message);
        Assert.Empty(exception3.ValidationResult.MemberNames);

        var exception4 =
            Assert.Throws<ValidationException>(() => valueValidator.Validate("Fu", ["new"]));
        Assert.Equal("The field Object must be a string or array type with a minimum length of '5'.",
            exception4.Message);
        Assert.Empty(exception4.ValidationResult.MemberNames);

        var exception5 =
            Assert.Throws<ValidationException>(() => valueValidator.Validate("Fur", ["new"]));
        Assert.Equal("The field Object must be a string or array type with a minimum length of '5'.",
            exception5.Message);
        Assert.Empty(exception5.ValidationResult.MemberNames);

        valueValidator.Validate("Furion", ["new"]);
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
    public void Validate_WithValueValidator_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>().Required().MinLength(3)
            .SetValidator(new StringValueValidator());

        var exception = Assert.Throws<ValidationException>(() => valueValidator.Validate(null));
        Assert.Equal("The String field is required.", exception.Message);
        Assert.Empty(exception.ValidationResult.MemberNames);

        var exception2 =
            Assert.Throws<ValidationException>(() => valueValidator.Validate("Fu"));
        Assert.Equal("The field String must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Empty(exception2.ValidationResult.MemberNames);

        var exception3 =
            Assert.Throws<ValidationException>(() => valueValidator.Validate("Fur"));
        Assert.Equal("The field String cannot be equal to 'Fur'.", exception3.Message);
        Assert.Empty(exception3.ValidationResult.MemberNames);

        valueValidator.Validate("Furion");
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
    public void Rule_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        valueValidator.Rule().MinLength(10);
        Assert.Single(valueValidator.Validators);
    }

    [Fact]
    public void RuleSet_Invalid_Parameters()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Throws<ArgumentNullException>(() => valueValidator.RuleSet((string?)null, (Action)null!));
        Assert.Throws<ArgumentNullException>(() => valueValidator.RuleSet((string?[]?)null, (Action)null!));
        Assert.Throws<ArgumentNullException>(() => valueValidator.RuleSet("login", (Action)null!));
        Assert.Throws<ArgumentNullException>(() => valueValidator.RuleSet(["login"], (Action)null!));

        Assert.Throws<ArgumentNullException>(() =>
            valueValidator.RuleSet((string?)null, (Action<ValueValidator<string>>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            valueValidator.RuleSet((string?[]?)null, (Action<ValueValidator<string>>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            valueValidator.RuleSet("login", (Action<ValueValidator<string>>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            valueValidator.RuleSet(["login"], (Action<ValueValidator<string>>)null!));
    }

    [Fact]
    public void RuleSet_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();

        var ruleSets = new List<string?>();
        valueValidator.RuleSet([], () =>
        {
            if (valueValidator._ruleSetStack.Count > 0)
            {
                ruleSets.Add(valueValidator._ruleSetStack.Peek());
            }
        });
        Assert.Empty(ruleSets);
        Assert.Empty(valueValidator._ruleSetStack);

        ruleSets.Clear();
        valueValidator.RuleSet(["login"], () => ruleSets.Add(valueValidator._ruleSetStack.Peek()));
        Assert.Equal(["login"], ruleSets);
        Assert.Empty(valueValidator._ruleSetStack);

        ruleSets.Clear();
        valueValidator.RuleSet(["login", "register"], () => ruleSets.Add(valueValidator._ruleSetStack.Peek()));
        Assert.Equal(["login", "register"], ruleSets);
        Assert.Empty(valueValidator._ruleSetStack);

        ruleSets.Clear();
        valueValidator.RuleSet([" login ", " register "], () => ruleSets.Add(valueValidator._ruleSetStack.Peek()));
        Assert.Equal(["login", "register"], ruleSets);
        Assert.Empty(valueValidator._ruleSetStack);

        ruleSets.Clear();
        Assert.Throws<Exception>(() =>
        {
            valueValidator.RuleSet([" login ", " register "], () =>
            {
                if (ruleSets.Count == 1)
                {
                    throw new Exception("出错了");
                }

                ruleSets.Add(valueValidator._ruleSetStack.Peek());
            });
        });
        Assert.Equal(["login"], ruleSets);
        Assert.Empty(valueValidator._ruleSetStack);

        ruleSets.Clear();
        valueValidator.RuleSet("login", () => ruleSets.Add(valueValidator._ruleSetStack.Peek()));
        Assert.Equal(["login"], ruleSets);
        Assert.Empty(valueValidator._ruleSetStack);

        ruleSets.Clear();
        valueValidator.RuleSet("login,register", () => ruleSets.Add(valueValidator._ruleSetStack.Peek()));
        Assert.Equal(["login,register"], ruleSets);
        Assert.Empty(valueValidator._ruleSetStack);

        ruleSets.Clear();
        valueValidator.RuleSet("login;register", () => ruleSets.Add(valueValidator._ruleSetStack.Peek()));
        Assert.Equal(["login;register"], ruleSets);
        Assert.Empty(valueValidator._ruleSetStack);

        ruleSets.Clear();
        valueValidator.RuleSet(" login , register ", () => ruleSets.Add(valueValidator._ruleSetStack.Peek()));
        Assert.Equal(["login , register"], ruleSets);
        Assert.Empty(valueValidator._ruleSetStack);
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
    public void SetValidator_Invalid_Parameters()
    {
        var valueValidator = new ValueValidator<string>().SetValidator(new StringValueValidator());

        Assert.Throws<ArgumentNullException>(() =>
            valueValidator.SetValidator((Func<IDictionary<object, object?>?, ValueValidator<string>?>)null!));

        var exception =
            Assert.Throws<InvalidOperationException>(() => valueValidator.SetValidator(new StringValueValidator()));
        Assert.Equal(
            "An value validator has already been assigned to this value. Only one value validator is allowed per value.",
            exception.Message);
    }

    [Fact]
    public void SetValidator_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();

        Assert.Null(valueValidator._valueValidator);
        valueValidator.SetValidator(new StringValueValidator());
        Assert.NotNull(valueValidator._valueValidator);
        Assert.Throws<InvalidOperationException>(() =>
            valueValidator.SetValidator((ValueValidator<string>?)null));
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
    public void ShouldValidate_WithAllowEmptyStrings_ReturnOK()
    {
        var valueValidator = new ValueValidator<string?>();

        Assert.True(valueValidator.ShouldValidate(null));
        Assert.True(valueValidator.ShouldValidate(string.Empty));
        Assert.True(valueValidator.ShouldValidate("Furion"));

        valueValidator.AllowEmptyStrings();
        Assert.True(valueValidator.ShouldValidate(null));
        Assert.False(valueValidator.ShouldValidate(string.Empty));
        Assert.True(valueValidator.ShouldValidate("Furion"));
    }

    [Fact]
    public void GetDisplayName_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Equal("String", valueValidator.GetDisplayName());

        var valueValidator2 = new ValueValidator<string>().WithDisplayName("Field");
        Assert.Equal("Field", valueValidator2.GetDisplayName());
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var subValidator = new StringValueValidator();
        var valueValidator = new ValueValidator<string>();
        valueValidator.AddAnnotations(new RequiredAttribute())
            .Composite(new MinLengthValidator(3)).SetValidator(subValidator);

        Assert.Null(valueValidator._serviceProvider);
        var valueAnnotationValidator = valueValidator.Validators[0] as ValueAnnotationValidator;
        Assert.NotNull(valueAnnotationValidator);
        Assert.Null(valueAnnotationValidator._serviceProvider);
        Assert.Null(subValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        valueValidator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(valueValidator._serviceProvider);
        var valueAnnotationValidator2 = valueValidator.Validators[0] as ValueAnnotationValidator;
        Assert.NotNull(valueAnnotationValidator2);
        Assert.NotNull(valueAnnotationValidator2._serviceProvider);
        Assert.NotNull(subValidator._serviceProvider);

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

    [Fact]
    public void GetCurrentRuleSets_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Null(valueValidator.GetCurrentRuleSets());

        valueValidator._ruleSetStack.Push("rule");
        Assert.Equal(["rule"], (string[]?)valueValidator.GetCurrentRuleSets()!);
        valueValidator._ruleSetStack.Pop();
        Assert.Null(valueValidator.GetCurrentRuleSets());
    }

    [Fact]
    public void ResolveValidationRuleSets_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Null(valueValidator.ResolveValidationRuleSets(null));
        Assert.Equal(["login"], (string[]?)valueValidator.ResolveValidationRuleSets(["login"])!);

        var services = new ServiceCollection();
        services.AddScoped<IValidationDataContext, ValidationDataContext>();
        using var serviceProvider = services.BuildServiceProvider();
        var dataContext = serviceProvider.GetRequiredService<IValidationDataContext>();
        dataContext.SetValidationOptions(new ValidationOptionsMetadata(["login", "email"]));

        valueValidator.InitializeServiceProvider(serviceProvider.GetService);
        Assert.Equal(["login", "email"], (string[]?)valueValidator.ResolveValidationRuleSets(null)!);
        Assert.Equal(["login"], (string[]?)valueValidator.ResolveValidationRuleSets(["login"])!);
    }

    [Fact]
    public void ToResults_Invalid_Parameters()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Throws<ArgumentNullException>(() => valueValidator.ToResults(null!));
    }

    [Fact]
    public void ToResults_ReturnOK()
    {
        var validationContext = new ValidationContext("Fur");
        var valueValidator = new ValueValidator<string>().NotEqualTo("Fur");

        Assert.Equal(["The field String cannot be equal to 'Fur'."],
            valueValidator.ToResults(validationContext).Select(u => u.ErrorMessage!).ToArray());

        var validationContext2 = new ValidationContext(new object()) { DisplayName = "Name" };
        var valueValidator2 = new ValueValidator<string>().Required().NotEqualTo("Fur");

        Assert.Equal(["The Name field is required."],
            valueValidator2.ToResults(validationContext2).Select(u => u.ErrorMessage!).ToArray());
    }

    [Fact]
    public void AllowEmptyStrings_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>();
        Assert.Null(valueValidator._allowEmptyStrings);
        valueValidator.AllowEmptyStrings();
        Assert.True(valueValidator._allowEmptyStrings);
        valueValidator.AllowEmptyStrings(false);
        Assert.False(valueValidator._allowEmptyStrings);
    }

    public class StringValueValidator : AbstractValueValidator<string>
    {
        public StringValueValidator() => Rule().MaxLength(10).NotEqualTo("Fur");
    }
}