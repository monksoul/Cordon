// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     链式条件验证器构建器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class FluentConditionBuilder<T> : FluentValidatorBuilder<T, FluentConditionBuilder<T>>;

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
    internal readonly ConditionBuilder<T> _parent;

    /// <summary>
    ///     构造函数
    /// </summary>
    /// <param name="parent">父条件构建器</param>
    /// <param name="condition">条件委托</param>
    internal ConditionThenBuilder(ConditionBuilder<T> parent, Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(parent);
        ArgumentNullException.ThrowIfNull(condition);

        _parent = parent;
        _condition = condition;
    }

    /// <summary>
    ///     配置满足条件时执行的验证规则
    /// </summary>
    /// <param name="configure">验证器配置委托</param>
    /// <returns>
    ///     <see cref="ConditionBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> Then(Action<FluentConditionBuilder<T>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 构建验证器集合
        var validators = ConditionBuilder<T>.BuildValidators(configure);

        // 将条件和验证器列表添加到父条件构建器
        _parent._conditions.Add((_condition, validators));

        return _parent;
    }
}

/// <summary>
///     条件验证构建器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ConditionBuilder<T>
{
    /// <summary>
    ///     条件与对应验证器列表
    /// </summary>
    internal readonly List<(Func<T, bool> Condition, IReadOnlyList<ValidatorBase> Validators)> _conditions;

    /// <summary>
    ///     默认验证器集合
    /// </summary>
    /// <remarks>当无条件匹配时使用。</remarks>
    internal IReadOnlyList<ValidatorBase>? _defaultValidators;

    /// <summary>
    ///     <inheritdoc cref="ConditionBuilder{T}" />
    /// </summary>
    internal ConditionBuilder() => _conditions = [];

    /// <summary>
    ///     定义满足指定条件时执行的验证规则
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
    ///     定义不满足指定条件时执行的验证规则
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="ConditionThenBuilder{T}" />
    /// </returns>
    public ConditionThenBuilder<T> Unless(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return new ConditionThenBuilder<T>(this, u => !condition(u));
    }

    /// <summary>
    ///     配置默认验证规则（当所有条件均不满足时使用）
    /// </summary>
    /// <param name="configure">验证器配置委托</param>
    /// <returns>
    ///     <see cref="ConditionBuilder{T}" />
    /// </returns>
    public ConditionBuilder<T> Otherwise(Action<FluentConditionBuilder<T>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        _defaultValidators = BuildValidators(configure);

        return this;
    }

    /// <summary>
    ///     构建验证器集合
    /// </summary>
    /// <param name="configure">验证器配置委托</param>
    /// <returns>
    ///     <see cref="IReadOnlyList{T}" />
    /// </returns>
    internal static IReadOnlyList<ValidatorBase> BuildValidators(Action<FluentConditionBuilder<T>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 初始化 FluentConditionBuilder<T> 实例
        var fluentConditionBuilder = new FluentConditionBuilder<T>();

        // 调用自定义配置委托
        configure.Invoke(fluentConditionBuilder);

        return fluentConditionBuilder.Build();
    }

    /// <summary>
    ///     构建最终的条件与默认验证器集合
    /// </summary>
    /// <returns>
    ///     <see cref="Tuple{T1,T2}" />
    /// </returns>
    internal (List<(Func<T, bool> Condition, IReadOnlyList<ValidatorBase> Validators)> Conditions,
        IReadOnlyList<ValidatorBase>? DefaultValidators) Build() => (_conditions, _defaultValidators);
}