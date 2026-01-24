// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

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
        Assert.NotNull(builder._conditionBuilder);
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

        Assert.Single(builder._conditionalRules);
        Assert.Equal(2, builder._conditionalRules.First().Validators.Count);
    }

    [Fact]
    public void ThenErrorMessage_ReturnOK()
    {
        var builder =
            new ConditionThenBuilder<int>(new ConditionBuilder<int>(), u => u > 10).ThenMessage("错误信息1")
                .When(u => u < 10).ThenMessage("错误信息2");

        Assert.Equal(2, builder._conditionalRules.Count);
        Assert.Equal(typeof(FailureValidator), builder._conditionalRules.First().Validators[0].GetType());

        var builder2 =
            new ConditionThenBuilder<int>(new ConditionBuilder<int>(), u => u > 10)
                .ThenMessage(typeof(TestValidationMessages), "TestValidator_ValidationError")
                .When(u => u < 10).ThenMessage(typeof(TestValidationMessages), "TestValidator_ValidationError2");

        Assert.Equal(2, builder._conditionalRules.Count);
        Assert.Equal(typeof(FailureValidator), builder2._conditionalRules.First().Validators[0].GetType());
    }
}