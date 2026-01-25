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
        Assert.Null(conditionBuilder._defaultRules);
    }

    [Fact]
    public void When_Invalid_Parameters()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        Assert.Throws<ArgumentNullException>(() =>
            conditionBuilder.When(null!).Then((Action<FluentValidatorBuilder<int>>)null!));
        Assert.Throws<ArgumentNullException>(() => conditionBuilder.When(u => u > 10).Then((ValidatorBase[])null!));
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
    public void Otherwise_Invalid_Parameters()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        Assert.Throws<ArgumentNullException>(() =>
            conditionBuilder.Otherwise((Action<FluentValidatorBuilder<int>>)null!));
        Assert.Throws<ArgumentNullException>(() => conditionBuilder.Otherwise((ValidatorBase[])null!));
    }

    [Fact]
    public void Otherwise_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.When(u => u > 10).Then(b => b.Min(10)).Otherwise(b => b.Min(50));
        Assert.NotNull(conditionBuilder._defaultRules);
        Assert.Single(conditionBuilder._defaultRules);

        var addValidator = conditionBuilder._defaultRules[0] as CompositeValidator<int>;
        Assert.NotNull(addValidator);
        Assert.Equal(CompositeMode.FailFast, addValidator.Mode);

        var conditionBuilder2 = new ConditionBuilder<int>();
        conditionBuilder2.When(u => u > 10).Then(b => b.Min(10)).Otherwise([new MinValidator(50)]);
        Assert.NotNull(conditionBuilder2._defaultRules);
        Assert.Single(conditionBuilder2._defaultRules);

        var addValidator2 = conditionBuilder2._defaultRules[0] as CompositeValidator<int>;
        Assert.NotNull(addValidator2);
        Assert.Equal(CompositeMode.FailFast, addValidator2.Mode);
    }

    [Fact]
    public void OtherwiseErrorMessage_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.When(u => u > 10).ThenMessage("错误信息").OtherwiseMessage("默认错误信息");
        Assert.NotNull(conditionBuilder._defaultRules);
        Assert.Equal(typeof(FailureValidator), conditionBuilder._defaultRules[0].GetType());

        var conditionBuilder2 = new ConditionBuilder<int>();
        conditionBuilder2.When(u => u > 10).ThenMessage("错误信息")
            .OtherwiseMessage(typeof(TestValidationMessages), "TestValidator_ValidationError");
        Assert.NotNull(conditionBuilder2._defaultRules);
        Assert.Equal(typeof(FailureValidator), conditionBuilder2._defaultRules[0].GetType());
    }

    [Fact]
    public void Clear_ReturnOK()
    {
        var conditionBuilder = new ConditionBuilder<int>();
        conditionBuilder.When(u => u > 10).Then(b => b.Min(10)).Otherwise(b => b.Min(50));
        Assert.Single(conditionBuilder._conditionalRules);
        Assert.NotNull(conditionBuilder._defaultRules);
        Assert.Single(conditionBuilder._defaultRules);

        conditionBuilder.Clear();
        Assert.Empty(conditionBuilder._conditionalRules);
        Assert.Null(conditionBuilder._defaultRules);
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