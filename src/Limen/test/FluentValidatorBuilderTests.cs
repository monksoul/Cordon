// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class FluentValidatorBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var builder = new TestValidatorBuilder<int>();
        Assert.NotNull(builder);
        Assert.Null(builder._items);

        var builder2 = new TestValidatorBuilder<int>(new Dictionary<object, object?>());
        Assert.NotNull(builder2);
        Assert.NotNull(builder2._items);
        Assert.Empty(builder2._items);
    }

    public class TestValidatorBuilder<T> : FluentValidatorBuilder<T, TestValidatorBuilder<T>>
    {
        public TestValidatorBuilder()
        {
        }

        public TestValidatorBuilder(IDictionary<object, object?>? items)
            : base(items)
        {
        }
    }
}