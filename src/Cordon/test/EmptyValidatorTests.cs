// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class EmptyValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new EmptyValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} must be empty.", validator._errorMessageResourceAccessor());

        Assert.True(typeof(IHighPriorityValidator).IsAssignableFrom(typeof(EmptyValidator)));
        Assert.Equal(20, ((IHighPriorityValidator)validator).Priority);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("  ", false)]
    [InlineData("a", false)]
    [InlineData('A', false)]
    [InlineData("Furion", false)]
    [InlineData(1, false)]
    [InlineData(false, false)]
    [InlineData("\u3000", false)] // 特殊 Unicode 空白字符
    [InlineData('\0', false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new EmptyValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WithCollection_ReturnOK()
    {
        var validator = new EmptyValidator();

        Assert.True(validator.IsValid(Array.Empty<string>()));
        Assert.True(validator.IsValid(Enumerable.Empty<string>()));
        Assert.True(validator.IsValid(new Dictionary<string, string>()));
        Assert.True(validator.IsValid(new List<string>()));
        Assert.True(validator.IsValid(new HashSet<string>()));

        Assert.False(validator.IsValid(new[] { 1, 2 }));
        Assert.False(validator.IsValid(new List<int> { 1, 2, 3 }));
        Assert.False(validator.IsValid(new Dictionary<string, string> { { "key", "value" } }));
        Assert.False(validator.IsValid(new HashSet<string> { "furion" }));

        var enumerable = Enumerable.Empty<string>();
        var newEnumerable = enumerable.Concat(["furion", "fur"]);
        Assert.False(validator.IsValid(newEnumerable));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new EmptyValidator();
        Assert.Null(validator.GetValidationResults(string.Empty, "data"));

        var validationResults = validator.GetValidationResults("Furion", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data must be empty.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("Furion", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new EmptyValidator();
        validator.Validate(string.Empty, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "data"));
        Assert.Equal("The field data must be empty.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("Furion", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}