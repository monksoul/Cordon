// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class DataAnnotationMessageProviderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.NotNull(DataAnnotationMessageProvider._lock);
        Assert.Null(DataAnnotationMessageProvider._originalResourceManager);
        Assert.Null(DataAnnotationMessageProvider._resourceManagerField);

        Assert.NotNull(DataAnnotationMessageProvider._overrides);
        Assert.Empty(DataAnnotationMessageProvider._overrides);
    }

    [Fact]
    public void AddOverride_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => DataAnnotationMessageProvider.AddOverride(null!, null!));
        Assert.Throws<ArgumentException>(() => DataAnnotationMessageProvider.AddOverride(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => DataAnnotationMessageProvider.AddOverride(" ", null!));
        Assert.Throws<ArgumentNullException>(() =>
            DataAnnotationMessageProvider.AddOverride("RequiredAttribute_ValidationError", null!));
    }

    [Fact]
    public void AddOverride_ReturnOK()
    {
        Assert.Empty(DataAnnotationMessageProvider._overrides);
        DataAnnotationMessageProvider.AddOverride("RequiredAttribute_ValidationError", "字段 {0} 是必填项。");
        Assert.Single(DataAnnotationMessageProvider._overrides);
        Assert.Equal("字段 {0} 是必填项。", DataAnnotationMessageProvider._overrides["RequiredAttribute_ValidationError"]);

        // 清除单元测试影响
        DataAnnotationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void AddOverrides_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => DataAnnotationMessageProvider.AddOverrides(null!));

    [Fact]
    public void AddOverrides_ReturnOK()
    {
        Assert.Empty(DataAnnotationMessageProvider._overrides);
        DataAnnotationMessageProvider.AddOverrides(new Dictionary<string, string>
        {
            { "RequiredAttribute_ValidationError", "字段 {0} 是必填项。" },
            { "StringLengthAttribute_ValidationError", "字段 {0} 必须是字符串，且最大长度为 {1}。" }
        });

        Assert.Equal(2, DataAnnotationMessageProvider._overrides.Count);
        Assert.Equal("字段 {0} 是必填项。", DataAnnotationMessageProvider._overrides["RequiredAttribute_ValidationError"]);
        Assert.Equal("字段 {0} 必须是字符串，且最大长度为 {1}。",
            DataAnnotationMessageProvider._overrides["StringLengthAttribute_ValidationError"]);

        // 清除单元测试影响
        DataAnnotationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void ClearOverrides_ReturnOK()
    {
        DataAnnotationMessageProvider.AddOverride("RequiredAttribute_ValidationError", "字段 {0} 是必填项。");
        Assert.Single(DataAnnotationMessageProvider._overrides);
        DataAnnotationMessageProvider.ClearOverrides();
        Assert.Empty(DataAnnotationMessageProvider._overrides);

        Assert.Null(DataAnnotationMessageProvider._originalResourceManager);
        Assert.Null(DataAnnotationMessageProvider._resourceManagerField);
    }

    [Fact]
    public void TryGetOverride_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => DataAnnotationMessageProvider.TryGetOverride(null!));
        Assert.Throws<ArgumentException>(() => DataAnnotationMessageProvider.TryGetOverride(string.Empty));
        Assert.Throws<ArgumentException>(() => DataAnnotationMessageProvider.TryGetOverride(" "));
    }

    [Fact]
    public void TryGetOverride_ReturnOK()
    {
        Assert.Null(DataAnnotationMessageProvider.TryGetOverride("RequiredAttribute_ValidationError"));
        DataAnnotationMessageProvider.AddOverride("RequiredAttribute_ValidationError", "字段 {0} 是必填项。");

        Assert.Equal("字段 {0} 是必填项。", DataAnnotationMessageProvider.TryGetOverride("RequiredAttribute_ValidationError"));

        // 清除单元测试影响
        DataAnnotationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void UseChineseMessages_ReturnOK()
    {
        DataAnnotationMessageProvider.UseChineseMessages();
        Assert.Equal(24, DataAnnotationMessageProvider._overrides.Count);

        // 清除单元测试影响
        DataAnnotationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void ApplyOverrides_ReturnOK()
    {
        DataAnnotationMessageProvider.ApplyOverrides();
        Assert.Null(DataAnnotationMessageProvider._resourceManagerField);

        DataAnnotationMessageProvider.AddOverride("RequiredAttribute_ValidationError", "字段 {0} 是必填项。");
        DataAnnotationMessageProvider.ApplyOverrides();
        Assert.NotNull(DataAnnotationMessageProvider._resourceManagerField);

        // 清除单元测试影响
        DataAnnotationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void OverrideResourceManager_New_ReturnOK()
    {
        DataAnnotationMessageProvider.AddOverrides(new Dictionary<string, string>
        {
            { "RequiredAttribute_ValidationError", "字段 {0} 是必填项。" },
            { "StringLengthAttribute_ValidationError", "字段 {0} 必须是字符串，且最大长度为 {1}。" }
        });
        var overrideResourceManager =
            new DataAnnotationMessageProvider.OverrideResourceManager(DataAnnotationMessageProvider._overrides);
        Assert.NotNull(overrideResourceManager);

        Assert.Null(overrideResourceManager.GetString("RangeAttribute_ValidationError"));
        Assert.Equal("字段 {0} 是必填项。", overrideResourceManager.GetString("RequiredAttribute_ValidationError"));
        Assert.Equal("字段 {0} 必须是字符串，且最大长度为 {1}。",
            overrideResourceManager.GetString("StringLengthAttribute_ValidationError"));

        // 清除单元测试影响
        DataAnnotationMessageProvider.ClearOverrides();
    }
}