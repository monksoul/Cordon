// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     属性验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TProperty">属性类型</typeparam>
public class PropertyValidator<T, TProperty> : PropertyValidator<T, TProperty, PropertyValidator<T, TProperty>>
{
    /// <inheritdoc />
    public PropertyValidator(Expression<Func<T, TProperty?>> selector, ObjectValidator<T> objectValidator) : base(
        selector, objectValidator)
    {
    }
}

/// <summary>
///     属性验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TProperty">属性类型</typeparam>
/// <typeparam name="TSelf">派生类型自身类型</typeparam>
public abstract partial class PropertyValidator<T, TProperty, TSelf> : FluentValidatorBuilder<TProperty, TSelf>,
    IPropertyValidator<T>
    where TSelf : PropertyValidator<T, TProperty, TSelf>
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
    ///     是否允许空字符串
    /// </summary>
    internal bool? _allowEmptyStrings;

    /// <summary>
    ///     属性验证前的预处理器
    /// </summary>
    /// <remarks>该预处理器仅用于验证，不会修改原始属性的值。</remarks>
    internal Func<TProperty, TProperty>? _preProcessor;

    /// <summary>
    ///     <inheritdoc cref="PropertyValidator{T,TProperty}" />
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="objectValidator">
    ///     <see cref="ObjectValidator{T}" />
    /// </param>
    internal PropertyValidator(Expression<Func<T, TProperty?>> selector, ObjectValidator<T> objectValidator)
        : base(null, (objectValidator ?? throw new ArgumentNullException(nameof(objectValidator))).Items)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(objectValidator);

        _selector = selector;
        _objectValidator = objectValidator;

        // 初始化 PropertyAnnotationValidator 实例
        _annotationValidator = new PropertyAnnotationValidator<T, TProperty>(selector!, null, objectValidator.Items);
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
    internal bool? SuppressAnnotationValidation { get; set; }

    /// <summary>
    ///     显示名称
    /// </summary>
    internal string? DisplayName { get; set; }

    /// <summary>
    ///     成员名称
    /// </summary>
    internal string? MemberName { get; set; }

    /// <summary>
    ///     验证条件
    /// </summary>
    /// <remarks>当条件满足时才进行验证。</remarks>
    internal Func<TProperty, ValidationContext<T>, bool>? WhenCondition { get; set; }

    /// <inheritdoc />
    string? IMemberPathRepairable.MemberPath { get; set; }

    /// <inheritdoc />
    void IMemberPathRepairable.RepairMemberPaths(string? memberPath) => RepairMemberPaths(memberPath);

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

        // 修复验证器及其子验证器的成员路径
        RepairMemberPaths(GetEffectiveMemberName());

        // 获取用于验证的属性值
        var propertyValue = GetValueForValidation(instance);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(instance, ruleSets);

        // 检查是否应该对该属性执行验证
        if (!ShouldValidate(instance, propertyValue, validationContext, ruleSets))
        {
            return true;
        }

        // 检查是否启用属性验证特性验证
        if (ShouldRunAnnotationValidation() && !_annotationValidator.IsValid(instance, validationContext))
        {
            return false;
        }

        // 创建 ValidationContext 实例（属性）
        var validationContextForProperty = CreateValidationContext(propertyValue, ruleSets);

        // 对于 ValidatorProxy<T, TValidator>（对象级代理），传入整个对象；否则传入属性值
        return Validators.All(u => u.IsValid(u.IsTypedProxy ? instance : propertyValue,
            u.IsTypedProxy ? validationContext : validationContextForProperty));
    }

    /// <inheritdoc />
    public virtual List<ValidationResult>? GetValidationResults(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 修复验证器及其子验证器的成员路径
        RepairMemberPaths(GetEffectiveMemberName());

        // 获取用于验证的属性值
        var propertyValue = GetValueForValidation(instance);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(instance, ruleSets);

        // 检查是否应该对该属性执行验证
        if (!ShouldValidate(instance, propertyValue, validationContext, ruleSets))
        {
            return null;
        }

        // 初始化验证结果集合
        var validationResults = new List<ValidationResult>();

        // 检查是否启用属性验证特性验证
        if (ShouldRunAnnotationValidation())
        {
            validationResults.AddRange(_annotationValidator.GetValidationResults(instance, validationContext) ?? []);
        }

        // 创建 ValidationContext 实例（属性）
        var validationContextForProperty = CreateValidationContext(propertyValue, ruleSets);

        // 获取所有验证器验证结果集合
        // 对于 ValidatorProxy<T, TValidator>（对象级代理），传入整个对象；否则传入属性值
        validationResults.AddRange(Validators.SelectMany(u =>
            u.GetValidationResults(u.IsTypedProxy ? instance : propertyValue,
                u.IsTypedProxy ? validationContext : validationContextForProperty) ?? []));

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public virtual void Validate(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 修复验证器及其子验证器的成员路径
        RepairMemberPaths(GetEffectiveMemberName());

        // 获取用于验证的属性值
        var propertyValue = GetValueForValidation(instance);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(instance, ruleSets);

        // 检查是否应该对该属性执行验证
        if (!ShouldValidate(instance, propertyValue, validationContext, ruleSets))
        {
            return;
        }

        // 检查是否启用属性验证特性验证
        if (ShouldRunAnnotationValidation())
        {
            _annotationValidator.Validate(instance, validationContext);
        }

        // 创建 ValidationContext 实例（属性）
        var validationContextForProperty = CreateValidationContext(propertyValue, ruleSets);

        // 遍历验证器集合
        foreach (var validator in Validators)
        {
            // 对于 ValidatorProxy<T, TValidator>（对象级代理），传入整个对象；否则传入属性值
            validator.Validate(validator.IsTypedProxy ? instance : propertyValue,
                validator.IsTypedProxy ? validationContext : validationContextForProperty);
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

    /// <inheritdoc />
    IPropertyValidator<T> IPropertyValidatorCloneable<T>.Clone(ObjectValidator<T> objectValidator) =>
        Clone(objectValidator);

    /// <inheritdoc />
    void IValidationAnnotationsConfigurable.UseAnnotationValidation(bool enabled) => UseAnnotationValidation(enabled);

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal override void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 同步基类 IServiceProvider 委托
        base.InitializeServiceProvider(serviceProvider);

        // 同步 _annotationValidator 实例 IServiceProvider 委托
        _annotationValidator.InitializeServiceProvider(serviceProvider);
    }

    /// <summary>
    ///     设置属性级别对象验证器
    /// </summary>
    /// <param name="validatorFactory">
    ///     <see cref="ObjectValidator{T}" /> 工厂委托
    /// </param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual TSelf SetValidator(
        Func<string?[]?, IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<TProperty>?> validatorFactory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorFactory);

        // 空检查
        if (Validators.OfType<ObjectValidator<TProperty>>().Any())
        {
            throw new InvalidOperationException(
                $"An object validator has already been assigned to this property. Only one object validator is allowed per property. To define nested rules, use `{nameof(ChildRules)}` within a single validator.");
        }

        // 调用工厂方法，传入当前 RuleSets、Items 和 Options
        var objectValidator = validatorFactory(RuleSets, _objectValidator.Items, _objectValidator.Options);

        // 空检查
        if (objectValidator is null)
        {
            return This;
        }

        // 标记为嵌套验证器
        objectValidator.IsNested = true;

        // 继承当前规则集
        objectValidator.SetInheritedRuleSetsIfNotSet(RuleSets);

        // 同步 IServiceProvider 委托
        objectValidator.InitializeServiceProvider(_serviceProvider);

        return AddValidator(objectValidator);
    }

    /// <summary>
    ///     设置属性级别对象验证器
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ObjectValidator{T}" />
    /// </param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual TSelf SetValidator(ObjectValidator<TProperty>? validator) =>
        SetValidator((_, _, _) => validator);

    /// <summary>
    ///     为子属性继续配置规则
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual TSelf ChildRules(Action<ObjectValidator<TProperty>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

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
    ///     设置验证条件
    /// </summary>
    /// <remarks>当条件满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf When(Func<TProperty, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        WhenCondition = (p, _) => condition(p);

        return This;
    }

    /// <summary>
    ///     设置验证条件
    /// </summary>
    /// <remarks>当条件满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf When(Func<TProperty, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        WhenCondition = condition;

        return This;
    }

    /// <summary>
    ///     设置属性验证前的预处理器
    /// </summary>
    /// <remarks>该预处理器仅用于验证，不会修改原始属性的值。</remarks>
    /// <param name="preProcess">预处理器（函数）</param>
    /// <returns>
    ///     <see cref="PropertyValidator{T, TProperty}" />
    /// </returns>
    public virtual TSelf PreProcess(Func<TProperty, TProperty>? preProcess)
    {
        _preProcessor = preProcess;

        return This;
    }

    /// <summary>
    ///     设置显示名称
    /// </summary>
    /// <param name="displayName">显示名称</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf WithDisplayName(string? displayName)
    {
        DisplayName = displayName;

        return This;
    }

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="memberName">成员名称</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf WithName(string? memberName)
    {
        MemberName = memberName;

        return This;
    }

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="jsonNamingPolicy">
    ///     <see cref="JsonNamingPolicy" />
    /// </param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf WithName(JsonNamingPolicy jsonNamingPolicy)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(jsonNamingPolicy);

        MemberName = jsonNamingPolicy.ConvertName(_annotationValidator.GetMemberName());

        return This;
    }

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="memberNameProvider">成员名称获取委托</param>
    /// <returns>
    ///     <typeparamref name="TSelf" />
    /// </returns>
    public virtual TSelf WithName(Func<PropertyInfo, string?> memberNameProvider)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(memberNameProvider);

        MemberName = memberNameProvider(_annotationValidator.Property);

        return This;
    }

    /// <summary>
    ///     检查是否应该对该属性执行验证
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="propertyValue">属性值</param>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldValidate(T instance, TProperty propertyValue, ValidationContext<T> validationContext,
        string?[]? ruleSets)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(validationContext);

        // 检查传入的规则集是否与指定的规则集匹配
        if (!RuleSetMatcher.Matches(RuleSets, ruleSets))
        {
            return false;
        }

        // 检查正向条件（When）
        if (WhenCondition is not null && !WhenCondition(propertyValue, validationContext))
        {
            return false;
        }

        // 处理空字符串验证问题
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_allowEmptyStrings == true && propertyValue is string { Length: 0 })
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
        var parentPath = _objectValidator._memberPath;

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
    ///     释放资源
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        // 遍历所有验证器
        foreach (var validator in Validators)
        {
            if (validator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <inheritdoc cref="IMemberPathRepairable.RepairMemberPaths" />
    internal virtual void RepairMemberPaths(string? memberPath)
    {
        // 遍历所有验证器
        foreach (var validator in Validators)
        {
            if (validator is IMemberPathRepairable memberPathRepairable)
            {
                memberPathRepairable.RepairMemberPaths(memberPath);
            }
        }
    }

    /// <inheritdoc cref="IPropertyValidatorCloneable{T}.Clone" />
    internal virtual IPropertyValidator<T> Clone(ObjectValidator<T> objectValidator)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(objectValidator);

        // 初始化 PropertyValidator 实例
        var propertyValidator = new PropertyValidator<T, TProperty>(_selector, objectValidator)
        {
            RuleSets = RuleSets,
            SuppressAnnotationValidation = SuppressAnnotationValidation,
            DisplayName = DisplayName,
            MemberName = MemberName,
            WhenCondition = WhenCondition
        };

        // 同步字段
        propertyValidator._allowEmptyStrings = _allowEmptyStrings;
        propertyValidator._preProcessor = _preProcessor;

        // 同步已设置的验证器
        propertyValidator.AddValidators(Validators);

        // 同步 IServiceProvider 委托
        propertyValidator.InitializeServiceProvider(objectValidator._serviceProvider);

        return propertyValidator;
    }

    /// <summary>
    ///     创建 <see cref="ValidationContext{T}" /> 实例
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <typeparam name="TInstance">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ValidationContext{T}" />
    /// </returns>
    internal ValidationContext<TInstance> CreateValidationContext<TInstance>(TInstance instance, string?[]? ruleSets)
    {
        // 获取显示名称和成员名称
        var displayName = GetDisplayName();
        var memberPath = GetEffectiveMemberName();

        return new ValidationContext<TInstance>(instance, _serviceProvider, Items)
        {
            DisplayName = displayName, MemberNames = [memberPath], RuleSets = ruleSets
        };
    }
}