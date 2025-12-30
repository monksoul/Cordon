// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidationServiceTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new ValidationService(null!));
        Assert.Throws<ArgumentNullException>(() => new ValidationService<ObjectModel>(null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);
        Assert.NotNull(validationService);
        Assert.NotNull(validationService._serviceProvider);

        var validationService2 = new ValidationService<ObjectModel>(serviceProvider);
        Assert.NotNull(validationService2);
        Assert.NotNull(validationService2._serviceProvider);
    }

    [Fact]
    public void For_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddTransient(typeof(IValidationService<>), typeof(ValidationService<>));
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);

        var validationService2 = validationService.For<ObjectModel>();
        Assert.NotNull(validationService2);
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService<ObjectModel>(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => validationService.IsValid(null!));
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService<ObjectModel>(serviceProvider);

        Assert.False(validationService.IsValid(new ObjectModel()));
        Assert.False(validationService.IsValid(new ObjectModel { Name = "Furion" }));
        Assert.False(validationService.IsValid(new ObjectModel { Name = "Furion", SomeProperty = "百小僧" }));
        Assert.True(
            validationService.IsValid(new ObjectModel { Name = "Furion", SomeProperty = "monksoul@outlook.com" }));
    }

    [Fact]
    public void GetValidationResults_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService<ObjectModel>(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => validationService.GetValidationResults(null!));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService<ObjectModel>(serviceProvider);

        var validationResults = validationService.GetValidationResults(new ObjectModel());
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(["The Name field is required."], validationResults.Select(u => u.ErrorMessage!).ToArray());

        var validationResults2 = validationService.GetValidationResults(new ObjectModel { Name = "Furion" });
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(["The SomeProperty field is required."],
            validationResults2.Select(u => u.ErrorMessage!).ToArray());

        var validationResults3 =
            validationService.GetValidationResults(new ObjectModel { Name = "Furion", SomeProperty = "百小僧" });
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal(["The SomeProperty field is not a valid e-mail address."],
            validationResults3.Select(u => u.ErrorMessage!).ToArray());

        Assert.Null(validationService.GetValidationResults(new ObjectModel
        {
            Name = "Furion", SomeProperty = "monksoul@outlook.com"
        }));
    }

    [Fact]
    public void Validate_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService<ObjectModel>(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => validationService.Validate(null!));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService<ObjectModel>(serviceProvider);

        var exception = Assert.Throws<ValidationException>(() => validationService.Validate(new ObjectModel()));
        Assert.Equal("The Name field is required.", exception.Message);

        var exception2 =
            Assert.Throws<ValidationException>(() => validationService.Validate(new ObjectModel { Name = "Furion" }));
        Assert.Equal("The SomeProperty field is required.", exception2.Message);

        var exception3 = Assert.Throws<ValidationException>(() =>
            validationService.Validate(new ObjectModel { Name = "Furion", SomeProperty = "百小僧" }));
        Assert.Equal("The SomeProperty field is not a valid e-mail address.",
            exception3.Message);

        validationService.Validate(new ObjectModel { Name = "Furion", SomeProperty = "monksoul@outlook.com" });
    }

    [Fact]
    public void CreateValidationContext_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var validationService = new ValidationService<ObjectModel>(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => validationService.CreateValidationContext(null!, null));
    }

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var validationService = new ValidationService<ObjectModel>(serviceProvider);

        var validationContext = validationService.CreateValidationContext(new ObjectModel(), null);
        Assert.NotNull(validationContext);
        Assert.Equal(typeof(ObjectModel), validationContext.ObjectType);
        Assert.Empty(validationContext.Items);
        Assert.NotNull(validationContext.GetService(typeof(IServiceProvider)));

        var validationContext2 = validationService.CreateValidationContext(new ObjectModel(), ["login"]);
        Assert.Single(validationContext2.Items);
        var metadata2 =
            validationContext2.Items[ValidationDataContext.ValidationOptionsKey] as ValidationOptionsMetadata;
        Assert.NotNull(metadata2);
        Assert.Equal(["login"], (string[]?)metadata2.RuleSets!);
    }

    public class ObjectModel : IValidatableObject
    {
        [Required] [MinLength(2)] public string? Name { get; set; }

        public string? SomeProperty { get; set; }

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) =>
            validationContext.With<ObjectModel>()
                .RuleFor(u => u.SomeProperty).Required().EmailAddress()
                .ToResults();
    }
}