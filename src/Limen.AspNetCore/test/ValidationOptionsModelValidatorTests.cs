// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using System.Reflection;

namespace Limen.AspNetCore.Tests;

public class ValidationOptionsModelValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var modelValidator = new ValidationOptionsModelValidator();
        Assert.NotNull(modelValidator);
    }

    [Fact]
    public void Validate_ReturnOK()
    {
    }

    [Fact]
    public void ExtractFromAction_ReturnOK()
    {
        Assert.Null(
            ValidationOptionsModelValidator.ExtractFromAction(
                typeof(TestController).GetMethod(nameof(TestController.Get))!));

        var metadata =
            ValidationOptionsModelValidator.ExtractFromAction(
                typeof(TestController2).GetMethod(nameof(TestController.Get))!);
        Assert.NotNull(metadata);
        Assert.Equal(["action"], (string[]?)metadata.RuleSets!);
    }

    [Fact]
    public void ExtractFromController_ReturnOK()
    {
        Assert.Null(ValidationOptionsModelValidator.ExtractFromController(typeof(TestController).GetTypeInfo()));

        var metadata = ValidationOptionsModelValidator.ExtractFromController(typeof(TestController2).GetTypeInfo());
        Assert.NotNull(metadata);
        Assert.Equal(["controller"], (string[]?)metadata.RuleSets!);
    }

    [Fact]
    public void CreateMetadata_ReturnOK()
    {
        Assert.Null(ValidationOptionsModelValidator.CreateMetadata(null));

        var metadata = ValidationOptionsModelValidator.CreateMetadata(new ValidationOptionsAttribute());
        Assert.NotNull(metadata);
        Assert.Null(metadata.RuleSets);

        var metadata2 = ValidationOptionsModelValidator.CreateMetadata(new ValidationOptionsAttribute(["email"]));
        Assert.NotNull(metadata2);
        Assert.Equal(["email"], (string[]?)metadata2.RuleSets!);
    }

    [Route("[controller]/[action]")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Get() => Content("OK");
    }

    [Route("[controller]/[action]")]
    [ValidationOptions(["controller"])]
    public class TestController2 : Controller
    {
        [HttpGet]
        [ValidationOptions(["action"])]
        public IActionResult Get() => Content("OK");
    }
}