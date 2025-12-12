// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ValidationBaseAttributeTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.NotNull(ValidationBaseAttribute._errorMessageResourceAccessorSetter);
        var attribute = new CustomAttribute();
        ValidationBaseAttribute._errorMessageResourceAccessorSetter.Value(attribute, () => "验证无效");

        Assert.Equal("验证无效", attribute.FormatErrorMessage("data"));
    }

    public class CustomAttribute : ValidationBaseAttribute;
}