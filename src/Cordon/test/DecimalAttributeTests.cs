// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class DecimalAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(DecimalAttribute);
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
        var attribute = new DecimalAttribute();
        Assert.Equal(18, attribute.Precision);
        Assert.Equal(2, attribute.Scale);
        Assert.False(attribute.AllowStringValues);
        Assert.False(attribute.AllowNegative);
        Assert.Null(attribute.ErrorMessage);
        Assert.NotNull(attribute._validator);
        Assert.Equal(18, attribute._validator.Precision);
        Assert.Equal(2, attribute._validator.Scale);
        Assert.False(attribute._validator.AllowNegative);
        Assert.False(attribute._validator.AllowStringValues);

        var attribute2 = new DecimalAttribute(10, 3) { AllowNegative = true, AllowStringValues = true };
        Assert.Equal(10, attribute2.Precision);
        Assert.Equal(3, attribute2.Scale);
        Assert.True(attribute2.AllowStringValues);
        Assert.True(attribute2.AllowNegative);
        Assert.Null(attribute2.ErrorMessage);
        Assert.NotNull(attribute2._validator);
        Assert.Equal(10, attribute2._validator.Precision);
        Assert.Equal(3, attribute2._validator.Scale);
        Assert.True(attribute2._validator.AllowNegative);
        Assert.True(attribute2._validator.AllowStringValues);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = 10.12, Data2 = "10.12" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = 10.123, Data2 = "10.12" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = 10.12, Data2 = "10.123" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = 10.123, Data2 = "10.123" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = 10.12, Data2 = "10.12" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = 10.123, Data2 = "10.12" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = 10.12, Data2 = "10.123" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 must be a valid decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = 10.123, Data2 = "10.123" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 must be a valid decimal within the DECIMAL(18,2) precision or scale limit.",
            validationResults4[1].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = 10.12, Data2 = "10.12" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = 10.123, Data2 = "10.12" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = 10.12, Data2 = "10.123" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 must be a valid decimal within the DECIMAL(18,2) precision or scale limit.",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = 10.123, Data2 = "10.123" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            exception3.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new DecimalAttribute();
        Assert.Equal("The field data must be a non-negative decimal within the DECIMAL(18,2) precision or scale limit.",
            attribute.FormatErrorMessage("data"));

        var attribute2 = new DecimalAttribute { AllowNegative = true };
        Assert.Equal("The field data must be a valid decimal within the DECIMAL(18,2) precision or scale limit.",
            attribute2.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new DecimalAttribute();
        Assert.Equal("DecimalValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new DecimalAttribute { AllowNegative = true };
        Assert.Equal("DecimalValidator_ValidationError_AllowNegative", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [Decimal] public double Data { get; set; }

        [Decimal(AllowNegative = true, AllowStringValues = true)]
        public string? Data2 { get; set; }
    }
}