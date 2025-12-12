// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class BankCardValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new BankCardValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} is not a valid bank card number.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("6228480402564890018", true)]
    [InlineData("4111111111111111", true)]
    [InlineData("3566002020360505", true)]
    [InlineData("378282246310005", true)]
    [InlineData("4111-1111-1111-1111", true)]
    [InlineData("4111 1111 1111 1111", true)]
    [InlineData(" 4111111111111111", true)]
    [InlineData("4111111111111111 ", true)]
    [InlineData(6228480402564890018, true)]
    [InlineData(4111111111111111, true)]
    [InlineData(3566002020360505, true)]
    [InlineData(378282246310005, true)]
    // =====
    [InlineData("5502092303469876", false)]
    [InlineData("6222220000000000", false)]
    [InlineData("6222220000000004", false)]
    [InlineData("5502092303469875", false)]
    [InlineData("0228480402564890018", false)]
    [InlineData("411111111111111", false)]
    [InlineData("41111111111111111111", false)]
    [InlineData("4111-1111-1111-111", false)]
    [InlineData("4111111111111112", false)]
    [InlineData("ABCDE12345678901", false)]
    [InlineData("4111.1111.1111.1111", false)]
    [InlineData(5502092303469876, false)]
    [InlineData(6222220000000000, false)]
    [InlineData(6222220000000004, false)]
    [InlineData(5502092303469875, false)]
    [InlineData(0228480402564890018, false)]
    [InlineData(411111111111111, false)]
    [InlineData(4111111111111112, false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new BankCardValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData("6228480402564890018", true)]
    [InlineData("4111111111111111", true)]
    [InlineData("3566002020360505", true)]
    [InlineData("378282246310005", true)]
    // =====
    [InlineData("4111-1111-1111-1111", false)]
    [InlineData("4111 1111 1111 1111", false)]
    [InlineData(" 4111111111111111", false)]
    [InlineData("4111111111111111 ", false)]
    [InlineData("5502092303469876", false)]
    [InlineData("6222220000000000", false)]
    [InlineData("6222220000000004", false)]
    [InlineData("5502092303469875", false)]
    [InlineData("0228480402564890018", false)]
    [InlineData("411111111111111", false)]
    [InlineData("41111111111111111111", false)]
    [InlineData("4111-1111-1111-111", false)]
    [InlineData("4111111111111112", false)]
    [InlineData("ABCDE12345678901", false)]
    [InlineData("4111.1111.1111.1111", false)]
    public void CheckLuhn_ReturnOK(string value, bool result) =>
        Assert.Equal(result, BankCardValidator.CheckLuhn(value));

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new BankCardValidator();
        Assert.Null(validator.GetValidationResults("6228480402564890018", "data"));

        var validationResults = validator.GetValidationResults("6222220000000000", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data is not a valid bank card number.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("6222220000000000", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new BankCardValidator();
        validator.Validate("6228480402564890018", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("6222220000000000", "data"));
        Assert.Equal("The field data is not a valid bank card number.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("6222220000000000", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}