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
        Assert.Null(validator._serviceProvider);
        Assert.Empty(validator.Items);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Null(validator._errorMessageResourceAccessor());

        var validator2 = new AttributeValueValidator([new RequiredAttribute(), new StringLengthAttribute(3)],
            new Dictionary<object, object?> { { "id", 1 } });
        Assert.Null(validator2._serviceProvider);
        Assert.NotNull(validator2.Items);
        Assert.Single(validator2.Items);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validator3 = new AttributeValueValidator([new RequiredAttribute(), new StringLengthAttribute(3)],
            serviceProvider, new Dictionary<object, object?> { { "id", 1 } });
        Assert.NotNull(validator3._serviceProvider);
        Assert.NotNull(validator3.Items);
        Assert.Single(validator3.Items);
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
    public void InitializeServiceProvider_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        Assert.Null(validator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(validator._serviceProvider);

        validator.InitializeServiceProvider(null);
        Assert.Null(validator._serviceProvider);
    }

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        var validator = new AttributeValueValidator(new StringLengthAttribute(3), new RequiredAttribute());
        var validationContext = validator.CreateValidationContext(new ObjectClassTest(), null, null, null);
        Assert.Equal("ObjectClassTest", validationContext.DisplayName);
        Assert.Null(validationContext.MemberName);

        var validationContext2 =
            validator.CreateValidationContext(new ObjectClassTest(), "DisplayName", "MemberName", null);
        Assert.Equal("DisplayName", validationContext2.DisplayName);
        Assert.Equal("MemberName", validationContext2.MemberName);

        var validationContext3 = validator.CreateValidationContext(new ObjectClassTest(), null, null, null);
        Assert.Equal("ObjectClassTest", validationContext3.DisplayName);
        Assert.Null(validationContext3.MemberName);

        var serviceProviderField = typeof(ValidationContext).GetField("_serviceProvider",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(serviceProviderField);

        Assert.Null(validator._serviceProvider);
        Assert.Null(serviceProviderField.GetValue(validationContext3));

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);
        Assert.NotNull(validator._serviceProvider);

        var validationContext4 = validator.CreateValidationContext(new ObjectClassTest(), null, null, null);
        Assert.NotNull(validator._serviceProvider);
        Assert.NotNull(serviceProviderField.GetValue(validationContext4));

        var validationContext5 = validator.CreateValidationContext(new ObjectClassTest(), null, null, ["login"]);
        Assert.NotNull(validator._serviceProvider);
        Assert.NotNull(serviceProviderField.GetValue(validationContext5));
        Assert.Single(validationContext5.Items);
        var metadata =
            validationContext5.Items[Constants.ValidationOptionsKey] as ValidationOptionsMetadata;
        Assert.NotNull(metadata);
        Assert.Equal(["login"], (string[]?)metadata.RuleSets!);
    }
}