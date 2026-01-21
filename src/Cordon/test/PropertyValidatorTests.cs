// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class PropertyValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new PropertyValidator<ObjectModel, string?>(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new PropertyValidator<ObjectModel, string?>(u => u.Name, null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Null(propertyValidator._serviceProvider);
        Assert.NotNull(propertyValidator._selector);
        Assert.NotNull(propertyValidator._objectValidator);
        Assert.Equal(objectValidator, propertyValidator._objectValidator);
        Assert.NotNull(propertyValidator._attributeValidator);
        Assert.Null(propertyValidator._attributeValidator._serviceProvider);
        Assert.Empty(propertyValidator._attributeValidator.Items);
        Assert.NotNull(propertyValidator.Validators);
        Assert.Null(propertyValidator._lastAddedValidator);
        Assert.Empty(propertyValidator.Validators);
        Assert.Equal(0, propertyValidator._highPriorityEndIndex);
        Assert.Equal(0, propertyValidator._objectValidatorStartIndex);
        Assert.Null(propertyValidator._preProcessor);
        Assert.Null(propertyValidator.RuleSets);
        Assert.Null(propertyValidator.DisplayName);
        Assert.Null(propertyValidator.MemberName);
        Assert.Null(propertyValidator.SuppressAttributeValidation);
        Assert.Null(propertyValidator.WhenCondition);
        Assert.NotNull(propertyValidator.This);
        Assert.Equal(propertyValidator.This, propertyValidator);
        Assert.Null(propertyValidator._allowEmptyStrings);
        Assert.Equal(CompositeMode.All, propertyValidator.Mode);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        using var objectValidator2 = new ObjectValidator<ObjectModel>(serviceProvider,
            new Dictionary<object, object?>());
        var propertyValidator2 = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator2);

        Assert.Null(propertyValidator2._serviceProvider); // RuleFor 时同步
        Assert.NotNull(propertyValidator2._objectValidator);
        Assert.Equal(objectValidator2, propertyValidator2._objectValidator);
        Assert.NotNull(propertyValidator2._attributeValidator);
        Assert.Null(propertyValidator2._attributeValidator._serviceProvider); // RuleFor 时同步
        Assert.NotNull(propertyValidator2._attributeValidator.Items);
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.IsValid(null!));
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(3));

        Assert.False(propertyValidator.IsValid(new ObjectModel()));
        Assert.False(propertyValidator.IsValid(new ObjectModel { FirstName = "Fu" }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { FirstName = "Furion" }));
    }

    [Fact]
    public void IsValid_WithRuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator) { RuleSets = ["rule"] }
                .AddValidators(
                    new RequiredValidator(), new MinLengthValidator(3));

        Assert.True(propertyValidator.IsValid(new ObjectModel()));
        Assert.True(propertyValidator.IsValid(new ObjectModel { FirstName = "Fu" }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { FirstName = "Furion" }));

        Assert.False(propertyValidator.IsValid(new ObjectModel(), ["*"]));
        Assert.False(propertyValidator.IsValid(new ObjectModel { FirstName = "Fu" }, ["*"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { FirstName = "Furion" }, ["*"]));

        Assert.False(propertyValidator.IsValid(new ObjectModel(), ["rule"]));
        Assert.False(propertyValidator.IsValid(new ObjectModel { FirstName = "Fu" }, ["rule"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { FirstName = "Furion" }, ["rule"]));
    }

    [Fact]
    public void IsValid_WithSuppressAttributeValidation_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(2));

        Assert.False(propertyValidator.IsValid(new ObjectModel()));
        Assert.False(propertyValidator.IsValid(new ObjectModel { Name = "Fu" }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Furion" }));

        Assert.False(propertyValidator.IsValid(new ObjectModel(), ["*"]));
        Assert.False(propertyValidator.IsValid(new ObjectModel { Name = "Fu" }, ["*"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Furion" }, ["*"]));

        Assert.True(propertyValidator.IsValid(new ObjectModel(), ["rule"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Fu" }, ["rule"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Furion" }, ["rule"]));

        propertyValidator.SkipAttributeValidation();

        Assert.False(propertyValidator.IsValid(new ObjectModel()));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Fu" }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Furion" }));

        Assert.False(propertyValidator.IsValid(new ObjectModel(), ["*"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Fu" }, ["*"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Furion" }, ["*"]));

        Assert.True(propertyValidator.IsValid(new ObjectModel(), ["rule"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Fu" }, ["rule"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Name = "Furion" }, ["rule"]));
    }

    [Fact]
    public void IsValid_WithPropertyValidator_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator).SetValidator(
                new SubModelValidator());

        Assert.False(propertyValidator.IsValid(new ObjectModel { Sub = new SubModel() }));
        Assert.False(propertyValidator.IsValid(new ObjectModel { Sub = new SubModel { Id = 1 } }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Sub = new SubModel { Id = 3 } }));
    }

    [Fact]
    public void IsValid_WithPropertyValidator_WithRuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator) { RuleSets = ["login"] }
                .SetValidator(new SubModelValidator());

        Assert.True(propertyValidator.IsValid(new ObjectModel { Sub = new SubModel() }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Sub = new SubModel { Id = 1 } }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Sub = new SubModel { Id = 3 } }));

        Assert.False(propertyValidator.IsValid(new ObjectModel { Sub = new SubModel() }, ["login"]));
        Assert.False(propertyValidator.IsValid(new ObjectModel { Sub = new SubModel { Id = 1 } }, ["login"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Sub = new SubModel { Id = 3 } }, ["login"]));
    }

    [Fact]
    public void IsValid_WithMode_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                    new RequiredValidator(), new MinLengthValidator(3), new ChineseValidator()).CustomOnly()
                .UseMode(CompositeMode.FailFast);

        Assert.False(propertyValidator.IsValid(new ObjectModel()));
        Assert.False(propertyValidator.IsValid(new ObjectModel { FirstName = "Fu" }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { FirstName = "百小僧" }));
    }

    [Fact]
    public void GetValidationResults_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.GetValidationResults(null!));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(3));

        var validationResults = propertyValidator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The FirstName field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Fu" });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field FirstName must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Furion" }));
    }

    [Fact]
    public void GetValidationResults_WithRuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator) { RuleSets = ["rule"] }
                .AddValidators(
                    new RequiredValidator(), new MinLengthValidator(3));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel()));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Fu" }));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Furion" }));

        var validationResults = propertyValidator.GetValidationResults(new ObjectModel(), ["*"]);
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The FirstName field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Fu" }, ["*"]);
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field FirstName must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Furion" }));

        var validationResults3 = propertyValidator.GetValidationResults(new ObjectModel(), ["rule"]);
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal(["The FirstName field is required."],
            validationResults3.Select(u => u.ErrorMessage));

        var validationResults4 = propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Fu" }, ["rule"]);
        Assert.NotNull(validationResults4);
        Assert.Single(validationResults4);
        Assert.Equal(["The field FirstName must be a string or array type with a minimum length of '3'."],
            validationResults4.Select(u => u.ErrorMessage));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Furion" }, ["rule"]));
    }

    [Fact]
    public void GetValidationResults_WithSuppressAttributeValidation_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(2));

        var validationResults = propertyValidator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Equal(2, validationResults.Count);
        Assert.Equal(["The Name field is required.", "The Name field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = propertyValidator.GetValidationResults(new ObjectModel { Name = "Fu" });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field Name must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Furion" }));

        var validationResults3 = propertyValidator.GetValidationResults(new ObjectModel(), ["*"]);
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal(["The Name field is required.", "The Name field is required."],
            validationResults3.Select(u => u.ErrorMessage));

        var validationResults4 = propertyValidator.GetValidationResults(new ObjectModel { Name = "Fu" }, ["*"]);
        Assert.NotNull(validationResults4);
        Assert.Single(validationResults4);
        Assert.Equal(["The field Name must be a string or array type with a minimum length of '3'."],
            validationResults4.Select(u => u.ErrorMessage));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Furion" }));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel(), ["rule"]));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Fu" }, ["rule"]));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Furion" }, ["rule"]));

        propertyValidator.SkipAttributeValidation();

        var validationResult7 = propertyValidator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResult7);
        Assert.Single(validationResult7);
        Assert.Equal(["The Name field is required."],
            validationResult7.Select(u => u.ErrorMessage));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Fu" }));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Furion" }));

        var validationResults8 = propertyValidator.GetValidationResults(new ObjectModel(), ["*"]);
        Assert.NotNull(validationResults8);
        Assert.Single(validationResults8);
        Assert.Equal(["The Name field is required."],
            validationResults8.Select(u => u.ErrorMessage));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Fu" }, ["*"]));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Furion" }, ["*"]));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel(), ["rule"]));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Fu" }, ["rule"]));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Name = "Furion" }, ["rule"]));
    }

    [Fact]
    public void GetValidationResults_WithDisplayName_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(3)).WithDisplayName("MyFirstName");

        var validationResults = propertyValidator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The MyFirstName field is required."],
            validationResults.Select(u => u.ErrorMessage));
        Assert.Equal("FirstName", validationResults.First().MemberNames.First());

        var validationResults2 = propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Fu" });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field MyFirstName must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));
        Assert.Equal("FirstName", validationResults2.First().MemberNames.First());
    }

    [Fact]
    public void GetValidationResults_WithName_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(3)).WithName("MyFirstName");

        var validationResults = propertyValidator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The MyFirstName field is required."],
            validationResults.Select(u => u.ErrorMessage));
        Assert.Equal("MyFirstName", validationResults.First().MemberNames.First());

        var validationResults2 = propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Fu" });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field MyFirstName must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));
        Assert.Equal("MyFirstName", validationResults2.First().MemberNames.First());

        var propertyValidator2 =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                    new RequiredValidator(), new MinLengthValidator(3)).WithName("MyFirstName")
                .WithDisplayName("DisplayFirstName");

        var validationResults3 = propertyValidator2.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal(["The DisplayFirstName field is required."],
            validationResults3.Select(u => u.ErrorMessage));
        Assert.Equal("MyFirstName", validationResults3.First().MemberNames.First());

        var validationResults4 = propertyValidator2.GetValidationResults(new ObjectModel { FirstName = "Fu" });
        Assert.NotNull(validationResults4);
        Assert.Single(validationResults4);
        Assert.Equal(["The field DisplayFirstName must be a string or array type with a minimum length of '3'."],
            validationResults4.Select(u => u.ErrorMessage));
        Assert.Equal("MyFirstName", validationResults4.First().MemberNames.First());
    }

    [Fact]
    public void GetValidationResults_WithPropertyValidator_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator).SetValidator(
                new SubModelValidator());

        var validationResults = propertyValidator.GetValidationResults(new ObjectModel { Sub = new SubModel() });
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The field Id must be greater than or equal to '3'."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 =
            propertyValidator.GetValidationResults(new ObjectModel { Sub = new SubModel { Id = 1 } });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field Id must be greater than or equal to '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Sub = new SubModel { Id = 3 } }));
    }

    [Fact]
    public void GetValidationResults_WithPropertyValidator_WithRuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator) { RuleSets = ["login"] }
                .SetValidator(new SubModelValidator());

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Sub = new SubModel() }));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Sub = new SubModel { Id = 1 } }));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Sub = new SubModel { Id = 3 } }));

        var validationResults =
            propertyValidator.GetValidationResults(new ObjectModel { Sub = new SubModel() }, ["login"]);
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The field Id must be greater than or equal to '3'."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 =
            propertyValidator.GetValidationResults(new ObjectModel { Sub = new SubModel { Id = 1 } }, ["login"]);
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field Id must be greater than or equal to '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(
            propertyValidator.GetValidationResults(new ObjectModel { Sub = new SubModel { Id = 3 } }, ["login"]));
    }

    [Fact]
    public void GetValidationResults_WithMode_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                    new RequiredValidator(), new MinLengthValidator(3), new ChineseValidator()).CustomOnly()
                .UseMode(CompositeMode.FailFast);

        var validationResults = propertyValidator.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The FirstName field is required."],
            validationResults.Select(u => u.ErrorMessage));

        var validationResults2 = propertyValidator.GetValidationResults(new ObjectModel { FirstName = "Fu" });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The field FirstName must be a string or array type with a minimum length of '3'."],
            validationResults2.Select(u => u.ErrorMessage));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { FirstName = "百小僧" }));

        var validationResults3 = propertyValidator.UseMode(CompositeMode.All)
            .GetValidationResults(new ObjectModel { FirstName = "Fu" });
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal(
            [
                "The field FirstName must be a string or array type with a minimum length of '3'.",
                "The field FirstName contains invalid Chinese characters."
            ],
            validationResults3.Select(u => u.ErrorMessage));
    }

    [Fact]
    public void Validate_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.Validate(null!));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(3));

        var exception = Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel()));
        Assert.Equal("The FirstName field is required.", exception.Message);
        Assert.Equal("FirstName", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel { FirstName = "Fu" }));
        Assert.Equal("The field FirstName must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Equal("FirstName", exception2.ValidationResult.MemberNames.First());

        propertyValidator.Validate(new ObjectModel { FirstName = "Furion" });
    }

    [Fact]
    public void Validate_WithRuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator) { RuleSets = ["rule"] }
                .AddValidators(
                    new RequiredValidator(), new MinLengthValidator(3));

        propertyValidator.Validate(new ObjectModel());
        propertyValidator.Validate(new ObjectModel { FirstName = "Fu" });
        propertyValidator.Validate(new ObjectModel { FirstName = "Furion" });

        var exception = Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel(), ["*"]));
        Assert.Equal("The FirstName field is required.", exception.Message);
        Assert.Equal("FirstName", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() =>
                propertyValidator.Validate(new ObjectModel { FirstName = "Fu" }, ["*"]));
        Assert.Equal("The field FirstName must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Equal("FirstName", exception2.ValidationResult.MemberNames.First());

        propertyValidator.Validate(new ObjectModel { FirstName = "Furion" });

        var exception3 =
            Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel(), ["rule"]));
        Assert.Equal("The FirstName field is required.", exception3.Message);
        Assert.Equal("FirstName", exception3.ValidationResult.MemberNames.First());

        var exception4 = Assert.Throws<ValidationException>(() =>
            propertyValidator.Validate(new ObjectModel { FirstName = "Fu" }, ["rule"]));
        Assert.Equal("The field FirstName must be a string or array type with a minimum length of '3'.",
            exception4.Message);
        Assert.Equal("FirstName", exception4.ValidationResult.MemberNames.First());

        propertyValidator.Validate(new ObjectModel { FirstName = "Furion" }, ["rule"]);
    }

    [Fact]
    public void Validate_WithSuppressAttributeValidation_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(2));

        var exception = Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel()));
        Assert.Equal("The Name field is required.", exception.Message);
        Assert.Equal("Name", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel { Name = "Fu" }));
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Equal("Name", exception2.ValidationResult.MemberNames.First());

        propertyValidator.Validate(new ObjectModel { Name = "Furion" });

        var exception3 =
            Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel(), ["*"]));
        Assert.Equal("The Name field is required.", exception3.Message);
        Assert.Equal("Name", exception3.ValidationResult.MemberNames.First());

        var exception4 =
            Assert.Throws<ValidationException>(() =>
                propertyValidator.Validate(new ObjectModel { Name = "Fu" }, ["*"]));
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.",
            exception4.Message);
        Assert.Equal("Name", exception4.ValidationResult.MemberNames.First());

        propertyValidator.Validate(new ObjectModel { Name = "Furion" });

        propertyValidator.Validate(new ObjectModel(), ["rule"]);
        propertyValidator.Validate(new ObjectModel { Name = "Fu" }, ["rule"]);
        propertyValidator.Validate(new ObjectModel { Name = "Furion" }, ["rule"]);

        propertyValidator.SkipAttributeValidation();

        var exception5 = Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel()));
        Assert.Equal("The Name field is required.", exception5.Message);
        Assert.Equal("Name", exception5.ValidationResult.MemberNames.First());
        propertyValidator.Validate(new ObjectModel { Name = "Fu" });
        propertyValidator.Validate(new ObjectModel { Name = "Furion" });

        var exception6 =
            Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel(), ["*"]));
        Assert.Equal("The Name field is required.", exception6.Message);
        Assert.Equal("Name", exception6.ValidationResult.MemberNames.First());
        propertyValidator.Validate(new ObjectModel { Name = "Fu" }, ["*"]);
        propertyValidator.Validate(new ObjectModel { Name = "Furion" }, ["*"]);

        propertyValidator.Validate(new ObjectModel(), ["rule"]);
        propertyValidator.Validate(new ObjectModel { Name = "Fu" }, ["rule"]);
        propertyValidator.Validate(new ObjectModel { Name = "Furion" }, ["rule"]);
    }

    [Fact]
    public void Validate_WithDisplayName_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(3)).WithDisplayName("MyFirstName");

        var exception = Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel()));
        Assert.Equal("The MyFirstName field is required.", exception.Message);
        Assert.Equal("FirstName", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel { FirstName = "Fu" }));
        Assert.Equal("The field MyFirstName must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Equal("FirstName", exception2.ValidationResult.MemberNames.First());
    }

    [Fact]
    public void Validate_WithName_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                new RequiredValidator(), new MinLengthValidator(3)).WithName("MyFirstName");

        var exception = Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel()));
        Assert.Equal("The MyFirstName field is required.", exception.Message);
        Assert.Equal("MyFirstName", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel { FirstName = "Fu" }));
        Assert.Equal("The field MyFirstName must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Equal("MyFirstName", exception2.ValidationResult.MemberNames.First());

        var propertyValidator2 =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                    new RequiredValidator(), new MinLengthValidator(3)).WithName("MyFirstName")
                .WithDisplayName("DisplayFirstName");

        var exception3 = Assert.Throws<ValidationException>(() => propertyValidator2.Validate(new ObjectModel()));
        Assert.Equal("The DisplayFirstName field is required.", exception3.Message);
        Assert.Equal("MyFirstName", exception3.ValidationResult.MemberNames.First());

        var exception4 =
            Assert.Throws<ValidationException>(() => propertyValidator2.Validate(new ObjectModel { FirstName = "Fu" }));
        Assert.Equal("The field DisplayFirstName must be a string or array type with a minimum length of '3'.",
            exception4.Message);
        Assert.Equal("MyFirstName", exception4.ValidationResult.MemberNames.First());
    }

    [Fact]
    public void Validate_WithPropertyValidator_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator).SetValidator(
                new SubModelValidator());

        var exception =
            Assert.Throws<ValidationException>(() =>
                propertyValidator.Validate(new ObjectModel { Sub = new SubModel() }));
        Assert.Equal("The field Id must be greater than or equal to '3'.", exception.Message);
        Assert.Equal("Sub.Id", exception.ValidationResult.MemberNames.First());

        var exception2 = Assert.Throws<ValidationException>(() =>
            propertyValidator.Validate(new ObjectModel { Sub = new SubModel { Id = 1 } }));
        Assert.Equal("The field Id must be greater than or equal to '3'.", exception2.Message);
        Assert.Equal("Sub.Id", exception2.ValidationResult.MemberNames.First());

        propertyValidator.Validate(new ObjectModel { Sub = new SubModel { Id = 3 } });
    }

    [Fact]
    public void Validate_WithPropertyValidator_WithRuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator) { RuleSets = ["login"] }
                .SetValidator(new SubModelValidator());

        propertyValidator.Validate(new ObjectModel { Sub = new SubModel() });
        propertyValidator.Validate(new ObjectModel { Sub = new SubModel { Id = 1 } });
        propertyValidator.Validate(new ObjectModel { Sub = new SubModel { Id = 3 } });

        var exception =
            Assert.Throws<ValidationException>(() =>
                propertyValidator.Validate(new ObjectModel { Sub = new SubModel() }, ["login"]));
        Assert.Equal("The field Id must be greater than or equal to '3'.", exception.Message);
        Assert.Equal("Sub.Id", exception.ValidationResult.MemberNames.First());

        var exception2 = Assert.Throws<ValidationException>(() =>
            propertyValidator.Validate(new ObjectModel { Sub = new SubModel { Id = 1 } }, ["login"]));
        Assert.Equal("The field Id must be greater than or equal to '3'.", exception2.Message);
        Assert.Equal("Sub.Id", exception2.ValidationResult.MemberNames.First());

        propertyValidator.Validate(new ObjectModel { Sub = new SubModel { Id = 3 } }, ["login"]);
    }

    [Fact]
    public void Validate_WithMode_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.FirstName, objectValidator).AddValidators(
                    new RequiredValidator(), new MinLengthValidator(3), new ChineseValidator()).CustomOnly()
                .UseMode(CompositeMode.FailFast);

        var exception = Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel()));
        Assert.Equal("The FirstName field is required.", exception.Message);
        Assert.Equal("FirstName", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() => propertyValidator.Validate(new ObjectModel { FirstName = "Fu" }));
        Assert.Equal("The field FirstName must be a string or array type with a minimum length of '3'.",
            exception2.Message);
        Assert.Equal("FirstName", exception2.ValidationResult.MemberNames.First());

        propertyValidator.Validate(new ObjectModel { FirstName = "百小僧" });

        var exception3 =
            Assert.Throws<ValidationException>(() =>
                propertyValidator.UseMode(CompositeMode.All).Validate(new ObjectModel { FirstName = "Fu" }));
        Assert.Equal("The field FirstName must be a string or array type with a minimum length of '3'.",
            exception3.Message);
        Assert.Equal("FirstName", exception3.ValidationResult.MemberNames.First());
    }

    [Fact]
    public void AddValidators_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.AddValidators(null!));
    }

    [Fact]
    public void AddValidators_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        propertyValidator.AddValidators(new AgeValidator(), new RequiredValidator(), new NotNullValidator(),
            new EmailAddressValidator(), new NotNullValidator());
        Assert.Equal(3, propertyValidator._highPriorityEndIndex);
        Assert.Equal(
        [
            typeof(NotNullValidator), typeof(NotNullValidator), typeof(RequiredValidator), typeof(AgeValidator),
            typeof(EmailAddressValidator)
        ], propertyValidator.Validators.Select(u => u.GetType()));
    }

    [Fact]
    public void AddValidator_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.AddValidator<ValidatorBase>(null!));
    }

    [Fact]
    public void AddValidator_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        propertyValidator.AddValidator(new AgeValidator());
        Assert.Single(propertyValidator.Validators);
        Assert.Equal(0, propertyValidator._highPriorityEndIndex);
        Assert.NotNull(propertyValidator._lastAddedValidator);
        Assert.True(propertyValidator._lastAddedValidator is AgeValidator);

        propertyValidator.AddValidator(new RequiredValidator());
        Assert.Equal(2, propertyValidator.Validators.Count);
        Assert.Equal(1, propertyValidator._highPriorityEndIndex);
        Assert.True(propertyValidator.Validators.First() is RequiredValidator);
        Assert.NotNull(propertyValidator._lastAddedValidator);
        Assert.True(propertyValidator._lastAddedValidator is RequiredValidator);

        propertyValidator.AddValidator(new NotNullValidator());
        Assert.Equal(3, propertyValidator.Validators.Count);
        Assert.Equal(2, propertyValidator._highPriorityEndIndex);
        Assert.True(propertyValidator.Validators.First() is NotNullValidator);
        Assert.NotNull(propertyValidator._lastAddedValidator);
        Assert.True(propertyValidator._lastAddedValidator is NotNullValidator);

        propertyValidator.AddValidator(new EmailAddressValidator());
        Assert.Equal(4, propertyValidator.Validators.Count);
        Assert.Equal(2, propertyValidator._highPriorityEndIndex);
        Assert.True(propertyValidator.Validators.Last() is EmailAddressValidator);
        Assert.NotNull(propertyValidator._lastAddedValidator);
        Assert.True(propertyValidator._lastAddedValidator is EmailAddressValidator);

        var newNullValidator = new NotNullValidator();
        propertyValidator.AddValidator(newNullValidator);
        Assert.Equal(5, propertyValidator.Validators.Count);
        Assert.Equal(3, propertyValidator._highPriorityEndIndex);
        Assert.Equal(newNullValidator, propertyValidator.Validators[1]);
        Assert.NotNull(propertyValidator._lastAddedValidator);
        Assert.True(propertyValidator._lastAddedValidator is NotNullValidator);

        propertyValidator.AddValidator(new ObjectValidator<string>());
        Assert.Equal(6, propertyValidator.Validators.Count);
        Assert.Equal(3, propertyValidator._highPriorityEndIndex);
        Assert.Equal(5, propertyValidator._objectValidatorStartIndex);
        Assert.True(propertyValidator.Validators.Last() is ObjectValidator<string>);
        Assert.NotNull(propertyValidator._lastAddedValidator);
        Assert.True(propertyValidator._lastAddedValidator is ObjectValidator<string>);

        propertyValidator.AddValidator(new ObjectValidator<string>());
        Assert.Equal(7, propertyValidator.Validators.Count);
        Assert.Equal(3, propertyValidator._highPriorityEndIndex);
        Assert.Equal(5, propertyValidator._objectValidatorStartIndex);
        Assert.True(propertyValidator.Validators.Last() is ObjectValidator<string>);
        Assert.NotNull(propertyValidator._lastAddedValidator);
        Assert.True(propertyValidator._lastAddedValidator is ObjectValidator<string>);
    }

    [Fact]
    public void AddValidator_WithConfigure_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        propertyValidator.AddValidator(new AgeValidator(), validator =>
        {
            validator.IsAdultOnly = true;
            validator.AllowStringValues = true;
        });

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as AgeValidator;
        Assert.NotNull(addedValidator);
        Assert.True(addedValidator.IsAdultOnly);
        Assert.True(addedValidator.AllowStringValues);
    }

    [Fact]
    public void SetValidator_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator).SetValidator(
                new SubModelValidator());

        Assert.Throws<ArgumentNullException>(() =>
            propertyValidator.SetValidator(
                (Func<string?[]?, IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<SubModel>?>)null!));

        var exception =
            Assert.Throws<InvalidOperationException>(() => propertyValidator.SetValidator(new SubModelValidator()));
        Assert.Equal(
            "An object validator has already been assigned to this property. Only one object validator is allowed per property. To define nested rules, use `ChildRules` within a single validator.",
            exception.Message);
    }

    [Fact]
    public void SetValidator_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator);

        Assert.False(propertyValidator.Validators.OfType<ObjectValidator<SubModel>>().Any());
        propertyValidator.SetValidator(new SubModelValidator());
        Assert.True(propertyValidator.Validators.OfType<ObjectValidator<SubModel>>().Any());

        var propertyObjectValidator =
            propertyValidator.Validators.OfType<ObjectValidator<SubModel>>().First();
        Assert.True(propertyObjectValidator.IsNested);
        Assert.Null(propertyObjectValidator.InheritedRuleSets);
        Assert.Throws<InvalidOperationException>(() =>
            propertyValidator.SetValidator((ObjectValidator<SubModel>?)null));

        var propertyValidator2 =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator) { RuleSets = ["login"] };

        Assert.False(propertyValidator2.Validators.OfType<ObjectValidator<SubModel>>().Any());
        propertyValidator2.SetValidator((_, _, _) => new SubModelValidator());
        Assert.True(propertyValidator2.Validators.OfType<ObjectValidator<SubModel>>().Any());

        var propertyObjectValidator2 =
            propertyValidator2.Validators.OfType<ObjectValidator<SubModel>>().First();
        Assert.NotNull(propertyObjectValidator2.InheritedRuleSets);
        Assert.Equal(["login"], (string[]?)propertyObjectValidator2.InheritedRuleSets!);
    }

    [Fact]
    public void ChildRules_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator);
        Assert.Throws<ArgumentNullException>(() => propertyValidator.ChildRules(null!));

        propertyValidator.SetValidator(new SubModelValidator());

        var exception = Assert.Throws<InvalidOperationException>(() => propertyValidator.ChildRules(_ => { }));
        Assert.Equal(
            "An object validator has already been assigned to this property. Only one object validator is allowed per property. To define nested rules, use `ChildRules` within a single validator.",
            exception.Message);
    }

    [Fact]
    public void ChildRules_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator);
        propertyValidator.ChildRules(o => o.RuleFor(u => u.Id).Min(1));
        Assert.True(propertyValidator.Validators.OfType<ObjectValidator<SubModel>>().Any());
        var propertyObjectValidator =
            propertyValidator.Validators.OfType<ObjectValidator<SubModel>>().First();
        Assert.Null(propertyObjectValidator.InheritedRuleSets);

        Assert.False(propertyObjectValidator.IsValid(new SubModel()));
        Assert.True(propertyObjectValidator.IsValid(new SubModel { Id = 1 }));

        var propertyValidator2 =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator) { RuleSets = ["login"] };
        propertyValidator2.ChildRules(o => o.RuleFor(u => u.Id).Min(1));
        Assert.True(propertyValidator2.Validators.OfType<ObjectValidator<SubModel>>().Any());

        var propertyObjectValidator2 =
            propertyValidator2.Validators.OfType<ObjectValidator<SubModel>>().First();
        Assert.NotNull(propertyObjectValidator2.InheritedRuleSets);
        Assert.Equal(["login"], (string[]?)propertyObjectValidator2.InheritedRuleSets!);
    }

    [Fact]
    public void When_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.When((Func<string?, bool>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            propertyValidator.When((Func<string?, ValidationContext<ObjectModel>, bool>)null!));
    }

    [Fact]
    public void When_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        propertyValidator.When(u => u is not null);
        Assert.NotNull(propertyValidator.WhenCondition);
        Assert.False(propertyValidator.WhenCondition(null,
            new ValidationContext<ObjectModel>(new ObjectModel(), (IServiceProvider?)null, null)));
        Assert.True(propertyValidator.WhenCondition("Furion",
            new ValidationContext<ObjectModel>(new ObjectModel { Name = "Furion" }, (IServiceProvider?)null, null)));

        var propertyValidator2 = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        propertyValidator2.When((_, ctx) => ctx.Instance.Name is not null);
        Assert.NotNull(propertyValidator2.WhenCondition);
        Assert.False(propertyValidator2.WhenCondition(null,
            new ValidationContext<ObjectModel>(new ObjectModel(), (IServiceProvider?)null, null)));
        Assert.True(propertyValidator2.WhenCondition("Furion",
            new ValidationContext<ObjectModel>(new ObjectModel { Name = "Furion" }, (IServiceProvider?)null, null)));
    }

    [Fact]
    public void PreProcess_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        propertyValidator.PreProcess(p => p?.Trim());
        Assert.NotNull(propertyValidator._preProcessor);

        propertyValidator.PreProcess(null);
        Assert.Null(propertyValidator._preProcessor);
    }

    [Fact]
    public void UseMode_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);
        Assert.Equal(CompositeMode.All, propertyValidator.Mode);
        propertyValidator.UseMode(CompositeMode.FailFast);
        Assert.Equal(CompositeMode.FailFast, propertyValidator.Mode);
    }

    [Fact]
    public void ShouldValidate_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() =>
            propertyValidator.ShouldValidate(null!, null, null!, null));

        var validationContext = new ValidationContext<ObjectModel>(new ObjectModel());
        Assert.Throws<ArgumentNullException>(() =>
            propertyValidator.ShouldValidate(null!, null, validationContext, null));
    }

    [Fact]
    public void ShouldValidate_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);
        var model = new ObjectModel();

        var validationContext = new ValidationContext<ObjectModel>(model);

        Assert.True(propertyValidator.ShouldValidate(model, null, validationContext, null));
        Assert.True(propertyValidator.ShouldValidate(model, null, validationContext, ["*"]));

        var propertyValidator2 = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator)
        {
            RuleSets = ["login", "register"]
        };
        Assert.False(propertyValidator2.ShouldValidate(model, null, validationContext, null));
        Assert.True(propertyValidator2.ShouldValidate(model, null, validationContext, ["*"]));
        Assert.True(propertyValidator2.ShouldValidate(model, null, validationContext, ["login"]));
        Assert.True(propertyValidator2.ShouldValidate(model, null, validationContext, ["register"]));
        Assert.False(propertyValidator2.ShouldValidate(model, null, validationContext, ["other"]));
    }

    [Fact]
    public void ShouldValidate_WithCondition_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator)
            .When(u => u is not null);
        var model = new ObjectModel();

        var validationContext = new ValidationContext<ObjectModel>(model);

        Assert.False(propertyValidator.ShouldValidate(model, null, validationContext, null));

        model.Name = "Furion";
        Assert.True(propertyValidator.ShouldValidate(model, "Furion", validationContext, null));
    }

    [Fact]
    public void ShouldValidate_WithAllowEmptyStrings_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);
        var model = new ObjectModel();

        var validationContext = new ValidationContext<ObjectModel>(model);

        model.Name = null;
        Assert.True(propertyValidator.ShouldValidate(model, null, validationContext, null));
        model.Name = string.Empty;
        Assert.True(propertyValidator.ShouldValidate(model, string.Empty, validationContext, null));
        model.Name = "Furion";
        Assert.True(propertyValidator.ShouldValidate(model, "Furion", validationContext, null));

        propertyValidator.AllowEmptyStrings();
        model.Name = null;
        Assert.True(propertyValidator.ShouldValidate(model, null, validationContext, null));
        model.Name = string.Empty;
        Assert.False(propertyValidator.ShouldValidate(model, string.Empty, validationContext, null));
        model.Name = "Furion";
        Assert.True(propertyValidator.ShouldValidate(model, "Furion", validationContext, null));
    }

    [Fact]
    public void ShouldRunAttributeValidation_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.True(propertyValidator.ShouldRunAttributeValidation());

        objectValidator.Options.SuppressAttributeValidation = true;
        Assert.False(propertyValidator.ShouldRunAttributeValidation());

        objectValidator.Options.SuppressAttributeValidation = false;
        propertyValidator.SkipAttributeValidation();
        Assert.False(propertyValidator.ShouldRunAttributeValidation());
    }

    [Fact]
    public void GetValue_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.GetValue(null!));
    }

    [Fact]
    public void GetValue_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        var model = new ObjectModel { Name = "Furion" };
        Assert.Equal("Furion", propertyValidator.GetValue(model));
    }

    [Fact]
    public void GetValidatingValue_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.GetValidatingValue(null!));
    }

    [Fact]
    public void GetValidatingValue_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        var model = new ObjectModel { Name = " Furion " };
        Assert.Equal(" Furion ", propertyValidator.GetValidatingValue(model));

        propertyValidator.PreProcess(u => u?.Trim());
        Assert.Equal("Furion", propertyValidator.GetValidatingValue(model));
    }

    [Fact]
    public void GetDisplayName_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();

        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);
        Assert.Equal("Name", propertyValidator.GetDisplayName());
        propertyValidator.WithDisplayName("CustomName");
        Assert.Equal("CustomName", propertyValidator.GetDisplayName());

        var propertyValidator2 = new PropertyValidator<ObjectModel, string?>(u => u.ChinaAddress, objectValidator);
        Assert.Equal("Address", propertyValidator2.GetDisplayName());

        var propertyValidator3 = new PropertyValidator<ObjectModel, int>(u => u.YourAge, objectValidator);
        Assert.Equal("Age", propertyValidator3.GetDisplayName());

        var propertyValidator4 =
            new PropertyValidator<ObjectModel, int>(u => u.YourAge, objectValidator).WithName("MyAge");
        Assert.Equal("MyAge", propertyValidator4.GetDisplayName());

        var propertyValidator5 = new PropertyValidator<ObjectModel, int>(u => u.YourAge, objectValidator)
            .WithName("MyAge").WithDisplayName("DisplayAge");
        Assert.Equal("DisplayAge", propertyValidator5.GetDisplayName());
    }

    [Fact]
    public void GetMemberPath_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();

        var propertyValidator = new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator);
        Assert.Equal("Sub", propertyValidator.GetMemberPath());
    }

    [Fact]
    public void GetEffectiveMemberName_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();

        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator).WithName("xxx");
        Assert.Equal("xxx", propertyValidator.GetEffectiveMemberName());
    }

    [Fact]
    public void RepairMemberPaths_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator).ChildRules(c =>
            {
                c.RuleFor(u => u.Id).Min(3);
            });
        propertyValidator.RepairMemberPaths("Sub");
        Assert.True(propertyValidator.Validators.OfType<ObjectValidator<SubModel>>().Any());
        var propertyObjectValidator =
            propertyValidator.Validators.OfType<ObjectValidator<SubModel>>().First();

        Assert.Equal("Sub", propertyObjectValidator._memberPath);
        var subPropertyValidator =
            propertyObjectValidator.Validators[0] as PropertyValidator<SubModel, int>;
        Assert.NotNull(subPropertyValidator);
        Assert.Equal("Sub.Id", subPropertyValidator.GetMemberPath());
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var subValidator = new SubModelValidator();
        var propertyValidator =
            new PropertyValidator<ObjectModel, SubModel>(u => u.Sub, objectValidator)
                .WithAttributes(new RequiredAttribute()).SetValidator(subValidator);

        Assert.Null(propertyValidator._serviceProvider);
        Assert.Null(propertyValidator._attributeValidator._serviceProvider);
        Assert.Null(subValidator._serviceProvider);
        var valueValidator = propertyValidator.Validators[0] as AttributeValueValidator;
        Assert.NotNull(valueValidator);
        Assert.Null(valueValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        propertyValidator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(propertyValidator._serviceProvider);
        Assert.NotNull(propertyValidator._attributeValidator._serviceProvider);
        Assert.NotNull(subValidator._serviceProvider);
        Assert.NotNull(valueValidator);
        Assert.NotNull(valueValidator._serviceProvider);
    }

    [Fact]
    public void AllowEmptyStrings_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);
        Assert.Null(propertyValidator._allowEmptyStrings);
        propertyValidator.AllowEmptyStrings();
        Assert.True(propertyValidator._allowEmptyStrings);
        propertyValidator.AllowEmptyStrings(false);
        Assert.False(propertyValidator._allowEmptyStrings);
    }

    [Fact]
    public void Clone_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.Clone(null!));
    }

    [Fact]
    public void Clone_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ObjectModel>().RuleFor(u => u.Name).Required().MinLength(3)
            .PreProcess(u => u.Trim()).AllowEmptyStrings();

        using var objectValidator = new ObjectValidator<ObjectModel>();
        var cloned = propertyValidator.Clone(objectValidator) as PropertyValidator<ObjectModel, string?>;
        Assert.NotNull(cloned);
        Assert.Equal(2, cloned.Validators.Count);
        Assert.NotNull(cloned._preProcessor);
        Assert.True(cloned._allowEmptyStrings);
    }

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        var validationContext = propertyValidator.CreateValidationContext("Furion", null);
        Assert.NotNull(validationContext);
        Assert.Equal("Furion", validationContext.Instance);
        Assert.Equal("Name", validationContext.DisplayName);
        Assert.Equal(["Name"], validationContext.MemberNames);
        Assert.Null(validationContext.RuleSets);
        Assert.Empty(validationContext.Items);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        propertyValidator.InitializeServiceProvider(serviceProvider.GetService);
        Assert.NotNull(propertyValidator._serviceProvider);

        var validationContext2 = propertyValidator.CreateValidationContext("Furion", ["Login"]);
        Assert.NotNull(validationContext2);
        Assert.Equal("Furion", validationContext2.Instance);
        Assert.Equal("Name", validationContext2.DisplayName);
        Assert.Equal(["Name"], validationContext2.MemberNames);
        Assert.Equal<string>(["Login"], validationContext2.RuleSets!);
        Assert.Empty(validationContext2.Items);
        Assert.NotNull(validationContext2._serviceProvider);
    }

    public class ObjectModel
    {
        [Required] [MinLength(3)] public string? Name { get; set; }

        public string? FirstName { get; set; }

        [Display(Name = "Address")] public string? ChinaAddress { get; set; }

        [DisplayName("Age")] public int YourAge { get; set; }

        public SubModel? Sub { get; set; }

        public List<Child>? Children { get; set; }
    }

    public class SubModel
    {
        public int Id { get; set; }

        public Nested? Nest { get; set; }
    }

    public class SubModelValidator : AbstractValidator<SubModel>
    {
        public SubModelValidator()
        {
            RuleFor(u => u.Id).GreaterThanOrEqualTo(3);

            RuleSet("login", () =>
            {
                RuleFor(u => u.Id).GreaterThanOrEqualTo(3);
            });

            RuleFor(u => u.Nest).ChildRules(n =>
            {
                n.RuleFor(b => b.Id).Min(1);
            });
        }
    }

    public class Nested
    {
        public int Id { get; set; }
    }

    public class Child
    {
        public int Id { get; set; }
    }
}