// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <inheritdoc cref="PropertyValidator{T,TProperty}" />
public partial class PropertyValidator<T, TProperty>
{
    /// <summary>
    ///     配置是否启用该属性上的验证特性验证
    /// </summary>
    /// <param name="enabled">是否启用</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> UseAnnotationValidation(bool? enabled)
    {
        SuppressAnnotationValidation = !enabled;

        return this;
    }

    /// <summary>
    ///     配置启用该属性上的验证特性验证
    /// </summary>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> UseAnnotationValidation() => UseAnnotationValidation(true);

    /// <summary>
    ///     配置跳过该属性上的验证特性验证
    /// </summary>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> SkipAnnotationValidation() => UseAnnotationValidation(false);

    /// <summary>
    ///     配置跳过该属性上的验证特性验证
    /// </summary>
    /// <remarks>仅验证自定义规则。</remarks>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> CustomOnly() => UseAnnotationValidation(false);

    /// <summary>
    ///     添加条件验证器
    /// </summary>
    /// <param name="buildConditions">条件构建器配置委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> Conditional(
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
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> Compare(Expression<Func<T, object?>> selector)
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
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> Compare(string propertyName)
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
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> EqualTo(Func<ValidationContext<T>, object?> compareValueAccessor)
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
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> GreaterThanOrEqualTo(
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
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> GreaterThan(Func<ValidationContext<T>, IComparable> compareValueAccessor)
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
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> LessThanOrEqualTo(
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
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> LessThan(Func<ValidationContext<T>, IComparable> compareValueAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValueAccessor);

        return ValidatorProxy<LessThanValidator>(context => [compareValueAccessor(context)]);
    }

    /// <summary>
    ///     添加自定义条件不成立时委托验证器
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> MustUnless(Func<TProperty, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return ValidatorProxy<MustUnlessValidator<TProperty>>(context =>
            [new Func<TProperty, bool>(u => condition(u, context))]);
    }

    /// <summary>
    ///     添加自定义条件成立时委托验证器
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> Must(Func<TProperty, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return ValidatorProxy<MustValidator<TProperty>>(context =>
            [new Func<TProperty, bool>(u => condition(u, context))]);
    }

    /// <summary>
    ///     添加不相等验证器
    /// </summary>
    /// <param name="compareValueAccessor">比较的值访问器</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> NotEqualTo(Func<ValidationContext<T>, object?> compareValueAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValueAccessor);

        return ValidatorProxy<NotEqualToValidator>(context => [compareValueAccessor(context)]);
    }

    /// <summary>
    ///     添加自定义条件成立时委托验证器
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> Predicate(Func<TProperty, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return ValidatorProxy<PredicateValidator<TProperty>>(context =>
            [new Func<TProperty, bool>(u => condition(u, context))]);
    }

    /// <summary>
    ///     添加验证器代理
    /// </summary>
    /// <param name="constructorArgsFactory"><typeparamref name="TValidator" /> 构造函数参数工厂</param>
    /// <param name="valueTransformer">验证前值转换器</param>
    /// <param name="configure">配置验证器实例</param>
    /// <typeparam name="TValidator">
    ///     <see cref="ValidatorBase" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> ValidatorProxy<TValidator>(
        Func<ValidationContext<T>, object?[]?>? constructorArgsFactory = null,
        Func<T, object?>? valueTransformer = null, Action<TValidator>? configure = null)
        where TValidator : ValidatorBase
    {
        // 初始化 ValidatorProxy<T, TValidator> 实例
        var validatorProxy = new ValidatorProxy<T, TValidator>(valueTransformer ?? (instance => GetValue(instance)),
            constructorArgsFactory is null
                ? null
                : instance => constructorArgsFactory(CreateValidationContext(instance)));

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
    public ObjectValidator<T> And() => _objectValidator;

    /// <summary>
    ///     结束当前属性验证器的配置，返回到对象验证器以继续链式操作
    /// </summary>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> Then() => _objectValidator;

    /// <summary>
    ///     结束当前属性验证器的配置，返回到对象验证器以继续链式操作
    /// </summary>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> End() => _objectValidator;

    /// <summary>
    ///     为指定属性配置验证规则
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="ruleSets">规则集</param>
    /// <typeparam name="TOtherProperty">属性类型</typeparam>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TOtherProperty> RuleFor<TOtherProperty>(Expression<Func<T, TOtherProperty?>> selector,
        string?[]? ruleSets = null) => _objectValidator.RuleFor(selector, ruleSets);

    /// <summary>
    ///     为集合类型属性中的每一个元素配置验证规则
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="ruleSets">规则集</param>
    /// <typeparam name="TOtherElement">元素类型</typeparam>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    public CollectionPropertyValidator<T, TOtherElement> RuleForEach<TOtherElement>(
        Expression<Func<T, IEnumerable<TOtherElement>?>> selector, string?[]? ruleSets = null)
        where TOtherElement : class =>
        _objectValidator.RuleForEach(selector, ruleSets);

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
    public List<ValidationResult> ToResults(bool disposeAfterValidation = true) =>
        _objectValidator.ToResults(disposeAfterValidation);

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext" />
    /// </param>
    /// <param name="disposeAfterValidation">是否在验证完成后自动释放当前实例。默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public List<ValidationResult> ToResults(ValidationContext validationContext, bool disposeAfterValidation = true) =>
        _objectValidator.ToResults(validationContext, disposeAfterValidation);
}