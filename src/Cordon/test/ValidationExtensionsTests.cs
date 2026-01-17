// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidationExtensionsTests
{
    [Fact]
    public void ToResults_ReturnOK()
    {
        var validationResults = new List<ValidationResult>();
        Assert.Null(validationResults.ToResults());

        validationResults.Add(new ValidationResult("验证失败"));
        Assert.NotNull(validationResults.ToResults());
        Assert.Single(validationResults);

        var validationResult2 = Enumerable.Empty<ValidationResult>();
        Assert.Null(validationResult2.ToResults());

        IEnumerable<ValidationResult> validationResult3 = [new("验证失败")];
        var results = validationResult3.ToResults();
        Assert.NotNull(results);
        Assert.Single(results);

        List<ValidationResult>? validationResults4 = null;
        Assert.Null(validationResults4.ToResults());

        IEnumerable<ValidationResult>? validationResults5 = null;
        Assert.Null(validationResults5.ToResults());
    }

    [Fact]
    public void WithRuleSets_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ValidationExtensions.WithRuleSets(null!));

    [Fact]
    public void With_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ValidationExtensions.With<ObjectModel>(null!));

    [Fact]
    public void ContinueWith_ReturnOK()
    {
        var validationContext = new ValidationContext(new ObjectModel(), null, null);
        var objectValidator = validationContext.With<Tests.ObjectModel>();
        Assert.NotNull(objectValidator);
        Assert.NotNull(objectValidator.Items);
        Assert.Single(objectValidator.Items);
        Assert.Equal(validationContext, objectValidator.Items[Constants.ValidationContextKey]);
    }

    [Fact]
    public void WithRuleSets_ReturnOK()
    {
        var validationContext = new ValidationContext(new object(), null, null);
        Assert.Empty(validationContext.Items);

        validationContext.WithRuleSets();
        Assert.Single(validationContext.Items);
        var metadata = validationContext.Items[Constants.ValidationOptionsKey] as ValidationOptionsMetadata;
        Assert.NotNull(metadata);
        Assert.Null(metadata.RuleSets);

        validationContext.WithRuleSets(["login", "register"]);
        Assert.Single(validationContext.Items);

        var metadata2 =
            validationContext.Items[Constants.ValidationOptionsKey] as ValidationOptionsMetadata;
        Assert.NotNull(metadata2);
        Assert.Equal(["login", "register"], (string[]?)metadata2.RuleSets!);
    }

    [Fact]
    public void ValidateWith_WithAction_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationExtensions.ValidateWith<ObjectModel>(null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new ValidationContext(new ObjectModel()).ValidateWith<ObjectModel>(null!));
        Assert.Throws<InvalidCastException>(() =>
            new ValidationContext(new object(), null, null).ValidateWith<ObjectModel>(_ => { }));
    }

    [Fact]
    public void ValidateWith_WithAction_ReturnOK()
    {
        var model1 = new ObjectModel();
        var exception = Assert.Throws<ValidationException>(() =>
            Validator.ValidateObject(model1, new ValidationContext(model1, null, null), true));
        Assert.Equal("The field Id must be greater than or equal to '1'.", exception.Message);

        var model2 = new ObjectModel { Id = 1 };
        var exception2 = Assert.Throws<ValidationException>(() =>
            Validator.ValidateObject(model2, new ValidationContext(model2, null, null), true));
        Assert.Equal("The Name field is required.", exception2.Message);

        var model3 = new ObjectModel { Id = 1, Name = "Fu" };
        var exception3 = Assert.Throws<ValidationException>(() =>
            Validator.ValidateObject(model3, new ValidationContext(model3, null, null), true));
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.", exception3.Message);

        var model4 = new ObjectModel { Id = 1, Name = "百小僧" };
        var exception4 = Assert.Throws<ValidationException>(() =>
            Validator.ValidateObject(model4, new ValidationContext(model4, null, null), true));
        Assert.Equal("The field Name is not a valid username.", exception4.Message);

        var model5 = new ObjectModel { Id = 1, Name = "百小僧" };
        var exception5 = Assert.Throws<ValidationException>(() =>
            Validator.ValidateObject(model5,
                new ValidationContext(model5, null,
                    new Dictionary<object, object?>
                    {
                        { Constants.ValidationOptionsKey, new ValidationOptionsMetadata(["email"]) }
                    }),
                true));
        Assert.Equal("The Name field is not a valid e-mail address.", exception5.Message);

        var services = new ServiceCollection();
        services.AddValidationCore();
        using var serviceProvider = services.BuildServiceProvider();
        var validationDataContext = serviceProvider.GetRequiredService<IValidationDataContext>();
        validationDataContext.SetValue(Constants.ValidationOptionsKey,
            new ValidationOptionsMetadata(["email"]));

        var model6 = new ObjectModel { Id = 1, Name = "百小僧" };
        var exception6 = Assert.Throws<ValidationException>(() =>
            Validator.ValidateObject(model6,
                new ValidationContext(model6, serviceProvider, null), true));
        Assert.Equal("The Name field is not a valid e-mail address.", exception6.Message);
    }

    [Fact]
    public void ValidateWith_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationExtensions.ValidateWith<ObjectModel>(null!, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new ValidationContext(new ObjectModel(), null, null).ValidateWith<ObjectModel>(null!));
        Assert.Throws<InvalidCastException>(() =>
            new ValidationContext(new object(), null, null).ValidateWith(new ObjectModelValidator()));
        Assert.Throws<ArgumentNullException>(() => ValidationExtensions.ValidateWith<ObjectModelValidator>(null!));
    }

    [Fact]
    public void ValidateWith_ReturnOK()
    {
        var validationContext = new ValidationContext(new ObjectModel(), null, null);
        var objectValidator = new ObjectModelValidator();
        Assert.Empty(validationContext.ValidateWith(objectValidator));

        var validationContext2 = new ValidationContext(new ObjectModel { Name = "Fu" }, null, null);
        var validationResults = validationContext2.ValidateWith(objectValidator).ToList();
        Assert.Equal(2, validationResults.Count);
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.",
            validationResults.First().ErrorMessage);
        Assert.Equal("The field Name is not a valid username.", validationResults.Last().ErrorMessage);

        var validationContext3 = new ValidationContext(new ObjectModel { Name = "Fu" }, null, null);
        var validationResults2 = validationContext3.ValidateWith<ObjectModelValidator>().ToList();
        Assert.Equal(2, validationResults2.Count);
        Assert.Equal("The field Name must be a string or array type with a minimum length of '3'.",
            validationResults2.First().ErrorMessage);
        Assert.Equal("The field Name is not a valid username.", validationResults2.Last().ErrorMessage);
    }

    public class ObjectModel : IValidatableObject
    {
        [Min(1)] public int Id { get; set; }

        [Required] public string? Name { get; set; }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            validationContext.ValidateWith<ObjectModel>(validator =>
            {
                validator.RuleFor(u => u.Name).MinLength(3).UserName()
                    .RuleSet("email", () => validator.RuleFor(u => u.Name).EmailAddress());
            });
    }

    public class ObjectModelValidator : AbstractValidator<ObjectModel>
    {
        public ObjectModelValidator() =>
            RuleFor(u => u.Name).MinLength(3).UserName()
                .RuleSet("email", () => RuleFor(u => u.Name).EmailAddress());
    }
}