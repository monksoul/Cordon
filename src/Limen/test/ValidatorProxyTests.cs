// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ValidatorProxyTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new ValidatorProxy<EmailAddressValidator>();
        Assert.NotNull(validator);

        var proxyValidator = GetProxyValidator(validator);
        Assert.True(proxyValidator is EmailAddressValidator);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Null(validator._errorMessageResourceAccessor());

        var validator2 = new ValidatorProxy<AllowedValuesValidator>("Furion", "Fur", "百小僧");
        var proxyValidator2 = GetProxyValidator(validator2);
        Assert.True(proxyValidator2 is AllowedValuesValidator);
        Assert.Equal(["Furion", "Fur", "百小僧"], (proxyValidator2 as AllowedValuesValidator)!.Values);
    }

    [Fact]
    public void Set_Properties_ReturnOK()
    {
        var validator = new ValidatorProxy<EmailAddressValidator> { ErrorMessage = "数据无效" };
        var proxyValidator = GetProxyValidator(validator);

        Assert.Equal("数据无效", (proxyValidator as EmailAddressValidator)!.ErrorMessage);
    }

    [Fact]
    public void Configure_Invalid_Parameters()
    {
        var validator = new ValidatorProxy<EmailAddressValidator>();
        Assert.Throws<ArgumentNullException>(() => validator.Configure(null!));
    }

    [Fact]
    public void Configure_ReturnOK()
    {
        var validator = new ValidatorProxy<EmailAddressValidator>();
        validator.Configure(v =>
        {
            v.ErrorMessage = "数据无效";
        });

        var proxyValidator = GetProxyValidator(validator);
        Assert.Equal("数据无效", (proxyValidator as EmailAddressValidator)!.ErrorMessage);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("monksoul@", false)]
    [InlineData("@monksoul", false)]
    [InlineData("monksoul@qq", false)]
    [InlineData("monksoul@outlook.com", true)]
    [InlineData("monksoul@outlook..com", false)]
    [InlineData("""
                "user@name"@domain.com
                """, true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new ValidatorProxy<EmailAddressValidator>();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new ValidatorProxy<EmailAddressValidator>();
        Assert.Null(validator.GetValidationResults("monksoul@outlook.com", "data"));

        var validationResults = validator.GetValidationResults("monksoul@qq", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field is not a valid e-mail address.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("monksoul@qq", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new ValidatorProxy<EmailAddressValidator>();
        validator.Validate("monksoul@outlook.com", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("monksoul@qq", "data"));
        Assert.Equal("The data field is not a valid e-mail address.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("monksoul@qq", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new ValidatorProxy<EmailAddressValidator>();
        Assert.Equal("The data field is not a valid e-mail address.", validator.FormatErrorMessage("data"));
    }

    [Fact]
    public void OnPropertyChanged_ReturnOK()
    {
        var validator = new ValidatorProxy<EmailAddressValidator>();
        var proxyValidator = GetProxyValidator(validator);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("Unknown", null));

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("ErrorMessage", "数据无效"));
        Assert.Equal("数据无效", (proxyValidator as EmailAddressValidator)!.ErrorMessage);

        var validator2 = new ValidatorProxy<AllowedValuesValidator>("Furion", "Fur", "百小僧");
        var proxyValidator2 = GetProxyValidator(validator2);
        validator2.OnPropertyChanged(validator,
            new ValidationPropertyChangedEventArgs("Values", new[] { "Furion", "Fur" }));
        Assert.Equal(["Furion", "Fur", "百小僧"], (proxyValidator2 as AllowedValuesValidator)!.Values);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new ValidatorProxy<EmailAddressValidator>();
        var proxyValidator = GetProxyValidator(validator) as EmailAddressValidator;
        Assert.NotNull(proxyValidator);

        validator.Dispose();

        validator.ErrorMessage = "数据无效";
        Assert.Null(proxyValidator.ErrorMessage);
    }

    private static object? GetProxyValidator<TValidator>(ValidatorProxy<TValidator> validator)
        where TValidator : ValidatorBase
    {
        var proxyValidatorProperty =
            validator.GetType().GetProperty("Validator", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(proxyValidatorProperty);

        var proxyValidator = proxyValidatorProperty.GetValue(validator);
        return proxyValidator;
    }
}