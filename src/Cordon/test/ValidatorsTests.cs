// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidatorsTests
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
    public void Object_ReturnOK()
    {
        var objectValidator = Validators.Object<ObjectModel>()
            .RuleFor(u => u.Name)
            .Required().MinLength(3);

        Assert.False(objectValidator.IsValid(new ObjectModel()));
        Assert.False(objectValidator.IsValid(new ObjectModel { Name = "Fu" }));
        Assert.True(objectValidator.IsValid(new ObjectModel { Name = "Furion" }));

        var objectValidator2 = Validators
            .Object<ObjectModel>(new Dictionary<object, object?>());
        Assert.NotNull(objectValidator2.Items);
        Assert.Null(objectValidator2._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var objectValidator3 = Validators
            .Object<ObjectModel>(serviceProvider, new Dictionary<object, object?>());
        Assert.NotNull(objectValidator3.Items);
        Assert.NotNull(objectValidator3._serviceProvider);
    }

    [Fact]
    public void Value_ReturnOK()
    {
        var valueValidator = Validators.Value<object>()
            .Required().MinLength(3);

        Assert.False(valueValidator.IsValid(null));
        Assert.False(valueValidator.IsValid("Fu"));
        Assert.True(valueValidator.IsValid("Furion"));
    }

    [Fact]
    public void Age_ReturnOK()
    {
        var validator = Validators.Age();
        Assert.False(validator.IsAdultOnly);
        Assert.False(validator.AllowStringValues);

        var validator2 = Validators.Age(true, true);
        Assert.True(validator2.IsAdultOnly);
        Assert.True(validator2.AllowStringValues);
    }

    [Fact]
    public void AllowedValues_ReturnOK()
    {
        var validator = Validators.AllowedValues(1, 2, 3);
        Assert.Equal([1, 2, 3], validator.Values);
    }

    [Fact]
    public void BankCard_ReturnOK()
    {
        var validator = Validators.BankCard();
        Assert.NotNull(validator);
    }

    [Fact]
    public void Base64String_ReturnOK()
    {
        var validator = Validators.Base64String();
        Assert.NotNull(validator);
    }

    [Fact]
    public void ChineseName_ReturnOK()
    {
        var validator = Validators.ChineseName();
        Assert.NotNull(validator);
    }

    [Fact]
    public void Chinese_ReturnOK()
    {
        var validator = Validators.Chinese();
        Assert.NotNull(validator);
    }

    [Fact]
    public void ColorValue_ReturnOK()
    {
        var validator = Validators.ColorValue();
        Assert.False(validator.FullMode);

        var validator2 = Validators.ColorValue(true);
        Assert.True(validator2.FullMode);
    }

    [Fact]
    public void Composite_ReturnOK()
    {
        var validator = Validators.Composite(Validators.ChineseName(), Validators.AllowedValues("百签", "百小僧"));
        Assert.Equal(2, validator.Validators.Count);
        Assert.Equal(CompositeMode.FailFast, validator.Mode);

        var validator2 = Validators.Composite([Validators.ChineseName(), Validators.AllowedValues("百签", "百小僧")],
            CompositeMode.Any);
        Assert.Equal(2, validator2.Validators.Count);
        Assert.Equal(CompositeMode.Any, validator2.Mode);
    }

    [Fact]
    public void All_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => Validators.All(null!));

    [Fact]
    public void All_ReturnOK()
    {
        var validator = Validators.All(u => u.ChineseName().AllowedValues("百签", "百小僧"));
        Assert.Equal(2, validator.Validators.Count);
        Assert.Equal(CompositeMode.All, validator.Mode);
    }

    [Fact]
    public void Any_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => Validators.Any(null!));

    [Fact]
    public void Any_ReturnOK()
    {
        var validator = Validators.Any(u => u.ChineseName().AllowedValues("百签", "百小僧"));
        Assert.Equal(2, validator.Validators.Count);
        Assert.Equal(CompositeMode.Any, validator.Mode);
    }

    [Fact]
    public void FailFast_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => Validators.FailFast(null!));

    [Fact]
    public void FailFast_ReturnOK()
    {
        var validator = Validators.FailFast(u => u.ChineseName().AllowedValues("百签", "百小僧"));
        Assert.Equal(2, validator.Validators.Count);
        Assert.Equal(CompositeMode.FailFast, validator.Mode);
    }

    [Fact]
    public void Conditional_ReturnOK()
    {
        var validator = Validators.Conditional<int>(builder =>
            builder.When(u => u > 10).Then(b => b.Age()).Otherwise(b => b.Min(5)));
        Assert.NotNull(validator);
        Assert.Single(validator._conditions);
        Assert.NotNull(validator._defaultValidators);
    }

    [Fact]
    public void WhenMatch_ReturnOK()
    {
        var validator = Validators.WhenMatch<int>(u => u > 10, b => b.Age(), b => b.Min(5));
        Assert.NotNull(validator);
        Assert.Single(validator._conditions);
        Assert.NotNull(validator._defaultValidators);

        var validator2 = Validators.WhenMatch<int>(u => u > 10, "错误消息1");
        Assert.NotNull(validator2);
        Assert.Single(validator2._conditions);
        Assert.Null(validator2._defaultValidators);

        var validator3 =
            Validators.WhenMatch<int>(u => u > 10, typeof(TestValidationMessages), "TestValidator_ValidationError");
        Assert.NotNull(validator3);
        Assert.Single(validator3._conditions);
        Assert.Null(validator3._defaultValidators);
    }

    [Fact]
    public void Failure_ReturnOK()
    {
        var validator = Validators.Failure();
        Assert.NotNull(validator);
    }

    [Fact]
    public void DateOnly_ReturnOK()
    {
        var validator = Validators.DateOnly("yyyy/MM/dd");
        Assert.Equal(["yyyy/MM/dd"], validator.Formats);
        Assert.Equal(CultureInfo.InvariantCulture, validator.Provider);
        Assert.Equal(DateTimeStyles.None, validator.Style);

        var validator2 =
            Validators.DateOnly(["yyyy/MM/dd"], CultureInfo.CurrentUICulture, DateTimeStyles.AllowInnerWhite);
        Assert.Equal(["yyyy/MM/dd"], validator2.Formats);
        Assert.Equal(CultureInfo.CurrentUICulture, validator2.Provider);
        Assert.Equal(DateTimeStyles.AllowInnerWhite, validator2.Style);
    }

    [Fact]
    public void DateTime_ReturnOK()
    {
        var validator = Validators.DateTime("yyyy/MM/dd HH:mm:ss");
        Assert.Equal(["yyyy/MM/dd HH:mm:ss"], validator.Formats);
        Assert.Equal(CultureInfo.InvariantCulture, validator.Provider);
        Assert.Equal(DateTimeStyles.None, validator.Style);

        var validator2 =
            Validators.DateTime(["yyyy/MM/dd HH:mm:ss"], CultureInfo.CurrentUICulture, DateTimeStyles.AllowInnerWhite);
        Assert.Equal(["yyyy/MM/dd HH:mm:ss"], validator2.Formats);
        Assert.Equal(CultureInfo.CurrentUICulture, validator2.Provider);
        Assert.Equal(DateTimeStyles.AllowInnerWhite, validator2.Style);
    }

    [Fact]
    public void DecimalPlaces_ReturnOK()
    {
        var validator = Validators.DecimalPlaces(1);
        Assert.Equal(1, validator.MaxDecimalPlaces);
        Assert.False(validator.AllowStringValues);

        var validator2 = Validators.DecimalPlaces(1, true);
        Assert.Equal(1, validator2.MaxDecimalPlaces);
        Assert.True(validator2.AllowStringValues);
    }

    [Fact]
    public void DeniedValues_ReturnOK()
    {
        var validator = Validators.DeniedValues(1, 2, 3);
        Assert.Equal([1, 2, 3], validator.Values);
    }

    [Fact]
    public void Domain_ReturnOK()
    {
        var validator = Validators.Domain();
        Assert.NotNull(validator);
    }

    [Fact]
    public void EmailAddress_ReturnOK()
    {
        var validator = Validators.EmailAddress();
        Assert.NotNull(validator);
    }

    [Fact]
    public void Empty_ReturnOK()
    {
        var validator = Validators.Empty();
        Assert.NotNull(validator);
    }

    [Fact]
    public void EndsWith_ReturnOK()
    {
        var validator = Validators.EndsWith("ion");
        Assert.Equal("ion", validator.SearchValue);
        Assert.Equal(StringComparison.Ordinal, validator.Comparison);

        var validator2 = Validators.EndsWith("ion", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("ion", validator2.SearchValue);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, validator2.Comparison);
    }

    [Fact]
    public void Enum_ReturnOK()
    {
        var validator = Validators.Enum(typeof(MyEnum));
        Assert.False(validator.SupportFlags);

        var validator2 = Validators.Enum(typeof(MyFlagsEnum), true);
        Assert.True(validator2.SupportFlags);

        var validator3 = Validators.Enum<MyEnum>();
        Assert.False(validator3.SupportFlags);

        var validator4 = Validators.Enum<MyFlagsEnum>(true);
        Assert.True(validator4.SupportFlags);
    }

    [Fact]
    public void EqualTo_ReturnOK()
    {
        var validator = Validators.EqualTo(1);
        Assert.Equal(1, validator.CompareValue);
    }

    [Fact]
    public void GreaterThanOrEqualTo_ReturnOK()
    {
        var validator = Validators.GreaterThanOrEqualTo(1);
        Assert.Equal(1, validator.CompareValue);
    }

    [Fact]
    public void GreaterThan_ReturnOK()
    {
        var validator = Validators.GreaterThan(1);
        Assert.Equal(1, validator.CompareValue);
    }

    [Fact]
    public void HaveLength_ReturnOK()
    {
        var validator = Validators.HaveLength(2);
        Assert.Equal(2, validator.Length);
        Assert.False(validator.AllowEmpty);

        var validator2 = Validators.HaveLength(2, true);
        Assert.Equal(2, validator2.Length);
        Assert.True(validator2.AllowEmpty);
    }

    [Fact]
    public void IDCard_ReturnOK()
    {
        var validator = Validators.IDCard();
        Assert.NotNull(validator);
    }

    [Fact]
    public void IpAddress_ReturnOK()
    {
        var validator = Validators.IpAddress();
        Assert.False(validator.AllowIPv6);

        var validator2 = Validators.IpAddress(true);
        Assert.True(validator2.AllowIPv6);
    }

    [Fact]
    public void Json_ReturnOK()
    {
        var validator = Validators.Json();
        Assert.False(validator.AllowTrailingCommas);

        var validator2 = Validators.Json(true);
        Assert.True(validator2.AllowTrailingCommas);
    }

    [Fact]
    public void Length_ReturnOK()
    {
        var validator = Validators.Length(1, 10);
        Assert.Equal(1, validator.MinimumLength);
        Assert.Equal(10, validator.MaximumLength);
    }

    [Fact]
    public void LessThanOrEqualTo_ReturnOK()
    {
        var validator = Validators.LessThanOrEqualTo(1);
        Assert.Equal(1, validator.CompareValue);
    }

    [Fact]
    public void LessThan_ReturnOK()
    {
        var validator = Validators.LessThan(1);
        Assert.Equal(1, validator.CompareValue);
    }

    [Fact]
    public void MaxLength_ReturnOK()
    {
        var validator = Validators.MaxLength(10);
        Assert.Equal(10, validator.Length);
    }

    [Fact]
    public void Max_ReturnOK()
    {
        var validator = Validators.Max(10);
        Assert.Equal(10, validator.CompareValue);
    }

    [Fact]
    public void MD5String_ReturnOK()
    {
        var validator = Validators.MD5String();
        Assert.False(validator.AllowShortFormat);

        var validator2 = Validators.MD5String(true);
        Assert.True(validator2.AllowShortFormat);
    }

    [Fact]
    public void MinLength_ReturnOK()
    {
        var validator = Validators.MinLength(1);
        Assert.Equal(1, validator.Length);
    }

    [Fact]
    public void Min_ReturnOK()
    {
        var validator = Validators.Min(1);
        Assert.Equal(1, validator.CompareValue);
    }

    [Fact]
    public void Must_ReturnOK()
    {
        var validator = Validators.Must<int>(u => u > 10);
        Assert.NotNull(validator.Condition);

        var validator2 = Validators.Must<int>((u, _) => u > 10);
        Assert.NotNull(validator2.Condition);
    }

    [Fact]
    public void MustIfNotNull_ReturnOK()
    {
        var validator = Validators.MustIfNotNull<string>(u => u != "furion");
        Assert.NotNull(validator.Condition);

        var validator2 = Validators.MustIfNotNull<string>((u, _) => u != "furion");
        Assert.NotNull(validator2.Condition);
    }

    [Fact]
    public void MustIfNotNullOrEmpty_ReturnOK()
    {
        var validator = Validators.MustIfNotNullOrEmpty<string>(u => u != "furion");
        Assert.NotNull(validator.Condition);

        var validator2 = Validators.MustIfNotNullOrEmpty<string>((u, _) => u != "furion");
        Assert.NotNull(validator2.Condition);
    }

    [Fact]
    public void MustAny_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => Validators.MustAny(null!, (Func<int, int, bool>?)null!));
        Assert.Throws<ArgumentNullException>(() =>
            Validators.MustAny([], (Func<int, int, ValidationContext<int>, bool>?)null!));
    }

    [Fact]
    public void MustAny_ReturnOK()
    {
        var validator = Validators.MustAny<int, int>([], (u, _) => u > 10);
        Assert.NotNull(validator.Condition);

        var validator2 = Validators.MustAny<int, int>([], (u, _, _) => u > 10);
        Assert.NotNull(validator2.Condition);
    }

    [Fact]
    public void MustAll_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => Validators.MustAll(null!, (Func<int, int, bool>?)null!));
        Assert.Throws<ArgumentNullException>(() =>
            Validators.MustAll([], (Func<int, int, ValidationContext<int>, bool>?)null!));
    }

    [Fact]
    public void MustAll_ReturnOK()
    {
        var validator = Validators.MustAny<int, int>([], (u, _) => u > 10);
        Assert.NotNull(validator.Condition);

        var validator2 = Validators.MustAny<int, int>([], (u, _, _) => u > 10);
        Assert.NotNull(validator2.Condition);
    }

    [Fact]
    public void NotBlank_ReturnOK()
    {
        var validator = Validators.NotBlank();
        Assert.NotNull(validator);
    }

    [Fact]
    public void NotEmpty_ReturnOK()
    {
        var validator = Validators.NotEmpty();
        Assert.NotNull(validator);
    }

    [Fact]
    public void NotEqualTo_ReturnOK()
    {
        var validator = Validators.NotEqualTo(1);
        Assert.Equal(1, validator.CompareValue);
    }

    [Fact]
    public void NotNull_ReturnOK()
    {
        var validator = Validators.NotNull();
        Assert.NotNull(validator);
    }

    [Fact]
    public void Null_ReturnOK()
    {
        var validator = Validators.Null();
        Assert.NotNull(validator);
    }

    [Fact]
    public void ObjectAnnotation_ReturnOK()
    {
        var validator = Validators.ObjectAnnotation();
        Assert.Null(validator._serviceProvider);
        Assert.Empty(validator.Items);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var validator2 = Validators.ObjectAnnotation(serviceProvider, new Dictionary<object, object?>());
        Assert.NotNull(validator2._serviceProvider);
        Assert.NotNull(validator2.Items);
    }

    [Fact]
    public void Password_ReturnOK()
    {
        var validator = Validators.Password();
        Assert.False(validator.Strong);

        var validator2 = Validators.Password(true);
        Assert.True(validator2.Strong);
    }

    [Fact]
    public void PhoneNumber_ReturnOK()
    {
        var validator = Validators.PhoneNumber();
        Assert.NotNull(validator);
    }

    [Fact]
    public void PostalCode_ReturnOK()
    {
        var validator = Validators.PostalCode();
        Assert.NotNull(validator);
    }

    [Fact]
    public void PropertyAnnotation_ReturnOK()
    {
        var validator = Validators.PropertyAnnotation<ObjectModel>(u => u.Name);
        Assert.NotNull(validator.Property);
        Assert.Null(validator._serviceProvider);
        Assert.Empty(validator.Items);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validator2 =
            Validators.PropertyAnnotation<ObjectModel>(u => u.Name, new Dictionary<object, object?>());
        Assert.NotNull(validator2.Property);
        Assert.Null(validator2._serviceProvider);
        Assert.NotNull(validator2.Items);

        var validator3 =
            Validators.PropertyAnnotation<ObjectModel>(u => u.Name, serviceProvider, new Dictionary<object, object?>());
        Assert.NotNull(validator3.Property);
        Assert.NotNull(validator3._serviceProvider);
        Assert.NotNull(validator3.Items);

        var validator4 = Validators.PropertyAnnotation<ObjectModel, string?>(u => u.Name);
        Assert.NotNull(validator4.Property);
        Assert.Null(validator4._serviceProvider);
        Assert.Empty(validator4.Items);

        var validator5 =
            Validators.PropertyAnnotation<ObjectModel, string?>(u => u.Name, new Dictionary<object, object?>());
        Assert.NotNull(validator5.Property);
        Assert.Null(validator5._serviceProvider);
        Assert.NotNull(validator5.Items);

        var validator6 =
            Validators.PropertyAnnotation<ObjectModel, string?>(u => u.Name, serviceProvider,
                new Dictionary<object, object?>());
        Assert.NotNull(validator6.Property);
        Assert.NotNull(validator6._serviceProvider);
        Assert.NotNull(validator6.Items);
    }

    [Fact]
    public void Range_ReturnOK()
    {
        var validator = Validators.Range(1, 10);
        Assert.Equal(1, validator.Minimum);
        Assert.Equal(10, validator.Maximum);
        Assert.False(validator.MinimumIsExclusive);
        Assert.False(validator.MaximumIsExclusive);
        Assert.False(validator.ParseLimitsInInvariantCulture);
        Assert.False(validator.ConvertValueInInvariantCulture);

        var validator2 = Validators.Range(1, 10, u =>
        {
            u.MinimumIsExclusive = true;
            u.MaximumIsExclusive = true;
            u.ParseLimitsInInvariantCulture = true;
            u.ConvertValueInInvariantCulture = true;
        });
        Assert.Equal(1, validator2.Minimum);
        Assert.Equal(10, validator2.Maximum);
        Assert.True(validator2.MinimumIsExclusive);
        Assert.True(validator2.MaximumIsExclusive);
        Assert.True(validator2.ParseLimitsInInvariantCulture);
        Assert.True(validator2.ConvertValueInInvariantCulture);

        var validator3 = Validators.Range(1.2, 10.3);
        Assert.Equal(1.2, validator3.Minimum);
        Assert.Equal(10.3, validator3.Maximum);
        Assert.False(validator3.MinimumIsExclusive);
        Assert.False(validator3.MaximumIsExclusive);
        Assert.False(validator3.ParseLimitsInInvariantCulture);
        Assert.False(validator3.ConvertValueInInvariantCulture);

        var validator4 = Validators.Range(1.2, 10.3, u =>
        {
            u.MinimumIsExclusive = true;
            u.MaximumIsExclusive = true;
            u.ParseLimitsInInvariantCulture = true;
            u.ConvertValueInInvariantCulture = true;
        });
        Assert.Equal(1.2, validator4.Minimum);
        Assert.Equal(10.3, validator4.Maximum);
        Assert.True(validator4.MinimumIsExclusive);
        Assert.True(validator4.MaximumIsExclusive);
        Assert.True(validator4.ParseLimitsInInvariantCulture);
        Assert.True(validator4.ConvertValueInInvariantCulture);

        var validator5 = Validators.Range(typeof(int), "1", "10");
        Assert.Equal(typeof(int), validator5.OperandType);
        Assert.Equal("1", validator5.Minimum);
        Assert.Equal("10", validator5.Maximum);
        Assert.False(validator5.MinimumIsExclusive);
        Assert.False(validator5.MaximumIsExclusive);
        Assert.False(validator5.ParseLimitsInInvariantCulture);
        Assert.False(validator5.ConvertValueInInvariantCulture);

        var validator6 = Validators.Range(typeof(int), "1", "10", u =>
        {
            u.MinimumIsExclusive = true;
            u.MaximumIsExclusive = true;
            u.ParseLimitsInInvariantCulture = true;
            u.ConvertValueInInvariantCulture = true;
        });
        Assert.Equal(typeof(int), validator6.OperandType);
        Assert.Equal("1", validator6.Minimum);
        Assert.Equal("10", validator6.Maximum);
        Assert.True(validator6.MinimumIsExclusive);
        Assert.True(validator6.MaximumIsExclusive);
        Assert.True(validator6.ParseLimitsInInvariantCulture);
        Assert.True(validator6.ConvertValueInInvariantCulture);
    }

    [Fact]
    public void Between_ReturnOK()
    {
        var validator = Validators.Between(1, 10);
        Assert.Equal(1, validator.Minimum);
        Assert.Equal(10, validator.Maximum);
        Assert.False(validator.MinimumIsExclusive);
        Assert.False(validator.MaximumIsExclusive);
        Assert.False(validator.ParseLimitsInInvariantCulture);
        Assert.False(validator.ConvertValueInInvariantCulture);

        var validator2 = Validators.Between(1, 10, u =>
        {
            u.MinimumIsExclusive = true;
            u.MaximumIsExclusive = true;
            u.ParseLimitsInInvariantCulture = true;
            u.ConvertValueInInvariantCulture = true;
        });
        Assert.Equal(1, validator2.Minimum);
        Assert.Equal(10, validator2.Maximum);
        Assert.True(validator2.MinimumIsExclusive);
        Assert.True(validator2.MaximumIsExclusive);
        Assert.True(validator2.ParseLimitsInInvariantCulture);
        Assert.True(validator2.ConvertValueInInvariantCulture);

        var validator3 = Validators.Between(1.2, 10.3);
        Assert.Equal(1.2, validator3.Minimum);
        Assert.Equal(10.3, validator3.Maximum);
        Assert.False(validator3.MinimumIsExclusive);
        Assert.False(validator3.MaximumIsExclusive);
        Assert.False(validator3.ParseLimitsInInvariantCulture);
        Assert.False(validator3.ConvertValueInInvariantCulture);

        var validator4 = Validators.Between(1.2, 10.3, u =>
        {
            u.MinimumIsExclusive = true;
            u.MaximumIsExclusive = true;
            u.ParseLimitsInInvariantCulture = true;
            u.ConvertValueInInvariantCulture = true;
        });
        Assert.Equal(1.2, validator4.Minimum);
        Assert.Equal(10.3, validator4.Maximum);
        Assert.True(validator4.MinimumIsExclusive);
        Assert.True(validator4.MaximumIsExclusive);
        Assert.True(validator4.ParseLimitsInInvariantCulture);
        Assert.True(validator4.ConvertValueInInvariantCulture);

        var validator5 = Validators.Between(typeof(int), "1", "10");
        Assert.Equal(typeof(int), validator5.OperandType);
        Assert.Equal("1", validator5.Minimum);
        Assert.Equal("10", validator5.Maximum);
        Assert.False(validator5.MinimumIsExclusive);
        Assert.False(validator5.MaximumIsExclusive);
        Assert.False(validator5.ParseLimitsInInvariantCulture);
        Assert.False(validator5.ConvertValueInInvariantCulture);

        var validator6 = Validators.Between(typeof(int), "1", "10", u =>
        {
            u.MinimumIsExclusive = true;
            u.MaximumIsExclusive = true;
            u.ParseLimitsInInvariantCulture = true;
            u.ConvertValueInInvariantCulture = true;
        });
        Assert.Equal(typeof(int), validator6.OperandType);
        Assert.Equal("1", validator6.Minimum);
        Assert.Equal("10", validator6.Maximum);
        Assert.True(validator6.MinimumIsExclusive);
        Assert.True(validator6.MaximumIsExclusive);
        Assert.True(validator6.ParseLimitsInInvariantCulture);
        Assert.True(validator6.ConvertValueInInvariantCulture);
    }

    [Fact]
    public void RegularExpression_ReturnOK()
    {
        var validator = Validators.RegularExpression("^[a-zA-Z]+$");
        Assert.Equal("^[a-zA-Z]+$", validator.Pattern);
        Assert.Equal(2000, validator.MatchTimeoutInMilliseconds);

        var validator2 = Validators.RegularExpression("^[a-zA-Z]+$", 3000);
        Assert.Equal("^[a-zA-Z]+$", validator2.Pattern);
        Assert.Equal(3000, validator2.MatchTimeoutInMilliseconds);
    }

    [Fact]
    public void Matches_ReturnOK()
    {
        var validator = Validators.Matches("^[a-zA-Z]+$");
        Assert.Equal("^[a-zA-Z]+$", validator.Pattern);
        Assert.Equal(2000, validator.MatchTimeoutInMilliseconds);

        var validator2 = Validators.Matches("^[a-zA-Z]+$", 3000);
        Assert.Equal("^[a-zA-Z]+$", validator2.Pattern);
        Assert.Equal(3000, validator2.MatchTimeoutInMilliseconds);
    }

    [Fact]
    public void Required_ReturnOK()
    {
        var validator = Validators.Required();
        Assert.False(validator.AllowEmptyStrings);

        var validator2 = Validators.Required(true);
        Assert.True(validator2.AllowEmptyStrings);
    }

    [Fact]
    public void Single_ReturnOK()
    {
        var validator = Validators.Single();
        Assert.NotNull(validator);
    }

    [Fact]
    public void StartsWith_ReturnOK()
    {
        var validator = Validators.StartsWith("ion");
        Assert.Equal("ion", validator.SearchValue);
        Assert.Equal(StringComparison.Ordinal, validator.Comparison);

        var validator2 = Validators.StartsWith("ion", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("ion", validator2.SearchValue);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, validator2.Comparison);
    }

    [Fact]
    public void StringContains_ReturnOK()
    {
        var validator = Validators.StringContains("ion");
        Assert.Equal("ion", validator.SearchValue);
        Assert.Equal(StringComparison.Ordinal, validator.Comparison);

        var validator2 = Validators.StringContains("ion", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("ion", validator2.SearchValue);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, validator2.Comparison);
    }

    [Fact]
    public void StringLength_ReturnOK()
    {
        var validator = Validators.StringLength(10);
        Assert.Equal(0, validator.MinimumLength);
        Assert.Equal(10, validator.MaximumLength);

        var validator2 = Validators.StringLength(1, 10);
        Assert.Equal(1, validator2.MinimumLength);
        Assert.Equal(10, validator2.MaximumLength);
    }

    [Fact]
    public void StringNotContains_ReturnOK()
    {
        var validator = Validators.StringNotContains("ion");
        Assert.Equal("ion", validator.SearchValue);
        Assert.Equal(StringComparison.Ordinal, validator.Comparison);

        var validator2 = Validators.StringNotContains("ion", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("ion", validator2.SearchValue);
        Assert.Equal(StringComparison.OrdinalIgnoreCase, validator2.Comparison);
    }

    [Fact]
    public void StrongPassword_ReturnOK()
    {
        var validator = Validators.StrongPassword();
        Assert.NotNull(validator);
    }

    [Fact]
    public void Telephone_ReturnOK()
    {
        var validator = Validators.Telephone();
        Assert.NotNull(validator);
    }

    [Fact]
    public void TimeOnly_ReturnOK()
    {
        var validator = Validators.TimeOnly("HH:mm:ss");
        Assert.Equal(["HH:mm:ss"], validator.Formats);
        Assert.Equal(CultureInfo.InvariantCulture, validator.Provider);
        Assert.Equal(DateTimeStyles.None, validator.Style);

        var validator2 =
            Validators.TimeOnly(["HH:mm:ss"], CultureInfo.CurrentUICulture, DateTimeStyles.AllowInnerWhite);
        Assert.Equal(["HH:mm:ss"], validator2.Formats);
        Assert.Equal(CultureInfo.CurrentUICulture, validator2.Provider);
        Assert.Equal(DateTimeStyles.AllowInnerWhite, validator2.Style);
    }

    [Fact]
    public void Url_ReturnOK()
    {
        var validator = Validators.Url();
        Assert.False(validator.SupportsFtp);

        var validator2 = Validators.Url(true);
        Assert.True(validator2.SupportsFtp);
    }

    [Fact]
    public void UserName_ReturnOK()
    {
        var validator = Validators.UserName();
        Assert.NotNull(validator);
    }

    [Fact]
    public void ValidatorProxy_ReturnOK()
    {
        var validator = Validators.ValidatorProxy<AllowedValuesValidator>(1, 2, 3);
        Assert.NotNull(validator);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var validator2 =
            Validators.ValidatorProxy<ObjectModel, PropertyAnnotationValidator<ObjectModel>>(
                instance => instance.Name,
                (_, _) => [services, new Dictionary<object, object?>()]);
        Assert.NotNull(validator2);
    }

    [Fact]
    public void ValueAnnotation_ReturnOK()
    {
        var validator = Validators.ValueAnnotation(new ChineseAttribute());
        Assert.Single(validator.Attributes);
        Assert.Null(validator._serviceProvider);
        Assert.Empty(validator.Items);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();
        var validator2 = Validators.ValueAnnotation([new ChineseAttribute()], new Dictionary<object, object?>());
        Assert.Single(validator2.Attributes);
        Assert.Null(validator2._serviceProvider);
        Assert.NotNull(validator2.Items);

        var validator3 = Validators.ValueAnnotation([new ChineseAttribute()], serviceProvider,
            new Dictionary<object, object?>());
        Assert.Single(validator3.Attributes);
        Assert.NotNull(validator3._serviceProvider);
        Assert.NotNull(validator3.Items);
    }

    public class ObjectModel
    {
        public string? Name { get; set; }
    }
}