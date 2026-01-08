// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <inheritdoc cref="PropertyValidator{T,TProperty,TSelf}" />
public abstract partial class PropertyValidator<T, TProperty, TSelf>
{
    /// <inheritdoc />
    public virtual List<ValidationResult> ToResults(ValidationContext validationContext,
        bool disposeAfterValidation = true) =>
        _objectValidator.ToResults(validationContext, disposeAfterValidation);

    /// <summary>
    ///     配置是否允许空字符串
    /// </summary>
    /// <param name="allowEmptyStrings">是否允许空字符串，默认值为：<c>true</c>。</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf AllowEmptyStrings(bool allowEmptyStrings = true)
    {
        _allowEmptyStrings = allowEmptyStrings;

        return This;
    }

    /// <summary>
    ///     配置是否启用该属性上的验证特性验证
    /// </summary>
    /// <param name="enabled">是否启用</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf UseAnnotationValidation(bool? enabled)
    {
        SuppressAnnotationValidation = !enabled;

        return This;
    }

    /// <summary>
    ///     配置启用该属性上的验证特性验证
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf UseAnnotationValidation() => UseAnnotationValidation(true);

    /// <summary>
    ///     配置跳过该属性上的验证特性验证
    /// </summary>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf SkipAnnotationValidation() => UseAnnotationValidation(false);

    /// <summary>
    ///     配置跳过该属性上的验证特性验证
    /// </summary>
    /// <remarks>仅验证自定义规则。</remarks>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf CustomOnly() => UseAnnotationValidation(false);

