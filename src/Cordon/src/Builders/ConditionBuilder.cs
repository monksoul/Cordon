// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     条件规则
/// </summary>
/// <param name="Condition">条件委托</param>
/// <param name="Validators">验证器列表</param>
/// <typeparam name="T">对象类型</typeparam>
internal record ConditionRule<T>(Func<T, bool> Condition, IReadOnlyList<ValidatorBase> Validators);

/// <summary>
///     <see cref="ConditionBuilder{T}" /> 构建结果
/// </summary>
/// <param name="ConditionalRules">条件规则集合</param>
/// <param name="DefaultRules">默认验证规则</param>
/// <typeparam name="T">对象类型</typeparam>
internal record ConditionResult<T>(
    IReadOnlyList<ConditionRule<T>> ConditionalRules,
    IReadOnlyList<ValidatorBase>? DefaultRules);

/// <summary>
///     条件验证构建器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ConditionBuilder<T>
{
    /// <summary>
    ///     条件规则集合
    /// </summary>
    internal readonly List<ConditionRule<T>> _conditionalRules = [];

    /// <summary>
    ///     默认验证规则
    /// </summary>
    /// <remarks>当所有条件均不满足时使用。</remarks>
    internal IReadOnlyList<ValidatorBase>? defaultRules;

    /// <summary>
    ///     定义满足指定的条件委托
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="ConditionThenBuilder{T}" />
    /// </returns>
    public ConditionThenBuilder<T> When(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return new ConditionThenBuilder<T>(this, condition);
    }

    /// <summary>
    ///     定义满足指定条件时执行的验证规则
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <param name="thenConfigure">验证器配置委托</param>
    /// <returns>
    ///     <see cref="ConditionThenBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> WhenMatch(Func<T, bool> condition, Action<FluentValidatorBuilder<T>> thenConfigure) =>
        When(condition).Then(thenConfigure);

    /// <summary>
    ///     定义满足指定条件时返回指定的错误信息
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <param name="errorMessage">错误信息</param>
    /// <returns>
    ///     <see cref="ConditionThenBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> WhenMatch(Func<T, bool> condition, string? errorMessage) =>
        When(condition).ThenMessage(errorMessage);

    /// <summary>
    ///     定义满足指定条件时返回指定的错误信息
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <param name="resourceType">错误信息资源类型</param>
    /// <param name="resourceName">错误信息资源名称</param>
    /// <returns>
    ///     <see cref="ConditionThenBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> WhenMatch(Func<T, bool> condition,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type resourceType, string resourceName) => When(condition).ThenMessage(resourceType, resourceName);

    /// <summary>
    ///     配置默认验证规则
    /// </summary>
    /// <remarks>当所有条件均不满足时使用。</remarks>
    /// <param name="configure">验证器配置委托</param>
    /// <returns>
    ///     <see cref="ConditionBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> Otherwise(Action<FluentValidatorBuilder<T>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        defaultRules = new FluentValidatorBuilder<T>().Build(configure);

        return this;
    }

    /// <summary>
    ///     配置默认错误信息
    /// </summary>
    /// <remarks>当所有条件均不满足时直接返回该消息，不执行任何验证逻辑。</remarks>
    /// <param name="errorMessage">错误信息</param>
    /// <returns>
    ///     <see cref="ConditionBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> OtherwiseMessage(string? errorMessage)
    {
        defaultRules = [new FailureValidator().WithMessage(errorMessage)];

        return this;
    }

    /// <summary>
    ///     配置默认错误信息
    /// </summary>
    /// <remarks>当所有条件均不满足时直接返回该消息，不执行任何验证逻辑。</remarks>
    /// <param name="resourceType">错误信息资源类型</param>
    /// <param name="resourceName">错误信息资源名称</param>
    /// <returns>
    ///     <see cref="ConditionBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> OtherwiseMessage([DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicProperties |
            DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type resourceType, string resourceName)
    {
        defaultRules = [new FailureValidator().WithMessage(resourceType, resourceName)];

        return this;
    }

    /// <summary>
    ///     构建
    /// </summary>
    /// <param name="buildConditions">条件验证构建器配置委托</param>
    /// <returns>
    ///     <see cref="ConditionResult{T}" />
    /// </returns>
    internal ConditionResult<T> Build(Action<ConditionBuilder<T>>? buildConditions = null)
    {
        // 调用条件验证构建器配置委托
        buildConditions?.Invoke(this);

        return new ConditionResult<T>(_conditionalRules, defaultRules);
    }
}

/// <summary>
///     Then 条件中间构建器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public sealed class ConditionThenBuilder<T>
{
    /// <summary>
    ///     条件委托
    /// </summary>
    internal readonly Func<T, bool> _condition;

    /// <inheritdoc cref="ConditionBuilder{T}" />
    internal readonly ConditionBuilder<T> _conditionBuilder;

    /// <summary>
    ///     构造函数
    /// </summary>
    /// <param name="conditionBuilder">
    ///     <see cref="ConditionBuilder{T}" />
    /// </param>
    /// <param name="condition">条件委托</param>
    internal ConditionThenBuilder(ConditionBuilder<T> conditionBuilder, Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(conditionBuilder);
        ArgumentNullException.ThrowIfNull(condition);

        _conditionBuilder = conditionBuilder;
        _condition = condition;
    }

    /// <summary>
    ///     配置满足条件时执行的验证规则
    /// </summary>
    /// <param name="configure">验证器配置委托</param>
    /// <returns>
    ///     <see cref="ConditionBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> Then(Action<FluentValidatorBuilder<T>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        _conditionBuilder._conditionalRules.Add(
            new ConditionRule<T>(_condition, new FluentValidatorBuilder<T>().Build(configure)));

        return _conditionBuilder;
    }

    /// <summary>
    ///     配置满足条件时直接返回指定的错误信息
    /// </summary>
    /// <remarks>不执行任何验证逻辑，仅用于输出错误提示。</remarks>
    /// <param name="errorMessage">错误信息</param>
    /// <returns>
    ///     <see cref="ConditionBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> ThenMessage(string? errorMessage)
    {
        _conditionBuilder._conditionalRules.Add(new ConditionRule<T>(_condition,
            [new FailureValidator().WithMessage(errorMessage)]));

        return _conditionBuilder;
    }

    /// <summary>
    ///     配置满足条件时直接返回指定的错误信息
    /// </summary>
    /// <remarks>不执行任何验证逻辑，仅用于输出错误提示。</remarks>
    /// <param name="resourceType">错误信息资源类型</param>
    /// <param name="resourceName">错误信息资源名称</param>
    /// <returns>
    ///     <see cref="ConditionBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> ThenMessage(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type resourceType, string resourceName)
    {
        _conditionBuilder._conditionalRules.Add(new ConditionRule<T>(_condition,
            [new FailureValidator().WithMessage(resourceType, resourceName)]));

        return _conditionBuilder;
    }
}