// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class StrongPasswordValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new StrongPasswordValidator();
        Assert.NotNull(validator);
        Assert.True(validator.Strong);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal(
            "The field {0} has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData(123456789, false)]
    [InlineData("abcdefghijklm", false)]
    [InlineData("q1w2e3r4", false)]
    [InlineData("-88809997", false)]
    [InlineData("-a88809997", false)]
    [InlineData("cln9987*_Q", false)]
    [InlineData("cln99871433*_Q", true)]
    [InlineData("TxyFxy1398*#13", true)]
    public void IsValid_ReturnOK(object? value, bool expected)
    {
        var validator = new StrongPasswordValidator();
        Assert.Equal(expected, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new StrongPasswordValidator();
        Assert.Null(validator.GetValidationResults("cln99871433*_Q", "data"));

        var validationResults = validator.GetValidationResults("cln9987*_Q", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal(
            "The field data has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("cln9987*_Q", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new StrongPasswordValidator();
        validator.Validate("cln99871433*_Q", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("cln9987*_Q", "data"));
        Assert.Equal(
            "The field data has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("cln9987*_Q", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new StrongPasswordValidator();
        Assert.Equal(
            "The field data has an invalid password format. It must be 12 to 64 characters long and contain uppercase letters, lowercase letters, numbers, and special characters.",
            validator.FormatErrorMessage("data"));
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new StrongPasswordValidator();
        Assert.Equal("PasswordValidator_ValidationError_Strong", validator.GetResourceKey());
    }
}