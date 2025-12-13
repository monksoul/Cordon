// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ConditionBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        Assert.NotNull(conditionBuilder._conditions);
        Assert.Empty(conditionBuilder._conditions);
        Assert.Null(conditionBuilder._defaultValidators);
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
        Assert.Single(conditionBuilder._conditions);

        conditionBuilder.When(u => u > 100).Then(b => b.Min(50));
        Assert.Equal(2, conditionBuilder._conditions.Count);
    }

    [Fact]
    public void Unless_Invalid_Parameters()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        Assert.Throws<ArgumentNullException>(() => conditionBuilder.Unless(null!).Then(null!));
        Assert.Throws<ArgumentNullException>(() => conditionBuilder.Unless(u => u > 10).Then(null!));
    }

    [Fact]
    public void Unless_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.Unless(u => u <= 10).Then(b => b.Min(10));
        Assert.Single(conditionBuilder._conditions);

        conditionBuilder.Unless(u => u <= 100).Then(b => b.Min(50));
        Assert.Equal(2, conditionBuilder._conditions.Count);
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
        Assert.NotNull(conditionBuilder._defaultValidators);
    }

    [Fact]
    public void BuildValidators_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ConditionBuilder<int>.BuildValidators(null!));

    [Fact]
    public void BuildValidators_ReturnOK()
    {
        var validators = ConditionBuilder<string>.BuildValidators(b => b.Required().MinLength(30));
        Assert.NotNull(validators);
        Assert.Equal(2, validators.Count);
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.When(u => u > 10).Then(b => b.Min(10)).Otherwise(b => b.Min(50));

        var result = conditionBuilder.Build();
        Assert.NotNull(result.Conditions);
        Assert.NotNull(result.DefaultValidators);
    }
}