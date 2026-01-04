// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ObjectValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IObjectValidator<ObjectModel>).IsAssignableFrom(typeof(ObjectValidator<ObjectModel>)));

        using var validator = new ObjectValidator<ObjectModel>();
        Assert.NotNull(validator.Options);
        Assert.False(validator.Options.SuppressAnnotationValidation);
        Assert.NotNull(validator.Validators);
        Assert.Null(validator._serviceProvider);
        Assert.Empty(validator.Items);
        Assert.Null(validator.InheritedRuleSets);
        Assert.Empty(validator.Validators);
        Assert.NotNull(validator._annotationValidator);
        Assert.True(validator._annotationValidator.ValidateAllProperties);
        Assert.Null(validator._annotationValidator._serviceProvider);
        Assert.NotNull(validator._ruleSetStack);
        Assert.Empty(validator._ruleSetStack);
        Assert.Null(validator.WhenCondition);
        Assert.Null(validator.MemberPath);
        Assert.NotNull(ObjectValidator<Tests.ObjectModel>.ValidationContextsKey);

        using var validator2 = new ObjectValidator<ObjectModel>(new Dictionary<object, object?>());
        Assert.NotNull(validator2.Options);
        Assert.NotNull(validator2.Validators);
        Assert.Null(validator2._serviceProvider);
        Assert.NotNull(validator2.Items);
        Assert.Empty(validator2.Items);
        Assert.Empty(validator2.Validators);
        Assert.NotNull(validator2._annotationValidator);
        Assert.True(validator2._annotationValidator.ValidateAllProperties);
        Assert.Null(validator2._annotationValidator._serviceProvider);
        Assert.NotNull(validator2._ruleSetStack);
        Assert.Empty(validator2._ruleSetStack);
        Assert.Null(validator2.WhenCondition);

        validator2.Options.SuppressAnnotationValidation = false;
        Assert.False(validator2.Options.SuppressAnnotationValidation);

        validator2.Options.ValidateAllProperties = false;
        Assert.False(validator2._annotationValidator.ValidateAllProperties);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        using var validator3 = new ObjectValidator<ObjectModel>(serviceProvider, new Dictionary<object, object?>());
        Assert.NotNull(validator3._serviceProvider);
        Assert.NotNull(validator3._annotationValidator._serviceProvider);
        Assert.NotNull(validator3.Items);
        Assert.NotNull(validator3._annotationValidator.Items);

        using var validator4 =
            new ObjectValidator<ObjectModel>(new Dictionary<object, object?>());
        Assert.NotNull(validator4.Items);
        Assert.NotNull(validator4._annotationValidator.Items);
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.IsValid(null!));
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>().SkipAnnotationValidation()
            .RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(3)).Then();

        Assert.False(validator.IsValid(new ObjectModel()));
        Assert.False(validator.IsValid(new ObjectModel { FirstName = "Fu" }));
        Assert.True(validator.IsValid(new ObjectModel { FirstName = "Furion" }));
        Assert.True(validator.IsValid(new ObjectModel { FirstName = "Furion.NET" }));
    }

    [Fact]
    public void IsValid_WithObjectValidator_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>().SkipAnnotationValidation()
            .RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(3)).Then()
            .SetValidator(new ObjectModelValidator().SkipAnnotationValidation());

        Assert.False(validator.IsValid(new ObjectModel()));
        Assert.False(validator.IsValid(new ObjectModel { FirstName = "Fu" }));
        Assert.True(validator.IsValid(new ObjectModel { FirstName = "Furion" }));
        Assert.False(validator.IsValid(new ObjectModel { FirstName = "Furion.NET" }));
    }

    [Fact]
    public void IsValid_WithRuleSet_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>().SkipAnnotationValidation()
            .RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(2))
            .RuleSet("login", chain =>
            {
                chain.RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(3));
            });

        Assert.False(validator.IsValid(new ObjectModel()));
        Assert.True(validator.IsValid(new ObjectModel { FirstName = "Fu" }));
        Assert.True(validator.IsValid(new ObjectModel { FirstName = "Furion" }));

        Assert.False(validator.IsValid(new ObjectModel(), ["login"]));
        Assert.False(validator.IsValid(new ObjectModel { FirstName = "Fu" }, ["login"]));
        Assert.True(validator.IsValid(new ObjectModel { FirstName = "Furion" }, ["login"]));
    }

    [Fact]
    public void IsValid_WithSuppressAnnotationValidation_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        Assert.False(validator.IsValid(new ObjectModel()));
        Assert.False(validator.IsValid(new ObjectModel { Id = 1 }));
        Assert.True(validator.IsValid(new ObjectModel { Id = 1, Name = "Furion" }));
        Assert.True(validator.IsValid(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省中山市" }));
        Assert.False(validator.IsValid(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省" }));

        validator.SkipAnnotationValidation();

        Assert.True(validator.IsValid(new ObjectModel()));
        Assert.True(validator.IsValid(new ObjectModel { Id = 1 }));
        Assert.True(validator.IsValid(new ObjectModel { Id = 1, Name = "Furion" }));
        Assert.True(validator.IsValid(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省中山市" }));
        Assert.True(validator.IsValid(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省" }));
    }

    [Fact]
    public void GetValidationResults_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.GetValidationResults(null!));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>().SkipAnnotationValidation()
            .RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(3)).Then();

        var validationResults = validator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The FirstName field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = validator.GetValidationResults(new ObjectModel { FirstName = "Fu" });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field FirstName must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(validator.GetValidationResults(new ObjectModel { FirstName = "Furion" }));
        Assert.Null(validator.GetValidationResults(new ObjectModel { FirstName = "Furion.NET" }));
    }

    [Fact]
    public void GetValidationResults_WithObjectValidator_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>().SkipAnnotationValidation()
            .RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(3)).Then()
            .SetValidator(new ObjectModelValidator().SkipAnnotationValidation());

        var validationResults = validator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The FirstName field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = validator.GetValidationResults(new ObjectModel { FirstName = "Fu" });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field FirstName must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(validator.GetValidationResults(new ObjectModel { FirstName = "Furion" }));

        var validationResults3 = validator.GetValidationResults(new ObjectModel { FirstName = "Furion.NET" });
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal(["The field FirstName must be a string or array type with a maximum length of '8'."],
            validationResults3.Select(u => u.ErrorMessage));
    }

    [Fact]
    public void GetValidationResults_WithRuleSet_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>().SkipAnnotationValidation()
            .RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(2))
            .RuleSet("login", chain =>
            {
                chain.RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(3));
            });

        var validationResults = validator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The FirstName field is required."],
            validationResults.Select(u => u.ErrorMessage));

        Assert.Null(validator.GetValidationResults(new ObjectModel { FirstName = "Fu" }));
        Assert.Null(validator.GetValidationResults(new ObjectModel { FirstName = "Furion" }));

        var validationResults2 = validator.GetValidationResults(new ObjectModel(), ["login"]);
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The FirstName field is required."],
            validationResults2.Select(u => u.ErrorMessage));

        var validationResults3 = validator.GetValidationResults(new ObjectModel { FirstName = "Fu" }, ["login"]);
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal(["The field FirstName must be a string or array type with a minimum length of '3'."],
            validationResults3.Select(u => u.ErrorMessage));

        Assert.Null(validator.GetValidationResults(new ObjectModel { FirstName = "Furion" }, ["login"]));
    }

    [Fact]
    public void GetValidationResults_WithSuppressAnnotationValidation_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        var validationResults = validator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Equal(2, validationResults.Count);
        Assert.Equal(["The field Id must be between 1 and 2147483647.", "The Name field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = validator.GetValidationResults(new ObjectModel { Id = 1 });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The Name field is required."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(validator.GetValidationResults(new ObjectModel { Id = 1, Name = "Furion" }));
        Assert.Null(validator.GetValidationResults(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省中山市" }));

        var validationResults3 =
            validator.GetValidationResults(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省" });
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal(["The field Address must be a string or array type with a minimum length of '5'."],
            validationResults3.Select(u => u.ErrorMessage));

        validator.SkipAnnotationValidation();

        Assert.Null(validator.GetValidationResults(new ObjectModel()));
        Assert.Null(validator.GetValidationResults(new ObjectModel { Id = 1 }));
        Assert.Null(validator.GetValidationResults(new ObjectModel { Id = 1, Name = "Furion" }));
        Assert.Null(validator.GetValidationResults(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省中山市" }));
        Assert.Null(validator.GetValidationResults(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省" }));
    }

    [Fact]
    public void Validate_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null!));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>().SkipAnnotationValidation()
            .RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(3)).Then();

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(new ObjectModel()));
        Assert.Equal("The FirstName field is required.", exception.Message);
        Assert.Equal("FirstName", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() => validator.Validate(new ObjectModel { FirstName = "Fu" }));
        Assert.Equal("The field FirstName must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Equal("FirstName", exception2.ValidationResult.MemberNames.First());

        validator.Validate(new ObjectModel { FirstName = "Furion" });
        validator.Validate(new ObjectModel { FirstName = "Furion.NET" });
    }

    [Fact]
    public void Validate_WithObjectValidator_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>().SkipAnnotationValidation()
            .RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(3)).Then()
            .SetValidator(new ObjectModelValidator().SkipAnnotationValidation());

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(new ObjectModel()));
        Assert.Equal("The FirstName field is required.", exception.Message);
        Assert.Equal("FirstName", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() => validator.Validate(new ObjectModel { FirstName = "Fu" }));
        Assert.Equal("The field FirstName must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Equal("FirstName", exception2.ValidationResult.MemberNames.First());

        validator.Validate(new ObjectModel { FirstName = "Furion" });

        var exception3 =
            Assert.Throws<ValidationException>(() => validator.Validate(new ObjectModel { FirstName = "Furion.NET" }));
        Assert.Equal("The field FirstName must be a string or array type with a maximum length of '8'.",
            exception3.Message);
        Assert.Equal("FirstName", exception3.ValidationResult.MemberNames.First());
    }

    [Fact]
    public void Validate_WithRuleSet_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>().SkipAnnotationValidation()
            .RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(2))
            .RuleSet("login", chain =>
            {
                chain.RuleFor(u => u.FirstName).AddValidators(new RequiredValidator(), new MinLengthValidator(3));
            });

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(new ObjectModel()));
        Assert.Equal("The FirstName field is required.", exception.Message);
        Assert.Equal("FirstName", exception.ValidationResult.MemberNames.First());

        validator.Validate(new ObjectModel { FirstName = "Fu" });
        validator.Validate(new ObjectModel { FirstName = "Furion" });

        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(new ObjectModel(), ["login"]));
        Assert.Equal("The FirstName field is required.", exception2.Message);
        Assert.Equal("FirstName", exception2.ValidationResult.MemberNames.First());

        var exception3 =
            Assert.Throws<ValidationException>(() =>
                validator.Validate(new ObjectModel { FirstName = "Fu" }, ["login"]));
        Assert.Equal("The field FirstName must be a string or array type with a minimum length of '3'.",
            exception3.Message);
        Assert.Equal("FirstName", exception3.ValidationResult.MemberNames.First());

        validator.Validate(new ObjectModel { FirstName = "Furion" }, ["login"]);
    }

    [Fact]
    public void Validate_WithSuppressAnnotationValidation_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(new ObjectModel()));
        Assert.Equal("The field Id must be between 1 and 2147483647.", exception.Message);
        Assert.Equal("Id", exception.ValidationResult.MemberNames.First());

        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(new ObjectModel { Id = 1 }));
        Assert.Equal("The Name field is required.", exception2.Message);
        Assert.Equal("Name", exception2.ValidationResult.MemberNames.First());

        validator.Validate(new ObjectModel { Id = 1, Name = "Furion" });
        validator.Validate(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省中山市" });

        var exception3 = Assert.Throws<ValidationException>(() =>
            validator.Validate(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省" }));
        Assert.Equal("The field Address must be a string or array type with a minimum length of '5'.",
            exception3.Message);
        Assert.Equal("Address", exception3.ValidationResult.MemberNames.First());

        validator.SkipAnnotationValidation();

        validator.Validate(new ObjectModel());
        validator.Validate(new ObjectModel { Id = 1 });
        validator.Validate(new ObjectModel { Id = 1, Name = "Furion" });
        validator.Validate(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省中山市" });
        validator.Validate(new ObjectModel { Id = 1, Name = "Furion", Address = "广东省" });
    }

    [Fact]
    public void When_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.When((Func<ObjectModel, bool>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            validator.When((Func<ObjectModel, ValidationContext<ObjectModel>, bool>)null!));
    }

    [Fact]
    public void When_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        validator.When(u => u.Name is not null);
        Assert.NotNull(validator.WhenCondition);

        var model = new ObjectModel();
        var validationContext = new ValidationContext<ObjectModel>(model);

        Assert.False(validator.WhenCondition(new ObjectModel(), validationContext));
        Assert.True(validator.WhenCondition(new ObjectModel { Name = "Furion" }, validationContext));
    }

    [Fact]
    public void RuleFor_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.RuleFor<string>(null!));
    }

    [Fact]
    public void RuleFor_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        validator.RuleFor(u => u.Address);
        Assert.Single(validator.Validators);
        var propertyValidator = validator.Validators.LastOrDefault() as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(propertyValidator);
        Assert.Null(propertyValidator.RuleSets);
    }

    [Fact]
    public void RuleFor_InRuleSet_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();

        validator.RuleSet([], () =>
        {
            validator.RuleFor(u => u.Address);
        });
        Assert.Single(validator.Validators);
        var propertyValidator = validator.Validators.LastOrDefault() as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(propertyValidator);
        Assert.Null(propertyValidator.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator.RuleSets);

        validator.Validators.Clear();
        validator.RuleSet(["login", "register"], () =>
        {
            validator.RuleFor(u => u.Address);
        });
        Assert.Equal(2, validator.Validators.Count);

        var propertyValidator2 =
            validator.Validators[0] as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(propertyValidator2);
        Assert.NotNull(propertyValidator2.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator2.RuleSets);

        var propertyValidator3 =
            validator.Validators[1] as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(propertyValidator3);
        Assert.NotNull(propertyValidator3.RuleSets);
        Assert.NotStrictEqual(["register"], propertyValidator3.RuleSets);

        validator.Validators.Clear();
        validator.RuleSet(["login", "register"], chain =>
        {
            chain.RuleFor(u => u.Address);
        });
        Assert.Equal(2, validator.Validators.Count);

        var propertyValidator4 =
            validator.Validators[0] as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(propertyValidator4);
        Assert.NotNull(propertyValidator4.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator4.RuleSets);

        var propertyValidator5 =
            validator.Validators[1] as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(propertyValidator5);
        Assert.NotNull(propertyValidator5.RuleSets);
        Assert.NotStrictEqual(["register"], propertyValidator5.RuleSets);

        validator.Validators.Clear();
        validator.RuleSet("login", () =>
        {
            validator.RuleFor(x => x.Name);
            validator.RuleSet("register", () =>
            {
                validator.RuleFor(x => x.FirstName);
            });
        });
        Assert.Equal(2, validator.Validators.Count);
    }

    [Fact]
    public void RuleFor_InitializeServiceProvider_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();
        var propertyValidator = validator.RuleFor(u => u.Name);

        Assert.Null(propertyValidator._serviceProvider);
        Assert.Null(propertyValidator._annotationValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(propertyValidator._serviceProvider);
        Assert.NotNull(propertyValidator._annotationValidator._serviceProvider);

        var propertyValidator2 = validator.RuleFor(u => u.Id);
        Assert.NotNull(propertyValidator2._serviceProvider);
        Assert.NotNull(propertyValidator2._annotationValidator._serviceProvider);
    }

    [Fact]
    public void RuleForCollection_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.RuleForCollection<Child>(null!));
    }

    [Fact]
    public void RuleForCollection_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        validator.RuleForCollection(u => u.Children);
        Assert.Single(validator.Validators);
        var propertyValidator =
            validator.Validators.LastOrDefault() as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator);
        Assert.Null(propertyValidator.RuleSets);
    }

    [Fact]
    public void RuleForCollection_InRuleSet_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();

        validator.RuleSet([], () =>
        {
            validator.RuleForCollection(u => u.Children);
        });
        Assert.Single(validator.Validators);
        var propertyValidator =
            validator.Validators.LastOrDefault() as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator);
        Assert.Null(propertyValidator.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator.RuleSets);

        validator.Validators.Clear();
        validator.RuleSet(["login", "register"], () =>
        {
            validator.RuleForCollection(u => u.Children);
        });
        Assert.Equal(2, validator.Validators.Count);

        var propertyValidator2 =
            validator.Validators[0] as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator2);
        Assert.NotNull(propertyValidator2.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator2.RuleSets);

        var propertyValidator3 =
            validator.Validators[1] as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator3);
        Assert.NotNull(propertyValidator3.RuleSets);
        Assert.NotStrictEqual(["register"], propertyValidator3.RuleSets);

        validator.Validators.Clear();
        validator.RuleSet(["login", "register"], chain =>
        {
            chain.RuleForCollection(u => u.Children);
        });
        Assert.Equal(2, validator.Validators.Count);

        var propertyValidator4 =
            validator.Validators[0] as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator4);
        Assert.NotNull(propertyValidator4.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator4.RuleSets);

        var propertyValidator5 =
            validator.Validators[1] as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator5);
        Assert.NotNull(propertyValidator5.RuleSets);
        Assert.NotStrictEqual(["register"], propertyValidator5.RuleSets);

        validator.Validators.Clear();
        validator.RuleSet("login", () =>
        {
            validator.RuleForCollection(x => x.Children);
            validator.RuleSet("register", () =>
            {
                validator.RuleForCollection(x => x.Children);
            });
        });
        Assert.Equal(2, validator.Validators.Count);
    }

    [Fact]
    public void RuleForCollection_InitializeServiceProvider_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();
        var propertyValidator = validator.RuleForCollection(u => u.Children);

        Assert.Null(propertyValidator._serviceProvider);
        Assert.Null(propertyValidator._annotationValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(propertyValidator._serviceProvider);
        Assert.NotNull(propertyValidator._annotationValidator._serviceProvider);

        var propertyValidator2 = validator.RuleForCollection(u => u.Children);
        Assert.NotNull(propertyValidator2._serviceProvider);
        Assert.NotNull(propertyValidator2._annotationValidator._serviceProvider);
    }

    [Fact]
    public void RuleSet_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.RuleSet((string?)null, (Action)null!));
        Assert.Throws<ArgumentNullException>(() => validator.RuleSet((string?[]?)null, (Action)null!));
        Assert.Throws<ArgumentNullException>(() => validator.RuleSet("login", (Action)null!));
        Assert.Throws<ArgumentNullException>(() => validator.RuleSet(["login"], (Action)null!));

        Assert.Throws<ArgumentNullException>(() =>
            validator.RuleSet((string?)null, (Action<ObjectValidator<ObjectModel>>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            validator.RuleSet((string?[]?)null, (Action<ObjectValidator<ObjectModel>>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            validator.RuleSet("login", (Action<ObjectValidator<ObjectModel>>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            validator.RuleSet(["login"], (Action<ObjectValidator<ObjectModel>>)null!));
    }

    [Fact]
    public void RuleSet_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();

        var ruleSets = new List<string?>();
        validator.RuleSet([], () =>
        {
            if (validator._ruleSetStack.Count > 0)
            {
                ruleSets.Add(validator._ruleSetStack.Peek());
            }
        });
        Assert.Empty(ruleSets);
        Assert.Empty(validator._ruleSetStack);

        ruleSets.Clear();
        validator.RuleSet(["login"], () => ruleSets.Add(validator._ruleSetStack.Peek()));
        Assert.Equal(["login"], ruleSets);
        Assert.Empty(validator._ruleSetStack);

        ruleSets.Clear();
        validator.RuleSet(["login", "register"], () => ruleSets.Add(validator._ruleSetStack.Peek()));
        Assert.Equal(["login", "register"], ruleSets);
        Assert.Empty(validator._ruleSetStack);

        ruleSets.Clear();
        validator.RuleSet([" login ", " register "], () => ruleSets.Add(validator._ruleSetStack.Peek()));
        Assert.Equal(["login", "register"], ruleSets);
        Assert.Empty(validator._ruleSetStack);

        ruleSets.Clear();
        Assert.Throws<Exception>(() =>
        {
            validator.RuleSet([" login ", " register "], () =>
            {
                if (ruleSets.Count == 1)
                {
                    throw new Exception("出错了");
                }

                ruleSets.Add(validator._ruleSetStack.Peek());
            });
        });
        Assert.Equal(["login"], ruleSets);
        Assert.Empty(validator._ruleSetStack);

        ruleSets.Clear();
        validator.RuleSet("login", () => ruleSets.Add(validator._ruleSetStack.Peek()));
        Assert.Equal(["login"], ruleSets);
        Assert.Empty(validator._ruleSetStack);

        ruleSets.Clear();
        validator.RuleSet("login,register", () => ruleSets.Add(validator._ruleSetStack.Peek()));
        Assert.Equal(["login,register"], ruleSets);
        Assert.Empty(validator._ruleSetStack);

        ruleSets.Clear();
        validator.RuleSet("login;register", () => ruleSets.Add(validator._ruleSetStack.Peek()));
        Assert.Equal(["login;register"], ruleSets);
        Assert.Empty(validator._ruleSetStack);

        ruleSets.Clear();
        validator.RuleSet(" login , register ", () => ruleSets.Add(validator._ruleSetStack.Peek()));
        Assert.Equal(["login , register"], ruleSets);
        Assert.Empty(validator._ruleSetStack);
    }

    [Fact]
    public void SetValidator_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>().RuleFor(u => u.Name).NotEqualTo("Fur").End()
            .SetValidator(new ObjectModelValidator());

        Assert.Throws<ArgumentNullException>(() =>
            objectValidator.SetValidator(
                (Func<IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<ObjectModel>?>)null!));

        var exception =
            Assert.Throws<InvalidOperationException>(() => objectValidator.SetValidator(new ObjectModelValidator()));
        Assert.Equal(
            "An object validator has already been assigned to this object. Only one object validator is allowed per object.",
            exception.Message);
    }

    [Fact]
    public void SetValidator_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>().RuleFor(u => u.Name).NotEqualTo("Fur").End();

        Assert.Null(objectValidator._objectValidator);
        objectValidator.SetValidator(new ObjectModelValidator());
        Assert.NotNull(objectValidator._objectValidator);
        Assert.Null(objectValidator._objectValidator.MemberPath);
        Assert.Null(objectValidator._objectValidator.InheritedRuleSets);
        Assert.Throws<InvalidOperationException>(() =>
            objectValidator.SetValidator((ObjectValidator<ObjectModel>?)null));
        Assert.Null(objectValidator._objectValidator.MemberPath);

        using var objectValidator2 = new ObjectValidator<ObjectModel> { MemberPath = "Sub" }.RuleFor(u => u.Name)
            .NotEqualTo("Fur").End();

        Assert.Null(objectValidator2._objectValidator);
        objectValidator2.SetValidator(new ObjectModelValidator());
        Assert.NotNull(objectValidator2._objectValidator);
        Assert.Null(objectValidator2._objectValidator.InheritedRuleSets);
        Assert.Throws<InvalidOperationException>(() =>
            objectValidator2.SetValidator((ObjectValidator<ObjectModel>?)null));
    }

    [Fact]
    public void ConfigureOptions_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.ConfigureOptions((ValidatorOptions)null!));
        Assert.Throws<ArgumentNullException>(() => validator.ConfigureOptions((Action<ValidatorOptions>)null!));
    }

    [Fact]
    public void ConfigureOptions_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.False(validator.Options.SuppressAnnotationValidation);
        validator.ConfigureOptions(options =>
        {
            options.SuppressAnnotationValidation = true;
        });
        Assert.True(validator.Options.SuppressAnnotationValidation);

        using var validator2 =
            new ObjectValidator<ObjectModel>().ConfigureOptions(options => options.SuppressAnnotationValidation = true);
        Assert.True(validator2.Options.SuppressAnnotationValidation);

        using var validator3 = new ObjectValidator<ObjectModel>();
        validator3.ConfigureOptions(new ValidatorOptions
        {
            SuppressAnnotationValidation = true, ValidateAllProperties = false
        });
        Assert.True(validator3.Options.SuppressAnnotationValidation);
        Assert.False(validator3.Options.ValidateAllProperties);
    }

    [Fact]
    public void UseAnnotationValidation_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.False(validator.Options.SuppressAnnotationValidation);

        validator.UseAnnotationValidation(false);
        Assert.True(validator.Options.SuppressAnnotationValidation);

        validator.UseAnnotationValidation(true);
        Assert.False(validator.Options.SuppressAnnotationValidation);

        validator.SkipAnnotationValidation();
        Assert.True(validator.Options.SuppressAnnotationValidation);

        validator.UseAnnotationValidation();
        Assert.False(validator.Options.SuppressAnnotationValidation);

        validator.CustomOnly();
        Assert.True(validator.Options.SuppressAnnotationValidation);
    }

    [Fact]
    public void ShouldValidate_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.ShouldValidate(null!, null!));
        Assert.Throws<ArgumentNullException>(() => validator.ShouldValidate(new ObjectModel(), null!));
    }

    [Fact]
    public void ShouldValidate_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        var model = new ObjectModel();

        var validationContext = new ValidationContext<ObjectModel>(model);

        Assert.True(validator.ShouldValidate(model, validationContext));
        Assert.True(validator.ShouldValidate(model, validationContext));
        Assert.True(validator.ShouldValidate(model, validationContext));
        Assert.True(validator.ShouldValidate(model, validationContext));
        Assert.True(validator.ShouldValidate(model, validationContext));
    }

    [Fact]
    public void ShouldValidate_WithCondition_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>()
            .When(u => u.Name is not null);
        var model = new ObjectModel();

        var validationContext = new ValidationContext<ObjectModel>(model);

        Assert.False(validator.ShouldValidate(model, validationContext));

        model.Name = "Furion";
        Assert.True(validator.ShouldValidate(model, validationContext));
    }

    [Fact]
    public void ShouldRunAnnotationValidation_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.True(validator.ShouldRunAnnotationValidation());

        validator.Options.SuppressAnnotationValidation = true;
        Assert.False(validator.ShouldRunAnnotationValidation());
    }

    [Fact]
    public void OptionsOnPropertyChanged_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.True(validator._annotationValidator.ValidateAllProperties);
        validator.Options.PropertyChanged -= validator.OptionsOnPropertyChanged;

        validator.Options.ValidateAllProperties = false;
        validator.OptionsOnPropertyChanged(validator.Options,
            new PropertyChangedEventArgs(nameof(ValidatorOptions.ValidateAllProperties)));

        Assert.False(validator._annotationValidator.ValidateAllProperties);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator =
            new ObjectValidator<ObjectModel>(new Dictionary<object, object?> { { "name", "Furion" } }).SetValidator(
                new ObjectModelValidator());
        Assert.NotNull(validator.Items);
        Assert.Single(validator.Items);

        validator.Dispose();

        validator.Options.ValidateAllProperties = false;
        Assert.True(validator._annotationValidator.ValidateAllProperties);
        Assert.Empty(validator.Items);
    }

    [Fact]
    public void ToResults_Invalid_Parameters()
    {
        var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.ToResults(null!));

        var exception = Assert.Throws<InvalidOperationException>(() => validator.ToResults());
        Assert.Equal(
            "The parameterless 'ToResults()' method can only be used when the validator is created via 'ValidationContext.With<T>()'. Ensure you are calling it inside 'IValidatableObject.Validate' and have used 'With' to configure inline validation rules.",
            exception.Message);
    }

    [Fact]
    public void ToResults_ReturnOK()
    {
        var validationContext = new ValidationContext(new ObjectModel());
        var validator = new ObjectValidator<ObjectModel>(
            new Dictionary<object, object?>
            {
                { ObjectValidator<ObjectModel>.ValidationContextsKey, validationContext }
            });

        Assert.Equal(["The field Id must be between 1 and 2147483647.", "The Name field is required."],
            validator.ToResults().Select(u => u.ErrorMessage!).ToArray());

        Assert.Equal(["The field Id must be between 1 and 2147483647.", "The Name field is required."],
            validator.ToResults(validationContext).Select(u => u.ErrorMessage!).ToArray());

        Assert.NotNull(validator.Items);
        Assert.Empty(validator.Items);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>().RuleFor(u => u.Name).Required().UserName().End();

        Assert.Null(validator._serviceProvider);
        Assert.Null(validator._annotationValidator._serviceProvider);

        foreach (var pValidator in validator.Validators.Select(propertyValidator =>
                     propertyValidator as PropertyValidator<ObjectModel, string?>))
        {
            Assert.NotNull(pValidator);
            Assert.Null(pValidator._serviceProvider);
            Assert.Null(pValidator._annotationValidator._serviceProvider);
        }

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(validator._serviceProvider);
        Assert.NotNull(validator._annotationValidator._serviceProvider);

        foreach (var pValidator in validator.Validators.Select(propertyValidator =>
                     propertyValidator as PropertyValidator<ObjectModel, string?>))
        {
            Assert.NotNull(pValidator);
            Assert.NotNull(pValidator._serviceProvider);
            Assert.NotNull(pValidator._annotationValidator._serviceProvider);
        }
    }

    [Fact]
    public void GetCurrentRuleSets_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();
        Assert.Null(validator.GetCurrentRuleSets());

        validator._ruleSetStack.Push("rule");
        Assert.Equal(["rule"], (string[]?)validator.GetCurrentRuleSets()!);
        validator._ruleSetStack.Pop();
        Assert.Null(validator.GetCurrentRuleSets());

        validator.SetInheritedRuleSetsIfNotSet(["email"]);
        Assert.Equal(["email"], (string[]?)validator.GetCurrentRuleSets()!);
    }

    [Fact]
    public void SetInheritedRuleSetsIfNotSet_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();
        validator.SetInheritedRuleSetsIfNotSet(["rule"]);
        Assert.NotNull(validator.InheritedRuleSets);
        Assert.Equal(["rule"], (string[]?)validator.InheritedRuleSets!);

        validator.SetInheritedRuleSetsIfNotSet(["login"]);
        Assert.NotNull(validator.InheritedRuleSets);
        Assert.Equal(["rule"], (string[]?)validator.InheritedRuleSets!);
    }

    [Fact]
    public void ResolveValidationRuleSets_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();
        Assert.Null(validator.ResolveValidationRuleSets(null));
        Assert.Equal(["login"], (string[]?)validator.ResolveValidationRuleSets(["login"])!);

        var services = new ServiceCollection();
        services.AddScoped<IValidationDataContext, ValidationDataContext>();
        using var serviceProvider = services.BuildServiceProvider();
        var dataContext = serviceProvider.GetRequiredService<IValidationDataContext>();
        dataContext.SetValidationOptions(new ValidationOptionsMetadata(["login", "email"]));

        validator.InitializeServiceProvider(serviceProvider.GetService);
        Assert.Equal(["login", "email"], (string[]?)validator.ResolveValidationRuleSets(null)!);
        Assert.Equal(["login"], (string[]?)validator.ResolveValidationRuleSets(["login"])!);
    }

    [Fact]
    public void RepairMemberPaths_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>().RuleFor(u => u.Name).Required()
            .RuleFor(u => u.Id).Min(1).End().SetValidator(new ObjectModelValidator());
        objectValidator.MemberPath = "Sub";
        objectValidator.RepairMemberPaths();

        var propertyValidator1 = objectValidator.Validators[0] as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(propertyValidator1);
        Assert.Equal("Sub.Name", propertyValidator1.GetEffectiveMemberName());

        var propertyValidator2 = objectValidator.Validators[1] as PropertyValidator<ObjectModel, int>;
        Assert.NotNull(propertyValidator2);
        Assert.Equal("Sub.Id", propertyValidator2.GetEffectiveMemberName());

        Assert.NotNull(objectValidator._objectValidator);
        Assert.Equal("Sub", objectValidator._objectValidator.MemberPath);
        var propertyValidator3 =
            objectValidator._objectValidator.Validators[0] as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(propertyValidator3);
        Assert.Equal("Sub.FirstName", propertyValidator3.GetEffectiveMemberName());
    }

    [Fact]
    public void Include_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>().RuleFor(u => u.Name).NotEqualTo("Fur").End()
            .SetValidator(new ObjectModelValidator());

        Assert.Throws<ArgumentNullException>(() =>
            objectValidator.Include(
                (Func<IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<ObjectModel>>)null!));
    }

    [Fact]
    public void Include_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>().RuleFor(u => u.Name).NotEqualTo("Fur").End();

        using var objectValidator2 = new ObjectValidator<ObjectModel>().RuleFor(u => u.Id).Min(1)
            .RuleForCollection(u => u.Children).MinLength(1).RuleFor(u => u.Address).Required().End();

        objectValidator.Include(objectValidator2);
        Assert.Equal(4, objectValidator.Validators.Count);
    }

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();

        var validationContext = objectValidator.CreateValidationContext(new ObjectModel(), null);
        Assert.NotNull(validationContext);
        Assert.NotNull(validationContext.Instance);
        Assert.Null(validationContext.DisplayName);
        Assert.Null(validationContext.MemberNames);
        Assert.Null(validationContext.RuleSets);
        Assert.Empty(validationContext.Items);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        objectValidator.InitializeServiceProvider(serviceProvider.GetService);
        Assert.NotNull(objectValidator._serviceProvider);

        var validationContext2 = objectValidator.CreateValidationContext(new ObjectModel(), ["Login"]);
        Assert.NotNull(validationContext2);
        Assert.NotNull(validationContext2.Instance);
        Assert.Null(validationContext2.DisplayName);
        Assert.Null(validationContext2.MemberNames);
        Assert.Equal<string>(["Login"], validationContext2.RuleSets!);
        Assert.Empty(validationContext2.Items);
        Assert.NotNull(validationContext2._serviceProvider);
    }

    public class ObjectModel
    {
        [Range(1, int.MaxValue)] public int Id { get; set; }

        public string? FirstName { get; set; }

        [Required] [MinLength(3)] public string? Name { get; set; }

        [MinLength(5)] public string? Address { get; set; }

        public List<Child>? Children { get; set; }
    }

    public class ObjectModelValidator : AbstractValidator<ObjectModel>
    {
        public ObjectModelValidator() => RuleFor(u => u.FirstName).MaxLength(8);
    }

    public class Child
    {
        public string? Name { get; set; }
    }
}