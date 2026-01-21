// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class FileExtensionsValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new FileExtensionsValidator();
        Assert.Equal("png,jpg,jpeg,gif", validator.Extensions);
        Assert.Equal("png,jpg,jpeg,gif", validator.ExtensionsNormalized);
        Assert.Equal([".png", ".jpg", ".jpeg", ".gif"], validator.ExtensionsParsed);
        Assert.Equal(".png, .jpg, .jpeg, .gif", validator.ExtensionsFormatted);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The {0} field only accepts files with the following extensions: {1}.",
            validator._errorMessageResourceAccessor());

        var validator2 = new FileExtensionsValidator { Extensions = "png,jpg,jpeg,gif" };
        Assert.Equal("png,jpg,jpeg,gif", validator2.Extensions);
        Assert.Equal("png,jpg,jpeg,gif", validator2.ExtensionsNormalized);
        Assert.Equal([".png", ".jpg", ".jpeg", ".gif"], validator2.ExtensionsParsed);
        Assert.Equal(".png, .jpg, .jpeg, .gif", validator2.ExtensionsFormatted);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("furion.zip", false)]
    [InlineData("furion.ico", false)]
    [InlineData("furion.png", true)]
    [InlineData("furion.jpg", true)]
    [InlineData("furion.jpeg", true)]
    [InlineData("furion.gif", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new FileExtensionsValidator { Extensions = "png,jpg,jpeg,gif" };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new FileExtensionsValidator { Extensions = "png,jpg,jpeg,gif" };
        Assert.Null(validator.GetValidationResults("furion.png", "data"));

        var validationResults = validator.GetValidationResults("furion.ico", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field only accepts files with the following extensions: .png, .jpg, .jpeg, .gif.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("furion.ico", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new FileExtensionsValidator { Extensions = "png,jpg,jpeg,gif" };
        validator.Validate("furion.png", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("furion.ico", "data"));
        Assert.Equal("The data field only accepts files with the following extensions: .png, .jpg, .jpeg, .gif.",
            exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("furion.ico", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new FileExtensionsValidator { Extensions = "png,jpg,jpeg,gif" };
        Assert.Equal("The data field only accepts files with the following extensions: .png, .jpg, .jpeg, .gif.",
            validator.FormatErrorMessage("data"));
    }

    [Theory]
    [InlineData("furion.png", true)]
    [InlineData("furion.jpg", true)]
    [InlineData("furion.jpeg", true)]
    [InlineData("furion.gif", true)]
    [InlineData("furion.ico", false)]
    public void ValidateExtension_ReturnOK(string fileName, bool result)
    {
        var validator = new FileExtensionsValidator { Extensions = "png,jpg,jpeg,gif" };
        Assert.Equal(result, validator.ValidateExtension(fileName));
    }
}