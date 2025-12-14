// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ConditionThenBuilderTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new ConditionThenBuilder<int>(null!, null!));
        Assert.Throws<ArgumentNullException>(() => new ConditionThenBuilder<int>(new ConditionBuilder<int>(), null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var builder = new ConditionThenBuilder<int>(new ConditionBuilder<int>(), u => u > 10);
        Assert.NotNull(builder._condition);
        Assert.False(builder._condition(9));
        Assert.True(builder._condition(11));
        Assert.NotNull(builder._parent);
    }

    [Fact]
    public void Then_Invalid_Parameters()
    {
        var builder = new ConditionThenBuilder<int>(new ConditionBuilder<int>(), u => u > 10);
        Assert.Throws<ArgumentNullException>(() => builder.Then(null!));
    }

    [Fact]
    public void Then_ReturnOK()
    {
        var builder =
            new ConditionThenBuilder<int>(new ConditionBuilder<int>(), u => u > 10).Then(u => u.Min(10).Max(100));

        Assert.Single(builder._conditions);
        Assert.Equal(2, builder._conditions.First().Validators.Count);
    }
}