// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidationServiceTests
{
    [Fact]
    public void New_Invalid_Parameters() => Assert.Throws<ArgumentNullException>(() => new ValidationService(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);
        Assert.NotNull(validationService);
        Assert.NotNull(validationService._serviceProvider);
        Assert.NotNull(validationService._attributeValidator);

        var validationService2 = new ValidationService();
        Assert.NotNull(validationService2);
        Assert.Null(validationService2._serviceProvider);
        Assert.NotNull(validationService2._attributeValidator);
    }

    [Fact]
    public void New_FromDI_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddTransient<IValidationService, ValidationService>();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = serviceProvider.GetRequiredService<IValidationService>();
        Assert.NotNull(validationService);
        Assert.NotNull((validationService as ValidationService)?._serviceProvider);
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var validationService = new ValidationService(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => validationService.IsValid(null!));
        Assert.Throws<ArgumentNullException>(() => validationService.IsValid((object?)null));
        Assert.Throws<ArgumentNullException>(() => validationService.IsValid([null]));
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);

        Assert.False(validationService.IsValid(new ObjectModel()));
        Assert.False(validationService.IsValid(new ObjectModel { Name = "Furion" }));
        Assert.False(validationService.IsValid(new ObjectModel { Name = "Furion", SomeProperty = "百小僧" }));
        Assert.True(
            validationService.IsValid(new ObjectModel { Name = "Furion", SomeProperty = "monksoul@outlook.com" }));
    }

    [Fact]
    public void IsValid_WithNoDI_ReturnOK()
    {
        var validationService = new ValidationService();

        Assert.False(validationService.IsValid(new ObjectModel()));
        Assert.False(validationService.IsValid(new ObjectModel { Name = "Furion" }));
        Assert.False(validationService.IsValid(new ObjectModel { Name = "Furion", SomeProperty = "百小僧" }));
        Assert.True(
            validationService.IsValid(new ObjectModel { Name = "Furion", SomeProperty = "monksoul@outlook.com" }));
    }

    [Fact]
    public void IsValid_WithMany_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);

        Assert.False(validationService.IsValid([new ObjectModel(), new OtherModel()]));
        Assert.True(validationService.IsValid([
            new ObjectModel { Name = "Furion", SomeProperty = "monksoul@outlook.com" },
            new OtherModel { Name = "Furion" }
        ]));
    }

    [Fact]
    public void GetValidationResults_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => validationService.GetValidationResults(null!));
        Assert.Throws<ArgumentNullException>(() => validationService.GetValidationResults((object?)null));
        Assert.Throws<ArgumentNullException>(() => validationService.GetValidationResults([null]));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);

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
    public void GetValidationResults_WithNoDI_ReturnOK()
    {
        var validationService = new ValidationService();

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
    public void GetValidationResults_WithMany_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);

        var validationResults = validationService.GetValidationResults([new ObjectModel(), new OtherModel()]);
        Assert.NotNull(validationResults);
        Assert.Equal(2, validationResults.Count);
        Assert.Equal(["The Name field is required.", "The Name field is required."],
            validationResults.Select(u => u.ErrorMessage!).ToArray());

        Assert.Null(validationService.GetValidationResults([
            new ObjectModel { Name = "Furion", SomeProperty = "monksoul@outlook.com" },
            new OtherModel { Name = "Furion" }
        ]));
    }

    [Fact]
    public void Validate_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => validationService.Validate(null!));
        Assert.Throws<ArgumentNullException>(() => validationService.Validate((object?)null));
        Assert.Throws<ArgumentNullException>(() => validationService.Validate([null]));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);

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
    public void Validate_WithNoDI_ReturnOK()
    {
        var validationService = new ValidationService();

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
    public void Validate_WithMany_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validationService = new ValidationService(serviceProvider);

        var exception =
            Assert.Throws<ValidationException>(() => validationService.Validate([new ObjectModel(), new OtherModel()]));
        Assert.Equal("The Name field is required.", exception.Message);

        validationService.Validate([
            new ObjectModel { Name = "Furion", SomeProperty = "monksoul@outlook.com" },
            new OtherModel { Name = "Furion" }
        ]);
    }

    [Fact]
    public void CreateValidationContext_Invalid_Parameters()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var validationService = new ValidationService(serviceProvider);

        Assert.Throws<ArgumentNullException>(() => validationService.CreateValidationContext(null!, null));
    }

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var validationService = new ValidationService(serviceProvider);

        var validationContext = validationService.CreateValidationContext(new ObjectModel(), null);
        Assert.NotNull(validationContext);
        Assert.Empty(validationContext.Items);
        Assert.NotNull(validationContext.GetService(typeof(IServiceProvider)));

        var validationContext2 = validationService.CreateValidationContext(new ObjectModel(), ["login"]);
        Assert.Empty(validationContext2.Items);
        Assert.Equal(["login"], (string[]?)validationContext2.RuleSets!);
    }

    [Fact]
    public void CreateValidationContext_WithNoDI_ReturnOK()
    {
        var validationService = new ValidationService();

        var validationContext = validationService.CreateValidationContext(new ObjectModel(), null);
        Assert.NotNull(validationContext);
        Assert.Empty(validationContext.Items);
        Assert.Null(validationContext.GetService(typeof(IServiceProvider)));

        var validationContext2 = validationService.CreateValidationContext(new ObjectModel(), ["login"]);
        Assert.Empty(validationContext2.Items);
        Assert.Equal(["login"], (string[]?)validationContext2.RuleSets!);
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

    public class OtherModel
    {
        [Required] [MinLength(2)] public string? Name { get; set; }
    }
}