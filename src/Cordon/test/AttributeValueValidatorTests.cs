// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class AttributeValueValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new AttributeValueValidator(null!));

        var exception = Assert.Throws<ArgumentException>(() =>
            new AttributeValueValidator(new RequiredAttribute(), null!, new StringLengthAttribute(3)));
        Assert.Equal("Attributes cannot contain null elements. (Parameter 'attributes')", exception.Message);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new AttributeValueValidator(new RequiredAttribute(), new StringLengthAttribute(3));

        Assert.NotNull(validator.Attributes);
        Assert.Equal(2, validator.Attributes.Length);

        Assert.NotNull(AttributeValueValidator._sentinel);
        Assert.Equal(["ErrorMessage", "ErrorMessageResourceType", "ErrorMessageResourceName"],
            validator._observedPropertyNames);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Null(validator._errorMessageResourceAccessor());
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3));
        Assert.True(validator.IsValid(null));

        var validator2 = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        Assert.False(validator2.IsValid(null));

        var validator3 = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        Assert.True(validator3.IsValid("Fur"));

        var validator4 = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        Assert.False(validator4.IsValid("Furion"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3));
        Assert.Null(validator.GetValidationResults(null, "Value"));

        var validator2 = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        Assert.Null(validator2.GetValidationResults("Fur", "Value"));
    }

    [Fact]
    public void GetValidationResults_WithNullValue_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        var validationResults = validator.GetValidationResults(null, "Value");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The Value field is required.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "自定义错误信息";
        var validationResults2 = validator.GetValidationResults(null, "Value");
        Assert.NotNull(validationResults2);
        Assert.Equal(2, validationResults2.Count);
        Assert.Equal("自定义错误信息", validationResults2.First().ErrorMessage);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var validationResults3 = validator.GetValidationResults(null, "Value");
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal("单元测试Value错误信息2", validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithNoNullValue_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        var validationResults = validator.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field Value must be a string with a maximum length of 3.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "自定义错误信息";
        var validationResults2 = validator.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults2);
        Assert.Equal(2, validationResults2.Count);
        Assert.Equal("自定义错误信息", validationResults2.First().ErrorMessage);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var validationResults3 = validator.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal("单元测试Value错误信息2", validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithNoName_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        var validationResults = validator.GetValidationResults(null, null!);
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The Object field is required.", validationResults.First().ErrorMessage);

        var validationResults2 = validator.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("The field Value must be a string with a maximum length of 3.",
            validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3));
        validator.Validate(null, "Value");

        var validator2 = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        validator2.Validate("Fur", "Value");
    }

    [Fact]
    public void Validate_WithNullValue_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        var exception = Assert.Throws<ValidationException>(() => validator.Validate(null, "Value"));
        Assert.Equal("The Value field is required.", exception.Message);
        Assert.True(exception.ValidationAttribute is RequiredAttribute);

        validator.ErrorMessage = "自定义错误信息";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(null, "Value"));
        Assert.Equal("自定义错误信息", exception2.Message);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var exception3 = Assert.Throws<ValidationException>(() => validator.Validate(null, "Value"));
        Assert.Equal("单元测试Value错误信息2", exception3.Message);
    }

    [Fact]
    public void Validate_WithNoNullValue_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        var exception = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "Value"));
        Assert.Equal("The field Value must be a string with a maximum length of 3.", exception.Message);
        Assert.True(exception.ValidationAttribute is StringLengthAttribute);

        validator.ErrorMessage = "自定义错误信息";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "Value"));
        Assert.Equal("自定义错误信息", exception2.Message);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var exception3 = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "Value"));
        Assert.Equal("单元测试Value错误信息2", exception3.Message);
    }

    [Fact]
    public void Validate_WithNoName_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        var exception = Assert.Throws<ValidationException>(() => validator.Validate(null, null!));
        Assert.Equal("The Object field is required.", exception.Message);

        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "Value"));
        Assert.Equal("The field Value must be a string with a maximum length of 3.", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        Assert.Null(validator.FormatErrorMessage(null!));

        validator.ErrorMessage = "自定义错误信息";
        Assert.Equal("自定义错误信息", validator.FormatErrorMessage(null!));
    }

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        const string model = "Furion";
        var validationContext = AttributeValueValidator.CreateValidationContext(model, null);
        Assert.NotNull(validationContext);
        Assert.Empty(validationContext.Items);
        Assert.Null(validationContext.GetService<IServiceProvider>());
        Assert.Null(validationContext.MemberName);
        Assert.Equal("String", validationContext.DisplayName);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var validationContext2 = AttributeValueValidator.CreateValidationContext(model,
            new ValidationContext<string>(model, serviceProvider, null)
            {
                RuleSets = ["login"], MemberNames = ["Model"], DisplayName = "ModelDisplay"
            });
        Assert.NotNull(validationContext2);
        Assert.Single(validationContext2.Items);
        var metadata =
            validationContext2.Items[Constants.ValidationOptionsKey] as ValidationOptionsMetadata;
        Assert.NotNull(metadata);
        Assert.Equal(["login"], (string[]?)metadata.RuleSets!);

        Assert.NotNull(validationContext2.GetService<IServiceProvider>());
        Assert.Equal("Model", validationContext2.MemberName);
        Assert.Equal("ModelDisplay", validationContext2.DisplayName);
    }

    [Fact]
    public void OnPropertyChanged_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        validator.OnPropertyChanged(validator, new ValidationPropertyChangedEventArgs("ErrorMessage", "错误信息"));
        Assert.Null(validator.Attributes[0].ErrorMessage);
        Assert.Null(validator.Attributes[1].ErrorMessage);

        validator.OnPropertyChanged(validator,
            new ValidationPropertyChangedEventArgs("ErrorMessageResourceType", typeof(TestValidationMessages)));
        Assert.Null(validator.Attributes[0].ErrorMessageResourceType);
        Assert.Null(validator.Attributes[1].ErrorMessageResourceType);

        validator.OnPropertyChanged(validator,
            new ValidationPropertyChangedEventArgs("ErrorMessageResourceName", "TestValidator_ValidationError"));
        Assert.Null(validator.Attributes[0].ErrorMessageResourceName);
        Assert.Null(validator.Attributes[1].ErrorMessageResourceName);

        var validator2 = new AttributeValueValidator(new StringLengthAttribute(3));
        validator2.OnPropertyChanged(validator2, new ValidationPropertyChangedEventArgs("ErrorMessage", "错误信息"));
        Assert.Equal("错误信息", validator2.Attributes[0].ErrorMessage);

        validator2.OnPropertyChanged(validator2,
            new ValidationPropertyChangedEventArgs("ErrorMessageResourceType", typeof(TestValidationMessages)));
        Assert.Equal(typeof(TestValidationMessages), validator2.Attributes[0].ErrorMessageResourceType);

        validator2.OnPropertyChanged(validator2,
            new ValidationPropertyChangedEventArgs("ErrorMessageResourceName", "TestValidator_ValidationError"));
        Assert.Equal("TestValidator_ValidationError", validator2.Attributes[0].ErrorMessageResourceName);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3));
        validator.ErrorMessage = "错误信息";
        Assert.Equal("错误信息", validator.Attributes[0].ErrorMessage);

        validator.Dispose();

        validator.ErrorMessage = "错误信息2";
        Assert.Equal("错误信息", validator.Attributes[0].ErrorMessage);
    }
}