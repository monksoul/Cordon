// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class CollectionPropertyValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);
        Assert.NotNull(propertyValidator);
        Assert.Null(propertyValidator._elementValidator);
        Assert.NotNull(propertyValidator._objectValidator);
        Assert.NotNull(propertyValidator._annotationValidator);
        Assert.Null(propertyValidator.ElementFilter);
    }

    [Fact]
    public void Where_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.Where((Func<Child?, bool>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            propertyValidator.Where((Func<Child?, ValidationContext<ObjectModel>, bool>)null!));
    }

    [Fact]
    public void Where_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        propertyValidator.Where(u => u.Name is not null);
        Assert.NotNull(propertyValidator.ElementFilter);

        var propertyValidator2 = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        propertyValidator2.Where((_, ctx) => ctx.Instance.Children?.All(c => c.Name is not null) == true);
        Assert.NotNull(propertyValidator2.ElementFilter);
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
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator).SetValidator(
                new ChildValidator());

        Assert.Throws<ArgumentNullException>(() =>
            propertyValidator.SetValidator(
                (Func<string?[]?, IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<Child>?>)null!));

        var exception =
            Assert.Throws<InvalidOperationException>(() => propertyValidator.SetValidator(new ChildValidator()));
        Assert.Equal(
            "An object validator has already been assigned to this element. Only one object validator is allowed per element. To define nested rules, use `ChildRules` within a single validator.",
            exception.Message);
    }

    [Fact]
    public void SetValidator_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        Assert.Null(propertyValidator._elementValidator);
        propertyValidator.SetValidator(new ChildValidator());
        Assert.NotNull(propertyValidator._elementValidator);
        Assert.Null(propertyValidator._elementValidator.InheritedRuleSets);
        Assert.Throws<InvalidOperationException>(() =>
            propertyValidator.SetValidator((ObjectValidator<Child>?)null));
        Assert.Equal("Children", propertyValidator._elementValidator.MemberPath);

        var propertyValidator2 =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator)
            {
                RuleSets = ["login"]
            };

        Assert.Null(propertyValidator2._elementValidator);
        propertyValidator2.SetValidator((_, _, _) => new ChildValidator());
        Assert.NotNull(propertyValidator2._elementValidator);
        Assert.NotNull(propertyValidator2._elementValidator.InheritedRuleSets);
        Assert.Equal(["login"], (string[]?)propertyValidator2._elementValidator.InheritedRuleSets!);
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
            "An object validator has already been assigned to this element. `ChildRules` cannot be applied after `SetValidator` or another `ChildRules` call.",
            exception.Message);
    }

    [Fact]
    public void ChildRules_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);
        propertyValidator.ChildRules(o => o.RuleFor(u => u.Name).Required());
        Assert.NotNull(propertyValidator._elementValidator);
        Assert.Null(propertyValidator._elementValidator.InheritedRuleSets);

        Assert.False(propertyValidator._elementValidator.IsValid(new Child()));
        Assert.True(propertyValidator._elementValidator.IsValid(new Child { Name = "Furion" }));

        var propertyValidator2 =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator)
            {
                RuleSets = ["login"]
            };
        propertyValidator2.ChildRules(_ => { });
        Assert.NotNull(propertyValidator2._elementValidator);
        Assert.NotNull(propertyValidator2._elementValidator.InheritedRuleSets);
        Assert.Equal(["login"], (string[]?)propertyValidator2._elementValidator.InheritedRuleSets!);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var subValidator = new ChildValidator();
        var collectionPropertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator)
                .SetValidator(subValidator)
                .AddAnnotations(new RequiredAttribute());

        var propertyValidator = collectionPropertyValidator as CollectionPropertyValidator<ObjectModel, Child>;
        Assert.NotNull(propertyValidator);

        Assert.Null(propertyValidator._serviceProvider);
        Assert.Null(propertyValidator._annotationValidator._serviceProvider);
        Assert.Null(subValidator._serviceProvider);
        var valueValidator = propertyValidator.Validators[0] as ValueAnnotationValidator;
        Assert.NotNull(valueValidator);
        Assert.Null(valueValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        propertyValidator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(propertyValidator._serviceProvider);
        Assert.NotNull(propertyValidator._annotationValidator._serviceProvider);
        Assert.NotNull(subValidator._serviceProvider);
        Assert.NotNull(valueValidator);
        Assert.NotNull(valueValidator._serviceProvider);
    }

    [Fact]
    public void GetValidatedElements_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        List<Child?> children = [new(), new(), null];
        Assert.Equal(3, propertyValidator.GetValidatedElements(children!, new ObjectModel()).Count());

        Assert.Equal(2,
            propertyValidator.Where(u => (Child?)u is not null).GetValidatedElements(children!, new ObjectModel())
                .Count());
    }

    [Fact]
    public void ForEachValidatedElement_Invalid_Parameters()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator);

        Assert.Throws<ArgumentNullException>(() => propertyValidator.ForEachValidatedElement(null!, null!));
        Assert.Throws<ArgumentNullException>(() => propertyValidator.ForEachValidatedElement(new ObjectModel(), null!));
    }

    [Fact]
    public void ForEachValidatedElement_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator = objectValidator.RuleForEach(u => u.Children)
            .ChildRules(c => c.RuleFor(b => b.Nest).ChildRules(d => d.RuleFor(z => z.Id)));

        Assert.Equal("Children", propertyValidator._elementValidator!.MemberPath);

        var subPropertyValidator =
            propertyValidator._elementValidator!.Validators.Last() as PropertyValidator<Child, Nested>;
        Assert.NotNull(subPropertyValidator);
        Assert.Equal("Children.Nest", subPropertyValidator.GetMemberPath());

        var nestedPropertyValidator =
            subPropertyValidator._propertyValidator!.Validators.First() as PropertyValidator<Nested, int>;
        Assert.NotNull(nestedPropertyValidator);
        Assert.Equal("Children.Nest.Id", nestedPropertyValidator.GetMemberPath());
    }

    [Fact]
    public void RepairMemberPaths_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new CollectionPropertyValidator<ObjectModel, Child>(u => u.Children, objectValidator).ChildRules(c =>
                c.RuleFor(b => b.Name));
        propertyValidator.RepairMemberPaths();
        Assert.NotNull(propertyValidator._elementValidator);
        Assert.Equal("Children", propertyValidator._elementValidator.MemberPath);
        var subPropertyValidator =
            propertyValidator._elementValidator.Validators[0] as PropertyValidator<Child, string>;
        Assert.NotNull(subPropertyValidator);
        Assert.Equal("Children.Name", subPropertyValidator.GetMemberPath());
    }

    public class ObjectModel
    {
        public List<Child>? Children { get; set; }
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
}