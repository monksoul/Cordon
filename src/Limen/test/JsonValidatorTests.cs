// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class JsonValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new JsonValidator();
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The {0} field must be a valid JSON object or array.", validator._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(" ", false)]
    [InlineData(1, false)]
    [InlineData("furion", false)]
    [InlineData("\"furion\"", false)]
    [InlineData("{}", true)]
    [InlineData("[]", true)]
    [InlineData("{\"id\":1,\"name\":\"furion\"}", true)]
    [InlineData("[1,2,3,true,false,\"furion\"]", true)]
    [InlineData("{\"id\":1,\"name\":\"furion\"]", false)]
    [InlineData("[1,2,3,true,false,\"furion\"}", false)]
    [InlineData("{\"id\":1,\"name\":\"furion\",}", false)]
    [InlineData("[1,2,3,true,false,\"furion\",]", false)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new JsonValidator();
        Assert.Equal(result, validator.IsValid(value));
    }

    [Theory]
    [InlineData("{\"id\":1,\"name\":\"furion\",}", true)]
    [InlineData("[1,2,3,true,false,\"furion\",]", true)]
    public void IsValid_WithAllowTrailingCommas_ReturnOK(object? value, bool result)
    {
        var validator = new JsonValidator { AllowTrailingCommas = true };
        Assert.Equal(result, validator.IsValid(value));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new JsonValidator();
        Assert.Null(validator.GetValidationResults("{\"id\":1,\"name\":\"furion\"}", "data"));

        var validationResults = validator.GetValidationResults("\"furion\"", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The data field must be a valid JSON object or array.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("\"furion\"", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new JsonValidator();
        validator.Validate("{\"id\":1,\"name\":\"furion\"}", "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("\"furion\"", "data"));
        Assert.Equal("The data field must be a valid JSON object or array.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("\"furion\"", "data"));
        Assert.Equal("数据无效", exception2.Message);
    }
}