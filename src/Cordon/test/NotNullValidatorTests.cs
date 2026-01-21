// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class NotNullValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new NotNullValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} does not allow null values.", validator._errorMessageResourceAccessor());

        Assert.True(typeof(IHighPriorityValidator).IsAssignableFrom(typeof(NotNullValidator)));
        Assert.Equal(0, ((IHighPriorityValidator)validator).Priority);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", true)]
    [InlineData("  ", true)]
    [InlineData("a", true)]
    [InlineData('A', true)]
    [InlineData("Furion", true)]
    [InlineData(1, true)]
    [InlineData(false, true)]
    [InlineData("\u3000", true)] // 特殊 Unicode 空白字符
    [InlineData('\0', true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new NotNullValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new NotNullValidator();
        Assert.Null(validator.GetValidationResults("Furion", "data"));

        var validationResults = validator.GetValidationResults(null, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data does not allow null values.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(null, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new NotNullValidator();
        validator.Validate("Furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(null, "data"));
        Assert.Equal("The field data does not allow null values.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(null, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}