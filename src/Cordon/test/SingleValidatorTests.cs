// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class SingleValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new SingleValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} only allows a single item.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData('a', true)]
    [InlineData("赢", true)]
    [InlineData("", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new SingleValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void IsValid_Collection_ReturnOK()
    {
        var validator = new SingleValidator();

        // Empty ===
        var emptyArray = Array.Empty<string>();
        Assert.False(validator.IsValid(emptyArray));

        var emptyList = new List<string>();
        Assert.False(validator.IsValid(emptyList));

        Assert.False(validator.IsValid(string.Empty));

        var emptyDictionary = new Dictionary<string, string>();
        Assert.False(validator.IsValid(emptyDictionary));

        IEnumerable<string> emptyEnumerable = new List<string>();
        Assert.False(validator.IsValid(emptyEnumerable));

        var emptyArrayList = new ArrayList();
        Assert.False(validator.IsValid(emptyArrayList));

        var emptyHashSet = new HashSet<string>();
        Assert.False(validator.IsValid(emptyHashSet));

        // Single ===

        var singleArray = new[] { "furion" };
        Assert.True(validator.IsValid(singleArray));

        var singleList = new List<string> { "Furion" };
        Assert.True(validator.IsValid(singleList));

        var singleDictionary = new Dictionary<string, string> { { "Furion", "百小僧" } };
        Assert.True(validator.IsValid(singleDictionary));

        IEnumerable<string> singleEnumerable = new List<string> { "Furion" };
        Assert.True(validator.IsValid(singleEnumerable));

        var singleArrayList = new ArrayList { "furion" };

        Assert.True(validator.IsValid(singleArrayList));

        var singleHashSet = new HashSet<string> { "Furion" };
        Assert.True(validator.IsValid(singleHashSet));

        // Not Single ===

        var notSingleArray = new[] { "furion", "百小僧" };
        Assert.False(validator.IsValid(notSingleArray));

        var notSingleList = new List<string> { "Furion", "百小僧" };
        Assert.False(validator.IsValid(notSingleList));

        var notSingleDictionary = new Dictionary<string, string> { { "Furion", "百小僧" }, { "Fur", "MonkSoul" } };
        Assert.False(validator.IsValid(notSingleDictionary));

        IEnumerable<string> notSingleEnumerable = new List<string> { "Furion", "百小僧" };
        Assert.False(validator.IsValid(notSingleEnumerable));

        var notSingleArrayList = new ArrayList { "furion", "百小僧" };

        Assert.False(validator.IsValid(notSingleArrayList));

        var notSingleHashSet = new HashSet<string> { "Furion", "百小僧" };
        Assert.False(validator.IsValid(notSingleHashSet));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new SingleValidator();
        Assert.Null(validator.GetValidationResults(new[] { "Furion" }, "data"));

        var validationResults = validator.GetValidationResults(new[] { "Furion", "百小僧" }, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data only allows a single item.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults(new[] { "Furion", "百小僧" }, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new SingleValidator();
        validator.Validate(new[] { "Furion" }, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate(new[] { "Furion", "百小僧" }, "data"));
        Assert.Equal("The field data only allows a single item.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 =
            Assert.Throws<ValidationException>(() => validator.Validate(new[] { "Furion", "百小僧" }, "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}