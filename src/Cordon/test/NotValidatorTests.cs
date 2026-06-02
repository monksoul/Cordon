// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class NotValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new NotValidator<string>((Action<FluentValidatorBuilder<string>>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new NotValidator<string>((ValidatorBase[])null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new NotValidator<string>(_ => { });
        Assert.NotNull(validator);
        Assert.NotNull(validator._validators);
        Assert.Empty(validator._validators);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is invalid.", validator._errorMessageResourceAccessor());

        var validator2 =
            new NotValidator<string>(u => u.Chinese().Required().NotNull());

        Assert.NotNull(validator2._validators);
        Assert.Equal(3, validator2._validators.Count);
        Assert.Equal([typeof(NotNullValidator), typeof(RequiredValidator), typeof(ChineseValidator)],
            validator2._validators.Select(u => u.GetType()));

        var validator3 =
            new NotValidator<string>([new ChineseValidator(), new RequiredValidator(), new NotNullValidator()]);

        Assert.NotNull(validator3._validators);
        Assert.Equal(3, validator3._validators.Count);
        Assert.Equal([typeof(NotNullValidator), typeof(RequiredValidator), typeof(ChineseValidator)],
            validator3._validators.Select(u => u.GetType()));

        Assert.NotNull(validator3._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is invalid.", validator3._errorMessageResourceAccessor());
    }


    [Fact]
    public void IsValid_ReturnOK()
    {
        var validator = new NotValidator<string>(u => u.Chinese().Required());
        Assert.True(validator.IsValid(null));
        Assert.False(validator.IsValid("中文"));
        Assert.True(validator.IsValid("Furion"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new NotValidator<string>(u => u.Chinese().Required());
        Assert.Null(validator.GetValidationResults(null, "data"));
        Assert.Null(validator.GetValidationResults("Furion", "data"));

        var validationResults = validator.GetValidationResults("中文", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is invalid.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("中文", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new NotValidator<string>(u => u.Chinese().Required());
        validator.Validate(null, "data");
        validator.Validate("Furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("中文", "data"));
        Assert.Equal("The field data is invalid.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("中文", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new NotValidator<string>(u => u.Chinese());
        validator.Dispose();
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var validator = new NotValidator<string>(u => u.WithAttributes(new RequiredAttribute()));
        var attributeValueValidator = validator._validators[0] as AttributeValueValidator;
        Assert.NotNull(attributeValueValidator);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);
    }
}