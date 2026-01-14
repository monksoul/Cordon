// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class CollectionPropertyValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);
        Assert.NotNull(propertyValidator);
        Assert.NotNull(propertyValidator._objectValidator);
        Assert.NotNull(propertyValidator._attributeValidator);
        Assert.Null(propertyValidator._elementFilter);
    }

    [Fact]
    public void Where_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        propertyValidator.Where(null);
        Assert.Null(propertyValidator._elementFilter);

        propertyValidator.Where(u => u.Name is not null);
        Assert.NotNull(propertyValidator._elementFilter);
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.IsValid(null!));
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator).ChildRules(c =>
                c.RuleFor(o => o.Name).MinLength(3));

        Assert.True(propertyValidator.IsValid(new ObjectModel()));
        Assert.False(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Fu" }] }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Furion" }] }));
    }

    [Fact]
    public void IsValid_WithWhere_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator).ChildRules(c =>
                c.RuleFor(o => o.Name).MinLength(3)).Where(e => e.Name != "Fu");

        Assert.True(propertyValidator.IsValid(new ObjectModel()));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Fu" }] }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Furion" }] }));
    }

    [Fact]
    public void IsValid_WithRuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator)
            {
                RuleSets = ["rule"]
            }.ChildRules(c => c.RuleFor(o => o.Name).MinLength(3));

        Assert.True(propertyValidator.IsValid(new ObjectModel()));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Fu" }] }));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Furion" }] }));

        Assert.True(propertyValidator.IsValid(new ObjectModel(), ["*"]));
        Assert.False(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Fu" }] }, ["*"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Furion" }] }, ["*"]));

        Assert.True(propertyValidator.IsValid(new ObjectModel(), ["rule"]));
        Assert.False(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Fu" }] }, ["rule"]));
        Assert.True(propertyValidator.IsValid(new ObjectModel { Children = [new Child { Name = "Furion" }] },
            ["rule"]));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator).ChildRules(c =>
                c.RuleFor(o => o.Name).MinLength(3));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel()));

        var validationResults =
            propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Fu" }] });
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.",
            validationResults[0].ErrorMessage);

        Assert.Null(
            propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Furion" }] }));
    }

    [Fact]
    public void GetValidationResults_WithWhere_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator).ChildRules(c =>
                c.RuleFor(o => o.Name).MinLength(3)).Where(e => e.Name != "Fu");

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel()));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Fu" }] }));
        Assert.Null(
            propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Furion" }] }));
    }

    [Fact]
    public void GetValidationResults_WithRuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator)
            {
                RuleSets = ["rule"]
            }.ChildRules(c => c.RuleFor(o => o.Name).MinLength(3));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel()));
        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Fu" }] }));
        Assert.Null(
            propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Furion" }] }));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel(), ["*"]));

        var validationResults =
            propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Fu" }] }, ["*"]);
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.",
            validationResults[0].ErrorMessage);

        Assert.Null(
            propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Furion" }] },
                ["*"]));

        Assert.Null(propertyValidator.GetValidationResults(new ObjectModel(), ["rule"]));

        var validationResults2 =
            propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Fu" }] }, ["*"]);
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.",
            validationResults2[0].ErrorMessage);

        Assert.Null(
            propertyValidator.GetValidationResults(new ObjectModel { Children = [new Child { Name = "Furion" }] },
                ["rule"]));
    }


    [Fact]
    public void Validate_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator).ChildRules(c =>
                c.RuleFor(o => o.Name).MinLength(3));

        propertyValidator.Validate(new ObjectModel());

        var exception = Assert.Throws<ValidationException>(() =>
            propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Fu" }] }));
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.", exception.Message);

        propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Furion" }] });
    }

    [Fact]
    public void Validate_WithWhere_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator).ChildRules(c =>
                c.RuleFor(o => o.Name).MinLength(3)).Where(e => e.Name != "Fu");

        propertyValidator.Validate(new ObjectModel());
        propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Fu" }] });
        propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Furion" }] });
    }

    [Fact]
    public void Validate_WithRuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator)
            {
                RuleSets = ["rule"]
            }.ChildRules(c => c.RuleFor(o => o.Name).MinLength(3));

        propertyValidator.Validate(new ObjectModel());
        propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Fu" }] });
        propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Furion" }] });

        propertyValidator.Validate(new ObjectModel(), ["*"]);

        var exception = Assert.Throws<ValidationException>(() =>
            propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Fu" }] }, ["*"]));
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.", exception.Message);

        propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Furion" }] }, ["*"]);

        propertyValidator.Validate(new ObjectModel(), ["rule"]);

        var exception2 = Assert.Throws<ValidationException>(() =>
            propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Fu" }] }, ["*"]));
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.", exception2.Message);

        propertyValidator.Validate(new ObjectModel { Children = [new Child { Name = "Furion" }] }, ["rule"]);
    }

    [Fact]
    public void SetValidator_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        Assert.Throws<ArgumentNullException>(() =>
            propertyValidator.SetValidator(
                (Func<string?[]?, IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<Child>?>)null!));

        propertyValidator.SetValidator(new ChildValidator());

        var exception =
            Assert.Throws<InvalidOperationException>(() => propertyValidator.SetValidator(new ChildValidator()));
        Assert.Equal(
            "An element validator has already been assigned. Only one is allowed per collection element. To configure nested rules, use `ChildRules` or `EachRules` within a single validator.",
            exception.Message);

        var propertyValidator2 =
            new CollectionPropertyValidator<ObjectModel, string?>(u => u.Names, objectValidator);
        var exception2 =
            Assert.Throws<InvalidOperationException>(() =>
                propertyValidator2.SetValidator(new ObjectValidator<string?>()));
        Assert.Equal(
            "Collection element type 'System.String' is not a reference type. `SetValidator` (object validator) and `ChildRules` are only supported for class types. For value types, use `EachRules` or `SetValidator` (value validator) instead.",
            exception2.Message);
    }

    [Fact]
    public void SetValidator_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        Assert.False(propertyValidator.Validators.OfType<CollectionValidator<Child>>().Any());
        propertyValidator.SetValidator(new ChildValidator());
        Assert.True(propertyValidator.Validators.OfType<CollectionValidator<Child>>().Any());

        var collectionValidator =
            propertyValidator.Validators.OfType<CollectionValidator<Child>>().First();
        Assert.True(collectionValidator.IsNested);
        var elementValidator = collectionValidator._elementValidator as ObjectValidator<Child>;
        Assert.NotNull(elementValidator);
        Assert.Null(elementValidator.InheritedRuleSets);
        Assert.Throws<InvalidOperationException>(() =>
            propertyValidator.SetValidator((ObjectValidator<Child>?)null));

        var propertyValidator2 =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator)
            {
                RuleSets = ["login"]
            };

        Assert.False(propertyValidator2.Validators.OfType<CollectionValidator<Child>>().Any());
        propertyValidator2.SetValidator((_, _, _) => new ChildValidator());
        Assert.True(propertyValidator2.Validators.OfType<CollectionValidator<Child>>().Any());

        var collectionValidator2 =
            propertyValidator2.Validators.OfType<CollectionValidator<Child>>().First();
        var elementValidator2 = collectionValidator2._elementValidator as ObjectValidator<Child>;
        Assert.NotNull(elementValidator2);
        Assert.NotNull(elementValidator2.InheritedRuleSets);
        Assert.Equal(["login"], (string[]?)elementValidator2.InheritedRuleSets!);

        var propertyValidator3 =
            new CollectionPropertyValidator<ObjectModel, string?>(u => u.Names, objectValidator).SetValidator(
                new StringValidator());

        var collectionValidator3 =
            propertyValidator3.Validators.OfType<CollectionValidator<string?>>().First();
        var elementValidator3 = collectionValidator3._elementValidator as ValueValidator<string?>;
        Assert.NotNull(elementValidator3);

        var propertyValidator4 =
            new CollectionPropertyValidator<ObjectModel, string?>(u => u.Names, objectValidator).SetValidator(_ =>
                new StringValidator());
        var collectionValidator4 =
            propertyValidator4.Validators.OfType<CollectionValidator<string?>>().First();
        Assert.True(collectionValidator4.IsNested);
        var elementValidator4 = collectionValidator4._elementValidator as ValueValidator<string?>;
        Assert.NotNull(elementValidator4);
    }

    [Fact]
    public void ChildRules_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);
        Assert.Throws<ArgumentNullException>(() => propertyValidator.ChildRules(null!));

        propertyValidator.SetValidator(new ChildValidator());

        var exception = Assert.Throws<InvalidOperationException>(() => propertyValidator.ChildRules(_ => { }));
        Assert.Equal(
            "An element validator has already been assigned. Only one is allowed per collection element. To configure nested rules, use `ChildRules` or `EachRules` within a single validator.",
            exception.Message);
    }

    [Fact]
    public void ChildRules_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);
        propertyValidator.ChildRules(o => o.RuleFor(u => u.Name).Required());
        Assert.True(propertyValidator.Validators.OfType<CollectionValidator<Child>>().Any());

        var collectionValidator =
            propertyValidator.Validators.OfType<CollectionValidator<Child>>().First();
        var elementValidator = collectionValidator._elementValidator as ObjectValidator<Child>;
        Assert.NotNull(elementValidator);
        Assert.Null(elementValidator.InheritedRuleSets);
        Assert.Null(elementValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        propertyValidator.InitializeServiceProvider(serviceProvider.GetService);
        Assert.NotNull(elementValidator._serviceProvider);

        Assert.False(elementValidator.IsValid(new Child()));
        Assert.True(elementValidator.IsValid(new Child { Name = "Furion" }));

        var propertyValidator2 =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator)
            {
                RuleSets = ["login"]
            };
        propertyValidator2.ChildRules(_ => { });
        Assert.True(propertyValidator2.Validators.OfType<CollectionValidator<Child>>().Any());

        var collectionValidator2 =
            propertyValidator2.Validators.OfType<CollectionValidator<Child>>().First();
        var elementValidator2 = collectionValidator2._elementValidator as ObjectValidator<Child>;
        Assert.NotNull(elementValidator2);
        Assert.NotNull(elementValidator2.InheritedRuleSets);
        Assert.Equal(["login"], (string[]?)elementValidator2.InheritedRuleSets!);
    }

    [Fact]
    public void EachRules_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.EachRules(null!));

        var propertyValidator2 =
            new CollectionPropertyValidator<ObjectModel, string?>(u => u.Names, objectValidator).EachRules(_ => { });
        var exception = Assert.Throws<InvalidOperationException>(() => propertyValidator2.EachRules(_ => { }));
        Assert.Equal(
            "An element validator has already been assigned. Only one is allowed per collection element. To configure nested rules, use `ChildRules` or `EachRules` within a single validator.",
            exception.Message);
    }

    [Fact]
    public void EachRules_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, string?>(u => u.Names, objectValidator).EachRules(u =>
                u.Required().MinLength(3));

        Assert.True(propertyValidator.Validators.OfType<CollectionValidator<string?>>().Any());

        var collectionValidator =
            propertyValidator.Validators.OfType<CollectionValidator<string?>>().First();
        var elementValidator = collectionValidator._elementValidator as ValueValidator<string?>;
        Assert.NotNull(elementValidator);
        Assert.Equal(2, elementValidator.Validators.Count);
        Assert.Null(elementValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        propertyValidator.InitializeServiceProvider(serviceProvider.GetService);
        Assert.NotNull(elementValidator._serviceProvider);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var subValidator = new ChildValidator();

        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator)
                .SetValidator(subValidator)
                .WithAttributes(new RequiredAttribute());

        var propertyValidator2 =
            new CollectionPropertyValidator<ObjectModel, string?>(u => u.Names, objectValidator)
                .EachRules(u => u.Required().MinLength(3))
                .WithAttributes(new RequiredAttribute());

        Assert.Null(propertyValidator._serviceProvider);
        Assert.Null(propertyValidator._attributeValidator._serviceProvider);
        Assert.Null(subValidator._serviceProvider);
        var valueValidator = propertyValidator.Validators.FirstOrDefault() as AttributeValueValidator;
        Assert.NotNull(valueValidator);
        Assert.Null(valueValidator._serviceProvider);


        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        propertyValidator.InitializeServiceProvider(serviceProvider.GetService);
        propertyValidator2.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(propertyValidator._serviceProvider);
        Assert.NotNull(propertyValidator._attributeValidator._serviceProvider);
        Assert.NotNull(subValidator._serviceProvider);
        Assert.NotNull(valueValidator);
        Assert.NotNull(valueValidator._serviceProvider);
    }

    [Fact]
    public void Clone_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ObjectModel>().RuleForCollection(u => u.Children).Required()
            .MinLength(3)
            .PreProcess(u => u).Where(u => u.Name is not null).AllowEmptyStrings();

        using var objectValidator = new ObjectValidator<ObjectModel>();
        var cloned = propertyValidator.Clone(objectValidator) as PropertyValidator<ObjectModel, IEnumerable<Child>>;
        Assert.NotNull(cloned);
        Assert.Equal(2, cloned.Validators.Count);
        Assert.NotNull(cloned._preProcessor);
        Assert.True(cloned._allowEmptyStrings);
    }

    [Fact]
    public void EnsureNoElementValidatorAssigned_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ObjectModel>().RuleForCollection(u => u.Children)
            .SetValidator(new ChildValidator());
        var exception =
            Assert.Throws<InvalidOperationException>(() => propertyValidator.EnsureNoElementValidatorAssigned());
        Assert.Equal(
            "An element validator has already been assigned. Only one is allowed per collection element. To configure nested rules, use `ChildRules` or `EachRules` within a single validator.",
            exception.Message);
    }

    public class ObjectModel
    {
        public List<Child>? Children { get; set; }

        public List<string?>? Names { get; set; }
    }

    public class Child
    {
        public string? Name { get; set; }

        public Nested? Nest { get; set; }
    }

    public class Nested
    {
        public int Id { get; set; }
    }

    public class ChildValidator : AbstractValidator<Child>;

    public class StringValidator : AbstractValueValidator<string?>;
}