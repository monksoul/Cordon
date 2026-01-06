// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ConditionBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        Assert.NotNull(conditionBuilder._conditionalRules);
        Assert.Empty(conditionBuilder._conditionalRules);
        Assert.Null(conditionBuilder.defaultRules);
    }

    [Fact]
    public void When_Invalid_Parameters()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        Assert.Throws<ArgumentNullException>(() => conditionBuilder.When(null!).Then(null!));
        Assert.Throws<ArgumentNullException>(() => conditionBuilder.When(u => u > 10).Then(null!));
    }

    [Fact]
    public void When_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.When(u => u > 10).Then(b => b.Min(10));
        Assert.Single(conditionBuilder._conditionalRules);

        conditionBuilder.When(u => u > 100).Then(b => b.Min(50));
        Assert.Equal(2, conditionBuilder._conditionalRules.Count);
    }

    [Fact]
    public void WhenMatch_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.WhenMatch(u => u > 10, u => u.Min(10));
        Assert.Single(conditionBuilder._conditionalRules);

        var conditionBuilder2 = new ConditionBuilder<int>();
        conditionBuilder2.WhenMatch(u => u > 10, "验证失败");
        Assert.Single(conditionBuilder2._conditionalRules);

        var conditionBuilder3 = new ConditionBuilder<int>();
        conditionBuilder3.WhenMatch(u => u > 10, typeof(TestValidationMessages), "TestValidator_ValidationError");
        Assert.Single(conditionBuilder3._conditionalRules);
    }

    [Fact]
    public void Otherwise_Invalid_Parameters()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        Assert.Throws<ArgumentNullException>(() => conditionBuilder.Otherwise(null!));
    }

    [Fact]
    public void Otherwise_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.When(u => u > 10).Then(b => b.Min(10)).Otherwise(b => b.Min(50));
        Assert.NotNull(conditionBuilder.defaultRules);
    }

    [Fact]
    public void OtherwiseErrorMessage_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.When(u => u > 10).ThenMessage("错误消息").OtherwiseMessage("默认错误消息");
        Assert.NotNull(conditionBuilder.defaultRules);
        Assert.Equal(typeof(FailureValidator), conditionBuilder.defaultRules[0].GetType());

        var conditionBuilder2 = new ConditionBuilder<int>();
        conditionBuilder2.When(u => u > 10).ThenMessage("错误消息")
            .OtherwiseMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");
        Assert.NotNull(conditionBuilder2.defaultRules);
        Assert.Equal(typeof(FailureValidator), conditionBuilder2.defaultRules[0].GetType());
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.When(u => u > 10).Then(b => b.Min(10)).Otherwise(b => b.Min(50));

        var result = conditionBuilder.Build();
        Assert.NotNull(result.ConditionalRules);
        Assert.NotNull(result.DefaultRules);
    }
}