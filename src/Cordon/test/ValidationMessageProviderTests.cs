// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidationMessageProviderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.NotNull(ValidationMessageProvider._overrides);
        Assert.Empty(ValidationMessageProvider._overrides);
    }

    [Fact]
    public void AddOverride_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationMessageProvider.AddOverride(null!, null!));
        Assert.Throws<ArgumentException>(() => ValidationMessageProvider.AddOverride(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => ValidationMessageProvider.AddOverride(" ", null!));
        Assert.Throws<ArgumentNullException>(() =>
            ValidationMessageProvider.AddOverride("AgeValidator_ValidationError", null!));
    }

    [Fact]
    public void AddOverride_ReturnOK()
    {
        Assert.Empty(ValidationMessageProvider._overrides);
        ValidationMessageProvider.AddOverride("AgeValidator_ValidationError", "字段 {0} 不是有效的年龄。");
        Assert.Single(ValidationMessageProvider._overrides);
        Assert.Equal("字段 {0} 不是有效的年龄。", ValidationMessageProvider._overrides["AgeValidator_ValidationError"]);

        // 清除单元测试影响
        ValidationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void AddOverrides_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ValidationMessageProvider.AddOverrides(null!));

    [Fact]
    public void AddOverrides_ReturnOK()
    {
        Assert.Empty(ValidationMessageProvider._overrides);
        ValidationMessageProvider.AddOverrides(new Dictionary<string, string>
        {
            { "AgeValidator_ValidationError", "字段 {0} 不是有效的年龄。" },
            { "BankCardValidator_ValidationError", "字段 {0} 不是有效的银行卡号。" }
        });

        Assert.Equal(2, ValidationMessageProvider._overrides.Count);
        Assert.Equal("字段 {0} 不是有效的年龄。", ValidationMessageProvider._overrides["AgeValidator_ValidationError"]);
        Assert.Equal("字段 {0} 不是有效的银行卡号。", ValidationMessageProvider._overrides["BankCardValidator_ValidationError"]);

        // 清除单元测试影响
        ValidationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void ClearOverrides_ReturnOK()
    {
        ValidationMessageProvider.AddOverride("AgeValidator_ValidationError", "字段 {0} 不是有效的年龄。");
        Assert.Single(ValidationMessageProvider._overrides);
        ValidationMessageProvider.ClearOverrides();
        Assert.Empty(ValidationMessageProvider._overrides);
    }

    [Fact]
    public void TryGetOverride_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationMessageProvider.TryGetOverride(null!));
        Assert.Throws<ArgumentException>(() => ValidationMessageProvider.TryGetOverride(string.Empty));
        Assert.Throws<ArgumentException>(() => ValidationMessageProvider.TryGetOverride(" "));
    }

    [Fact]
    public void TryGetOverride_ReturnOK()
    {
        Assert.Null(ValidationMessageProvider.TryGetOverride("AgeValidator_ValidationError"));
        ValidationMessageProvider.AddOverride("AgeValidator_ValidationError", "字段 {0} 不是有效的年龄。");

        Assert.Equal("字段 {0} 不是有效的年龄。", ValidationMessageProvider.TryGetOverride("AgeValidator_ValidationError"));

        // 清除单元测试影响
        ValidationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void UseChineseMessages_ReturnOK()
    {
        ValidationMessageProvider.UseChineseMessages();
        Assert.Equal(67, ValidationMessageProvider._overrides.Count);

        // 清除单元测试影响
        ValidationMessageProvider.ClearOverrides();
    }
}