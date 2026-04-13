// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class LessThanValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new LessThanValidator(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        Assert.Equal(10, validator.CompareValue);
        Assert.Null(validator.Conversion);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be less than '{1}'.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(0, true)]
    [InlineData(8, true)]
    [InlineData(9, true)]
    [InlineData(10, false)]
    [InlineData(11, false)]
    [InlineData(30, false)]
    [InlineData(10.1, false)]
    [InlineData(9.99, false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new LessThanValidator(10);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WithDateTimeType_ReturnOK()
    {
        var validator = new LessThanValidator(new DateTime(2020, 1, 1));
        Assert.False(validator.IsValid(new DateTime(2020, 1, 1)));
        Assert.False(validator.IsValid(new DateTime(2020, 1, 2)));
        Assert.True(validator.IsValid(new DateTime(2019, 12, 31)));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        Assert.Null(validator.GetValidationResults(9, "data"));

        var validationResults = validator.GetValidationResults(30, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be less than '10'.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(30, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        validator.Validate(9, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(30, "data"));
        Assert.Equal("The field data must be less than '10'.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(30, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        Assert.Equal("The field data must be less than '10'.", validator.FormatErrorMessage("data"));
    }

    [Fact]
    public void SetupConversion_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        Assert.Null(validator.Conversion);
        validator.SetupConversion();
        Assert.NotNull(validator.Conversion);

        Assert.Equal(10, validator.Conversion(10));
        Assert.Equal(10, validator.Conversion(9.99));

        var validator2 = new LessThanValidator(10.1);
        Assert.Null(validator2.Conversion);
        validator2.SetupConversion();
        Assert.NotNull(validator2.Conversion);

        Assert.Equal(10.0, validator2.Conversion(10));
        Assert.Equal(9.99, validator2.Conversion(9.99));

        // var validator3 = new LessThanValidator((long)10);
        // Assert.Null(validator3.Conversion);
        // validator3.SetupConversion();
        // Assert.NotNull(validator3.Conversion);
        //
        // Assert.Equal((long)10, validator3.Conversion(10));
        // Assert.Equal((long)10, validator3.Conversion(9.99));
    }

    [Fact]
    public void CreateDefaultConversion_ReturnOK()
    {
        var validator = new LessThanValidator(10);
        var createDefaultConversionMethod = typeof(ComparisonValidator).GetMethod("CreateDefaultConversion",
                BindingFlags.NonPublic | BindingFlags.Instance)
            ?.CreateDelegate<Func<Type, Func<object, object>>>(validator);
        Assert.NotNull(createDefaultConversionMethod);

        var conversion = createDefaultConversionMethod(typeof(int));
        Assert.Equal(10, conversion(10));
    }
}