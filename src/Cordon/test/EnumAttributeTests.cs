// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class EnumAttributeTests
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
    public void Attribute_Metadata()
    {
        var attributeType = typeof(EnumAttribute);
        Assert.True(typeof(ValidationAttribute).IsAssignableFrom(attributeType));

        var attributeUsageAttribute = attributeType.GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsageAttribute);
        Assert.Equal(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
            attributeUsageAttribute.ValidOn);
        Assert.False(attributeUsageAttribute.AllowMultiple);
        Assert.True(attributeUsageAttribute.Inherited);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var attribute = new EnumAttribute<MyEnum>();
        Assert.Equal(typeof(MyEnum), attribute.EnumType);
        Assert.False(attribute.SupportFlags);
        Assert.Null(attribute.ErrorMessage);
        Assert.NotNull(attribute._validator);
        Assert.False(attribute._validator.SupportFlags);

        var attribute2 = new EnumAttribute<MyFlagsEnum> { SupportFlags = true };
        Assert.Equal(typeof(MyFlagsEnum), attribute2.EnumType);
        Assert.True(attribute2.SupportFlags);
        Assert.Null(attribute2.ErrorMessage);
        Assert.NotNull(attribute2._validator);
        Assert.True(attribute2._validator.SupportFlags);

        var attribute3 = new EnumAttribute(typeof(MyEnum));
        Assert.Equal(typeof(MyEnum), attribute3.EnumType);
        Assert.False(attribute3.SupportFlags);
        Assert.Null(attribute3.ErrorMessage);
        Assert.NotNull(attribute3._validator);
        Assert.False(attribute3._validator.SupportFlags);

        var attribute4 = new EnumAttribute(typeof(MyFlagsEnum)) { SupportFlags = true };
        Assert.Equal(typeof(MyFlagsEnum), attribute4.EnumType);
        Assert.True(attribute4.SupportFlags);
        Assert.Null(attribute4.ErrorMessage);
        Assert.NotNull(attribute4._validator);
        Assert.True(attribute4._validator.SupportFlags);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = MyEnum.Enum1, Data2 = MyFlagsEnum.Enum1 };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = MyFlagsEnum.Enum1, Data2 = MyFlagsEnum.Enum1 };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = MyEnum.Enum1, Data2 = MyEnum.Enum1 };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = MyFlagsEnum.Enum1, Data2 = MyEnum.Enum1 };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = MyEnum.Enum1, Data2 = MyFlagsEnum.Enum1 };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = MyFlagsEnum.Enum1, Data2 = MyFlagsEnum.Enum1 };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data must be a defined value of enum MyEnum.",
            validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = MyEnum.Enum1, Data2 = MyEnum.Enum1 };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 must be a valid combination of values defined in enum MyFlagsEnum.",
            validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = MyFlagsEnum.Enum1, Data2 = MyEnum.Enum1 };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data must be a defined value of enum MyEnum.",
            validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 must be a valid combination of values defined in enum MyFlagsEnum.",
            validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = MyEnum.Enum1, Data2 = MyFlagsEnum.Enum1 };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = MyFlagsEnum.Enum1, Data2 = MyFlagsEnum.Enum1 };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data must be a defined value of enum MyEnum.",
            exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = MyEnum.Enum1, Data2 = MyEnum.Enum1 };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 must be a valid combination of values defined in enum MyFlagsEnum.",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = MyFlagsEnum.Enum1, Data2 = MyEnum.Enum1 };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data must be a defined value of enum MyEnum.",
            exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new EnumAttribute<MyEnum>();
        Assert.Equal("The field data must be a defined value of enum MyEnum.",
            attribute.FormatErrorMessage("data"));

        var attribute2 = new EnumAttribute<MyFlagsEnum> { SupportFlags = true };
        Assert.Equal("The field data must be a valid combination of values defined in enum MyFlagsEnum.",
            attribute2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new EnumAttribute<MyEnum>();
        Assert.Equal("EnumValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new EnumAttribute<MyFlagsEnum> { SupportFlags = true };
        Assert.Equal("EnumValidator_ValidationError_SupportFlags", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [Enum<MyEnum>] public object? Data { get; set; }

        [Enum<MyFlagsEnum>(SupportFlags = true)]
        public object? Data2 { get; set; }
    }
}