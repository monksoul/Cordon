// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidatorBaseTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new TestValidator();
        Assert.Null(validator._errorMessage);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("单元测试{0}错误信息", validator._errorMessageResourceAccessor());
        Assert.Null(validator._errorMessageResourceName);
        Assert.Null(validator._errorMessageResourceType);
        Assert.False(validator.SupportsAsync);

        Assert.Null(validator.ErrorMessage);
        Assert.Null(validator.ErrorMessageResourceName);
        Assert.Null(validator.ErrorMessageResourceType);
        Assert.False(validator.CustomErrorMessageSet);
        Assert.Null(validator.RuleSets);

        Assert.Equal("Cordon.Resources.Overrides.ValidationMessages",
            ValidatorBase.ExternalValidationMessagesFullTypeName);

        var validatorType = typeof(TestValidator);

        var errorMessageResourceAccessorProperty =
            validatorType.GetProperty("ErrorMessageResourceAccessor", BindingFlags.Instance | BindingFlags.NonPublic);
        errorMessageResourceAccessorProperty?.SetValue(validator, () => "自定义错误信息");
        Assert.Equal("自定义错误信息", validator._errorMessageResourceAccessor());

        Assert.NotNull(validatorType
            .GetProperty("ErrorMessageString", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(validator));

        Assert.NotNull(validatorType.GetEvent("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic));
    }

    [Fact]
    public void NewOfT_ReturnOK()
    {
        var validator = new TestOfTValidator();
        Assert.Null(validator._errorMessage);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("单元测试{0}错误信息", validator._errorMessageResourceAccessor());
        Assert.Null(validator._errorMessageResourceName);
        Assert.Null(validator._errorMessageResourceType);

        Assert.Null(validator.ErrorMessage);
        Assert.Null(validator.ErrorMessageResourceName);
        Assert.Null(validator.ErrorMessageResourceType);
        Assert.False(validator.CustomErrorMessageSet);

        var validatorType = typeof(TestValidator);

        var errorMessageResourceAccessorProperty =
            validatorType.GetProperty("ErrorMessageResourceAccessor", BindingFlags.Instance | BindingFlags.NonPublic);
        errorMessageResourceAccessorProperty?.SetValue(validator, () => "自定义错误信息");
        Assert.Equal("自定义错误信息", validator._errorMessageResourceAccessor());

        Assert.NotNull(validatorType
            .GetProperty("ErrorMessageString", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(validator));

        Assert.NotNull(validatorType.GetEvent("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic));
    }

    [Fact]
    public void Set_ErrorMessage_ReturnOK()
    {
        var validator = new TestValidator();

        var i = 0;
        var propertyChangedEventMethod =
            typeof(TestValidator).GetMethod("add_PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, args) =>
            {
                Assert.Equal("ErrorMessage", args.PropertyName);
                Assert.Equal("自定义错误信息", args.PropertyValue);
                i++;
            })
        ]);

        validator.ErrorMessage = "自定义错误信息";
        Assert.NotNull(validator._errorMessage);
        Assert.Null(validator._errorMessageResourceAccessor);
        Assert.True(validator.CustomErrorMessageSet);
        Assert.Equal(1, i);
    }

    [Fact]
    public void Set_ErrorMessageResourceName_ReturnOK()
    {
        var validator = new TestValidator();

        var i = 0;
        var propertyChangedEventMethod =
            typeof(TestValidator).GetMethod("add_PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, args) =>
            {
                Assert.Equal("ErrorMessageResourceName", args.PropertyName);
                Assert.Equal("TestValidator_ValidationError", args.PropertyValue);
                i++;
            })
        ]);

        validator.ErrorMessageResourceName = "TestValidator_ValidationError";
        Assert.NotNull(validator._errorMessageResourceName);
        Assert.Null(validator._errorMessageResourceAccessor);
        Assert.True(validator.CustomErrorMessageSet);
        Assert.Equal(1, i);
    }

    [Fact]
    public void Set_ErrorMessageResourceType_ReturnOK()
    {
        var validator = new TestValidator();

        var i = 0;
        var propertyChangedEventMethod =
            typeof(TestValidator).GetMethod("add_PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, args) =>
            {
                Assert.Equal("ErrorMessageResourceType", args.PropertyName);
                Assert.Equal(typeof(TestValidationMessages), args.PropertyValue);
                i++;
            })
        ]);

        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        Assert.NotNull(validator._errorMessageResourceType);
        Assert.Null(validator._errorMessageResourceAccessor);
        Assert.True(validator.CustomErrorMessageSet);
        Assert.Equal(1, i);
    }

    [Fact]
    public void Get_ErrorMessageString_ReturnOK()
    {
        var validator = new TestValidator();
        var errorMessageStringProperty =
            typeof(TestValidator).GetProperty("ErrorMessageString", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.Equal("单元测试{0}错误信息", errorMessageStringProperty?.GetValue(validator));

        validator.ErrorMessage = "自定义错误信息";
        Assert.Equal("自定义错误信息", errorMessageStringProperty?.GetValue(validator));

        var validator2 = new TestValidator
        {
            ErrorMessageResourceType = typeof(TestValidationMessages),
            ErrorMessageResourceName = "TestValidator_ValidationError2"
        };
        Assert.Equal("单元测试{0}错误信息2", errorMessageStringProperty?.GetValue(validator2));
    }

    [Fact]
    public void UseResourceKey_Invalid_Parameters()
    {
        var validator = new TestValidator();

        var userResourceMethod =
            typeof(ValidatorBase).GetMethod("UseResourceKey", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.CreateDelegate<Action<Func<string>>>(validator);
        Assert.NotNull(userResourceMethod);

        Assert.Throws<ArgumentNullException>(() => userResourceMethod(null!));
    }

    [Fact]
    public void UseResourceKey_ReturnOK()
    {
        var validator = new TestValidator();

        var userResourceMethod =
            typeof(ValidatorBase).GetMethod("UseResourceKey", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.CreateDelegate<Action<Func<string>>>(validator);
        Assert.NotNull(userResourceMethod);

        userResourceMethod(() => "TestValidator_Error");
        Assert.Equal("[TestValidator_Error]", validator._errorMessageResourceAccessor!());
    }

    [Fact]
    public void GetResourceString_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidatorBase.GetResourceString(null!));
        Assert.Throws<ArgumentException>(() => ValidatorBase.GetResourceString(string.Empty));
        Assert.Throws<ArgumentException>(() => ValidatorBase.GetResourceString(" "));
    }

    [Fact]
    public void GetResourceString_ReturnOK()
    {
        Assert.Null(ValidatorBase.GetResourceString("Test_Property"));

        var errorMessage = ValidatorBase.GetResourceString("AgeValidator_ValidationError");
        Assert.NotNull(errorMessage);
        Assert.Equal("The field {0} is not a valid age.", errorMessage);
    }

    [Fact]
    public void OnPropertyChanged_Invalid_Parameters()
    {
        var validator = new TestValidator();
        var onPropertyChangedMethod =
            typeof(TestValidator).GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var exception =
            Assert.Throws<TargetInvocationException>(() => onPropertyChangedMethod.Invoke(validator, [null, null]));
        Assert.True(exception.InnerException is ArgumentNullException);

        var exception2 =
            Assert.Throws<TargetInvocationException>(() =>
                onPropertyChangedMethod.Invoke(validator, [string.Empty, null]));
        Assert.True(exception2.InnerException is ArgumentException);

        var exception3 =
            Assert.Throws<TargetInvocationException>(() =>
                onPropertyChangedMethod.Invoke(validator, [" ", null]));
        Assert.True(exception3.InnerException is ArgumentException);
    }

    [Fact]
    public void OnPropertyChanged_ReturnOK()
    {
        var validator = new TestValidator();

        var i = 0;
        var propertyChangedEventMethod =
            typeof(TestValidator).GetMethod("add_PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;
        propertyChangedEventMethod.Invoke(validator, [
            new EventHandler<ValidationPropertyChangedEventArgs>((_, args) =>
            {
                Assert.Equal("ErrorMessage", args.PropertyName);
                Assert.Equal("自定义错误信息", args.PropertyValue);
                i++;
            })
        ]);

        var onPropertyChangedMethod =
            typeof(TestValidator).GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic)!;

        onPropertyChangedMethod.Invoke(validator, ["自定义错误信息", "ErrorMessage"]);

        Assert.Equal(1, i);
    }

    [Fact]
    public void SetupResourceAccessor_Invalid_Parameters()
    {
        var validator = new TestValidator();
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            validator.ErrorMessage = "自定义错误信息";
            validator.ErrorMessageResourceName = "TestValidator_ValidationError";
            validator.ErrorMessageResourceType = typeof(TestValidationMessages);

            validator.SetupResourceAccessor();
        });
        Assert.Equal("Either ErrorMessageString or ErrorMessageResourceName must be set, but not both.",
            exception.Message);

        var validator2 = new TestValidator();
        var exception2 = Assert.Throws<InvalidOperationException>(() =>
        {
            validator2.ErrorMessageResourceType = typeof(TestValidationMessages);
            validator2.SetupResourceAccessor();
        });
        Assert.Equal("Either ErrorMessageString or ErrorMessageResourceName must be set, but not both.",
            exception2.Message);

        var validator3 = new TestValidator();
        var exception3 = Assert.Throws<InvalidOperationException>(() =>
        {
            validator3.ErrorMessageResourceName = "TestValidator_ValidationError";
            validator3.SetupResourceAccessor();
        });
        Assert.Equal("Both ErrorMessageResourceType and ErrorMessageResourceName need to be set on this validator.",
            exception3.Message);
    }

    [Fact]
    public void SetupResourceAccessor_ReturnOK()
    {
        var validator = new TestValidator();
        validator.SetupResourceAccessor();
        Assert.Equal("单元测试{0}错误信息", validator._errorMessageResourceAccessor!());

        validator.ErrorMessage = "自定义错误信息";
        validator.SetupResourceAccessor();
        Assert.Equal("自定义错误信息", validator._errorMessageResourceAccessor!());

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        validator.SetupResourceAccessor();
        Assert.Equal("单元测试{0}错误信息2", validator._errorMessageResourceAccessor!());
    }

    [Fact]
    public void SetResourceAccessorByPropertyLookup_Invalid_Parameters()
    {
        var validator = new TestValidator();
        Assert.Throws<ArgumentNullException>(() => validator.SetResourceAccessorByPropertyLookup());

        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        Assert.Throws<ArgumentNullException>(() => validator.SetResourceAccessorByPropertyLookup());

        validator.ErrorMessageResourceName = string.Empty;
        Assert.Throws<ArgumentException>(() => validator.SetResourceAccessorByPropertyLookup());

        validator.ErrorMessageResourceName = "NotFound";
        var exception = Assert.Throws<InvalidOperationException>(() => validator.SetResourceAccessorByPropertyLookup());
        Assert.Equal(
            "The resource type `Cordon.Tests.TestValidationMessages` does not have an accessible static property named `NotFound`.",
            exception.Message);

        validator.ErrorMessageResourceName = "Culture";
        var exception2 =
            Assert.Throws<InvalidOperationException>(() => validator.SetResourceAccessorByPropertyLookup());
        Assert.Equal(
            "The property `Culture` on resource type `Cordon.Tests.TestValidationMessages` is not a string type.",
            exception2.Message);
    }

    [Fact]
    public void SetResourceAccessorByPropertyLookup_ReturnOK()
    {
        var validator = new TestValidator
        {
            ErrorMessageResourceType = typeof(TestValidationMessages),
            ErrorMessageResourceName = "TestValidator_ValidationError2"
        };
        validator.SetResourceAccessorByPropertyLookup();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("单元测试{0}错误信息2", validator._errorMessageResourceAccessor());
    }

    [Fact]
    public void GetValidationMessagesProperty_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidatorBase.GetValidationMessagesProperty(null!));
        Assert.Throws<ArgumentException>(() => ValidatorBase.GetValidationMessagesProperty(string.Empty));
        Assert.Throws<ArgumentException>(() => ValidatorBase.GetValidationMessagesProperty(" "));
    }

    [Fact]
    public void GetValidationMessagesProperty_ReturnOK()
    {
        Assert.Null(ValidatorBase.GetValidationMessagesProperty("Test_Property"));

        var property = ValidatorBase.GetValidationMessagesProperty("AgeValidator_ValidationError");
        Assert.NotNull(property);
        Assert.Equal("The field {0} is not a valid age.", property.GetValue(null, null));
    }

    [Fact]
    public void TryGetPropertyFromAssembly_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidatorBase.TryGetPropertyFromAssembly(null!, null!, null!));

        Assert.Throws<ArgumentNullException>(() =>
            ValidatorBase.TryGetPropertyFromAssembly(Assembly.GetEntryAssembly()!, null!, null!));
        Assert.Throws<ArgumentException>(() =>
            ValidatorBase.TryGetPropertyFromAssembly(Assembly.GetEntryAssembly()!, string.Empty, null!));
        Assert.Throws<ArgumentException>(() =>
            ValidatorBase.TryGetPropertyFromAssembly(Assembly.GetEntryAssembly()!, " ", null!));

        Assert.Throws<ArgumentNullException>(() =>
            ValidatorBase.TryGetPropertyFromAssembly(Assembly.GetEntryAssembly()!, "Tests.ValidationMessages", null!));
        Assert.Throws<ArgumentException>(() =>
            ValidatorBase.TryGetPropertyFromAssembly(Assembly.GetEntryAssembly()!, "Tests.ValidationMessages",
                string.Empty));
        Assert.Throws<ArgumentException>(() =>
            ValidatorBase.TryGetPropertyFromAssembly(Assembly.GetEntryAssembly()!, "Tests.ValidationMessages", " "));
    }

    [Fact]
    public void TryGetPropertyFromAssembly_ReturnOK()
    {
        Assert.Null(ValidatorBase.TryGetPropertyFromAssembly(Assembly.GetEntryAssembly()!, "Tests.ValidationMessages",
            "Test_Property"));

        var property = ValidatorBase.TryGetPropertyFromAssembly(typeof(TestValidationMessages).Assembly,
            "Cordon.Tests.TestValidationMessages", "TestValidator_ValidationError");
        Assert.NotNull(property);
        Assert.Equal("单元测试{0}错误信息", property.GetValue(null, null));
    }

    [Fact]
    public void IsValidOfT_Invalid_Parameters()
    {
        var validator = new TestOfTValidator();
        Assert.Throws<InvalidCastException>(() => validator.IsValid(5));
    }

    [Theory]
    [InlineData("Validation", false)]
    [InlineData("Furion", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new TestValidator();
        Assert.Equal(result, validator.IsValid(value));

        var validator2 = new TestOfTValidator();
        Assert.Equal(result, validator2.IsValid(value));
        Assert.Equal(result, validator2.IsValid((string?)value));
    }

    [Fact]
    public void GetValidationResultsOfT_Invalid_Parameters()
    {
        var validator = new TestOfTValidator();
        Assert.Throws<InvalidCastException>(() => validator.GetValidationResults(5, "data"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new TestValidator();
        Assert.Null(validator.GetValidationResults("Furion", "data"));
        var validationResult1 = validator.GetValidationResults("Validation", "data");
        Assert.NotNull(validationResult1);
        Assert.Equal("单元测试data错误信息", validationResult1.First().ErrorMessage);

        var validator2 = new TestOfTValidator();
        Assert.Null(validator2.GetValidationResults("Furion", "data"));
        var validationResult2 = validator2.GetValidationResults((object)"Validation", "data");
        Assert.NotNull(validationResult2);
        Assert.Equal("单元测试data错误信息", validationResult2.First().ErrorMessage);
        var validationResult3 = validator2.GetValidationResults("Validation", "data");
        Assert.NotNull(validationResult3);
        Assert.Equal("单元测试data错误信息", validationResult3.First().ErrorMessage);
    }

    [Fact]
    public void ValidateOfT_Invalid_Parameters()
    {
        var validator = new TestOfTValidator();
        Assert.Throws<InvalidCastException>(() => validator.Validate(5, "data"));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new TestValidator();
        validator.Validate("Furion", "data");
        var exception = Assert.Throws<ValidationException>(() => validator.Validate("Validation", "data"));
        Assert.Equal("单元测试data错误信息", exception.Message);

        var validator2 = new TestOfTValidator();
        validator2.Validate("Furion", "data");
        var exception2 = Assert.Throws<ValidationException>(() => validator2.Validate((object)"Validation", "data"));
        Assert.Equal("单元测试data错误信息", exception2.Message);
        var exception3 = Assert.Throws<ValidationException>(() => validator2.Validate("Validation", "data"));
        Assert.Equal("单元测试data错误信息", exception3.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new TestValidator();
        Assert.Equal("单元测试data错误信息", validator.FormatErrorMessage("data"));

        var validator2 = new TestOfTValidator();
        Assert.Equal("单元测试data错误信息", validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void ConvertValue_Invalid_Parameters() =>
        Assert.Throws<InvalidCastException>(() => TestOfTValidator.ConvertValue(5));

    [Fact]
    public void ConvertValue_ReturnOK()
    {
        Assert.Null(TestOfTValidator.ConvertValue(null));
        Assert.Equal("Furion", TestOfTValidator.ConvertValue("Furion"));
    }
}

public class TestValidator() : ValidatorBase(TestValidationMessages.TestValidator_ValidationError)
{
    /// <inheritdoc />
    public override bool IsValid(object? value) => value?.ToString() == "Furion";
}

public class TestOfTValidator() : ValidatorBase<string>(TestValidationMessages.TestValidator_ValidationError)
{
    /// <inheritdoc />
    public override bool IsValid(string? instance) => instance == "Furion";
}