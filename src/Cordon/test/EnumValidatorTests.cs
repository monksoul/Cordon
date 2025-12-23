// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class EnumValidatorTests
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
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new EnumValidator(null!));

        var exception = Assert.Throws<ArgumentException>(() => new EnumValidator(typeof(object)));
        Assert.Equal("The type 'Object' is not an enumeration type. (Parameter 'enumType')", exception.Message);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new EnumValidator(typeof(MyEnum));
        Assert.NotNull(validator.EnumType);
        Assert.Equal(typeof(MyEnum), validator.EnumType);
        Assert.False(validator.SupportFlags);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a defined value of enum {1}.",
            validator._errorMessageResourceAccessor());

        var validator2 = new EnumValidator(typeof(MyFlagsEnum)) { SupportFlags = true };
        Assert.NotNull(validator2.EnumType);
        Assert.Equal(typeof(MyFlagsEnum), validator2.EnumType);
        Assert.True(validator2.SupportFlags);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be a valid combination of values defined in enum {1}.",
            validator2._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData(MyEnum.Enum1, true)]
    [InlineData(MyEnum.Enum2, true)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData("Enum1", true)]
    [InlineData("Enum2", true)]
    [InlineData(MyFlagsEnum.Enum1, false)]
    [InlineData(MyFlagsEnum.Enum2, false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new EnumValidator<MyEnum>();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData(MyEnum.Enum1, false)]
    [InlineData(MyEnum.Enum2, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData("Enum1", true)]
    [InlineData("Enum2", true)]
    [InlineData(MyFlagsEnum.Enum1, true)]
    [InlineData(MyFlagsEnum.Enum2, true)]
    public void IsValid_WithSupportFlags_ReturnOK(object? value, bool result)
    {
        var validator = new EnumValidator<MyFlagsEnum> { SupportFlags = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new EnumValidator<MyEnum>();
        Assert.Null(validator.GetValidationResults("Enum1", "data"));

        var validationResults = validator.GetValidationResults(MyFlagsEnum.Enum1, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a defined value of enum MyEnum.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(MyFlagsEnum.Enum1, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithSupportFlags_ReturnOK()
    {
        var validator = new EnumValidator<MyFlagsEnum> { SupportFlags = true };
        Assert.Null(validator.GetValidationResults(MyFlagsEnum.Enum1, "data"));

        var validationResults = validator.GetValidationResults(MyEnum.Enum1, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be a valid combination of values defined in enum MyFlagsEnum.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(MyEnum.Enum1, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new EnumValidator<MyEnum>();
        validator.Validate("Enum1", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(MyFlagsEnum.Enum1, "data"));
        Assert.Equal("The field data must be a defined value of enum MyEnum.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(MyFlagsEnum.Enum1, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithSupportFlags_ReturnOK()
    {
        var validator = new EnumValidator<MyFlagsEnum> { SupportFlags = true };
        validator.Validate(MyFlagsEnum.Enum1, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(MyEnum.Enum1, "data"));
        Assert.Equal("The field data must be a valid combination of values defined in enum MyFlagsEnum.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(MyEnum.Enum1, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new EnumValidator<MyEnum>();
        Assert.Equal("The field data must be a defined value of enum MyEnum.",
            validator.FormatErrorMessage("data"));

        var validator2 = new EnumValidator<MyFlagsEnum> { SupportFlags = true };
        Assert.Equal("The field data must be a valid combination of values defined in enum MyFlagsEnum.",
            validator2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new EnumValidator<MyEnum>();
        Assert.Equal("EnumValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new EnumValidator<MyFlagsEnum> { SupportFlags = true };
        Assert.Equal("EnumValidator_ValidationError_SupportFlags", validator2.GetResourceKey());
    }

    [Fact]
    public void IsEnumValueDefined_Invalid_Parameters()
    {
        var validator = new EnumValidator<MyEnum>();
        Assert.Throws<ArgumentNullException>(() => validator.IsEnumValueDefined(null!));
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(MyEnum.Enum1, true)]
    [InlineData(MyEnum.Enum2, true)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData("Enum1", true)]
    [InlineData("Enum2", true)]
    public void IsEnumValueDefined_ReturnOK(object? value, bool result)
    {
        var validator = new EnumValidator<MyEnum>();
        Assert.Equal(result, validator.IsEnumValueDefined(value!));
    }

    [Theory]
    [InlineData("", false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData("Enum1", true)]
    [InlineData("Enum2", true)]
    [InlineData(MyFlagsEnum.Enum1, true)]
    [InlineData(MyFlagsEnum.Enum2, true)]
    public void IsEnumValueDefined_WithSupportFlags_ReturnOK(object? value, bool result)
    {
        var validator = new EnumValidator<MyFlagsEnum> { SupportFlags = true };
        Assert.Equal(result, validator.IsEnumValueDefined(value!));
    }
}