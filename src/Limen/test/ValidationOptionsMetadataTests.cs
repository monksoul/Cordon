// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ValidationOptionsMetadataTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var metadata = new ValidationOptionsMetadata(null);
        Assert.Null(metadata.RuleSets);

        var metadata2 = new ValidationOptionsMetadata([]);
        Assert.NotNull(metadata2.RuleSets);
        Assert.Empty(metadata2.RuleSets);

        var metadata3 = new ValidationOptionsMetadata(["email"]);
        Assert.Equal(["email"], (string[]?)metadata3.RuleSets!);
    }
}