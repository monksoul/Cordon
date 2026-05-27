// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

[CollectionDefinition("SensitiveWordTests", DisableParallelization = true)]
public class SensitiveWordTestCollection : ICollectionFixture<SensitiveWordFixture>;

public class SensitiveWordFixture : IDisposable
{
    public SensitiveWordFixture() => SensitiveWordSanitizerFactory.Clear();

    /// <inheritdoc />
    public void Dispose() => SensitiveWordSanitizerFactory.Clear();
}