// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class PredicateValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new PredicateValidator<int>(u => u > 10);
        Assert.NotNull(validator.Condition);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is invalid.", validator._errorMessageResourceAccessor());

        var validator2 = new PredicateValidator<int>((u, _) => u > 10);
        Assert.NotNull(validator2.Condition);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(9, false)]
    [InlineData(10, false)]
    [InlineData(11, true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new PredicateValidator<int?>(u => u is null or > 10);
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WithValidatorException_ReturnOK()
    {
        var validator = new PredicateValidator<int?>(u =>
        {
            switch (u)
            {
                case < 10:
                    return false;
                case 10:
                    Must.False("不能等于 10");
                    break;
            }

            return true;
        });
        Assert.True(validator.IsValid(null));
        Assert.True(validator.IsValid(11));
        Assert.False(validator.IsValid(10));
        Assert.False(validator.IsValid(9));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new PredicateValidator<int>(u => u > 10);
        Assert.Null(validator.GetValidationResults(15, "data"));

        var validationResults = validator.GetValidationResults(9, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is invalid.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(9, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithValidatorException_ReturnOK()
    {
        var validator = new PredicateValidator<int?>(u =>
        {
            switch (u)
            {
                case < 10:
                    return false;
                case 10:
                    Must.False("不能等于 10");
                    break;
            }

            return true;
        });
        Assert.Null(validator.GetValidationResults(15, "data"));

        var validationResults = validator.GetValidationResults(9, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is invalid.", validationResults.First().ErrorMessage);

        var validationResults2 = validator.GetValidationResults(10, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("不能等于 10", validationResults2.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults3 = validator.GetValidationResults(9, "data");
        Assert.NotNull(validationResults3);
        Assert.Single(validationResults3);
        Assert.Equal("数据无效", validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new PredicateValidator<int>(u => u > 10);
        validator.Validate(15, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(9, "data"));
        Assert.Equal("The field data is invalid.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(9, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void Validate_WithValidatorException_ReturnOK()
    {
        var validator = new PredicateValidator<int?>(u =>
        {
            switch (u)
            {
                case < 10:
                    return false;
                case 10:
                    Must.False("不能等于 10");
                    break;
            }

            return true;
        });
        validator.Validate(15, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(9, "data"));
        Assert.Equal("The field data is invalid.", exception.Message);

        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(10, "data"));
        Assert.Equal("不能等于 10", exception2.Message);

        validator.ErrorMessage = "数据无效";
        var exception3 = Assert.Throws<ValidationException>(() => validator.Validate(9, "data"));
        Assert.Equal("数据无效", exception3.Message);
    }
}