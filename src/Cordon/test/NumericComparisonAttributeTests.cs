// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class NumericComparisonAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(TestNumericAttribute);
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
        var attribute = new TestNumericAttribute(10);
        Assert.Null(attribute.ErrorMessage);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var model = new TestModel { Data = 30 };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = 9 };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var model = new TestModel { Data = 30 };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = 9 };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data must be greater than or equal to '10'.", validationResults2[0].ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var model = new TestModel { Data = 30 };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = 9 };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data must be greater than or equal to '10'.", exception.ValidationResult.ErrorMessage);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new TestNumericAttribute(10);
        Assert.Equal("The field data must be greater than or equal to '10'.", attribute.FormatErrorMessage("data"));
    }

    public class TestModel
    {
        [TestNumeric(10)] public decimal? Data { get; set; }
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class TestNumericAttribute : NumericComparisonAttribute
{
    /// <inheritdoc />
    public TestNumericAttribute(int compareValue)
        : this(compareValue as IComparable)
    {
    }

    /// <inheritdoc />
    public TestNumericAttribute(double compareValue)
        : this(compareValue as IComparable)
    {
    }

    /// <inheritdoc />
    public TestNumericAttribute(decimal compareValue)
        : this(compareValue as IComparable)
    {
    }

    /// <inheritdoc />
    public TestNumericAttribute(IComparable compareValue)
        : base(compareValue, () => "The field {0} must be greater than or equal to '{1}'.")
    {
    }

    /// <inheritdoc />
    protected override bool IsValid(decimal value, decimal compareValue) => value >= compareValue;
}