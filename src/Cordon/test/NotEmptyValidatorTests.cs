// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class NotEmptyValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new NotEmptyValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} does not allow empty values.", validator._errorMessageResourceAccessor());

        Assert.True(typeof(IHighPriorityValidator).IsAssignableFrom(typeof(NotEmptyValidator)));
        Assert.Equal(20, ((IHighPriorityValidator)validator).Priority);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("  ", true)]
    [InlineData("a", true)]
    [InlineData('A', true)]
    [InlineData("Furion", true)]
    [InlineData(1, false)]
    [InlineData(false, false)]
    [InlineData("\u3000", true)] // 特殊 Unicode 空白字符
    [InlineData('\0', true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new NotEmptyValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_WithCollection_ReturnOK()
    {
        var validator = new NotEmptyValidator();

        Assert.False(validator.IsValid(Array.Empty<string>()));
        Assert.False(validator.IsValid(Enumerable.Empty<string>()));
        Assert.False(validator.IsValid(new Dictionary<string, string>()));
        Assert.False(validator.IsValid(new List<string>()));
        Assert.False(validator.IsValid(new HashSet<string>()));

        Assert.True(validator.IsValid(new[] { 1, 2 }));
        Assert.True(validator.IsValid(new List<int> { 1, 2, 3 }));
        Assert.True(validator.IsValid(new Dictionary<string, string> { { "key", "value" } }));
        Assert.True(validator.IsValid(new HashSet<string> { "furion" }));

        var enumerable = Enumerable.Empty<string>();
        var newEnumerable = enumerable.Concat(["furion", "fur"]);
        Assert.True(validator.IsValid(newEnumerable));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new NotEmptyValidator();
        Assert.Null(validator.GetValidationResults("Furion", "data"));

        var validationResults = validator.GetValidationResults(string.Empty, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data does not allow empty values.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(string.Empty, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new NotEmptyValidator();
        validator.Validate("Furion", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(string.Empty, "data"));
        Assert.Equal("The field data does not allow empty values.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(string.Empty, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}