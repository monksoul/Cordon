// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     属性验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TProperty">属性类型</typeparam>
public partial class PropertyValidator<T, TProperty> :
    FluentValidatorBuilder<TProperty, PropertyValidator<T, TProperty>>, IObjectValidator<T>, IMemberPathRepairable
{
    /// <inheritdoc cref="PropertyAnnotationValidator{T,TProperty}" />
    internal readonly PropertyAnnotationValidator<T, TProperty> _annotationValidator;

    /// <inheritdoc cref="ObjectValidator{T}" />
    internal readonly ObjectValidator<T> _objectValidator;

    /// <summary>
    ///     属性选择器
    /// </summary>
    internal readonly Expression<Func<T, TProperty?>> _selector;

    /// <summary>
    ///     属性验证前的预处理器
    /// </summary>
    /// <remarks>该预处理器仅用于验证，不会修改原始属性的值。</remarks>
    internal Func<TProperty, TProperty>? _preProcessor;

    /// <inheritdoc cref="ObjectValidator{T}" />
    /// <remarks>属性级别对象验证器。</remarks>
    internal ObjectValidator<TProperty>? _propertyValidator;

    /// <summary>
    ///     <inheritdoc cref="PropertyValidator{T,TProperty}" />
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="objectValidator">
    ///     <see cref="ObjectValidator{T}" />
    /// </param>
    internal PropertyValidator(Expression<Func<T, TProperty?>> selector, ObjectValidator<T> objectValidator)
        : base(null, (objectValidator ?? throw new ArgumentNullException(nameof(objectValidator)))._items)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(objectValidator);

        _selector = selector;
        _objectValidator = objectValidator;

        // 初始化 PropertyAnnotationValidator 实例
        _annotationValidator = new PropertyAnnotationValidator<T, TProperty>(selector!, null, objectValidator._items);

        // 同步 IServiceProvider 委托（已在 RuleFor 创建时同步）
        // InitializeServiceProvider(objectValidator._serviceProvider);
    }

    /// <summary>
    ///     规则集
    /// </summary>
    internal string?[]? RuleSets { get; init; }

    /// <summary>
    ///     是否禁用属性验证特性验证
    /// </summary>
    /// <remarks>
    ///     默认值为：<c>null</c>。当此值为 <c>null</c> 时，是否启用注解验证将由 <see cref="ObjectValidator{T}.Options" /> 中的
    ///     <see cref="ValidatorOptions.SuppressAnnotationValidation" /> 配置项决定。
    /// </remarks>
    internal bool? SuppressAnnotationValidation { get; private set; }

    /// <summary>
    ///     显示名称
    /// </summary>
    internal string? DisplayName { get; private set; }

    /// <summary>
    ///     成员名称
    /// </summary>
    internal string? MemberName { get; private set; }

    /// <summary>
    ///     验证条件
    /// </summary>
    /// <remarks>当条件满足时才进行验证。</remarks>
    internal Func<TProperty, ValidationContext<T>, bool>? WhenCondition { get; private set; }

    /// <summary>
    ///     逆向验证条件
    /// </summary>
    /// <remarks>当条件不满足时才进行验证。</remarks>
    internal Func<TProperty, ValidationContext<T>, bool>? UnlessCondition { get; private set; }

    /// <inheritdoc />
    void IMemberPathRepairable.RepairMemberPaths() => RepairMemberPaths();

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual bool IsValid(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 检查是否应该对该属性执行验证
        if (!ShouldValidate(instance, ruleSets))
        {
            return true;
        }

        // 检查是否启用属性验证特性验证
        if (ShouldRunAnnotationValidation() && !_annotationValidator.IsValid(instance))
        {
            return false;
        }

        // 获取用于验证的属性值
        var propertyValue = GetValueForValidation(instance);

        // 检查是否设置了属性级别对象验证器
        if (propertyValue is not null && _propertyValidator is not null &&
            !_propertyValidator.IsValid(propertyValue, ruleSets))
        {
            return false;
        }

        return Validators.All(u => u.IsValid(GetValidatedObject(instance, u, propertyValue)));
    }

    /// <inheritdoc />
    public virtual List<ValidationResult>? GetValidationResults(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 检查是否应该对该属性执行验证
        if (!ShouldValidate(instance, ruleSets))
        {
            return null;
        }

        // 获取显示名称、属性路径和初始化验证结果集合
        var (displayName, memberPath) = (GetDisplayName(), GetEffectiveMemberName());
        var validationResults = new List<ValidationResult>();

        // 检查是否启用属性验证特性验证
        if (ShouldRunAnnotationValidation())
        {
            validationResults.AddRange(_annotationValidator.GetValidationResults(instance, displayName, [memberPath]) ??
                                       []);
        }

        // 获取用于验证的属性值
        var propertyValue = GetValueForValidation(instance);

        // 检查是否设置了属性级别对象验证器
        if (propertyValue is not null && _propertyValidator is not null)
        {
            validationResults.AddRange(_propertyValidator.GetValidationResults(propertyValue, ruleSets) ?? []);
        }

        // 获取所有验证器验证结果集合
        validationResults.AddRange(Validators.SelectMany(u =>
            u.GetValidationResults(GetValidatedObject(instance, u, propertyValue), displayName, [memberPath]) ?? []));

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public virtual void Validate(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 检查是否应该对该属性执行验证
        if (!ShouldValidate(instance, ruleSets))
        {
            return;
        }

        // 获取显示名称和属性路径
        var (displayName, memberPath) = (GetDisplayName(), GetEffectiveMemberName());

        // 检查是否启用属性验证特性验证
        if (ShouldRunAnnotationValidation())
        {
            _annotationValidator.Validate(instance, displayName, [memberPath]);
        }

        // 获取用于验证的属性值
        var propertyValue = GetValueForValidation(instance);

        // 检查是否设置了属性级别对象验证器
        if (propertyValue is not null && _propertyValidator is not null)
        {
            _propertyValidator.Validate(propertyValue, ruleSets);
        }

        // 遍历验证器集合
        foreach (var validator in Validators)
        {
            validator.Validate(GetValidatedObject(instance, validator, propertyValue), displayName, [memberPath]);
        }
    }

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <inheritdoc />
    bool IObjectValidator.IsValid(object? instance, string?[]? ruleSets) => IsValid((T?)instance, ruleSets);

    /// <inheritdoc />
    List<ValidationResult>? IObjectValidator.GetValidationResults(object? instance, string?[]? ruleSets) =>
        GetValidationResults((T?)instance, ruleSets);

    /// <inheritdoc />
    void IObjectValidator.Validate(object? instance, string?[]? ruleSets) => Validate((T?)instance, ruleSets);

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal new void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 同步基类 IServiceProvider 委托
        base.InitializeServiceProvider(serviceProvider);

        // 同步 _annotationValidator 实例 IServiceProvider 委托
        _annotationValidator.InitializeServiceProvider(serviceProvider);

        // 同步 _propertyValidator 实例 IServiceProvider 委托
        _propertyValidator?.InitializeServiceProvider(serviceProvider);
    }

    /// <summary>
    ///     设置属性级别对象验证器
    /// </summary>
    /// <param name="validatorFactory">
    ///     <see cref="ObjectValidator{T}" /> 工厂委托
    /// </param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public PropertyValidator<T, TProperty> SetValidator(
        Func<string?[]?, IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<TProperty>?> validatorFactory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorFactory);

        // 空检查
        if (_propertyValidator is not null)
        {
            throw new InvalidOperationException(
                $"An object validator has already been assigned to this property. Only one object validator is allowed per property. To define nested rules, use `{nameof(ChildRules)}` within a single validator.");
        }

        // 调用工厂方法，传入当前 RuleSets、_items 和 Options
        _propertyValidator = validatorFactory(RuleSets, _objectValidator._items, _objectValidator.Options);

        // 空检查
        if (_propertyValidator is null)
        {
            return this;
        }

        // 继承当前规则集
        _propertyValidator.SetInheritedRuleSetsIfNotSet(RuleSets);

        // 同步 IServiceProvider 委托
        _propertyValidator.InitializeServiceProvider(_serviceProvider);

        // 修复整个子验证器树的成员路径
        RepairMemberPaths();

        return this;
    }

    /// <summary>
    ///     设置属性级别对象验证器
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ObjectValidator{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public PropertyValidator<T, TProperty> SetValidator(ObjectValidator<TProperty>? validator) =>
        SetValidator((_, _, _) => validator);

    /// <summary>
    ///     为子属性继续配置规则
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public PropertyValidator<T, TProperty> ChildRules(Action<ObjectValidator<TProperty>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 空检查
        if (_propertyValidator is not null)
        {
            throw new InvalidOperationException(
                $"An object validator has already been assigned to this property. `{nameof(ChildRules)}` cannot be applied after `{nameof(SetValidator)}` or another `{nameof(ChildRules)}` call.");
        }

        return SetValidator((ruleSets, items, options) =>
        {
            // 初始化属性级别对象验证器实例
            var propertyValidator =
                new ObjectValidator<TProperty>(items) { InheritedRuleSets = ruleSets }.ConfigureOptions(options);

            // 调用自定义配置委托
            configure(propertyValidator);

            return propertyValidator;
        });
    }

    /// <summary>
    ///     为集合元素继续配置规则
    /// </summary>
    /// <remarks>建议优先使用 <see cref="ObjectValidator{T}.RuleForEach" />。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public CollectionPropertyValidator<T, TElement> Each<TElement>(Action<ObjectValidator<TElement>> configure)
        where TElement : class
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 检查属性类型是否兼容 IEnumerable<TElement>
        if (!typeof(IEnumerable<TElement>).IsAssignableFrom(typeof(TProperty)))
        {
            throw new InvalidOperationException(
                $"The property '{GetMemberPath()}' does not implement IEnumerable<{typeof(TElement).Name}>. Use {nameof(ObjectValidator<T>.RuleForEach)} instead, or ensure the property type implements IEnumerable<{typeof(TElement).Name}>.");
        }

        // 强制 Each 方法必须在 ChildRules/SetValidator/When/Unless 方法前调用
        if (_propertyValidator is not null || WhenCondition is not null || UnlessCondition is not null)
        {
            throw new InvalidOperationException(
                $".{nameof(Each)}() must be called immediately after {nameof(ObjectValidator<T>.RuleFor)}(). Do not call {nameof(ChildRules)}, {nameof(SetValidator)}, {nameof(When)}, or {nameof(Unless)} before {nameof(Each)}. To validate the entire collection, use {nameof(ObjectValidator<T>.RuleForEach)}() instead.");
        }

        // 从父对象验证器中移除当前实例
        _objectValidator.Validators.Remove(this);

        // 重建属性选择器为 Expression<Func<T, IEnumerable<TElement>?>> 类型
        var collectionSelector =
            Expression.Lambda<Func<T, IEnumerable<TElement>?>>(_selector.Body, _selector.Parameters);

        // 创建新的 CollectionPropertyValidator 实例
        var collectionValidator = new CollectionPropertyValidator<T, TElement>(collectionSelector, _objectValidator)
        {
            RuleSets = RuleSets,
            SuppressAnnotationValidation = SuppressAnnotationValidation,
            DisplayName = DisplayName,
            MemberName = MemberName
        };

        // 同步已设置的验证器
        collectionValidator.AddValidators(Validators);

        // 为集合元素继续配置规则
        collectionValidator.ChildRules(configure);

        // 将该实例添加到父对象验证器中
        _objectValidator.Validators.Add(collectionValidator);

        return collectionValidator;
    }

    /// <summary>
    ///     设置验证条件
    /// </summary>
    /// <remarks>当条件满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> When(Func<TProperty, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        WhenCondition = (p, _) => condition(p);

        return this;
    }

    /// <summary>
    ///     设置逆向验证条件
    /// </summary>
    /// <remarks>当条件不满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> Unless(Func<TProperty, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        UnlessCondition = (p, _) => condition(p);

        return this;
    }

    /// <summary>
    ///     设置验证条件
    /// </summary>
    /// <remarks>当条件满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> When(Func<TProperty, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        WhenCondition = condition;

        return this;
    }

    /// <summary>
    ///     设置逆向验证条件
    /// </summary>
    /// <remarks>当条件不满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> Unless(Func<TProperty, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        UnlessCondition = condition;

        return this;
    }

    /// <summary>
    ///     设置属性验证前的预处理器
    /// </summary>
    /// <remarks>该预处理器仅用于验证，不会修改原始属性的值。</remarks>
    /// <param name="preProcess">预处理器（函数）</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T, TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> PreProcess(Func<TProperty, TProperty>? preProcess)
    {
        _preProcessor = preProcess;

        return this;
    }

    /// <summary>
    ///     设置显示名称
    /// </summary>
    /// <param name="displayName">显示名称</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> WithDisplayName(string? displayName)
    {
        DisplayName = displayName;

        return this;
    }

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="memberName">成员名称</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> WithMemberName(string? memberName)
    {
        MemberName = memberName;

        return this;
    }

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="memberName">成员名称</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> WithName(string? memberName) => WithMemberName(memberName);

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="jsonNamingPolicy">
    ///     <see cref="JsonNamingPolicy" />
    /// </param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> WithMemberName(JsonNamingPolicy jsonNamingPolicy)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(jsonNamingPolicy);

        MemberName = jsonNamingPolicy.ConvertName(_annotationValidator.GetMemberName());

        return this;
    }

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="jsonNamingPolicy">
    ///     <see cref="JsonNamingPolicy" />
    /// </param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> WithName(JsonNamingPolicy jsonNamingPolicy) =>
        WithMemberName(jsonNamingPolicy);

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="memberNameProvider">成员名称获取委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> WithMemberName(Func<PropertyInfo, string?> memberNameProvider)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(memberNameProvider);

        MemberName = memberNameProvider(_annotationValidator.Property);

        return this;
    }

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="memberNameProvider">成员名称获取委托</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> WithName(Func<PropertyInfo, string?> memberNameProvider) =>
        WithMemberName(memberNameProvider);

    /// <summary>
    ///     检查是否应该对该属性执行验证
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldValidate(T instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 检查传入的规则集是否与指定的规则集匹配
        if (!RuleSetMatcher.Matches(RuleSets, ruleSets))
        {
            return false;
        }

        // 检查正向条件（When）
        if (WhenCondition is not null &&
            !WhenCondition(GetValueForValidation(instance), CreateValidationContext(instance)))
        {
            return false;
        }

        // 检查逆向条件（Unless）
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (UnlessCondition is not null &&
            UnlessCondition(GetValueForValidation(instance), CreateValidationContext(instance)))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     检查是否启用属性验证特性验证
    /// </summary>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldRunAnnotationValidation()
    {
        // 属性级配置优先
        if (SuppressAnnotationValidation.HasValue)
        {
            return !SuppressAnnotationValidation.Value;
        }

        return !_objectValidator.Options.SuppressAnnotationValidation;
    }

    /// <summary>
    ///     获取用于验证的对象
    /// </summary>
    /// <remarks>用于确定在 <see cref="ValidatorBase" /> 中实际被验证的对象（即验证的主体）。</remarks>
    /// <param name="instance">对象</param>
    /// <param name="validator">
    ///     <see cref="ValidatorBase" />
    /// </param>
    /// <param name="propertyValue">属性值</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    internal static object? GetValidatedObject(T instance, ValidatorBase validator, TProperty propertyValue)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(validator);

        // 获取验证器类型
        var validatorType = validator.GetType();

        // 检查验证器类型是否是验证器代理类型
        if (validatorType.IsGenericType && validatorType.GetGenericTypeDefinition() == typeof(ValidatorProxy<,>))
        {
            return instance;
        }

        return propertyValue;
    }

    /// <summary>
    ///     获取属性值
    /// </summary>
    /// <param name="instance">对象</param>
    /// <returns>
    ///     <typeparamref name="TProperty" />
    /// </returns>
    internal TProperty GetValue(T instance) => _annotationValidator.GetValue(instance)!;

    /// <summary>
    ///     获取用于验证的属性值
    /// </summary>
    /// <param name="instance">对象</param>
    /// <returns>
    ///     <typeparamref name="TProperty" />
    /// </returns>
    internal TProperty GetValueForValidation(T instance)
    {
        // 获取属性值
        var propertyValue = GetValue(instance);

        return _preProcessor is not null ? _preProcessor(propertyValue) : propertyValue;
    }

    /// <summary>
    ///     获取显示名称
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetDisplayName() => DisplayName ?? MemberName ?? _annotationValidator.GetDisplayName(null);

    /// <summary>
    ///     获取属性路径
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetMemberPath()
    {
        // 获取属性（成员）名称和父级属性路径
        var memberName = _annotationValidator.GetMemberName();
        var parentPath = _objectValidator.MemberPath;

        return string.IsNullOrWhiteSpace(parentPath) ? memberName : $"{parentPath}.{memberName}";
    }

    /// <summary>
    ///     获取用于 <see cref="ValidationResult.MemberNames" /> 的最终成员名称
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetEffectiveMemberName() => MemberName ?? GetMemberPath();

    /// <summary>
    ///     创建 <see cref="ValidationContext{T}" /> 实例
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="ValidationContext{T}" />
    /// </returns>
    internal ValidationContext<T> CreateValidationContext(T value)
    {
        // 初始化 ValidationContext 实例
        var validationContext = new ValidationContext<T>(value, null, _items);

        // 同步 IServiceProvider 委托
        validationContext.InitializeServiceProvider(_serviceProvider);

        return validationContext;
    }

    /// <summary>
    ///     释放资源
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        // 释放所有验证器资源
        foreach (var validator in Validators)
        {
            if (validator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        // 释放属性级别对象验证器资源
        if (_propertyValidator is IDisposable propertyValidatorDisposable)
        {
            propertyValidatorDisposable.Dispose();
        }
    }

    /// <inheritdoc cref="IMemberPathRepairable.RepairMemberPaths" />
    internal void RepairMemberPaths()
    {
        // 空检查
        if (_propertyValidator is null)
        {
            return;
        }

        // 设置当前子验证器的完整成员路径
        _propertyValidator.MemberPath = GetEffectiveMemberName();

        // 检查属性级别对象验证器是否实现 IMemberPathRepairable 接口
        if (_propertyValidator is IMemberPathRepairable repairable)
        {
            // 修复验证器及其子验证器的成员路径
            repairable.RepairMemberPaths();
        }
    }
}