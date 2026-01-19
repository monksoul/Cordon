// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class PasswordAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(PasswordAttribute);
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
        var attribute = new PasswordAttribute();
        Assert.False(attribute.Strong);
        Assert.Null(attribute.ErrorMessage);
        Assert.NotNull(attribute._validator);
        Assert.False(attribute._validator.Strong);

        var attribute2 = new PasswordAttribute { Strong = true };
        Assert.True(attribute2.Strong);
        Assert.Null(attribute2.ErrorMessage);
        Assert.NotNull(attribute2._validator);
        Assert.True(attribute2._validator.Strong);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "q1w2e3r4", Data2 = "cln99871433*_Q" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "abcdefghijklm", Data2 = "cln99871433*_Q" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = "q1w2e3r4", Data2 = "q1w2e3r4" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "abcdefghijklm", Data2 = "q1w2e3r4" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "q1w2e3r4", Data2 = "cln99871433*_Q" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "abcdefghijklm", Data2 = "cln99871433*_Q" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal(
            "The field Data has an invalid password format. It must be 8 to 64 characters long and contain at least one letter and one number.",
            validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "q1w2e3r4", Data2 = "q1w2e3r4" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal(
            "The field Data2 has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "abcdefghijklm", Data2 = "q1w2e3r4" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal(
            "The field Data has an invalid password format. It must be 8 to 64 characters long and contain at least one letter and one number.",
            validationResults4[0].ErrorMessage);
        Assert.Equal(
            "The field Data2 has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "q1w2e3r4", Data2 = "cln99871433*_Q" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "abcdefghijklm", Data2 = "cln99871433*_Q" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal(
            "The field Data has an invalid password format. It must be 8 to 64 characters long and contain at least one letter and one number.",
            exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "q1w2e3r4", Data2 = "q1w2e3r4" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal(
            "The field Data2 has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "abcdefghijklm", Data2 = "q1w2e3r4" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal(
            "The field Data has an invalid password format. It must be 8 to 64 characters long and contain at least one letter and one number.",
            exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new PasswordAttribute();
        Assert.Equal(
            "The field data has an invalid password format. It must be 8 to 64 characters long and contain at least one letter and one number.",
            attribute.FormatErrorMessage("data"));

        var attribute2 = new PasswordAttribute { Strong = true };
        Assert.Equal(
            "The field data has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            attribute2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new PasswordAttribute();
        Assert.Equal("PasswordValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new PasswordAttribute { Strong = true };
        Assert.Equal("PasswordValidator_ValidationError_Strong", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [Password] public string? Data { get; set; }

        [Password(Strong = true)] public string? Data2 { get; set; }
    }
}