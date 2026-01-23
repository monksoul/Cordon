// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ConditionalValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ConditionalValidator<int>(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new ConditionalValidator<int>(_ => { });
        Assert.NotNull(validator._conditionResult);
        Assert.Empty(validator._conditionResult.ConditionalRules);
        Assert.Null(validator._conditionResult.DefaultRules);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Null(validator._errorMessageResourceAccessor());

        var validator2 = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));
        Assert.NotNull(validator2._conditionResult);
        Assert.Single(validator2._conditionResult.ConditionalRules);
        Assert.NotNull(validator2._conditionResult.DefaultRules);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));

        Assert.True(validator.IsValid("monksoul@outlook.com"));
        Assert.False(validator.IsValid("monksoul@outlook"));
        Assert.True(validator.IsValid("monksoul"));
        Assert.False(validator.IsValid("monk__soul"));
    }

    [Fact]
    public void IsValid_WithoutOtherwise_ReturnOK()
    {
        var validator =
            new ConditionalValidator<string>(builder =>
                builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()));

        Assert.True(validator.IsValid("monksoul@outlook.com"));
        Assert.False(validator.IsValid("monksoul@outlook"));
        Assert.True(validator.IsValid("monksoul"));
        Assert.True(validator.IsValid("monk__soul"));
    }

    [Fact]
    public void IsValid_WithoutConditions_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(_ => { });
        Assert.True(validator.IsValid("monksoul@outlook.com"));
        Assert.True(validator.IsValid("monksoul@outlook"));
        Assert.True(validator.IsValid("monksoul"));
        Assert.True(validator.IsValid("monk__soul"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));

        Assert.Null(validator.GetValidationResults("monksoul@outlook.com", "Value"));

        var validationResults = validator.GetValidationResults("monksoul@outlook", "Value");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The Value field is not a valid e-mail address.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "自定义错误信息";
        var validationResults2 = validator.GetValidationResults("monksoul@outlook", "Value");
        Assert.NotNull(validationResults2);
        Assert.Equal(2, validationResults2.Count);
        Assert.Equal("自定义错误信息", validationResults2.First().ErrorMessage);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var validationResults3 = validator.GetValidationResults("monksoul@outlook", "Value");
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal("单元测试Value错误信息2", validationResults3.First().ErrorMessage);

        Assert.Null(validator.GetValidationResults("monksoul", "Value"));

        var validator2 = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));
        var validationResults4 = validator2.GetValidationResults("monk__soul", "Value");
        Assert.NotNull(validationResults4);
        Assert.Single(validationResults4);
        Assert.Equal("The field Value is not a valid username.", validationResults4.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithoutOtherwise_ReturnOK()
    {
        var validator =
            new ConditionalValidator<string>(builder =>
                builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()));

        Assert.Null(validator.GetValidationResults("monksoul@outlook.com", "Value"));

        var validationResults = validator.GetValidationResults("monksoul@outlook", "Value");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The Value field is not a valid e-mail address.", validationResults.First().ErrorMessage);

        Assert.Null(validator.GetValidationResults("monksoul", "Value"));
        Assert.Null(validator.GetValidationResults("monk__soul", "Value"));
    }

    [Fact]
    public void GetValidationResults_WithoutConditions_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(_ => { });
        Assert.Null(validator.GetValidationResults("monksoul@outlook.com", "Value"));
        Assert.Null(validator.GetValidationResults("monksoul@outlook", "Value"));
        Assert.Null(validator.GetValidationResults("monksoul", "Value"));
        Assert.Null(validator.GetValidationResults("monk__soul", "Value"));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));

        validator.Validate("monksoul@outlook.com", "Value");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("monksoul@outlook", "Value"));
        Assert.Equal("The Value field is not a valid e-mail address.", exception.Message);

        validator.ErrorMessage = "自定义错误信息";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("monksoul@outlook", "Value"));
        Assert.Equal("自定义错误信息", exception2.Message);

        validator.ErrorMessage = null;
        validator.ErrorMessageResourceType = typeof(TestValidationMessages);
        validator.ErrorMessageResourceName = "TestValidator_ValidationError2";
        var exception3 = Assert.Throws<ValidationException>(() => validator.Validate("monksoul@outlook", "Value"));
        Assert.Equal("单元测试Value错误信息2", exception3.Message);

        validator.Validate("monksoul", "Value");

        var validator2 = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));
        var exception4 = Assert.Throws<ValidationException>(() => validator2.Validate("monk__soul", "Value"));
        Assert.Equal("The field Value is not a valid username.", exception4.Message);
    }

    [Fact]
    public void Validate_WithoutOtherwise_ReturnOK()
    {
        var validator =
            new ConditionalValidator<string>(builder =>
                builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()));

        validator.Validate("monksoul@outlook.com", "Value");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("monksoul@outlook", "Value"));
        Assert.Equal("The Value field is not a valid e-mail address.", exception.Message);

        validator.Validate("monksoul", "Value");
        validator.Validate("monk__soul", "Value");
    }

    [Fact]
    public void Validate_WithoutConditions_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(_ => { });
        validator.Validate("monksoul@outlook.com", "Value");
        validator.Validate("monksoul@outlook", "Value");
        validator.Validate("monksoul", "Value");
        validator.Validate("monk__soul", "Value");
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));
        Assert.Null(validator.FormatErrorMessage(null!));

        validator.ErrorMessage = "自定义错误信息";
        Assert.Equal("自定义错误信息", validator.FormatErrorMessage(null!));
    }

    [Fact]
    public void Dispose_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(_ => { });
        validator.Dispose();
    }

    [Fact]
    public void ThrowValidationException_Invalid_Parameters()
    {
        var validator = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));
        Assert.Throws<ArgumentNullException>(() => validator.ThrowValidationException(null, null!, null!));
    }

    [Fact]
    public void ThrowValidationException_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));

        var exception = Assert.Throws<ValidationException>(() =>
            validator.ThrowValidationException("monk__soul",
                validator._conditionResult.ConditionalRules.Last().Validators[0],
                new ValidationContext<string>("monk_soul") { DisplayName = "Value" }));
        Assert.Equal("The Value field is not a valid e-mail address.", exception.Message);

        var exception2 = Assert.Throws<ValidationException>(() =>
            validator.ThrowValidationException("monksoul@qq", validator._conditionResult.DefaultRules![0],
                new ValidationContext<string>("monksoul@qq") { DisplayName = "Value" }));
        Assert.Equal("The field Value is not a valid username.", exception2.Message);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.WithAttributes(new RequiredAttribute()))
                .Otherwise(b => b.WithAttributes(new UserNameAttribute())));

        var conditionAttributeValueValidator =
            validator._conditionResult.ConditionalRules.SelectMany(u => u.Validators).First() as
                AttributeValueValidator;
        Assert.NotNull(conditionAttributeValueValidator);
        var defaultAttributeValueValidator =
            validator._conditionResult.DefaultRules?[0] as AttributeValueValidator;
        Assert.NotNull(defaultAttributeValueValidator);

        Assert.Null(conditionAttributeValueValidator._serviceProvider);
        Assert.Null(defaultAttributeValueValidator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(conditionAttributeValueValidator._serviceProvider);
        Assert.NotNull(defaultAttributeValueValidator._serviceProvider);
    }

    [Fact]
    public void GetMatchedValidators_ReturnOK()
    {
        var validator = new ConditionalValidator<string>(builder =>
            builder.When(u => u.Contains('@')).Then(b => b.EmailAddress()).Otherwise(b => b.UserName()));

        var matchedValidators = validator.GetMatchedValidators("monksoul@outlook");
        Assert.NotNull(matchedValidators);
        Assert.Single(matchedValidators);
        Assert.True(matchedValidators[0] is EmailAddressValidator);

        var matchedValidators2 = validator.GetMatchedValidators("monk__soul");
        Assert.NotNull(matchedValidators2);
        Assert.Single(matchedValidators2);
        Assert.True(matchedValidators2[0] is UserNameValidator);

        var validator2 = new ConditionalValidator<string>(_ => { });
        var matchedValidators3 = validator2.GetMatchedValidators("monksoul@outlook.com");
        Assert.Null(matchedValidators3);
    }
}