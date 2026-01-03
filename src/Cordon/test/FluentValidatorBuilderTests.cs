// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class FluentValidatorBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var builder = new TestValidatorBuilder<int>();
        Assert.NotNull(builder);
        Assert.Empty(builder.Items);

        var builder2 = new TestValidatorBuilder<int>(new Dictionary<object, object?>());
        Assert.NotNull(builder2);
        Assert.NotNull(builder2.Items);
        Assert.Empty(builder2.Items);

        var builder3 = new FluentValidatorBuilder<int>();
        Assert.NotNull(builder3);
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var builder = new FluentValidatorBuilder<int>();
        var validators = builder.Build(c => c.Min(1).Max(20));
        Assert.Equal(2, validators.Count);
        Assert.Equal(2, builder.Validators.Count);
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