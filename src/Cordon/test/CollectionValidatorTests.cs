// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class CollectionValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new CollectionValidator<string?>(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        Assert.NotNull(collectionValidator._elementValidator);
        Assert.True(collectionValidator._elementValidator is ValueValidator<string?>);
        Assert.NotNull(collectionValidator._memberPathRepairable);
        Assert.Null(collectionValidator._memberPath);
        Assert.Null(collectionValidator._elementFilter);
        Assert.False(collectionValidator.IsNested);

        Assert.NotNull(collectionValidator._errorMessageResourceAccessor);
        Assert.Null(collectionValidator._errorMessageResourceAccessor());
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        Assert.Throws<ArgumentNullException>(() => collectionValidator.IsValid(null!));
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2));

        Assert.True(collectionValidator.IsValid(["Furion", "Fur"]));
        Assert.False(collectionValidator.IsValid(["Furion", null]));
        Assert.False(collectionValidator.IsValid(["Furion", "F"]));
    }

    [Fact]
    public void GetValidationResults_Invalid_Parameters()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        Assert.Throws<ArgumentNullException>(() => collectionValidator.GetValidationResults(null!));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2));

        Assert.Null(collectionValidator.GetValidationResults(["Furion", "Fur"]));

        var validationResults = collectionValidator.GetValidationResults(["Furion", null]);
        Assert.NotNull(validationResults);
        Assert.Equal("The String field is required.", validationResults.First().ErrorMessage);
        Assert.Equal("[1]", validationResults.First().MemberNames.First());

        var validationResults2 = collectionValidator.GetValidationResults(["Furion", "F"]);
        Assert.NotNull(validationResults2);
        Assert.Equal("The field String must be a string or array type with a minimum length of '2'.",
            validationResults2.First().ErrorMessage);
        Assert.Equal("[1]", validationResults2.First().MemberNames.First());
    }

    [Fact]
    public void GetValidationResults_WithMemberPath_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2));
        collectionValidator.RepairMemberPaths("Hobbies");

        Assert.Null(collectionValidator.GetValidationResults(["Furion", "Fur"]));

        var validationResults = collectionValidator.GetValidationResults(["Furion", null]);
        Assert.NotNull(validationResults);
        Assert.Equal("The String field is required.", validationResults.First().ErrorMessage);
        Assert.Equal("Hobbies[1]", validationResults.First().MemberNames.First());

        var validationResults2 = collectionValidator.GetValidationResults(["Furion", "F"]);
        Assert.NotNull(validationResults2);
        Assert.Equal("The field String must be a string or array type with a minimum length of '2'.",
            validationResults2.First().ErrorMessage);
        Assert.Equal("Hobbies[1]", validationResults2.First().MemberNames.First());
    }

    [Fact]
    public void Validate_Invalid_Parameters()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        Assert.Throws<ArgumentNullException>(() => collectionValidator.Validate(null!));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2));

        collectionValidator.Validate(["Furion", "Fur"]);

        var exception = Assert.Throws<ValidationException>(() => collectionValidator.Validate(["Furion", null]));
        Assert.Equal("The String field is required.", exception.Message);
        Assert.Equal("[1]", exception.ValidationResult.MemberNames.First());

        var exception2 = Assert.Throws<ValidationException>(() => collectionValidator.Validate(["Furion", "F"]));
        Assert.Equal("The field String must be a string or array type with a minimum length of '2'.",
            exception2.Message);
        Assert.Equal("[1]", exception2.ValidationResult.MemberNames.First());
    }

    [Fact]
    public void Validate_WithMemberPath_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2));
        collectionValidator.RepairMemberPaths("Hobbies");

        collectionValidator.Validate(["Furion", "Fur"]);

        var exception = Assert.Throws<ValidationException>(() => collectionValidator.Validate(["Furion", null]));
        Assert.Equal("The String field is required.", exception.Message);
        Assert.Equal("Hobbies[1]", exception.ValidationResult.MemberNames.First());

        var exception2 = Assert.Throws<ValidationException>(() => collectionValidator.Validate(["Furion", "F"]));
        Assert.Equal("The field String must be a string or array type with a minimum length of '2'.",
            exception2.Message);
        Assert.Equal("Hobbies[1]", exception2.ValidationResult.MemberNames.First());
    }

    [Fact]
    public void TryValidate_Invalid_Parameters()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        Assert.Throws<ArgumentNullException>(() => collectionValidator.TryValidate(null!).ThrowIfInvalid());
    }

    [Fact]
    public void TryValidate_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2));

        collectionValidator.TryValidate(["Furion", "Fur"]).ThrowIfInvalid();

        var exception =
            Assert.Throws<ValidationException>(() =>
                collectionValidator.TryValidate(["Furion", null]).ThrowIfInvalid());
        Assert.Equal("The String field is required.", exception.Message);
        Assert.Equal("[1]", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() => collectionValidator.TryValidate(["Furion", "F"]).ThrowIfInvalid());
        Assert.Equal("The field String must be a string or array type with a minimum length of '2'.",
            exception2.Message);
        Assert.Equal("[1]", exception2.ValidationResult.MemberNames.First());
    }

    [Fact]
    public void TryValidate_WithMemberPath_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2));
        collectionValidator.RepairMemberPaths("Hobbies");

        collectionValidator.TryValidate(["Furion", "Fur"]).ThrowIfInvalid();

        var exception =
            Assert.Throws<ValidationException>(() =>
                collectionValidator.TryValidate(["Furion", null]).ThrowIfInvalid());
        Assert.Equal("The String field is required.", exception.Message);
        Assert.Equal("Hobbies[1]", exception.ValidationResult.MemberNames.First());

        var exception2 =
            Assert.Throws<ValidationException>(() => collectionValidator.TryValidate(["Furion", "F"]).ThrowIfInvalid());
        Assert.Equal("The field String must be a string or array type with a minimum length of '2'.",
            exception2.Message);
        Assert.Equal("Hobbies[1]", exception2.ValidationResult.MemberNames.First());
    }

    [Fact]
    public void Where_ReturnOK()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        Assert.Null(collectionValidator._elementFilter);

        collectionValidator.Where(u => u is not null);
        Assert.NotNull(collectionValidator._elementFilter);

        collectionValidator.Where(null);
        Assert.Null(collectionValidator._elementFilter);
    }

    [Fact]
    public void GetValidatingElements_Invalid_Parameters()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        Assert.Throws<ArgumentNullException>(() => collectionValidator.GetValidatingElements(null!));
    }

    [Fact]
    public void GetValidatingElements_ReturnOK()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        string[] list = ["furion", "fur", "百小僧"];
        Assert.Equal(["furion", "fur", "百小僧"], collectionValidator.GetValidatingElements(list));

        collectionValidator.Where(u => u != "fur");
        Assert.Equal(["furion", "百小僧"], collectionValidator.GetValidatingElements(list));
    }

    [Fact]
    public void RepairMemberPaths_ReturnOK()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        Assert.Null(collectionValidator._memberPath);

        collectionValidator.RepairMemberPaths("Sub");
        Assert.Equal("Sub", collectionValidator._memberPath);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var collectionValidator = new CollectionValidator<string?>(new ValueValidator<string?>());
        var valueValidator = collectionValidator._elementValidator as ValueValidator<string?>;
        Assert.NotNull(valueValidator);
        Assert.Null(valueValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        collectionValidator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(valueValidator._serviceProvider);
    }

    [Fact]
    public void IsValid_ValidatorBase_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2)) { IsNested = true };
        collectionValidator.IsValid(null, new ValidationContext<IEnumerable<string?>>(null!));
    }

    [Fact]
    public void GetValidationResults_ValidatorBase_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2)) { IsNested = true };
        Assert.Null(
            collectionValidator.GetValidationResults(null, new ValidationContext<IEnumerable<string?>>(null!)));
    }

    [Fact]
    public void Validate_ValidatorBase_ReturnOK()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2)) { IsNested = true };
        collectionValidator.Validate(null, new ValidationContext<IEnumerable<string?>>(null!));
    }

    [Fact]
    public void ToResults_Invalid_Parameters()
    {
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2));
        Assert.Throws<ArgumentNullException>(() => collectionValidator.ToResults(null!));
    }

    [Fact]
    public void ToResults_ReturnOK()
    {
        var validationContext = new ValidationContext(new[] { null, "F" });
        var collectionValidator =
            new CollectionValidator<string?>(new ValueValidator<string?>().Required().MinLength(2));

        Assert.Equal(
            [
                "The String field is required.",
                "The field String must be a string or array type with a minimum length of '2'."
            ],
            collectionValidator.ToResults(validationContext).Select(u => u.ErrorMessage!).ToArray());
    }
}