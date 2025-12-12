// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ObjectValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ObjectValidator<ObjectModel>(null!, null));

    [Fact]
    public void New_ReturnOK()
    {
        Assert.True(typeof(IObjectValidator<ObjectModel>).IsAssignableFrom(typeof(ObjectValidator<ObjectModel>)));

        using var validator = new ObjectValidator<ObjectModel>();
        Assert.NotNull(validator.Options);
        Assert.False(validator.Options.SuppressAnnotationValidation);
        Assert.NotNull(validator.Validators);
        Assert.Null(validator._serviceProvider);
        Assert.Null(validator._items);
        Assert.Null(validator._inheritedRuleSets);
        Assert.Empty(validator.Validators);
        Assert.NotNull(validator._annotationValidator);
        Assert.True(validator._annotationValidator.ValidateAllProperties);
        Assert.Null(validator._annotationValidator._serviceProvider);
        Assert.NotNull(validator._ruleSetStack);
        Assert.Empty(validator._ruleSetStack);
        Assert.Null(validator.WhenCondition);
        Assert.Null(validator.UnlessCondition);
        Assert.Null(validator.MemberPath);

        using var validator2 =
            new ObjectValidator<ObjectModel>(new ValidatorOptions { SuppressAnnotationValidation = true }, null);
        Assert.NotNull(validator2.Options);
        Assert.True(validator2.Options.SuppressAnnotationValidation);
        Assert.NotNull(validator2.Validators);
        Assert.Null(validator2._serviceProvider);
        Assert.Null(validator2._items);
        Assert.Empty(validator2.Validators);
        Assert.NotNull(validator2._annotationValidator);
        Assert.True(validator2._annotationValidator.ValidateAllProperties);
        Assert.Null(validator2._annotationValidator._serviceProvider);
        Assert.NotNull(validator2._ruleSetStack);
        Assert.Empty(validator2._ruleSetStack);
        Assert.Null(validator2.WhenCondition);
        Assert.Null(validator2.UnlessCondition);

        validator2.Options.SuppressAnnotationValidation = false;
        Assert.False(validator2.Options.SuppressAnnotationValidation);

        validator2.Options.ValidateAllProperties = false;
        Assert.False(validator2._annotationValidator.ValidateAllProperties);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        using var validator3 = new ObjectValidator<ObjectModel>(
            new ValidatorOptions { SuppressAnnotationValidation = true }, serviceProvider,
            new Dictionary<object, object?>());
        Assert.NotNull(validator3._serviceProvider);
        Assert.NotNull(validator3._annotationValidator._serviceProvider);
        Assert.NotNull(validator3._items);
        Assert.NotNull(validator3._annotationValidator._items);

        using var validator4 =
            new ObjectValidator<ObjectModel>(new ValidatorOptions(), new Dictionary<object, object?>());
        Assert.NotNull(validator4._items);
        Assert.NotNull(validator4._annotationValidator._items);
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
        Assert.Throws<ArgumentNullException>(() => validator.When(null!));
    }

    [Fact]
    public void When_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        validator.When(u => u.Name is not null);
        Assert.NotNull(validator.WhenCondition);
        Assert.False(validator.WhenCondition(new ObjectModel()));
        Assert.True(validator.WhenCondition(new ObjectModel { Name = "Furion" }));
    }

    [Fact]
    public void Unless_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        Assert.Throws<ArgumentNullException>(() => validator.Unless(null!));
    }

    [Fact]
    public void Unless_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        validator.Unless(u => u.Name is null);
        Assert.NotNull(validator.UnlessCondition);
        Assert.True(validator.UnlessCondition(new ObjectModel()));
        Assert.False(validator.UnlessCondition(new ObjectModel { Name = "Furion" }));
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

        validator.RuleFor(u => u.Name, ["login"]);
        Assert.Equal(2, validator.Validators.Count);
        var propertyValidator2 = validator.Validators.LastOrDefault() as PropertyValidator<ObjectModel, string>;
        Assert.NotNull(propertyValidator2);
        Assert.NotNull(propertyValidator2.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator2.RuleSets);

        validator.RuleFor(u => u.Id, ["login", "register"]);
        Assert.Equal(3, validator.Validators.Count);
        var propertyValidator3 = validator.Validators.LastOrDefault() as PropertyValidator<ObjectModel, int>;
        Assert.NotNull(propertyValidator3);
        Assert.NotNull(propertyValidator3.RuleSets);
        Assert.NotStrictEqual(["login", "register"], propertyValidator3.RuleSets);

        validator.RuleFor(u => u.Name, ["login; register"]);
        Assert.Equal(4, validator.Validators.Count);
        var propertyValidator4 = validator.Validators.LastOrDefault() as PropertyValidator<ObjectModel, string>;
        Assert.NotNull(propertyValidator4);
        Assert.NotNull(propertyValidator4.RuleSets);
        Assert.NotStrictEqual(["login", "register"], propertyValidator4.RuleSets);
    }

    [Fact]
    public void RuleFor_InRuleSet_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();

        validator.RuleSet([], () =>
        {
            validator.RuleFor(u => u.Address, ["login"]);
        });
        Assert.Single(validator.Validators);
        var propertyValidator = validator.Validators.LastOrDefault() as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(propertyValidator);
        Assert.NotNull(propertyValidator.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator.RuleSets);

        validator.Validators.Clear();
        validator.RuleSet(["login", "register"], () =>
        {
            validator.RuleFor(u => u.Address, ["owner"]);
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
            chain.RuleFor(u => u.Address, ["owner"]);
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
    public void RuleForEach_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.RuleForEach<Child>(null!));
    }

    [Fact]
    public void RuleForEach_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();

        validator.RuleForEach(u => u.Children);
        Assert.Single(validator.Validators);
        var propertyValidator =
            validator.Validators.LastOrDefault() as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator);
        Assert.Null(propertyValidator.RuleSets);

        validator.RuleForEach(u => u.Children, ["login"]);
        Assert.Equal(2, validator.Validators.Count);
        var propertyValidator2 =
            validator.Validators.LastOrDefault() as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator2);
        Assert.NotNull(propertyValidator2.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator2.RuleSets);

        validator.RuleForEach(u => u.Children, ["login", "register"]);
        Assert.Equal(3, validator.Validators.Count);
        var propertyValidator3 =
            validator.Validators.LastOrDefault() as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator3);
        Assert.NotNull(propertyValidator3.RuleSets);
        Assert.NotStrictEqual(["login", "register"], propertyValidator3.RuleSets);

        validator.RuleForEach(u => u.Children, ["login; register"]);
        Assert.Equal(4, validator.Validators.Count);
        var propertyValidator4 =
            validator.Validators.LastOrDefault() as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator4);
        Assert.NotNull(propertyValidator4.RuleSets);
        Assert.NotStrictEqual(["login", "register"], propertyValidator4.RuleSets);
    }

    [Fact]
    public void RuleForEach_InRuleSet_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();

        validator.RuleSet([], () =>
        {
            validator.RuleForEach(u => u.Children, ["login"]);
        });
        Assert.Single(validator.Validators);
        var propertyValidator =
            validator.Validators.LastOrDefault() as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator);
        Assert.NotNull(propertyValidator.RuleSets);
        Assert.NotStrictEqual(["login"], propertyValidator.RuleSets);

        validator.Validators.Clear();
        validator.RuleSet(["login", "register"], () =>
        {
            validator.RuleForEach(u => u.Children, ["owner"]);
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
            chain.RuleForEach(u => u.Children, ["owner"]);
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
            validator.RuleForEach(x => x.Children);
            validator.RuleSet("register", () =>
            {
                validator.RuleForEach(x => x.Children);
            });
        });
        Assert.Equal(2, validator.Validators.Count);
    }

    [Fact]
    public void RuleForEach_InitializeServiceProvider_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();
        var propertyValidator = validator.RuleForEach(u => u.Children);

        Assert.Null(propertyValidator._serviceProvider);
        Assert.Null(propertyValidator._annotationValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(propertyValidator._serviceProvider);
        Assert.NotNull(propertyValidator._annotationValidator._serviceProvider);

        var propertyValidator2 = validator.RuleForEach(u => u.Children);
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
    public void ConfigureOptions_Invalid_Parameters()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        Assert.Throws<ArgumentNullException>(() => validator.ConfigureOptions(null!));
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
    public void ShouldValidate_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>();
        var model = new ObjectModel();

        Assert.True(validator.ShouldValidate(model));
        Assert.True(validator.ShouldValidate(model, ["*"]));
        Assert.True(validator.ShouldValidate(model, ["login"]));
        Assert.True(validator.ShouldValidate(model, ["register"]));
        Assert.True(validator.ShouldValidate(model, ["other"]));
    }

    [Fact]
    public void ShouldValidate_WithCondition_ReturnOK()
    {
        using var validator = new ObjectValidator<ObjectModel>()
            .When(u => u.Name is not null).Unless(u => u.Name?.Equals("Fur") == true);
        var model = new ObjectModel();

        Assert.False(validator.ShouldValidate(model));

        model.Name = "Furion";
        Assert.True(validator.ShouldValidate(model));

        model.Name = "Fur";
        Assert.False(validator.ShouldValidate(model));
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
        var validator = new ObjectValidator<ObjectModel>();
        validator.Dispose();

        validator.Options.ValidateAllProperties = false;
        Assert.True(validator._annotationValidator.ValidateAllProperties);
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
    public void GetCurrentRuleSetScope_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();
        Assert.Null(validator.GetCurrentRuleSetScope());
        validator._ruleSetStack.Push("rule");
        Assert.Equal(["rule"], (string[]?)validator.GetCurrentRuleSetScope()!);
        validator._ruleSetStack.Pop();
        Assert.Null(validator.GetCurrentRuleSetScope());

        Assert.Equal(["login"], (string[]?)validator.GetCurrentRuleSetScope(["login"])!);
        validator.SetInheritedRuleSetsIfNotSet(["email"]);
        Assert.Equal(["email"], (string[]?)validator.GetCurrentRuleSetScope()!);
    }

    [Fact]
    public void SetInheritedRuleSetsIfNotSet_ReturnOK()
    {
        var validator = new ObjectValidator<ObjectModel>();
        validator.SetInheritedRuleSetsIfNotSet(["rule"]);
        Assert.NotNull(validator._inheritedRuleSets);
        Assert.Equal(["rule"], (string[]?)validator._inheritedRuleSets!);

        validator.SetInheritedRuleSetsIfNotSet(["login"]);
        Assert.NotNull(validator._inheritedRuleSets);
        Assert.Equal(["rule"], (string[]?)validator._inheritedRuleSets!);
    }

    public class ObjectModel
    {
        [Range(1, int.MaxValue)] public int Id { get; set; }

        public string? FirstName { get; set; }

        [Required] [MinLength(3)] public string? Name { get; set; }

        [MinLength(5)] public string? Address { get; set; }

        public List<Child>? Children { get; set; }
    }

    public class Child
    {
        public string? Name { get; set; }
    }
}