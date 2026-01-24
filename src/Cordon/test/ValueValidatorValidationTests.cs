// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValueValidatorValidationTests
{
    public enum MyEnum
    {
        Enum1,
        Enum2
    }

    [Flags]
    public enum MyFlagsEnum
    {
        Enum1,
        Enum2
    }

    [Fact]
    public void GetValidators_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Required().NotBlank();
        Assert.Equal(2, valueValidator.GetValidators().Count);
    }

    [Fact]
    public void WithMessage_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>();

        valueValidator.WithMessage("错误信息");
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
    public void WithName_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>();
        Assert.Null(valueValidator.DisplayName);
        Assert.Null(valueValidator.MemberName);

        valueValidator.WithName("MyName");
        Assert.Equal("MyName", valueValidator.MemberName);
        valueValidator.WithName(null);
        Assert.Null(valueValidator.MemberName);
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
    public void Composite_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<object>().Composite(null!));

    [Fact]
    public void Composite_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Composite(u => u.NotNull().MinLength(3));

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as CompositeValidator<object>;
        Assert.NotNull(addedValidator);
        Assert.Equal(2, addedValidator._validators.Count);

        Assert.False(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("百小僧"));

        var valueValidator2 =
            new ValueValidator<object>().Composite(u => u.EmailAddress().UserName(), CompositeMode.Any);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as CompositeValidator<object>;
        Assert.NotNull(addedValidator2);
        Assert.Equal(2, addedValidator2._validators.Count);

        Assert.True(valueValidator2.IsValid(null));
        Assert.True(valueValidator2.IsValid("monksoul@outlook.com"));
        Assert.True(valueValidator2.IsValid("monksoul"));
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
    public void WhenMatch_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>().WhenMatch(u => u?.Contains('@') == true,
            b => b.EmailAddress(), b => b.UserName());

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as ConditionalValidator<string>;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid("monksoul@outlook.com"));
        Assert.False(valueValidator.IsValid("monk__soul"));

        var valueValidator2 = new ValueValidator<string>().WhenMatch(u => u?.Contains('@') == true, "错误信息1");
        Assert.False(valueValidator2.IsValid("monksoul@outlook.com"));
        Assert.True(valueValidator2.IsValid("monk__soul"));

        var valueValidator3 = new ValueValidator<string>().WhenMatch(u => u?.Contains('@') == true,
            typeof(TestValidationMessages), "TestValidator_ValidationError");
        Assert.False(valueValidator3.IsValid("monksoul@outlook.com"));
        Assert.True(valueValidator3.IsValid("monk__soul"));
    }

    [Fact]
    public void Failure_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>().Failure();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as FailureValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("蒙奇·D·路飞"));
        Assert.False(valueValidator.IsValid("百小僧"));
    }

    [Fact]
    public void FileExtensions_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>().FileExtensions();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as FileExtensionsValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("furion.ico"));
        Assert.True(valueValidator.IsValid("furion.png"));

        var valueValidator2 = new ValueValidator<string>().FileExtensions("png,jpg,jpeg,gif");

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as FileExtensionsValidator;
        Assert.NotNull(addedValidator2);

        Assert.False(valueValidator2.IsValid("furion.ico"));
        Assert.True(valueValidator2.IsValid("furion.png"));
    }

    [Fact]
    public void CustomValidation_ReturnOK()
    {
        var valueValidator =
            new ValueValidator<object>().CustomValidation(typeof(CustomValidators),
                nameof(CustomValidators.ValidateValue));

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as AttributeValueValidator;
        Assert.NotNull(addedValidator);
        var customValidationAttribute = addedValidator.Attributes[0] as CustomValidationAttribute;
        Assert.NotNull(customValidationAttribute);
        Assert.Equal(typeof(CustomValidators), customValidationAttribute.ValidatorType);
        Assert.Equal("ValidateValue", customValidationAttribute.Method);

        Assert.False(valueValidator.IsValid("fu"));
        Assert.True(valueValidator.IsValid("Furion"));
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
    public void Empty_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Empty();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as EmptyValidator;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(string.Empty));
        Assert.False(valueValidator.IsValid("Furion"));
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
    public void Enum_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Enum(typeof(MyEnum));

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as EnumValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.SupportFlags);

        Assert.False(valueValidator.IsValid(MyFlagsEnum.Enum1));
        Assert.False(valueValidator.IsValid(2));
        Assert.True(valueValidator.IsValid(MyEnum.Enum1));

        var valueValidator2 = new ValueValidator<object>().Enum(typeof(MyFlagsEnum), true);

        Assert.Single(valueValidator.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as EnumValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.SupportFlags);

        Assert.False(valueValidator2.IsValid(MyEnum.Enum1));
        Assert.False(valueValidator2.IsValid(2));
        Assert.True(valueValidator2.IsValid(MyFlagsEnum.Enum1));

        var valueValidator3 = new ValueValidator<object>().Enum<MyEnum>();

        Assert.Single(valueValidator3.Validators);

        var addedValidator3 = valueValidator3._lastAddedValidator as EnumValidator;
        Assert.NotNull(addedValidator3);
        Assert.False(addedValidator3.SupportFlags);

        Assert.False(valueValidator3.IsValid(MyFlagsEnum.Enum1));
        Assert.False(valueValidator3.IsValid(2));
        Assert.True(valueValidator3.IsValid(MyEnum.Enum1));

        var valueValidator4 = new ValueValidator<object>().Enum<MyFlagsEnum>(true);

        Assert.Single(valueValidator3.Validators);

        var addedValidator4 = valueValidator4._lastAddedValidator as EnumValidator;
        Assert.NotNull(addedValidator4);
        Assert.True(addedValidator4.SupportFlags);

        Assert.False(valueValidator4.IsValid(MyEnum.Enum1));
        Assert.False(valueValidator4.IsValid(2));
        Assert.True(valueValidator4.IsValid(MyFlagsEnum.Enum1));
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
    public void HaveLength_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().HaveLength(2);

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as HaveLengthValidator;
        Assert.NotNull(addedValidator);
        Assert.False(addedValidator.AllowEmpty);

        Assert.True(valueValidator.IsValid(new[] { "fur", "furion" }));
        Assert.False(valueValidator.IsValid(Array.Empty<string>()));
        Assert.False(valueValidator.IsValid("fur"));

        var valueValidator2 = new ValueValidator<object>().HaveLength(2, true);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as HaveLengthValidator;
        Assert.NotNull(addedValidator2);
        Assert.True(addedValidator2.AllowEmpty);

        Assert.True(valueValidator2.IsValid(new[] { "fur", "furion" }));
        Assert.True(valueValidator2.IsValid(Array.Empty<string>()));
        Assert.False(valueValidator2.IsValid("fur"));
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
        Assert.Equal(3, addedValidator.Length);

        Assert.False(valueValidator.IsValid("Furion"));
        Assert.True(valueValidator.IsValid("百小僧"));

        var valueValidator2 = new ValueValidator<object>().MaxLength(-1);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as MaxLengthValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(-1, addedValidator2.Length);

        Assert.True(valueValidator2.IsValid("Furion"));
        Assert.True(valueValidator2.IsValid("百小僧"));
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
    public void MustIfNotNull_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new ValueValidator<int>().MustIfNotNull((Func<int, bool>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<int>().MustIfNotNull((Func<int, ValidationContext<int>, bool>)null!));
    }

    [Fact]
    public void MustIfNotNull_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>().MustIfNotNull(u => u != "furion");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MustValidator<string>;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("furion"));

        var valueValidator2 = new ValueValidator<string>().MustIfNotNull((_, ctx) => ctx.Instance != "furion");

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as MustValidator<string>;
        Assert.NotNull(addedValidator2);

        Assert.True(valueValidator2.IsValid(null));
        Assert.False(valueValidator2.IsValid("furion"));
    }

    [Fact]
    public void MustIfNotNullOrEmpty_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<int>().MustIfNotNullOrEmpty((Func<int, bool>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<int>().MustIfNotNullOrEmpty((Func<int, ValidationContext<int>, bool>)null!));
    }

    [Fact]
    public void MustIfNotNullOrEmpty_ReturnOK()
    {
        var valueValidator = new ValueValidator<string>().MustIfNotNullOrEmpty(u => u != "furion");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MustValidator<string>;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(null));
        Assert.True(valueValidator.IsValid(string.Empty));
        Assert.False(valueValidator.IsValid("furion"));

        var valueValidator2 = new ValueValidator<string>().MustIfNotNullOrEmpty((_, ctx) => ctx.Instance != "furion");

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as MustValidator<string>;
        Assert.NotNull(addedValidator2);

        Assert.True(valueValidator2.IsValid(null));
        Assert.True(valueValidator2.IsValid(string.Empty));
        Assert.False(valueValidator2.IsValid("furion"));
    }

    [Fact]
    public void MustAny_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<string>()
                .MustAny(null!, (Func<string, string, bool>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<string>()
                .MustAny([], (Func<string, string, bool>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<string>()
                .MustAny(null!, (Func<string, string, ValidationContext<string>, bool>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<string>()
                .MustAny([], (Func<string, string, ValidationContext<string>, bool>)null!));
    }

    [Fact]
    public void MustAny_ReturnOK()
    {
        string[] allowedDomains = ["@outlook.com", "@qq.com", "@163.com"];

        var valueValidator = new ValueValidator<string>()
            .MustAny(allowedDomains, (u, x) => u.EndsWith(x, StringComparison.OrdinalIgnoreCase));

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MustValidator<string>;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid("monksoul@outlook.com"));
        Assert.False(valueValidator.IsValid("monksoul@gmail.com"));

        var valueValidator2 = new ValueValidator<string>().MustAny(allowedDomains,
            (_, x, ctx) => ctx.Instance.EndsWith(x, StringComparison.OrdinalIgnoreCase));

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 =
            valueValidator2._lastAddedValidator as MustValidator<string>;
        Assert.NotNull(addedValidator2);

        Assert.True(valueValidator2.IsValid("monksoul@outlook.com"));
        Assert.False(valueValidator2.IsValid("monksoul@gmail.com"));
    }

    [Fact]
    public void MustAll_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<string>()
                .MustAll(null!, (Func<string, string, bool>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<string>()
                .MustAll([], (Func<string, string, bool>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<string>()
                .MustAll(null!, (Func<string, string, ValidationContext<string>, bool>)null!));

        Assert.Throws<ArgumentNullException>(() =>
            new ValueValidator<string>()
                .MustAll([], (Func<string, string, ValidationContext<string>, bool>)null!));
    }

    [Fact]
    public void MustAll_ReturnOK()
    {
        string[] allowedDomains = ["@outlook.com"];

        var valueValidator = new ValueValidator<string>()
            .MustAll(allowedDomains, (u, x) => u.EndsWith(x, StringComparison.OrdinalIgnoreCase));

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as MustValidator<string>;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid("monksoul@outlook.com"));
        Assert.False(valueValidator.IsValid("monksoul@gmail.com"));

        var valueValidator2 = new ValueValidator<string>().MustAll(allowedDomains,
            (_, x, ctx) => ctx.Instance.EndsWith(x, StringComparison.OrdinalIgnoreCase));

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 =
            valueValidator2._lastAddedValidator as MustValidator<string>;
        Assert.NotNull(addedValidator2);

        Assert.True(valueValidator2.IsValid("monksoul@outlook.com"));
        Assert.False(valueValidator2.IsValid("monksoul@gmail.com"));
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
    public void Null_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Null();

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as NullValidator;
        Assert.NotNull(addedValidator);

        Assert.True(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("Furion"));
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
    public void StringNotContains_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().StringNotContains("ion");

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as StringNotContainsValidator;
        Assert.NotNull(addedValidator);
        Assert.Equal(StringComparison.Ordinal, addedValidator.Comparison);

        Assert.True(valueValidator.IsValid("free"));
        Assert.False(valueValidator.IsValid("furion"));
        Assert.True(valueValidator.IsValid("FURION"));

        var valueValidator2 = new ValueValidator<object>().StringNotContains("ion", StringComparison.OrdinalIgnoreCase);

        Assert.Single(valueValidator2.Validators);

        var addedValidator2 = valueValidator2._lastAddedValidator as StringNotContainsValidator;
        Assert.NotNull(addedValidator2);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, addedValidator2.Comparison);

        Assert.True(valueValidator2.IsValid("free"));
        Assert.False(valueValidator2.IsValid("furion"));
        Assert.False(valueValidator2.IsValid("FURION"));
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
    public void WithAttribute_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().WithAttribute(new UserNameAttribute());

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as AttributeValueValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("monk__soul"));
        Assert.True(valueValidator.IsValid("monksoul"));

        var validationResults = valueValidator.GetValidationResults("monk__soul");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field Object is not a valid username.", validationResults[0].ErrorMessage);
        Assert.Empty(validationResults[0].MemberNames);

        var exception = Assert.Throws<ValidationException>(() =>
            valueValidator.Validate("monk__soul"));
        Assert.Equal("The field Object is not a valid username.", exception.Message);
        Assert.Empty(exception.ValidationResult.MemberNames);

        valueValidator.WithName("Value");
        var validationResults2 = valueValidator.GetValidationResults("monk__soul");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("The field Value is not a valid username.", validationResults2[0].ErrorMessage);
        Assert.Equal("Value", validationResults2[0].MemberNames.FirstOrDefault());

        var exception2 = Assert.Throws<ValidationException>(() =>
            valueValidator.Validate("monk__soul"));
        Assert.Equal("The field Value is not a valid username.", exception2.Message);
        Assert.Equal("Value", exception2.ValidationResult.MemberNames.FirstOrDefault());
    }

    [Fact]
    public void WithAttributes_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().WithAttributes(new UserNameAttribute());

        Assert.Single(valueValidator.Validators);

        var addedValidator = valueValidator._lastAddedValidator as AttributeValueValidator;
        Assert.NotNull(addedValidator);

        Assert.False(valueValidator.IsValid("monk__soul"));
        Assert.True(valueValidator.IsValid("monksoul"));

        var validationResults = valueValidator.GetValidationResults("monk__soul");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field Object is not a valid username.", validationResults[0].ErrorMessage);
        Assert.Empty(validationResults[0].MemberNames);

        var exception = Assert.Throws<ValidationException>(() =>
            valueValidator.Validate("monk__soul"));
        Assert.Equal("The field Object is not a valid username.", exception.Message);
        Assert.Empty(exception.ValidationResult.MemberNames);

        valueValidator.WithName("Value");
        var validationResults2 = valueValidator.GetValidationResults("monk__soul");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("The field Value is not a valid username.", validationResults2[0].ErrorMessage);
        Assert.Equal("Value", validationResults2[0].MemberNames.FirstOrDefault());

        var exception2 = Assert.Throws<ValidationException>(() =>
            valueValidator.Validate("monk__soul"));
        Assert.Equal("The field Value is not a valid username.", exception2.Message);
        Assert.Equal("Value", exception2.ValidationResult.MemberNames.FirstOrDefault());
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var valueValidator = new ValueValidator<object>().Required().NotBlank();
        Assert.Equal(2, valueValidator.Build().Count);
    }

    public static class CustomValidators
    {
        public static ValidationResult? ValidateValue(object? value, ValidationContext context) =>
            value switch
            {
                null => ValidationResult.Success,
                string { Length: < 3 } => new ValidationResult("不能小于或等于 3"),
                _ => ValidationResult.Success
            };
    }
}