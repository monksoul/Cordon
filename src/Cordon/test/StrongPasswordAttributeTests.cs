// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class StrongPasswordAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(StrongPasswordAttribute);
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
        var attribute = new StrongPasswordAttribute();
        Assert.True(attribute.Strong);
        Assert.Null(attribute.ErrorMessage);
        var validator2 = Helpers.GetValidator(attribute) as PasswordValidator;
        Assert.NotNull(validator2);
        Assert.True(validator2.Strong);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = "cln99871433*_Q" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "q1w2e3r4" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = "cln99871433*_Q" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "q1w2e3r4" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal(
            "The field Data has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            validationResults2[0].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = "cln99871433*_Q" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "q1w2e3r4" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal(
            "The field Data has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            exception.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new StrongPasswordAttribute();
        Assert.Equal(
            "The field data has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            attribute.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new StrongPasswordAttribute();
        Assert.Equal("PasswordValidator_ValidationError_Strong", attribute.GetResourceKey());
    }

    public class TestModel
    {
        [Password(Strong = true)] public string? Data { get; set; }
    }
}