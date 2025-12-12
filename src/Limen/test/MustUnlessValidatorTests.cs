// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class MustUnlessValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new MustUnlessValidator<int>(u => u < 10);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is invalid.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(9, false)]
    [InlineData(10, false)]
    [InlineData(11, true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new MustUnlessValidator<int?>(u => u is not (null or > 10));
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new MustUnlessValidator<int>(u => u <= 10);
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
    public void Validate_ReturnOK()
    {
        var validator = new MustUnlessValidator<int>(u => u <= 10);
        validator.Validate(15, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(9, "data"));
        Assert.Equal("The field data is invalid.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(9, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}