// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ValidationDataContextTest
{
    [Fact]
    public void New_ReturnOK()
    {
        var context = new ValidationDataContext();
        Assert.NotNull(context);
        Assert.NotNull(context._items);
        Assert.Empty(context._items);
        Assert.NotNull(context.Items);
        Assert.Empty(context.Items);
        Assert.NotNull(ValidationDataContext.ValidationOptionsKey);
    }

    [Fact]
    public void SetValue_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationDataContext().SetValue(null!, null));

    [Fact]
    public void SetValue_ReturnOK()
    {
        var context = new ValidationDataContext();
        context.SetValue("key", "value");
        Assert.Equal("value", context.Items["key"]);

        context.SetValue("key", "value1");
        Assert.Equal("value1", context.Items["key"]);
    }

    [Fact]
    public void TryGetValue_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationDataContext().TryGetValue(null!, out _));

    [Fact]
    public void TryGetValue_ReturnOK()
    {
        var context = new ValidationDataContext();
        Assert.False(context.TryGetValue("key", out _));

        context.SetValue("key", "value");
        Assert.True(context.TryGetValue("key", out var value));
        Assert.Equal("value", value);
    }

    [Fact]
    public void ContainsKey_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationDataContext().ContainsKey(null!));

    [Fact]
    public void ContainsKey_ReturnOK()
    {
        var context = new ValidationDataContext();
        Assert.False(context.ContainsKey("key"));

        context.SetValue("key", "value");
        Assert.True(context.ContainsKey("key"));
    }

    [Fact]
    public void SetValidationOptions_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationDataContext().SetValidationOptions(null!));

    [Fact]
    public void SetValidationOptions_ReturnOK()
    {
        var context = new ValidationDataContext();
        context.SetValidationOptions(new ValidationOptionsMetadata(["email"]));
        var metadata = context.GetValidationOptions();
        Assert.NotNull(metadata);
        Assert.Equal(["email"], (string[]?)metadata.RuleSets!);
    }

    [Fact]
    public void GetValidationOptions_ReturnOK()
    {
        var context = new ValidationDataContext();
        Assert.Null(context.GetValidationOptions());

        context.SetValidationOptions(new ValidationOptionsMetadata(["email"]));
        var metadata = context.GetValidationOptions();
        Assert.NotNull(metadata);
        Assert.Equal(["email"], (string[]?)metadata.RuleSets!);
    }

    [Fact]
    public void HasValidationOptions_ReturnOK()
    {
        var context = new ValidationDataContext();
        Assert.False(context.HasValidationOptions());

        context.SetValidationOptions(new ValidationOptionsMetadata(["email"]));
        Assert.True(context.HasValidationOptions());
    }
}