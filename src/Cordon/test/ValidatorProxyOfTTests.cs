// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidatorProxyOfTTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        Assert.NotNull(validator);

        Assert.Null(validator._constructorArgsFactory);
        Assert.NotNull(validator._propertyChanges);
        Assert.Empty(validator._propertyChanges);
        Assert.NotNull(validator._validatorCache);
        Assert.Empty(validator._validatorCache);
        Assert.NotNull(validator._validatorConfigurations);
        Assert.Empty(validator._validatorConfigurations);

        Assert.NotNull(validator._validatingObjectFactory);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Null(validator._errorMessageResourceAccessor());

        var validator2 =
            new ValidatorProxy<ValidatorProxyClass, AllowedValuesValidator>(u => u.Value,
                (_, _) => ["Furion", "Fur", "百小僧"]);
        Assert.NotNull(validator2);
    }

    [Fact]
    public void GetValidator_Invalid_Parameters()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        var getValidatorMethod =
            validator.GetType().GetMethod("GetValidator", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(getValidatorMethod);

        var exception =
            Assert.Throws<TargetInvocationException>(() => getValidatorMethod.Invoke(validator, [null, null]));
        Assert.IsType<ArgumentNullException>(exception.InnerException);
    }

    [Fact]
    public void GetValidator_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        var getValidator = GetProxyValidator(validator);

        var instance = new ValidatorProxyClass { Value = "monksoul@qq.com" };
        var proxyValidator = getValidator(instance, null!);
        Assert.NotNull(proxyValidator);

        Assert.Single(validator._validatorCache);
        Assert.Equal(RuntimeHelpers.GetHashCode(instance), validator._validatorCache.Keys.First());

        var proxyValidator2 = getValidator(instance, null!);
        Assert.NotNull(proxyValidator2);

        Assert.Same(proxyValidator, proxyValidator2);
        Assert.Single(validator._validatorCache);
        Assert.Equal(RuntimeHelpers.GetHashCode(instance), validator._validatorCache.Keys.First());
    }

    [Fact]
    public void Set_Properties_ReturnOK()
    {
        var validator =
            new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value) { ErrorMessage = "数据无效" };
        Assert.Single(validator._propertyChanges);
        Assert.Equal("ErrorMessage", validator._propertyChanges.Keys.First());
        Assert.Equal("数据无效", validator._propertyChanges.Values.First());

        var getValidator = GetProxyValidator(validator);
        var instance = new ValidatorProxyClass { Value = "monksoul@qq.com" };
        var proxyValidator = getValidator(instance, null!);

        Assert.Equal("数据无效", proxyValidator.ErrorMessage);
    }

    [Fact]
    public void Configure_Invalid_Parameters()
    {
        var validator =
            new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value) { ErrorMessage = "数据无效" };
        Assert.Throws<ArgumentNullException>(() => validator.Configure(null!));
    }

    [Fact]
    public void Configure_ReturnOK()
    {
        var validator =
            new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value) { ErrorMessage = "数据无效" };
        validator.Configure(v =>
        {
            v.ErrorMessage = "数据无效";
        });

        Assert.Empty(validator._validatorCache);

        var getValidator = GetProxyValidator(validator);
        var instance = new ValidatorProxyClass { Value = "monksoul@qq.com" };
        var proxyValidator = getValidator(instance, null!);
        Assert.Equal("数据无效", proxyValidator.ErrorMessage);

        Assert.Single(validator._validatorCache);
        validator.Configure(v =>
        {
            v.ErrorMessage = "数据无效";
        });

        Assert.Empty(validator._validatorCache);
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
    public void IsValid_ReturnOK(string? value, bool result)
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        Assert.Equal(result, validator.IsValid(new ValidatorProxyClass { Value = value }));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        Assert.Null(validator.GetValidationResults(new ValidatorProxyClass { Value = "monksoul@outlook.com" }, "data"));

        var validationResults =
            validator.GetValidationResults(new ValidatorProxyClass { Value = "monksoul@qq" }, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field is not a valid e-mail address.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 =
            validator.GetValidationResults(new ValidatorProxyClass { Value = "monksoul@qq" }, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        validator.Validate(new ValidatorProxyClass { Value = "monksoul@outlook.com" }, "data");

        var exception = Assert.Throws<ValidationException>(() =>
            validator.Validate(new ValidatorProxyClass { Value = "monksoul@qq" }, "data"));
        Assert.Equal("The data field is not a valid e-mail address.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() =>
            validator.Validate(new ValidatorProxyClass { Value = "monksoul@qq" }, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_Invalid_Parameters()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        var exception = Assert.Throws<NotSupportedException>(() => validator.FormatErrorMessage("data"));
        Assert.Equal("Use FormatErrorMessage(string name, T? instance) instead.", exception.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        Assert.Equal("The data field is not a valid e-mail address.",
            validator.FormatErrorMessage("data", new ValidatorProxyClass { Value = "monksoul@qq" }, null!));
    }

    [Fact]
    public void GetValidatingObject_Invalid_Parameters()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        var getValidationValueMethod =
            validator.GetType()
                .GetMethod("GetValidatingObject", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(getValidationValueMethod);

        var exception =
            Assert.Throws<TargetInvocationException>(() => getValidationValueMethod.Invoke(validator, [null]));
        Assert.IsType<ArgumentNullException>(exception.InnerException);
    }

    [Fact]
    public void GetValidatingObject_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        var getValidationValueMethod =
            validator.GetType()
                .GetMethod("GetValidatingObject", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(getValidationValueMethod);

        var instance = new ValidatorProxyClass { Value = "monksoul@outlook.com" };
        Assert.Equal("monksoul@outlook.com", getValidationValueMethod.Invoke(validator, [instance]));
    }

    [Fact]
    public void ApplyPropertyChanges_Invalid_Parameters()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        Assert.Throws<ArgumentNullException>(() => validator.ApplyPropertyChanges(null!));
    }

    [Fact]
    public void ApplyPropertyChanges_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        var getValidator = GetProxyValidator(validator);
        var instance = new ValidatorProxyClass { Value = "monksoul@qq.com" };
        var proxyValidator = getValidator(instance, null!);

        Assert.Null(proxyValidator.ErrorMessage);
        validator._propertyChanges.TryAdd("ErrorMessage", "数据无效");
        validator.ApplyPropertyChanges(proxyValidator);
        Assert.Equal("数据无效", proxyValidator.ErrorMessage);
    }

    [Fact]
    public void OnPropertyChanged_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        var getValidator = GetProxyValidator(validator);
        var instance = new ValidatorProxyClass { Value = "monksoul@qq.com" };
        _ = getValidator(instance, null!);

        Assert.Single(validator._validatorCache);

        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("ErrorMessage", "数据无效"));
        Assert.Equal("ErrorMessage", validator._propertyChanges.Keys.First());
        Assert.Equal("数据无效", validator._propertyChanges.Values.First());
        Assert.Empty(validator._validatorCache);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, EmailAddressValidator>(u => u.Value);
        var instance = new ValidatorProxyClass { Value = "monksoul@qq.com" };
        var proxyValidator = GetProxyValidator(validator)(instance, null!);
        Assert.NotNull(proxyValidator);

        validator.Dispose();

        validator.ErrorMessage = "数据无效";
        Assert.Null(proxyValidator.ErrorMessage);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var validator = new ValidatorProxy<ValidatorProxyClass, AttributeObjectValidator>(u => u);
        var instance = new ValidatorProxyClass { Value = "monksoul@qq.com" };
        var attributeObjectValidator = GetProxyValidator(validator)(instance, null!);

        Assert.NotNull(attributeObjectValidator);
        Assert.Null(attributeObjectValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(attributeObjectValidator._serviceProvider);
    }

    private static Func<T, ValidationContext<T>, TValidator> GetProxyValidator<T, TValidator>(
        ValidatorProxy<T, TValidator> validator)
        where TValidator : ValidatorBase
    {
        var getValidatorMethod =
            validator.GetType().GetMethod("GetValidator", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(getValidatorMethod);

        return getValidatorMethod.CreateDelegate<Func<T, ValidationContext<T>, TValidator>>(validator);
    }
}

public class ValidatorProxyClass
{
    public string? Value { get; set; }
}