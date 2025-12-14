// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ValueValidatorValidationTests
{
    [Fact]
    public void GetValidators_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Required().NotBlank();
        Assert.Equal(2, valueValidator.GetValidators().Count);
    }

    [Fact]
    public void WithErrorMessage_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>();

        valueValidator.WithErrorMessage("错误消息");
        valueValidator.WithErrorMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");

        valueValidator.AddValidator(new MinLengthValidator(3));
        Assert.NotNull(valueValidator._lastAddedValidator);
        valueValidator.WithErrorMessage("错误信息");
        Assert.Null(valueValidator._lastAddedValidator);
        Assert.Equal("错误信息", valueValidator.Validators.Last().ErrorMessage);

        valueValidator.AddValidator(new MaxLengthValidator(10));
        Assert.NotNull(valueValidator._lastAddedValidator);
        valueValidator.WithErrorMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");
        Assert.Null(valueValidator._lastAddedValidator);
        Assert.Equal(typeof(TestValidationMessages), valueValidator.Validators.Last().ErrorMessageResourceType);
        Assert.Equal("TestValidator_ValidationError", valueValidator.Validators.Last().ErrorMessageResourceName);
    }

    [Fact]
    public void WithMessage_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>();

        valueValidator.WithMessage("错误消息");
        valueValidator.WithMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");

        valueValidator.AddValidator(new MinLengthValidator(3));
        Assert.NotNull(valueValidator._lastAddedValidator);
        valueValidator.WithMessage("错误信息");
        Assert.Null(valueValidator._lastAddedValidator);
        Assert.Equal("错误信息", valueValidator.Validators.Last().ErrorMessage);

        valueValidator.AddValidator(new MaxLengthValidator(10));
        Assert.NotNull(valueValidator._lastAddedValidator);
        valueValidator.WithMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");
        Assert.Null(valueValidator._lastAddedValidator);
        Assert.Equal(typeof(TestValidationMessages), valueValidator.Validators.Last().ErrorMessageResourceType);
        Assert.Equal("TestValidator_ValidationError", valueValidator.Validators.Last().ErrorMessageResourceName);
    }

    [Fact]
    public void WithDisplayName_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>();
        Assert.Null(valueValidator.DisplayName);

        valueValidator.WithDisplayName("MyName");
        Assert.Equal("MyName", valueValidator.DisplayName);
        valueValidator.WithDisplayName(null);
        Assert.Null(valueValidator.DisplayName);
    }

    [Fact]
    public void Age_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Age();
        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as AgeValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.IsAdultOnly);
        Assert.False(addedValidator.AllowStringValues);

        Assert.True(valueValidator.IsValid(8));
        Assert.True(valueValidator.IsValid(120));
        Assert.False(valueValidator.IsValid(121));

        var valueValidator2 = new ValueValidator<object>().Age(true, true);
        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as AgeValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.IsAdultOnly);
        Assert.True(addedValidator2.AllowStringValues);

        Assert.False(valueValidator2.IsValid(8));
        Assert.True(valueValidator2.IsValid(120));
        Assert.False(valueValidator2.IsValid(121));
    }

    [Fact]
    public void AllowedValues_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AllowedValues("Furion", "百小僧", "MonkSoul");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as AllowedValuesValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("Fur"));
        Assert.True(valueValidator.IsValid("Furion"));
        Assert.True(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void BankCard_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().BankCard();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as BankCardValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("5502092303469876"));
        Assert.True(valueValidator.IsValid("6228480402564890018"));
    }

    [Fact]
    public void Base64String_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Base64String();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as Base64StringValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("Furion"));
        Assert.True(valueValidator.IsValid("ZnVyaW9u"));
    }

    [Fact]
    public void ChineseName_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().ChineseName();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as ChineseNameValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("蒙奇·D·路飞"));
        Assert.True(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void Chinese_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Chinese();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as ChineseValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("Furion"));
        Assert.True(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void ColorValue_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().ColorValue();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as ColorValueValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.FullMode);

        Assert.False(valueValidator.IsValid("hsl(0, 100%, 50%)"));
        Assert.True(valueValidator.IsValid("#ffffff"));

        var valueValidator2 = new ValueValidator<object>().ColorValue(true);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as ColorValueValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.FullMode);

        Assert.True(valueValidator2.IsValid("hsl(0, 100%, 50%)"));
        Assert.True(valueValidator2.IsValid("#ffffff"));
    }

    [Fact]
    public void Composite_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Composite(new NotNullValidator(), new MinLengthValidator(3));

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as CompositeValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(2, addedValidator.Validators.Count);

        Assert.False(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("百小僧"));

        var valueValidator2 =
            new ValueValidator<object>().Composite([new EmailAddressValidator(), new UserNameValidator()],
                ValidationMode.BreakOnFirstSuccess);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as CompositeValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(2, addedValidator2.Validators.Count);

        Assert.True(valueValidator2.IsValid(null));
        Assert.True(valueValidator2.IsValid("monksoul@outlook.com"));
        Assert.True(valueValidator2.IsValid("monksoul"));
    }

    [Fact]
    public void ValidateAll_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValueValidator<object>().ValidateAll(null!));

    [Fact]
    public void ValidateAll_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().ValidateAll(u => u.NotNull().MinLength(3));

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as CompositeValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(ValidationMode.ValidateAll, addedValidator.Mode);
        Assert.Equal(2, addedValidator.Validators.Count);

        Assert.False(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void BreakOnFirstSuccess_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<object>().BreakOnFirstSuccess(null!));

    [Fact]
    public void BreakOnFirstSuccess_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().BreakOnFirstSuccess(u => u.EmailAddress().UserName());

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as CompositeValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(ValidationMode.BreakOnFirstSuccess, addedValidator.Mode);
        Assert.Equal(2, addedValidator.Validators.Count);

        Assert.True(valueValidator.IsValid(null));
        Assert.True(valueValidator.IsValid("monksoul@outlook.com"));
        Assert.True(valueValidator.IsValid("monksoul"));
    }

    [Fact]
    public void BreakOnFirstError_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<object>().BreakOnFirstError(null!));

    [Fact]
    public void BreakOnFirstError_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().BreakOnFirstError(u => u.NotNull().MinLength(3));

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as CompositeValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(ValidationMode.BreakOnFirstError, addedValidator.Mode);
        Assert.Equal(2, addedValidator.Validators.Count);

        Assert.False(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void Conditional_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<object>().Conditional(null!));

    [Fact]
    public void Conditional_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>().Conditional(builder => builder
            .When(u => u?.Contains('@') == true).Then(b => b.EmailAddress())
            .Otherwise(b => b.UserName()));

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as ConditionalValidator<string>;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid("monksoul@outlook.com"));
        Assert.False(valueValidator.IsValid("monk__soul"));
    }

    [Fact]
    public void DateOnly_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().DateOnly();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as DateOnlyValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator.Style);

        Assert.False(valueValidator.IsValid("2023-09-26 16:41:56"));
        Assert.False(valueValidator.IsValid("26/09/2023"));
        Assert.True(valueValidator.IsValid("2023-09-26"));

        var valueValidator2 =
            new ValueValidator<object>().DateOnly(["yyyy-MM-dd", "dd/MM/yyyy"], CultureInfo.InvariantCulture);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as DateOnlyValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator2.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator2.Style);

        Assert.False(valueValidator2.IsValid("2023-09-26 16:41:56"));
        Assert.True(valueValidator2.IsValid("26/09/2023"));
        Assert.True(valueValidator2.IsValid("2023-09-26"));
    }

    [Fact]
    public void DateTime_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().DateTime();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as DateTimeValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator.Style);

        Assert.False(valueValidator.IsValid("26/09/2023"));
        Assert.True(valueValidator.IsValid("2023-09-26 16:41:56"));

        var valueValidator2 =
            new ValueValidator<object>().DateTime(["yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy"], CultureInfo.InvariantCulture);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as DateTimeValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator2.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator2.Style);

        Assert.True(valueValidator2.IsValid("26/09/2023"));
        Assert.True(valueValidator2.IsValid("2023-09-26 16:41:56"));
    }

    [Fact]
    public void DecimalPlaces_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().DecimalPlaces(1);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as DecimalPlacesValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(1, addedValidator.MaxDecimalPlaces);
        Assert.False(addedValidator.AllowStringValues);

        Assert.False(valueValidator.IsValid(2.23));
        Assert.True(valueValidator.IsValid(2));
        Assert.False(valueValidator.IsValid("2.2"));

        var valueValidator2 = new ValueValidator<object>().DecimalPlaces(1, true);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as DecimalPlacesValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(1, addedValidator2.MaxDecimalPlaces);
        Assert.True(addedValidator2.AllowStringValues);

        Assert.False(valueValidator2.IsValid(2.23));
        Assert.True(valueValidator2.IsValid(2));
        Assert.True(valueValidator2.IsValid("2.2"));
    }

    [Fact]
    public void DeniedValues_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().DeniedValues("Furion", "百小僧", "MonkSoul");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as DeniedValuesValidator;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid("Fur"));
        Assert.False(valueValidator.IsValid("Furion"));
        Assert.False(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void Domain_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Domain();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as DomainValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("https://furion.net"));
        Assert.True(valueValidator.IsValid("furion.net"));
    }

    [Fact]
    public void EmailAddress_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().EmailAddress();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as EmailAddressValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("monksoul@"));
        Assert.True(valueValidator.IsValid("monksoul@outlook.com"));
    }

    [Fact]
    public void EndsWith_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().EndsWith("ion");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as EndsWithValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(StringComparison.Ordinal, addedValidator.Comparison);

        Assert.True(valueValidator.IsValid("Furion"));
        Assert.False(valueValidator.IsValid("Fur"));
        Assert.False(valueValidator.IsValid("FURION"));

        var valueValidator2 = new ValueValidator<object>().EndsWith("ion", StringComparison.OrdinalIgnoreCase);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as EndsWithValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, addedValidator2.Comparison);

        Assert.True(valueValidator2.IsValid("Furion"));
        Assert.False(valueValidator2.IsValid("Fur"));
        Assert.True(valueValidator2.IsValid("FURION"));
    }

    [Fact]
    public void EqualTo_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().EqualTo("Furion");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as EqualToValidator;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid("Furion"));
        Assert.False(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void GreaterThanOrEqualTo_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValueValidator<object>().GreaterThanOrEqualTo(null!));

    [Fact]
    public void GreaterThanOrEqualTo_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().GreaterThanOrEqualTo(10);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as GreaterThanOrEqualToValidator;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(11));
        Assert.False(valueValidator.IsValid(9));
    }

    [Fact]
    public void GreaterThan_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValueValidator<object>().GreaterThan(null!));

    [Fact]
    public void GreaterThan_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().GreaterThan(10);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as GreaterThanValidator;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(11));
        Assert.False(valueValidator.IsValid(9));
    }

    [Fact]
    public void IDCard_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().IDCard();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as IDCardValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("1234569910101933"));
        Assert.True(valueValidator.IsValid("622223199912051311"));
    }

    [Fact]
    public void IpAddress_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().IpAddress();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as IpAddressValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.AllowIPv6);

        Assert.False(valueValidator.IsValid("2001:0db8:85a3:0000:0000:8a2e:0370:7334"));
        Assert.True(valueValidator.IsValid("192.168.1.1"));

        var valueValidator2 = new ValueValidator<object>().IpAddress(true);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as IpAddressValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.AllowIPv6);

        Assert.True(valueValidator2.IsValid("2001:0db8:85a3:0000:0000:8a2e:0370:7334"));
        Assert.True(valueValidator2.IsValid("192.168.1.1"));
    }

    [Fact]
    public void Json_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Json();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as JsonValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.AllowTrailingCommas);

        Assert.False(valueValidator.IsValid("""{"id":1,"name":"furion",}"""));
        Assert.True(valueValidator.IsValid("""{"id":1,"name":"furion"}"""));

        var valueValidator2 = new ValueValidator<object>().Json(true);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as JsonValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.AllowTrailingCommas);

        Assert.True(valueValidator2.IsValid("""{"id":1,"name":"furion",}"""));
        Assert.True(valueValidator2.IsValid("""{"id":1,"name":"furion"}"""));
    }

    [Fact]
    public void Length_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Length(3, 5);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as LengthValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void LessThanOrEqualTo_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValueValidator<object>().LessThanOrEqualTo(null!));

    [Fact]
    public void LessThanOrEqualTo_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().LessThanOrEqualTo(10);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as LessThanOrEqualToValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid(11));
        Assert.True(valueValidator.IsValid(9));
    }

    [Fact]
    public void LessThan_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValueValidator<object>().LessThan(null!));

    [Fact]
    public void LessThan_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().LessThan(10);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as LessThanValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid(11));
        Assert.True(valueValidator.IsValid(9));
    }

    [Fact]
    public void MaxLength_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().MaxLength(3);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MaxLengthValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("Furion"));
        Assert.True(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void Max_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Max(10);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MaxValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid(11));
        Assert.True(valueValidator.IsValid(9));
    }

    [Fact]
    public void MD5String_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().MD5String();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MD5StringValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.AllowShortFormat);

        Assert.False(valueValidator.IsValid("EF4DF562719E70E4"));
        Assert.True(valueValidator.IsValid("3f2d0ea0ef4df562719e70e41413658e"));

        var valueValidator2 = new ValueValidator<object>().MD5String(true);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as MD5StringValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.AllowShortFormat);

        Assert.True(valueValidator2.IsValid("EF4DF562719E70E4"));
        Assert.True(valueValidator2.IsValid("3f2d0ea0ef4df562719e70e41413658e"));
    }

    [Fact]
    public void MinLength_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().MinLength(3);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MinLengthValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void Min_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Min(10);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MinValidator;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(11));
        Assert.False(valueValidator.IsValid(9));
    }

    [Fact]
    public void MustUnless_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new ValueValidator<int>().MustUnless((Func<int, bool>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<int>().MustUnless((Func<int, ValidationContext<int>, bool>)null!));
    }

    [Fact]
    public void MustUnless_ReturnOK()
    {
        var valueValidator = new ValueValidator<int>().MustUnless(u => u <= 10);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MustUnlessValidator<int>;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(11));
        Assert.False(valueValidator.IsValid(9));

        var valueValidator2 = new ValueValidator<int>().MustUnless((_, ctx) => ctx.Instance <= 10);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as MustUnlessValidator<int>;
        Assert.NotNull(addedValidator2);

        Assert.True(valueValidator2.IsValid(11));
        Assert.False(valueValidator2.IsValid(9));
    }

    [Fact]
    public void Must_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new ValueValidator<int>().Must((Func<int, bool>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<int>().Must((Func<int, ValidationContext<int>, bool>)null!));
    }

    [Fact]
    public void Must_ReturnOK()
    {
        var valueValidator = new ValueValidator<int>().Must(u => u > 10);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MustValidator<int>;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(11));
        Assert.False(valueValidator.IsValid(9));

        var valueValidator2 = new ValueValidator<int>().Must((_, ctx) => ctx.Instance > 10);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as MustValidator<int>;
        Assert.NotNull(addedValidator2);

        Assert.True(valueValidator2.IsValid(11));
        Assert.False(valueValidator2.IsValid(9));
    }

    [Fact]
    public void NotBlank_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().NotBlank();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as NotBlankValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid(string.Empty));
        Assert.True(valueValidator.IsValid("Furion"));
    }

    [Fact]
    public void NotEqualTo_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().NotEqualTo("Furion");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as NotEqualToValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("Furion"));
        Assert.True(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void NotEmpty_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().NotEmpty();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as NotEmptyValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid(string.Empty));
        Assert.True(valueValidator.IsValid("Furion"));
    }

    [Fact]
    public void NotNull_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().NotNull();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as NotNullValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid(null));
        Assert.True(valueValidator.IsValid("Furion"));
    }

    [Fact]
    public void Password_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Password();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as PasswordValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.Strong);

        Assert.False(valueValidator.IsValid("q1w2e3"));
        Assert.True(valueValidator.IsValid("q1w2e3r4"));

        var valueValidator2 = new ValueValidator<object>().Password(true);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as PasswordValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.Strong);

        Assert.False(valueValidator2.IsValid("q1w2e3r4"));
        Assert.True(valueValidator2.IsValid("cln99871433*_Q"));
    }

    [Fact]
    public void PhoneNumber_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().PhoneNumber();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as PhoneNumberValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("14000000000"));
        Assert.True(valueValidator.IsValid("13800138000"));
    }

    [Fact]
    public void PostalCode_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().PostalCode();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as PostalCodeValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("1001001"));
        Assert.True(valueValidator.IsValid("100101"));
    }

    [Fact]
    public void Predicate_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new ValueValidator<int>().Predicate((Func<int, bool>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<int>().Predicate((Func<int, ValidationContext<int>, bool>)null!));
    }

    [Fact]
    public void Predicate_ReturnOK()
    {
        var valueValidator = new ValueValidator<int>().Predicate(u => u > 10);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as PredicateValidator<int>;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(11));
        Assert.False(valueValidator.IsValid(9));

        var valueValidator2 = new ValueValidator<int>().Predicate((_, ctx) => ctx.Instance > 10);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as PredicateValidator<int>;
        Assert.NotNull(addedValidator2);

        Assert.True(valueValidator2.IsValid(11));
        Assert.False(valueValidator2.IsValid(9));
    }

    [Fact]
    public void Range_ReturnOK()
    {
        var valueValidator = new ValueValidator<int>().Range(2, 5);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid(1));
        Assert.True(valueValidator.IsValid(3));
        Assert.True(valueValidator.IsValid(5));

        var valueValidator2 = new ValueValidator<object>().Range(2.0, 5.0);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator2);

        Assert.False(valueValidator2.IsValid(1.0));
        Assert.True(valueValidator2.IsValid(3.0));

        var valueValidator3 = new ValueValidator<int>().Range(typeof(int), "2", "5");

        Assert.Single(valueValidator3.Validators);

        var addedValidator3 = valueValidator3._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator3);

        Assert.False(valueValidator3.IsValid(1));
        Assert.True(valueValidator3.IsValid(3));

        var valueValidator4 = new ValueValidator<int>().Range(2, 5, u => u.MaximumIsExclusive = true);

        Assert.Single(valueValidator4.Validators);

        var addedValidator4 = valueValidator4._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator4);

        Assert.False(valueValidator4.IsValid(1));
        Assert.True(valueValidator4.IsValid(3));
        Assert.False(valueValidator4.IsValid(5));
    }

    [Fact]
    public void Between_ReturnOK()
    {
        var valueValidator = new ValueValidator<int>().Between(2, 5);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid(1));
        Assert.True(valueValidator.IsValid(3));
        Assert.True(valueValidator.IsValid(5));

        var valueValidator2 = new ValueValidator<object>().Between(2.0, 5.0);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator2);

        Assert.False(valueValidator2.IsValid(1.0));
        Assert.True(valueValidator2.IsValid(3.0));

        var valueValidator3 = new ValueValidator<int>().Between(typeof(int), "2", "5");

        Assert.Single(valueValidator3.Validators);

        var addedValidator3 = valueValidator3._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator3);

        Assert.False(valueValidator3.IsValid(1));
        Assert.True(valueValidator3.IsValid(3));

        var valueValidator4 = new ValueValidator<int>().Between(2, 5, u => u.MaximumIsExclusive = true);

        Assert.Single(valueValidator4.Validators);

        var addedValidator4 = valueValidator4._lastAddedValidator as RangeValidator;
        Assert.NotNull(addedValidator4);

        Assert.False(valueValidator4.IsValid(1));
        Assert.True(valueValidator4.IsValid(3));
        Assert.False(valueValidator4.IsValid(5));
    }

    [Fact]
    public void RegularExpression_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().RegularExpression("^[1-9]{2,5}$");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as RegularExpressionValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(2000, addedValidator.MatchTimeoutInMilliseconds);

        Assert.False(valueValidator.IsValid("123456"));
        Assert.True(valueValidator.IsValid("12345"));
    }

    [Fact]
    public void Matches_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Matches("^[1-9]{2,5}$");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as RegularExpressionValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(2000, addedValidator.MatchTimeoutInMilliseconds);

        Assert.False(valueValidator.IsValid("123456"));
        Assert.True(valueValidator.IsValid("12345"));
    }

    [Fact]
    public void Required_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Required();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as RequiredValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.AllowEmptyStrings);

        Assert.False(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid(string.Empty));
        Assert.True(valueValidator.IsValid("Furion"));

        var valueValidator2 = new ValueValidator<object>().Required(true);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as RequiredValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.AllowEmptyStrings);

        Assert.False(valueValidator2.IsValid(null));
        Assert.True(valueValidator2.IsValid(string.Empty));
        Assert.True(valueValidator2.IsValid("Furion"));
    }

    [Fact]
    public void Single_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Single();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as SingleValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid(new List<int>()));
        Assert.True(valueValidator.IsValid(new List<int> { 1 }));
    }

    [Fact]
    public void StartsWith_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().StartsWith("Fu");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as StartsWithValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(StringComparison.Ordinal, addedValidator.Comparison);

        Assert.True(valueValidator.IsValid("Furion"));
        Assert.False(valueValidator.IsValid("Free"));
        Assert.False(valueValidator.IsValid("FURION"));

        var valueValidator2 = new ValueValidator<object>().StartsWith("Fu", StringComparison.OrdinalIgnoreCase);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as StartsWithValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, addedValidator2.Comparison);

        Assert.True(valueValidator2.IsValid("Furion"));
        Assert.False(valueValidator2.IsValid("Free"));
        Assert.True(valueValidator2.IsValid("FURION"));
    }

    [Fact]
    public void StringContains_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().StringContains("ion");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as StringContainsValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(StringComparison.Ordinal, addedValidator.Comparison);

        Assert.True(valueValidator.IsValid("Furion"));
        Assert.False(valueValidator.IsValid("Free"));
        Assert.False(valueValidator.IsValid("FURION"));

        var valueValidator2 = new ValueValidator<object>().StringContains("ion", StringComparison.OrdinalIgnoreCase);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as StringContainsValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, addedValidator2.Comparison);

        Assert.True(valueValidator2.IsValid("Furion"));
        Assert.False(valueValidator2.IsValid("Free"));
        Assert.True(valueValidator2.IsValid("FURION"));
    }

    [Fact]
    public void StringLength_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().StringLength(6);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as StringLengthValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("monksoul"));
        Assert.True(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("Furion"));

        var valueValidator2 = new ValueValidator<object>().StringLength(3, 6);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as StringLengthValidator;
        Assert.NotNull(addedValidator2);

        Assert.False(valueValidator2.IsValid("monksoul"));
        Assert.False(valueValidator2.IsValid("Fu"));
        Assert.True(valueValidator2.IsValid("Furion"));
    }

    [Fact]
    public void StrongPassword_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().StrongPassword();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as StrongPasswordValidator;
        Assert.NotNull(addedValidator);
        Assert.True(addedValidator.Strong);

        Assert.False(valueValidator.IsValid("q1w2e3r4"));
        Assert.True(valueValidator.IsValid("cln99871433*_Q"));
    }

    [Fact]
    public void Telephone_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Telephone();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as TelephoneValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("076088809963"));
        Assert.True(valueValidator.IsValid("0760-88809963"));
    }

    [Fact]
    public void TimeOnly_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().TimeOnly();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as TimeOnlyValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator.Style);

        Assert.False(valueValidator.IsValid("2023-09-26 16:41:56"));
        Assert.False(valueValidator.IsValid("14:32-30"));
        Assert.True(valueValidator.IsValid("14:32:30"));

        var valueValidator2 =
            new ValueValidator<object>().TimeOnly(["HH:mm:ss", "HH:mm-ss"], CultureInfo.InvariantCulture);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as TimeOnlyValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(CultureInfo.InvariantCulture, addedValidator2.Provider);
        Assert.Equal(DateTimeStyles.None, addedValidator2.Style);

        Assert.False(valueValidator2.IsValid("2023-09-26 16:41:56"));
        Assert.True(valueValidator2.IsValid("14:32-30"));
        Assert.True(valueValidator2.IsValid("14:32:30"));
    }

    [Fact]
    public void Url_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Url();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as UrlValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.SupportsFtp);

        Assert.False(valueValidator.IsValid("furion.net"));
        Assert.False(valueValidator.IsValid("ftp://furion.net/"));
        Assert.True(valueValidator.IsValid("https://furion.net/"));

        var valueValidator2 = new ValueValidator<object>().Url(true);

        Assert.Single(valueValidator.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as UrlValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.SupportsFtp);

        Assert.False(valueValidator2.IsValid("furion.net"));
        Assert.True(valueValidator2.IsValid("ftp://furion.net/"));
        Assert.True(valueValidator2.IsValid("https://furion.net/"));
    }

    [Fact]
    public void UserName_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().UserName();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as UserNameValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("monk__soul"));
        Assert.True(valueValidator.IsValid("monksoul"));
    }

    [Fact]
    public void ValidatorProxy_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().ValidatorProxy<UserNameValidator>(null);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as ValidatorProxy<UserNameValidator>;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("monk__soul"));
        Assert.True(valueValidator.IsValid("monksoul"));
    }

    [Fact]
    public void AddAnnotations_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().AddAnnotations(new UserNameAttribute());

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as ValueAnnotationValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("monk__soul"));
        Assert.True(valueValidator.IsValid("monksoul"));
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Required().NotBlank();
        Assert.Equal(2, valueValidator.Build().Count);
    }
}