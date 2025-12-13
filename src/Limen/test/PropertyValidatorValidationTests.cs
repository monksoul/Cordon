// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class PropertyValidatorValidationTests
{
    [Fact]
    public void GetValidators_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ValidationModel>();
        var propertyValidator = new PropertyValidator<ValidationModel, object?>(u => u.Data1, objectValidator)
            .Required().NotBlank();

        Assert.Equal(2, propertyValidator.GetValidators().Count);
    }

    [Fact]
    public void WithErrorMessage_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ValidationModel>();
        var propertyValidator = new PropertyValidator<ValidationModel, object?>(u => u.Data1, objectValidator);

        propertyValidator.WithErrorMessage("错误消息");
        propertyValidator.WithErrorMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");

        propertyValidator.AddValidator(new MinLengthValidator(3));
        Assert.NotNull(propertyValidator._lastAddedValidator);
        propertyValidator.WithErrorMessage("错误信息");
        Assert.Null(propertyValidator._lastAddedValidator);
        Assert.Equal("错误信息", propertyValidator.Validators.Last().ErrorMessage);

        propertyValidator.AddValidator(new MaxLengthValidator(10));
        Assert.NotNull(propertyValidator._lastAddedValidator);
        propertyValidator.WithErrorMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");
        Assert.Null(propertyValidator._lastAddedValidator);
        Assert.Equal(typeof(TestValidationMessages), propertyValidator.Validators.Last().ErrorMessageResourceType);
        Assert.Equal("TestValidator_ValidationError", propertyValidator.Validators.Last().ErrorMessageResourceName);
    }

    [Fact]
    public void WithMessage_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ValidationModel>();
        var propertyValidator = new PropertyValidator<ValidationModel, object?>(u => u.Data1, objectValidator);

        propertyValidator.WithMessage("错误消息");
        propertyValidator.WithMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");

        propertyValidator.AddValidator(new MinLengthValidator(3));
        Assert.NotNull(propertyValidator._lastAddedValidator);
        propertyValidator.WithMessage("错误信息");
        Assert.Null(propertyValidator._lastAddedValidator);
        Assert.Equal("错误信息", propertyValidator.Validators.Last().ErrorMessage);

        propertyValidator.AddValidator(new MaxLengthValidator(10));
        Assert.NotNull(propertyValidator._lastAddedValidator);
        propertyValidator.WithMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");
        Assert.Null(propertyValidator._lastAddedValidator);
        Assert.Equal(typeof(TestValidationMessages), propertyValidator.Validators.Last().ErrorMessageResourceType);
        Assert.Equal("TestValidator_ValidationError", propertyValidator.Validators.Last().ErrorMessageResourceName);
    }

    [Fact]
    public void WithDisplayName_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ValidationModel>();
        var propertyValidator = new PropertyValidator<ValidationModel, object?>(u => u.Data1, objectValidator);
        Assert.Null(propertyValidator.DisplayName);

        propertyValidator.WithDisplayName("MyName");
        Assert.Equal("MyName", propertyValidator.DisplayName);
        propertyValidator.WithDisplayName(null);
        Assert.Null(propertyValidator.DisplayName);
    }

    [Fact]
    public void WithMemberName_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ValidationModel>();
        var propertyValidator = new PropertyValidator<ValidationModel, object?>(u => u.Data1, objectValidator);
        Assert.Null(propertyValidator.MemberName);

        propertyValidator.WithMemberName("MyName");
        Assert.Equal("MyName", propertyValidator.MemberName);
        propertyValidator.WithMemberName(null);
        Assert.Null(propertyValidator.MemberName);
    }

    [Fact]
    public void WithName_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ValidationModel>();
        var propertyValidator = new PropertyValidator<ValidationModel, object?>(u => u.Data1, objectValidator);
        Assert.Null(propertyValidator.MemberName);

        propertyValidator.WithName("MyName");
        Assert.Equal("MyName", propertyValidator.MemberName);
        propertyValidator.WithName(null);
        Assert.Null(propertyValidator.MemberName);
    }

    [Fact]
    public void UseAnnotationValidation_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ValidationModel>();
        var propertyValidator = new PropertyValidator<ValidationModel, object?>(u => u.Data1, objectValidator);

        Assert.Null(propertyValidator.SuppressAnnotationValidation);

        propertyValidator.UseAnnotationValidation(false);
        Assert.True(propertyValidator.SuppressAnnotationValidation);

        propertyValidator.UseAnnotationValidation(true);
        Assert.False(propertyValidator.SuppressAnnotationValidation);

        propertyValidator.SkipAnnotationValidation();
        Assert.True(propertyValidator.SuppressAnnotationValidation);

        propertyValidator.UseAnnotationValidation();
        Assert.False(propertyValidator.SuppressAnnotationValidation);

        propertyValidator.CustomOnly();
        Assert.True(propertyValidator.SuppressAnnotationValidation);

        propertyValidator.UseAnnotationValidation(null);
        Assert.Null(propertyValidator.SuppressAnnotationValidation);
    }

    [Fact]
    public void Age_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Age();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as AgeValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.IsAdultOnly);
        Assert.False(addedValidator.AllowStringValues);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = 8 }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = 120 }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = 121 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Age(true, true);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as AgeValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.IsAdultOnly);
        Assert.True(addedValidator2.AllowStringValues);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = 8 }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = 120 }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = 121 }));
    }

    [Fact]
    public void AllowedValues_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .AllowedValues("Furion", "百小僧", "MonkSoul");

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as AllowedValuesValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Fur" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));
    }

    [Fact]
    public void BankCard_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .BankCard();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as BankCardValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "5502092303469876" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "6228480402564890018" }));
    }

    [Fact]
    public void Base64String_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Base64String();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as Base64StringValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "ZnVyaW9u" }));
    }

    [Fact]
    public void ChineseName_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .ChineseName();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as ChineseNameValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "蒙奇·D·路飞" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));
    }

    [Fact]
    public void Chinese_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Chinese();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as ChineseValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));
    }

    [Fact]
    public void ColorValue_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .ColorValue();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as ColorValueValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.FullMode);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "hsl(0, 100%, 50%)" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "#ffffff" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .ColorValue(true);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as ColorValueValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.FullMode);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "hsl(0, 100%, 50%)" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "#ffffff" }));
    }

    [Fact]
    public void Composite_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Composite(new NotNullValidator(), new MinLengthValidator(3));

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as CompositeValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(2, addedValidator.Validators.Count);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = null }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Fu" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Composite([new EmailAddressValidator(), new UserNameValidator()], ValidationMode.BreakOnFirstSuccess);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as CompositeValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(2, addedValidator2.Validators.Count);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = null }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "monksoul@outlook.com" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "monksoul" }));
    }

    [Fact]
    public void Conditional_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.String1)
                .Conditional(null!));

    [Fact]
    public void Conditional_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.String1)
            .Conditional(builder => builder.When(u => u?.Contains('@') == true).Then(b => b.EmailAddress())
                .Otherwise(b => b.UserName()));

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as ConditionalValidator<string>;
        Assert.NotNull(addedValidator);

        Assert.True(propertyValidator.IsValid(new ValidationModel { String1 = "monksoul@outlook.com" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { String1 = "monk__soul" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.String1)
            .Conditional((builder, _) => builder.When(u => u?.Contains('@') == true).Then(b => b.EmailAddress())
                .Otherwise(b => b.UserName()));

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, ConditionalValidator<string>>;
        Assert.NotNull(addedValidator2);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { String1 = "monksoul@outlook.com" }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { String1 = "monk__soul" }));
    }

    [Fact]
    public void DateOnly_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .DateOnly();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as DateOnlyValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator.Style);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "2023-09-26 16:41:56" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "26/09/2023" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "2023-09-26" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .DateOnly(["yyyy-MM-dd", "dd/MM/yyyy"], CultureInfo.InvariantCulture);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as DateOnlyValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator2.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator2.Style);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "2023-09-26 16:41:56" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "26/09/2023" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "2023-09-26" }));
    }

    [Fact]
    public void DateTime_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .DateTime();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as DateTimeValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator.Style);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "26/09/2023" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "2023-09-26 16:41:56" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .DateTime(["yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy"], CultureInfo.InvariantCulture);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as DateTimeValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator2.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator2.Style);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "26/09/2023" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "2023-09-26 16:41:56" }));
    }

    [Fact]
    public void DecimalPlaces_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .DecimalPlaces(1);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as DecimalPlacesValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(1, addedValidator.MaxDecimalPlaces);
        Assert.False(addedValidator.AllowStringValues);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = 2.23 }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = 2 }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "2.2" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .DecimalPlaces(1, true);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as DecimalPlacesValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(1, addedValidator2.MaxDecimalPlaces);
        Assert.True(addedValidator2.AllowStringValues);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = 2.23 }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = 2 }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "2.2" }));
    }

    [Fact]
    public void DeniedValues_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .DeniedValues("Furion", "百小僧", "MonkSoul");

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as DeniedValuesValidator;
        Assert.NotNull(addedValidator);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Fur" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));
    }

    [Fact]
    public void Domain_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Domain();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as DomainValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "https://furion.net" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "furion.net" }));
    }

    [Fact]
    public void EmailAddress_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .EmailAddress();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as EmailAddressValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "monksoul@" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "monksoul@outlook.com" }));
    }

    [Fact]
    public void EndsWith_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .EndsWith("ion");

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as EndsWithValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(StringComparison.Ordinal, addedValidator.Comparison);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Fur" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "FURION" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .EndsWith("ion", StringComparison.OrdinalIgnoreCase);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as EndsWithValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, addedValidator2.Comparison);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "Fur" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "FURION" }));
    }

    [Fact]
    public void EqualTo_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Data1).EqualTo(null!));

    [Fact]
    public void EqualTo_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .EqualTo("Furion");

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as EqualToValidator;
        Assert.NotNull(addedValidator);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .EqualTo(u => u.Instance.Data2);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, EqualToValidator>;
        Assert.NotNull(addedValidator2);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "Furion", Data2 = "Furion" }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "百小僧", Data2 = "Furion" }));
    }

    [Fact]
    public void GreaterThanOrEqualTo_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Data1)
                .GreaterThanOrEqualTo(null!));

    [Fact]
    public void GreaterThanOrEqualTo_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .GreaterThanOrEqualTo(10);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as GreaterThanOrEqualToValidator;
        Assert.NotNull(addedValidator);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 9 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .GreaterThanOrEqualTo(u => u.Instance.Number2);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, GreaterThanOrEqualToValidator>;
        Assert.NotNull(addedValidator2);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Number1 = 10, Number2 = 9 }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Number1 = 10, Number2 = 11 }));
    }

    [Fact]
    public void GreaterThan_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Data1)
                .GreaterThan(null!));

    [Fact]
    public void GreaterThan_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .GreaterThan(10);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as GreaterThanValidator;
        Assert.NotNull(addedValidator);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 9 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .GreaterThan(u => u.Instance.Number2);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, GreaterThanValidator>;
        Assert.NotNull(addedValidator2);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Number1 = 10, Number2 = 9 }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Number1 = 10, Number2 = 11 }));
    }

    [Fact]
    public void IDCard_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .IDCard();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as IDCardValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "1234569910101933" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "622223199912051311" }));
    }

    [Fact]
    public void IpAddress_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .IpAddress();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as IpAddressValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.AllowIPv6);

        Assert.False(
            propertyValidator.IsValid(new ValidationModel { Data1 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "192.168.1.1" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .IpAddress(true);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as IpAddressValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.AllowIPv6);

        Assert.True(
            propertyValidator2.IsValid(new ValidationModel { Data1 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "192.168.1.1" }));
    }

    [Fact]
    public void Json_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Json();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as JsonValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.AllowTrailingCommas);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = """{"id":1,"name":"furion",}""" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = """{"id":1,"name":"furion"}""" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Json(true);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as JsonValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.AllowTrailingCommas);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = """{"id":1,"name":"furion",}""" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = """{"id":1,"name":"furion"}""" }));
    }

    [Fact]
    public void Length_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Length(3, 5);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as LengthValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Fu" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));
    }

    [Fact]
    public void LessThanOrEqualTo_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Data1)
                .LessThanOrEqualTo(null!));

    [Fact]
    public void LessThanOrEqualTo_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .LessThanOrEqualTo(10);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as LessThanOrEqualToValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 9 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .LessThanOrEqualTo(u => u.Instance.Number2);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, LessThanOrEqualToValidator>;
        Assert.NotNull(addedValidator2);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Number1 = 10, Number2 = 9 }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Number1 = 10, Number2 = 11 }));
    }

    [Fact]
    public void LessThan_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Data1)
                .LessThan(null!));

    [Fact]
    public void LessThan_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .LessThan(10);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as LessThanValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 9 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .LessThan(u => u.Instance.Number2);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, LessThanValidator>;
        Assert.NotNull(addedValidator2);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Number1 = 10, Number2 = 9 }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Number1 = 10, Number2 = 11 }));
    }

    [Fact]
    public void MaxLength_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .MaxLength(3);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as MaxLengthValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));
    }

    [Fact]
    public void Max_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Max(10);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as MaxValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 9 }));
    }

    [Fact]
    public void MD5String_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .MD5String();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as MD5StringValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.AllowShortFormat);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "EF4DF562719E70E4" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "3f2d0ea0ef4df562719e70e41413658e" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .MD5String(true);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as MD5StringValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.AllowShortFormat);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "EF4DF562719E70E4" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "3f2d0ea0ef4df562719e70e41413658e" }));
    }

    [Fact]
    public void MinLength_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .MinLength(3);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as MinLengthValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Fu" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));
    }

    [Fact]
    public void Min_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Min(10);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as MinValidator;
        Assert.NotNull(addedValidator);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 9 }));
    }

    [Fact]
    public void MustUnless_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Number1)
                .MustUnless((Func<int, bool>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Number1)
                .MustUnless(null!));
    }

    [Fact]
    public void MustUnless_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .MustUnless(u => u <= 10);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as MustUnlessValidator<int>;
        Assert.NotNull(addedValidator);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 9 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .MustUnless((_, ctx) => ctx.Instance.Number1 <= 10);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, MustUnlessValidator<int>>;
        Assert.NotNull(addedValidator2);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Number1 = 9 }));
    }

    [Fact]
    public void Must_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Number1)
                .Must((Func<int, bool>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Number1)
                .Must(null!));
    }

    [Fact]
    public void Must_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Must(u => u > 10);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as MustValidator<int>;
        Assert.NotNull(addedValidator);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 9 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Must((_, ctx) => ctx.Instance.Number1 > 10);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, MustValidator<int>>;
        Assert.NotNull(addedValidator2);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Number1 = 9 }));
    }

    [Fact]
    public void NotBlank_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .NotBlank();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as NotBlankValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = string.Empty }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
    }

    [Fact]
    public void NotEqualTo_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Data1).NotEqualTo(null!));

    [Fact]
    public void NotEqualTo_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .NotEqualTo("Furion");

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as NotEqualToValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "百小僧" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .NotEqualTo(u => u.Instance.Data2);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, NotEqualToValidator>;
        Assert.NotNull(addedValidator2);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "Furion", Data2 = "Furion" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "百小僧", Data2 = "Furion" }));
    }

    [Fact]
    public void NotEmpty_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .NotEmpty();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as NotEmptyValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = string.Empty }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
    }

    [Fact]
    public void NotNull_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .NotNull();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as NotNullValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = null }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
    }

    [Fact]
    public void Password_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Password();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as PasswordValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.Strong);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "q1w2e3" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "q1w2e3r4" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Password(true);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as PasswordValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.Strong);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "q1w2e3r4" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "cln99871433*_Q" }));
    }

    [Fact]
    public void PhoneNumber_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .PhoneNumber();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as PhoneNumberValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "14000000000" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "13800138000" }));
    }

    [Fact]
    public void PostalCode_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .PostalCode();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as PostalCodeValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "1001001" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "100101" }));
    }

    [Fact]
    public void Predicate_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Number1)
                .Predicate((Func<int, bool>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new ObjectValidator<ValidationModel>().RuleFor(u => u.Number1)
                .Predicate(null!));
    }

    [Fact]
    public void Predicate_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Predicate(u => u > 10);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as PredicateValidator<int>;
        Assert.NotNull(addedValidator);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 9 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Predicate((_, ctx) => ctx.Instance.Number1 > 10);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, PredicateValidator<int>>;
        Assert.NotNull(addedValidator2);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Number1 = 11 }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Number1 = 9 }));
    }

    [Fact]
    public void Range_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Range(2, 5);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 1 }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 3 }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 5 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Range(2.0, 5.0);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator2);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = 1.0 }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = 3.0 }));

        var propertyValidator3 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Range(typeof(int), "2", "5");

        Assert.Single(propertyValidator3.Validators);

        var addedValidator3 = propertyValidator3._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator3);

        Assert.False(propertyValidator3.IsValid(new ValidationModel { Number1 = 1 }));
        Assert.True(propertyValidator3.IsValid(new ValidationModel { Number1 = 3 }));

        var propertyValidator4 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Range(2, 5, u => u.MaximumIsExclusive = true);

        Assert.Single(propertyValidator4.Validators);

        var addedValidator4 = propertyValidator4._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator4);

        Assert.False(propertyValidator4.IsValid(new ValidationModel { Number1 = 1 }));
        Assert.True(propertyValidator4.IsValid(new ValidationModel { Number1 = 3 }));
        Assert.False(propertyValidator4.IsValid(new ValidationModel { Number1 = 5 }));
    }

    [Fact]
    public void Between_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Between(2, 5);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Number1 = 1 }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 3 }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Number1 = 5 }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Between(2.0, 5.0);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator2);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = 1.0 }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = 3.0 }));

        var propertyValidator3 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Between(typeof(int), "2", "5");

        Assert.Single(propertyValidator3.Validators);

        var addedValidator3 = propertyValidator3._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator3);

        Assert.False(propertyValidator3.IsValid(new ValidationModel { Number1 = 1 }));
        Assert.True(propertyValidator3.IsValid(new ValidationModel { Number1 = 3 }));

        var propertyValidator4 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Number1)
            .Between(2, 5, u => u.MaximumIsExclusive = true);

        Assert.Single(propertyValidator4.Validators);

        var addedValidator4 = propertyValidator4._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator4);

        Assert.False(propertyValidator4.IsValid(new ValidationModel { Number1 = 1 }));
        Assert.True(propertyValidator4.IsValid(new ValidationModel { Number1 = 3 }));
        Assert.False(propertyValidator4.IsValid(new ValidationModel { Number1 = 5 }));
    }

    [Fact]
    public void RegularExpression_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .RegularExpression("^[1-9]{2,5}$");

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as RegularExpressionValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(2000, addedValidator.MatchTimeoutInMilliseconds);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "123456" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "12345" }));
    }

    [Fact]
    public void Matches_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Matches("^[1-9]{2,5}$");

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as RegularExpressionValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(2000, addedValidator.MatchTimeoutInMilliseconds);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "123456" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "12345" }));
    }

    [Fact]
    public void Required_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Required();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as RequiredValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.AllowEmptyStrings);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = null }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = string.Empty }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Required(true);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as RequiredValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.AllowEmptyStrings);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = null }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = string.Empty }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "Furion" }));
    }

    [Fact]
    public void Single_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Single();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as SingleValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = new List<int>() }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = new List<int> { 1 } }));
    }

    [Fact]
    public void StartsWith_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .StartsWith("Fu");

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as StartsWithValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(StringComparison.Ordinal, addedValidator.Comparison);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Free" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "FURION" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .StartsWith("Fu", StringComparison.OrdinalIgnoreCase);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as StartsWithValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, addedValidator2.Comparison);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "Free" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "FURION" }));
    }

    [Fact]
    public void StringContains_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .StringContains("ion");

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as StringContainsValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(StringComparison.Ordinal, addedValidator.Comparison);

        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "Free" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "FURION" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .StringContains("ion", StringComparison.OrdinalIgnoreCase);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as StringContainsValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, addedValidator2.Comparison);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "Furion" }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "Free" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "FURION" }));
    }

    [Fact]
    public void StringLength_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .StringLength(6);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as StringLengthValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "monksoul" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Fu" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "Furion" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .StringLength(3, 6);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as StringLengthValidator;
        Assert.NotNull(addedValidator2);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "monksoul" }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "Fu" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "Furion" }));
    }

    [Fact]
    public void StrongPassword_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .StrongPassword();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as StrongPasswordValidator;
        Assert.NotNull(addedValidator);
        Assert.True(addedValidator.Strong);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "q1w2e3r4" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "cln99871433*_Q" }));
    }

    [Fact]
    public void Telephone_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Telephone();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as TelephoneValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "076088809963" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "0760-88809963" }));
    }

    [Fact]
    public void TimeOnly_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .TimeOnly();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as TimeOnlyValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator.Style);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "2023-09-26 16:41:56" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "14:32-30" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "14:32:30" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .TimeOnly(["HH:mm:ss", "HH:mm-ss"], CultureInfo.InvariantCulture);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as TimeOnlyValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator2.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator2.Style);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "2023-09-26 16:41:56" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "14:32-30" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "14:32:30" }));
    }

    [Fact]
    public void Url_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Url();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as UrlValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.SupportsFtp);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "furion.net" }));
        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "ftp://furion.net/" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "https://furion.net/" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .Url(true);

        Assert.Single(propertyValidator.Validators);

        var addedValidator2 = propertyValidator2._lastAddedValidator as UrlValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.SupportsFtp);

        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "furion.net" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "ftp://furion.net/" }));
        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "https://furion.net/" }));
    }

    [Fact]
    public void UserName_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .UserName();

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as UserNameValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "monk__soul" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "monksoul" }));
    }

    [Fact]
    public void ValidatorProxy_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .ValidatorProxy<UserNameValidator>((object?[]?)null);

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as ValidatorProxy<UserNameValidator>;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "monk__soul" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "monksoul" }));

        var propertyValidator2 = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .ValidatorProxy<EqualToValidator>(context => [context.Instance.Data2]);

        Assert.Single(propertyValidator2.Validators);

        var addedValidator2 =
            propertyValidator2._lastAddedValidator as ValidatorProxy<ValidationModel, EqualToValidator>;
        Assert.NotNull(addedValidator2);

        Assert.True(propertyValidator2.IsValid(new ValidationModel { Data1 = "Furion", Data2 = "Furion" }));
        Assert.False(propertyValidator2.IsValid(new ValidationModel { Data1 = "百小僧", Data2 = "Furion" }));
    }

    [Fact]
    public void AddAnnotations_ReturnOK()
    {
        var propertyValidator = new ObjectValidator<ValidationModel>()
            .RuleFor(u => u.Data1)
            .AddAnnotations(new UserNameAttribute());

        Assert.Single(propertyValidator.Validators);

        var addedValidator = propertyValidator._lastAddedValidator as ValueAnnotationValidator;
        Assert.NotNull(addedValidator);

        Assert.False(propertyValidator.IsValid(new ValidationModel { Data1 = "monk__soul" }));
        Assert.True(propertyValidator.IsValid(new ValidationModel { Data1 = "monksoul" }));
    }


    [Fact]
    public void And_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Equal(objectValidator, propertyValidator.And());
    }

    [Fact]
    public void Then_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Equal(objectValidator, propertyValidator.Then());
    }

    [Fact]
    public void End_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>();
        var propertyValidator =
            new PropertyValidator<ObjectModel, string?>(u => u.Name, objectValidator);

        Assert.Equal(objectValidator, propertyValidator.End());
    }

    [Fact]
    public void RuleFor_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>().RuleFor(u => u.Name)
            .RuleFor(u => u.FirstName).End();
        Assert.Equal(2, objectValidator.Validators.Count);
    }

    [Fact]
    public void RuleForEach_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>().RuleForEach(u => u.Children)
            .RuleForEach(u => u.Children).End();
        Assert.Equal(2, objectValidator.Validators.Count);
    }

    [Fact]
    public void RuleSet_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ObjectModel>()
            .RuleFor(u => u.Name)
            .RuleSet("login", chain =>
            {
                chain.RuleFor(u => u.FirstName).RuleFor(u => u.Name);
            });
        Assert.Equal(3, objectValidator.Validators.Count);

        using var objectValidator2 = new ObjectValidator<ObjectModel>()
            .RuleFor(u => u.Name)
            .RuleSet(["login", "register"], chain =>
            {
                chain.RuleFor(u => u.FirstName).RuleFor(u => u.Name);
            });
        Assert.Equal(5, objectValidator2.Validators.Count);

        var objectValidator3 = new ObjectValidator<ObjectModel>();
        objectValidator3.RuleFor(u => u.Name)
            .RuleSet("login", _ => objectValidator3.RuleFor(u => u.FirstName));
        Assert.Equal(2, objectValidator3.Validators.Count);

        var objectValidator4 = new ObjectValidator<ObjectModel>();
        objectValidator4.RuleFor(u => u.Name)
            .RuleSet(["login", "register"], () => objectValidator4.RuleFor(u => u.FirstName));
        Assert.Equal(3, objectValidator4.Validators.Count);

        var objectValidator5 = new ObjectValidator<ObjectModel>();
        objectValidator5.RuleFor(u => u.Name)
            .RuleSet("login", () => objectValidator5.RuleFor(u => u.FirstName));
        Assert.Equal(2, objectValidator5.Validators.Count);
    }

    [Fact]
    public void Build_ReturnOK()
    {
        using var objectValidator = new ObjectValidator<ValidationModel>();
        var propertyValidator = new PropertyValidator<ValidationModel, object?>(u => u.Data1, objectValidator)
            .Required().NotBlank();

        Assert.Equal(2, propertyValidator.Build().Count);
    }

    public class ObjectModel
    {
        [Required] [MinLength(3)] public string? Name { get; set; }

        public string? FirstName { get; set; }

        public List<Child>? Children { get; set; }
    }

    public class ValidationModel
    {
        public object? Data1 { get; set; }

        public object? Data2 { get; set; }

        public int Number1 { get; set; }
        public int Number2 { get; set; }

        public string? String1 { get; set; }
    }

    public class Child
    {
        public string? Name { get; set; }
    }
}