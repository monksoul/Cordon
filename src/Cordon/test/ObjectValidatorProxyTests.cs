// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ObjectValidatorProxyTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ObjectValidatorProxy<string?>(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var objectValidatorProxy = new ObjectValidatorProxy<string?>(new ValueValidator<string?>());
        Assert.NotNull(objectValidatorProxy._objectValidator);
        Assert.True(objectValidatorProxy._objectValidator is ValueValidator<string?>);

        Assert.NotNull(objectValidatorProxy._errorMessageResourceAccessor);
        Assert.Null(objectValidatorProxy._errorMessageResourceAccessor());
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var objectValidatorProxy =
            new ObjectValidatorProxy<string?>(new ValueValidator<string?>().Required().MinLength(2));

        Assert.True(objectValidatorProxy.IsValid("Furion", new ValidationContext<string?>("Furion")));
        Assert.False(objectValidatorProxy.IsValid(null, new ValidationContext<string?>(null!)));
        Assert.False(objectValidatorProxy.IsValid("F", new ValidationContext<string?>("F")));

        var objectValidatorProxy2 =
            new ObjectValidatorProxy<string?>(new ValueValidator<string?>().MinLength(2));

        Assert.True(objectValidatorProxy2.IsValid(null, new ValidationContext<string?>(null!)));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var objectValidatorProxy =
            new ObjectValidatorProxy<string?>(new ValueValidator<string?>().Required().MinLength(2));

        Assert.Null(objectValidatorProxy.GetValidationResults("Furion", new ValidationContext<string?>("Furion")));

        var validationResults = objectValidatorProxy.GetValidationResults(null, new ValidationContext<string?>(null!));
        Assert.NotNull(validationResults);
        Assert.Equal("The String field is required.", validationResults.First().ErrorMessage);

        var validationResults2 = objectValidatorProxy.GetValidationResults("F", new ValidationContext<string?>("F"));
        Assert.NotNull(validationResults2);
        Assert.Equal("The field String must be a string or array type with a minimum length of '2'.",
            validationResults2.First().ErrorMessage);

        var objectValidatorProxy2 =
            new ObjectValidatorProxy<string?>(new ValueValidator<string?>().MinLength(2));

        Assert.Null(objectValidatorProxy2.GetValidationResults(null, new ValidationContext<string?>(null!)));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var objectValidatorProxy =
            new ObjectValidatorProxy<string?>(new ValueValidator<string?>().Required().MinLength(2));

        objectValidatorProxy.Validate("Furion", new ValidationContext<string?>("Furion"));

        var exception = Assert.Throws<ValidationException>(() =>
            objectValidatorProxy.Validate(null, new ValidationContext<string?>(null!)));
        Assert.Equal("The String field is required.", exception.Message);

        var exception2 = Assert.Throws<ValidationException>(() =>
            objectValidatorProxy.Validate("F", new ValidationContext<string?>("F")));
        Assert.Equal("The field String must be a string or array type with a minimum length of '2'.",
            exception2.Message);

        var objectValidatorProxy2 =
            new ObjectValidatorProxy<string?>(new ValueValidator<string?>().MinLength(2));

        objectValidatorProxy2.Validate(null, new ValidationContext<string?>(null!));
    }

    [Fact]
    public void RepairMemberPaths_ReturnOK()
    {
        var objectValidatorProxy = new ObjectValidatorProxy<string?>(new ValueValidator<string?>());
        var valueValidator = objectValidatorProxy._objectValidator as ValueValidator<string?>;
        Assert.NotNull(valueValidator);
        Assert.Null(valueValidator._memberPath);

        objectValidatorProxy.RepairMemberPaths("Sub");
        Assert.Equal("Sub", valueValidator._memberPath);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var objectValidatorProxy = new ObjectValidatorProxy<string?>(new ValueValidator<string?>());
        var valueValidator = objectValidatorProxy._objectValidator as ValueValidator<string?>;
        Assert.NotNull(valueValidator);
        Assert.Null(valueValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        objectValidatorProxy.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(valueValidator._serviceProvider);
    }
}