    /// <summary>
    ///     添加条件验证器
    /// </summary>
    /// <param name="buildConditions">条件构建器配置委托</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf Conditional(
        Action<ConditionBuilder<TProperty>, ValidationContext<T>> buildConditions)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(buildConditions);

        return ValidatorProxy<ConditionalValidator<TProperty>>(context =>
            [new Action<ConditionBuilder<TProperty>>(u => buildConditions(u, context))]);
    }

    /// <summary>
    ///     添加比较两个属性验证器
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf Compare(Expression<Func<T, object?>> selector)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(selector);

        return ValidatorProxy<CompareValidator<T, TProperty>>(_ => [_selector, selector], instance => instance);
    }

    /// <summary>
    ///     添加比较两个属性验证器
    /// </summary>
    /// <param name="propertyName">其他属性的名称</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf Compare(string propertyName)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        return ValidatorProxy<CompareValidator<T, TProperty>>(_ => [_selector, propertyName], instance => instance);
    }

    /// <summary>
    ///     添加相等验证器
    /// </summary>
    /// <param name="compareValueAccessor">比较的值访问器</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf EqualTo(Func<ValidationContext<T>, object?> compareValueAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValueAccessor);

        return ValidatorProxy<EqualToValidator>(context => [compareValueAccessor(context)]);
    }

    /// <summary>
    ///     添加大于等于验证器
    /// </summary>
    /// <param name="compareValueAccessor">比较的值访问器</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf GreaterThanOrEqualTo(
        Func<ValidationContext<T>, IComparable> compareValueAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValueAccessor);

        return ValidatorProxy<GreaterThanOrEqualToValidator>(context => [compareValueAccessor(context)]);
    }

    /// <summary>
    ///     添加大于验证器
    /// </summary>
    /// <param name="compareValueAccessor">比较的值访问器</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf GreaterThan(
        Func<ValidationContext<T>, IComparable> compareValueAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValueAccessor);

        return ValidatorProxy<GreaterThanValidator>(context => [compareValueAccessor(context)]);
    }

    /// <summary>
    ///     添加小于等于验证器
    /// </summary>
    /// <param name="compareValueAccessor">比较的值访问器</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf LessThanOrEqualTo(
        Func<ValidationContext<T>, IComparable> compareValueAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValueAccessor);

        return ValidatorProxy<LessThanOrEqualToValidator>(context => [compareValueAccessor(context)]);
    }

    /// <summary>
    ///     添加小于验证器
    /// </summary>
    /// <param name="compareValueAccessor">比较的值访问器</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf LessThan(
        Func<ValidationContext<T>, IComparable> compareValueAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValueAccessor);

        return ValidatorProxy<LessThanValidator>(context => [compareValueAccessor(context)]);
    }

    /// <summary>
    ///     添加自定义条件成立时委托验证器
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf Must(Func<TProperty, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return ValidatorProxy<MustValidator<TProperty>>(context =>
            [new Func<TProperty, bool>(u => condition(u, context))]);
    }

    /// <summary>
    ///     添加自定义条件成立时委托验证器
    /// </summary>
    /// <remarks>仅当被验证的值不为 <c>null</c> 时才执行验证逻辑。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf MustIfNotNull(Func<TProperty, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return ValidatorProxy<MustValidator<TProperty>>(context =>
            [new Func<TProperty, bool>(u => u is null || condition(u, context))]);
    }

    /// <summary>
    ///     添加自定义条件成立时委托验证器
    /// </summary>
    /// <remarks>仅当被验证的值不为 <c>null</c>、非空集合、数组和字符串时才执行验证逻辑。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf MustIfNotNullOrEmpty(Func<TProperty, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return ValidatorProxy<MustValidator<TProperty>>(context =>
        [
            new Func<TProperty, bool>(u =>
                u is null || (u.TryGetCount(out var count) && count == 0) || condition(u, context))
        ]);
    }

    /// <summary>
    ///     添加自定义条件成立时委托验证器
    /// </summary>
    /// <param name="enumerable">集合</param>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf MustAny<TElement>(IEnumerable<TElement> enumerable,
        Func<TProperty, TElement, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(condition);

        return ValidatorProxy<MustValidator<TProperty>>(context =>
            [new Func<TProperty, bool>(u => enumerable.Any(x => condition(u, x, context)))]);
    }

    /// <summary>
    ///     添加自定义条件成立时委托验证器
    /// </summary>
    /// <param name="enumerable">集合</param>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf MustAll<TElement>(IEnumerable<TElement> enumerable,
        Func<TProperty, TElement, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(condition);

        return ValidatorProxy<MustValidator<TProperty>>(context =>
            [new Func<TProperty, bool>(u => enumerable.All(x => condition(u, x, context)))]);
    }

    /// <summary>
    ///     添加不相等验证器
    /// </summary>
    /// <param name="compareValueAccessor">比较的值访问器</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf NotEqualTo(Func<ValidationContext<T>, object?> compareValueAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValueAccessor);

        return ValidatorProxy<NotEqualToValidator>(context => [compareValueAccessor(context)]);
    }

    /// <summary>
    ///     添加验证器代理
    /// </summary>
    /// <param name="constructorArgsFactory"><typeparamref name="TValidator" /> 构造函数参数工厂</param>
    /// <param name="validatedObjectProvider">被验证对象的提供器</param>
    /// <param name="configure">配置验证器实例</param>
    /// <typeparam name="TValidator">
    ///     <see cref="ValidatorBase" />
    /// </typeparam>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf ValidatorProxy<TValidator>(
        Func<ValidationContext<T>, object?[]?>? constructorArgsFactory = null,
        Func<T, object?>? validatedObjectProvider = null, Action<TValidator>? configure = null)
        where TValidator : ValidatorBase
    {
        // 初始化 ValidatorProxy<T, TValidator> 实例
        var validatorProxy = new ValidatorProxy<T, TValidator>(
            validatedObjectProvider ?? (instance => GetValueForValidation(instance)),
            constructorArgsFactory is null
                ? null
                : (_, context) => constructorArgsFactory(context));

        // 空检查
        if (configure is not null)
        {
            validatorProxy.Configure(configure);
        }

        return AddValidator(validatorProxy);
    }

    /// <summary>
    ///     结束当前属性验证器的配置，返回到对象验证器以继续链式操作
    /// </summary>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public virtual ObjectValidator<T> And() => _objectValidator;

    /// <summary>
    ///     结束当前属性验证器的配置，返回到对象验证器以继续链式操作
    /// </summary>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public virtual ObjectValidator<T> Then() => _objectValidator;

    /// <summary>
    ///     结束当前属性验证器的配置，返回到对象验证器以继续链式操作
    /// </summary>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public virtual ObjectValidator<T> End() => _objectValidator;

    /// <summary>
    ///     为指定属性配置验证规则
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="configure">自定义配置委托</param>
    /// <typeparam name="TProperty1">属性类型</typeparam>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty1}" />
    /// </returns>
    public PropertyValidator<T, TProperty1> RuleFor<TProperty1>(Expression<Func<T, TProperty1?>> selector,
        Action<PropertyValidator<T, TProperty1>>? configure = null) =>
        _objectValidator.RuleFor(selector, configure);

    /// <summary>
    ///     为集合类型属性中的每一个元素配置验证规则
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="configure">自定义配置委托</param>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    public CollectionPropertyValidator<T, TElement> RuleForCollection<TElement>(
        Expression<Func<T, IEnumerable<TElement?>?>> selector,
        Action<CollectionPropertyValidator<T, TElement>>? configure = null) =>
        _objectValidator.RuleForCollection(selector, configure);

    /// <summary>
    ///     在指定规则集上下文中为指定属性配置验证规则
    /// </summary>
    /// <param name="ruleSet">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> RuleSet(string? ruleSet, Action setAction) =>
        _objectValidator.RuleSet(ruleSet, setAction);

    /// <summary>
    ///     在指定规则集上下文中为指定属性配置验证规则
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> RuleSet(string?[]? ruleSets, Action setAction) =>
        _objectValidator.RuleSet(ruleSets, setAction);

    /// <summary>
    ///     在指定规则集上下文中为指定属性配置验证规则
    /// </summary>
    /// <param name="ruleSet">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> RuleSet(string? ruleSet, Action<ObjectValidator<T>> setAction) =>
        _objectValidator.RuleSet(ruleSet, setAction);

    /// <summary>
    ///     在指定规则集上下文中为指定属性配置验证规则
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> RuleSet(string?[]? ruleSets, Action<ObjectValidator<T>> setAction) =>
        _objectValidator.RuleSet(ruleSets, setAction);

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="disposeAfterValidation">是否在验证完成后自动释放当前实例。默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public virtual List<ValidationResult> ToResults(bool disposeAfterValidation = true) =>
        _objectValidator.ToResults(disposeAfterValidation);
}