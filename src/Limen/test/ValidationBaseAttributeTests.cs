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
        Assert.Equal("The field data is invalid.", attribute.FormatErrorMessage("data"));
        ValidationBaseAttribute._errorMessageResourceAccessorSetter.Value(attribute, () => "验证无效");
        Assert.Equal("验证无效", attribute.FormatErrorMessage("data"));

        var attribute2 = new CustomAttribute2();
        Assert.Equal("错误消息", attribute2.FormatErrorMessage("data"));
        ValidationBaseAttribute._errorMessageResourceAccessorSetter.Value(attribute2, () => "验证无效");
        Assert.Equal("验证无效", attribute2.FormatErrorMessage("data"));

        var attribute3 = new CustomAttribute3();
        Assert.Equal("错误消息", attribute3.FormatErrorMessage("data"));
        ValidationBaseAttribute._errorMessageResourceAccessorSetter.Value(attribute3, () => "验证无效");
        Assert.Equal("验证无效", attribute3.FormatErrorMessage("data"));
    }

    [Fact]
    public void UseResourceKey_Invalid_Parameters()
    {
        var attribute = new CustomAttribute();

        var userResourceMethod =
            typeof(ValidationBaseAttribute).GetMethod("UseResourceKey", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.CreateDelegate<Action<Func<string>>>(attribute);
        Assert.NotNull(userResourceMethod);

        Assert.Throws<ArgumentNullException>(() => userResourceMethod(null!));
    }

    [Fact]
    public void UseResourceKey_ReturnOK()
    {
        var attribute = new CustomAttribute();

        var userResourceMethod =
            typeof(ValidationBaseAttribute).GetMethod("UseResourceKey", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.CreateDelegate<Action<Func<string>>>(attribute);
        Assert.NotNull(userResourceMethod);

        userResourceMethod(() => "TestValidator_Error");

        var errorMessageResourceAccessorProperty = typeof(ValidationAttribute).GetField(
            "_errorMessageResourceAccessor",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(errorMessageResourceAccessorProperty);
        var errorMessageResourceAccessorGetter =
            (Func<string>)errorMessageResourceAccessorProperty.GetValue(attribute)!;

        Assert.Equal("[TestValidator_Error]", errorMessageResourceAccessorGetter());
    }

    public class CustomAttribute : ValidationBaseAttribute;

    public class CustomAttribute2() : ValidationBaseAttribute("错误消息");

    public class CustomAttribute3() : ValidationBaseAttribute(() => "错误消息");
}