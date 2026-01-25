// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class CompositeValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CompositeValidator<string>((Action<FluentValidatorBuilder<string>>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new CompositeValidator<string>((ValidatorBase[])null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new CompositeValidator<string>(_ => { });
        Assert.NotNull(validator);
        Assert.NotNull(validator._validators);
        Assert.Empty(validator._validators);
        Assert.Equal(CompositeMode.FailFast, validator.Mode);

        var validator2 =
            new CompositeValidator<string>(u => u.Chinese().Required().NotNull());

        Assert.NotNull(validator2._validators);
        Assert.Equal(3, validator2._validators.Count);
        Assert.Equal([typeof(NotNullValidator), typeof(RequiredValidator), typeof(ChineseValidator)],
            validator2._validators.Select(u => u.GetType()));
        Assert.Equal(CompositeMode.FailFast, validator2.Mode);

        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Null(validator2._errorMessageResourceAccessor());

        var validator3 =
            new CompositeValidator<string>([new ChineseValidator(), new RequiredValidator(), new NotNullValidator()]);

        Assert.NotNull(validator3._validators);
        Assert.Equal(3, validator3._validators.Count);
        Assert.Equal([typeof(NotNullValidator), typeof(RequiredValidator), typeof(ChineseValidator)],
            validator3._validators.Select(u => u.GetType()));
        Assert.Equal(CompositeMode.FailFast, validator3.Mode);

        var validator4 = new CompositeValidator<string>(_ => { }, CompositeMode.All);
        Assert.Equal(CompositeMode.All, validator4.Mode);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese());
        Assert.True(validator.IsValid(null));

        var validator2 = new CompositeValidator<string>(u => u.Chinese().Required());
        Assert.False(validator2.IsValid(null));

        var validator3 = new CompositeValidator<string>(u => u.Chinese().NotNull());
        Assert.False(validator3.IsValid(null));

        var validator4 = new CompositeValidator<string>(u => u.Chinese().NotNull());
        Assert.True(validator4.IsValid("百小僧"));

        var validator5 = new CompositeValidator<string>(u => u.Chinese().NotNull());
        Assert.False(validator5.IsValid("Furion"));
    }

    [Fact]
    public void IsValid_WithMode_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese().ChineseName());
        Assert.False(validator.IsValid("凯文·杜兰特"));

        validator.Mode = CompositeMode.FailFast;
        Assert.False(validator.IsValid("凯文·杜兰特"));

        validator.Mode = CompositeMode.Any;
        Assert.True(validator.IsValid("凯文·杜兰特"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese());
        Assert.Null(validator.GetValidationResults(null, "Value"));

        var validator2 = new CompositeValidator<string>(u => u.Chinese().Required());
        Assert.Null(validator2.GetValidationResults("百小僧", "Value"));
    }

    [Fact]
    public void GetValidationResults_WithNullValue_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese().Required());
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
        var validator = new CompositeValidator<string>(u => u.Chinese().Required());
        var validationResults = validator.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field Value contains invalid Chinese characters.",
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
    public void GetValidationResults_WithMode_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese().ChineseName());
        Assert.Null(validator.GetValidationResults(null, "Value"));

        validator.Mode = CompositeMode.FailFast;
        Assert.Null(validator.GetValidationResults(null, "Value"));

        validator.Mode = CompositeMode.Any;
        Assert.Null(validator.GetValidationResults(null, "Value"));

        var validator2 =
            new CompositeValidator<string>(u => u.Chinese().ChineseName()).UseMode(
                CompositeMode.All);
        var validationResults = validator2.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults);
        Assert.Equal(2, validationResults.Count);

        validator2.Mode = CompositeMode.FailFast;
        validationResults = validator2.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);

        validator2.Mode = CompositeMode.Any;
        validationResults = validator2.GetValidationResults("Furion", "Value");
        Assert.NotNull(validationResults);
        Assert.Equal(2, validationResults.Count);

        var validator3 = new CompositeValidator<string>(u => u.Chinese().ChineseName());
        var validationResults2 = validator3.GetValidationResults("凯文·杜兰特", "Value");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);

        validator3.Mode = CompositeMode.FailFast;
        validationResults2 = validator3.GetValidationResults("凯文·杜兰特", "Value");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);

        validator3.Mode = CompositeMode.Any;
        validationResults2 = validator3.GetValidationResults("凯文·杜兰特", "Value");
        Assert.Null(validationResults2);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese());
        validator.Validate(null, "Value");

        var validator2 = new CompositeValidator<string>(u => u.Chinese().Required());
        validator2.Validate("百小僧", "Value");
    }

    [Fact]
    public void Validate_WithNullValue_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese().Required());
        var exception = Assert.Throws<ValidationException>(() => validator.Validate(null, "Value"));
        Assert.Equal("The Value field is required.", exception.Message);

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
        var validator = new CompositeValidator<string>(u => u.Chinese().Required());
        var exception = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "Value"));
        Assert.Equal("The field Value contains invalid Chinese characters.", exception.Message);

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
    public void Validate_WithMode_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese().ChineseName());
        validator.Validate(null, "Value");

        validator.Mode = CompositeMode.FailFast;
        validator.Validate(null, "Value");

        validator.Mode = CompositeMode.Any;
        validator.Validate(null, "Value");

        var validator2 = new CompositeValidator<string>(u => u.Chinese().ChineseName());
        Assert.Throws<ValidationException>(() => validator2.Validate("Furion", "Value"));

        validator2.Mode = CompositeMode.FailFast;
        Assert.Throws<ValidationException>(() => validator2.Validate("Furion", "Value"));

        validator2.Mode = CompositeMode.Any;
        Assert.Throws<ValidationException>(() => validator2.Validate("Furion", "Value"));

        var validator3 = new CompositeValidator<string>(u => u.Chinese().ChineseName());
        Assert.Throws<ValidationException>(() => validator3.Validate("凯文·杜兰特", "Value"));

        validator3.Mode = CompositeMode.FailFast;
        Assert.Throws<ValidationException>(() => validator3.Validate("凯文·杜兰特", "Value"));

        validator3.Mode = CompositeMode.Any;
        validator3.Validate("凯文·杜兰特", "Value");
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese().Required());
        Assert.Null(validator.FormatErrorMessage(null!));

        validator.ErrorMessage = "自定义错误信息";
        Assert.Equal("自定义错误信息", validator.FormatErrorMessage(null!));
    }

    [Fact]
    public void UseMode_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese());
        Assert.Equal(CompositeMode.FailFast, validator.Mode);
        validator.UseMode(CompositeMode.All);
        Assert.Equal(CompositeMode.All, validator.Mode);
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese());
        validator.Dispose();
    }

    [Fact]
    public void ThrowValidationException_Invalid_Parameters()
    {
        var validator = new CompositeValidator<string>(u => u.Chinese().Required());
        Assert.Throws<ArgumentNullException>(() => validator.ThrowValidationException(null, null!, null!));
    }

    [Fact]
    public void ThrowValidationException_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.Required().Chinese());

        var exception = Assert.Throws<ValidationException>(() =>
            validator.ThrowValidationException(null, validator._validators.First(),
                new ValidationContext<string>(null!) { DisplayName = "Value" }));
        Assert.Equal("The Value field is required.", exception.Message);

        var exception2 = Assert.Throws<ValidationException>(() =>
            validator.ThrowValidationException("Furion", validator._validators.Last(),
                new ValidationContext<string>("Furion") { DisplayName = "Value" }));
        Assert.Equal("The field Value contains invalid Chinese characters.", exception2.Message);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var validator = new CompositeValidator<string>(u => u.WithAttributes(new RequiredAttribute()));
        var attributeValueValidator = validator._validators[0] as AttributeValueValidator;
        Assert.NotNull(attributeValueValidator);
        Assert.Null(attributeValueValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(attributeValueValidator._serviceProvider);
    }
}