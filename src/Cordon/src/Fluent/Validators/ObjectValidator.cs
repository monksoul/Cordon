// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     对象验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ObjectValidator<T> : ValidatorBase<T>, IObjectValidator<T>, IMemberPathRepairable, IRuleSetContextProvider,
    IValidationAnnotationsConfigurable
{
    /// <summary>
    ///     验证上下文键
    /// </summary>
    /// <remarks>
    ///     用于使用 <![CDATA[ValidationContext.With<T>()]]> 时设置。
    /// </remarks>
    internal static readonly object ValidationContextsKey = new();

    /// <inheritdoc cref="ObjectAnnotationValidator" />
    internal readonly ObjectAnnotationValidator _annotationValidator;

    /// <summary>
    ///     当前规则集上下文栈
    /// </summary>
    internal readonly Stack<string?> _ruleSetStack;

    /// <summary>
    ///     对象图中的属性路径
    /// </summary>
    internal string? _memberPath;

    /// <inheritdoc cref="ObjectValidator{T}" />
    /// <remarks>对象级别验证器。</remarks>
    internal ObjectValidator<T>? _objectValidator;

    /// <summary>
    ///     <see cref="IServiceProvider" /> 委托
    /// </summary>
    internal Func<Type, object?>? _serviceProvider;

    /// <summary>
    ///     <inheritdoc cref="ObjectValidator{T}" />
    /// </summary>
    public ObjectValidator()
        : this(null, null)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ObjectValidator{T}" />
    /// </summary>
    /// <param name="items">共享数据</param>
    public ObjectValidator(IDictionary<object, object?>? items)
        : this(null, items)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ObjectValidator{T}" />
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    public ObjectValidator(IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
    {
        // 初始化 ValidatorOptions 实例
        Options = new ValidatorOptions();

        // 空检查
        if (serviceProvider is not null)
        {
            _serviceProvider = serviceProvider.GetService;
        }

        Items = items is not null ? new Dictionary<object, object?>(items) : new Dictionary<object, object?>();

        // 初始化 ObjectAnnotationValidator 实例
        _annotationValidator = new ObjectAnnotationValidator(serviceProvider, items)
        {
            ValidateAllProperties = Options.ValidateAllProperties
        };

        Validators = [];
        _ruleSetStack = new Stack<string?>();

        // 订阅 ValidatorOptions 属性变更事件
        Options.PropertyChanged += OptionsOnPropertyChanged;

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <summary>
    ///     共享数据
    /// </summary>
    public IDictionary<object, object?> Items { get; }

    /// <inheritdoc cref="ValidatorOptions" />
    internal ValidatorOptions Options { get; }

    /// <summary>
    ///     验证条件
    /// </summary>
    /// <remarks>当条件满足时才进行验证。</remarks>
    internal Func<T, ValidationContext<T>, bool>? WhenCondition { get; private set; }

    /// <summary>
    ///     属性验证器集合
    /// </summary>
    internal List<IPropertyValidator<T>> Validators { get; }

    /// <summary>
    ///     从父级继承的规则集
    /// </summary>
    /// <remarks>用于 <see cref="PropertyValidator{T,TProperty,TSelf}.ChildRules" /> 场景。</remarks>
    internal string?[]? InheritedRuleSets { get; set => field = value?.Select(u => u?.Trim()).ToArray(); }

    /// <inheritdoc />
    string? IMemberPathRepairable.MemberPath
    {
        get => _memberPath;
        set => _memberPath = value;
    }

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

        // 解析验证时使用的规则集
        var resolvedRuleSets = ResolveValidationRuleSets(ruleSets);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(instance, resolvedRuleSets);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(instance, validationContext))
        {
            return true;
        }

        // 检查是否启用对象属性验证特性验证
        // 此处可能存在验证特性重复执行的问题，可通过启用 SuppressAnnotationValidation 或调用 CustomOnly() 方法解决
        if (ShouldRunAnnotationValidation() && !_annotationValidator.IsValid(instance, validationContext))
        {
            return false;
        }

        // 检查是否设置了对象级别验证器
        if (_objectValidator is not null && !_objectValidator.IsValid(instance, ruleSets))
        {
            return false;
        }

        return Validators.All(u => u.IsValid(instance, resolvedRuleSets));
    }

    /// <inheritdoc />
    public virtual List<ValidationResult>? GetValidationResults(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 解析验证时使用的规则集
        var resolvedRuleSets = ResolveValidationRuleSets(ruleSets);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(instance, resolvedRuleSets);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(instance, validationContext))
        {
            return null;
        }

        // 初始化验证结果集合
        var validationResults = new List<ValidationResult>();

        // 检查是否启用对象属性验证特性验证
        // 此处可能存在验证特性重复执行的问题，可通过启用 SuppressAnnotationValidation 或调用 CustomOnly() 方法解决
        if (ShouldRunAnnotationValidation())
        {
            validationResults.AddRange(_annotationValidator.GetValidationResults(instance, validationContext) ?? []);
        }

        // 检查是否设置了对象级别验证器
        if (_objectValidator is not null)
        {
            validationResults.AddRange(_objectValidator.GetValidationResults(instance, ruleSets) ?? []);
        }

        // 获取所有属性验证器验证结果集合
        validationResults.AddRange(
            Validators.SelectMany(u => u.GetValidationResults(instance, resolvedRuleSets) ?? []));

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public virtual void Validate(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 解析验证时使用的规则集
        var resolvedRuleSets = ResolveValidationRuleSets(ruleSets);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(instance, resolvedRuleSets);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(instance, validationContext))
        {
            return;
        }

        // 检查是否启用对象属性验证特性验证
        // 此处可能存在验证特性重复执行的问题，可通过启用 SuppressAnnotationValidation 或调用 CustomOnly() 方法解决
        if (ShouldRunAnnotationValidation())
        {
            _annotationValidator.Validate(instance, validationContext);
        }

        // 检查是否设置了对象级别验证器
        // ReSharper disable once UseNullPropagation
        if (_objectValidator is not null)
        {
            _objectValidator.Validate(instance, ruleSets);
        }

        // 遍历属性验证器集合
        foreach (var validator in Validators)
        {
            validator.Validate(instance, resolvedRuleSets);
        }
    }

    /// <inheritdoc />
    public virtual void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        _serviceProvider = serviceProvider;

        // 同步 _annotationValidator 实例 IServiceProvider 委托
        _annotationValidator.InitializeServiceProvider(serviceProvider);

        // 同步 _objectValidator 实例 IServiceProvider 委托
        _objectValidator?.InitializeServiceProvider(serviceProvider);

        // 遍历所有属性验证器并尝试同步 IServiceProvider 委托
        foreach (var propertyValidator in Validators)
        {
            // 同步 IServiceProvider 委托
            propertyValidator.InitializeServiceProvider(serviceProvider);
        }
    }

    /// <inheritdoc />
    bool IObjectValidator.IsValid(object? instance, string?[]? ruleSets) => IsValid((T?)instance, ruleSets);

    /// <inheritdoc />
    List<ValidationResult>? IObjectValidator.GetValidationResults(object? instance, string?[]? ruleSets) =>
        GetValidationResults((T?)instance, ruleSets);

    /// <inheritdoc />
    void IObjectValidator.Validate(object? instance, string?[]? ruleSets) => Validate((T?)instance, ruleSets);

    /// <inheritdoc />
    public virtual List<ValidationResult> ToResults(ValidationContext validationContext,
        bool disposeAfterValidation = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);

        // 同步 IServiceProvider 委托
        InitializeServiceProvider(validationContext.GetService);

        // 尝试从 ValidationContext.Items 中解析验证选项中的规则集
        string?[]? ruleSets = null;
        if (validationContext.Items.TryGetValue(ValidationDataContext.ValidationOptionsKey, out var metadataObj) &&
            metadataObj is ValidationOptionsMetadata metadata)
        {
            ruleSets = metadata.RuleSets;
        }

        try
        {
            // 获取对象验证结果集合
            return GetValidationResults((T)validationContext.ObjectInstance, ruleSets) ?? [];
        }
        finally
        {
            // 自动释放资源
            if (disposeAfterValidation)
            {
                Dispose();
            }
        }
    }

    /// <inheritdoc />
    string?[]? IRuleSetContextProvider.GetCurrentRuleSets() => GetCurrentRuleSets();

    /// <inheritdoc />
    void IValidationAnnotationsConfigurable.UseAnnotationValidation(bool enabled) => UseAnnotationValidation(enabled);

    /// <inheritdoc />
    public override bool IsValid(T? instance, ValidationContext<T> validationContext) =>
        instance is null || IsValid(instance, validationContext.RuleSets);

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext) =>
        instance is null ? null : GetValidationResults(instance, validationContext.RuleSets);

    /// <inheritdoc />
    public override void Validate(T? instance, ValidationContext<T> validationContext)
    {
        // 空检查
        if (instance is not null)
        {
            Validate(instance, validationContext.RuleSets);
        }
    }

    /// <summary>
    ///     为指定属性配置验证规则
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="configure">自定义配置委托</param>
    /// <typeparam name="TProperty">属性类型</typeparam>
    /// <returns>
    ///     <see cref="PropertyValidator{T,TProperty}" />
    /// </returns>
    public PropertyValidator<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty?>> selector,
        Action<PropertyValidator<T, TProperty>>? configure = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(selector);

        // 初始化 PropertyValidator 实例
        var propertyValidator = new PropertyValidator<T, TProperty>(selector, this) { RuleSets = GetCurrentRuleSets() };

        // 调用自定义配置委托
        configure?.Invoke(propertyValidator);

        // 将实例添加到集合中
        Validators.Add(propertyValidator);

        // 同步 IServiceProvider 委托
        propertyValidator.InitializeServiceProvider(_serviceProvider);

        return propertyValidator;
    }

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
        Action<CollectionPropertyValidator<T, TElement>>? configure = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(selector);

        // 初始化 CollectionPropertyValidator 实例
        var propertyValidator =
            new CollectionPropertyValidator<T, TElement>(selector, this) { RuleSets = GetCurrentRuleSets() };

        // 调用自定义配置委托
        configure?.Invoke(propertyValidator);

        // 将实例添加到集合中
        Validators.Add(propertyValidator);

        // 同步 IServiceProvider 委托
        propertyValidator.InitializeServiceProvider(_serviceProvider);

        return propertyValidator;
    }

    /// <summary>
    ///     在指定规则集上下文中为指定属性配置验证规则
    /// </summary>
    /// <param name="ruleSet">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> RuleSet(string? ruleSet, Action setAction)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(setAction);

        return RuleSet(ruleSet, _ => setAction());
    }

    /// <summary>
    ///     在指定规则集上下文中为指定属性配置验证规则
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> RuleSet(string?[]? ruleSets, Action setAction)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(setAction);

        return RuleSet(ruleSets, _ => setAction());
    }

    /// <summary>
    ///     在指定规则集上下文中为指定属性配置验证规则
    /// </summary>
    /// <param name="ruleSet">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> RuleSet(string? ruleSet, Action<ObjectValidator<T>> setAction) =>
        RuleSet(ruleSet is null ? null : [ruleSet], setAction);

    /// <summary>
    ///     在指定规则集上下文中为指定属性配置验证规则
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> RuleSet(string?[]? ruleSets, Action<ObjectValidator<T>> setAction)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(setAction);

        // 规范化规则集：去除前后空格
        var normalizeRuleSet = (ruleSets ?? []).Select(u => u?.Trim()).ToArray();

        // 空检查
        if (normalizeRuleSet is { Length: 0 })
        {
            // 调用自定义配置委托
            setAction(this);

            return this;
        }

        // 为每个规则集创建独立作用域
        foreach (var ruleSet in normalizeRuleSet)
        {
            // 将当前规则集压入上下文栈，使后续的 RuleFor() 调用能感知到该规则集
            _ruleSetStack.Push(ruleSet);

            try
            {
                // 调用自定义配置委托
                setAction(this);
            }
            // 确保即使发生异常，也能正确退出当前规则集作用域
            finally
            {
                _ruleSetStack.Pop();
            }
        }

        return this;
    }

    /// <summary>
    ///     包含（组合）其他对象验证器
    /// </summary>
    /// <param name="validatorFactory">
    ///     <see cref="ObjectValidator{T}" /> 工厂委托
    /// </param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> Include(
        Func<IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<T>> validatorFactory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorFactory);

        // 调用工厂方法，传入当前 Items 和 Options
        var objectValidator = validatorFactory(Items, Options);

        // 空检查
        ArgumentNullException.ThrowIfNull(objectValidator);

        // 遍历所有属性验证器
        foreach (var propertyValidator in objectValidator.Validators)
        {
            // 检查是否实现 IPropertyValidatorCloneable<T> 接口
            if (propertyValidator is IPropertyValidatorCloneable<T> cloneable)
            {
                // 添加至集合中
                Validators.Add(cloneable.Clone(this));
            }
        }

        return this;
    }

    /// <summary>
    ///     包含（组合）其他对象验证器
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ObjectValidator{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> Include(ObjectValidator<T> validator) => Include((_, _) => validator);

    /// <summary>
    ///     配置 <see cref="ValidatorOptions" /> 示例
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> ConfigureOptions(Action<ValidatorOptions> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 调用自定义配置委托
        configure(Options);

        return this;
    }

    /// <summary>
    ///     配置 <see cref="ValidatorOptions" /> 示例
    /// </summary>
    /// <param name="options">
    ///     <see cref="ValidatorOptions" />
    /// </param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public ObjectValidator<T> ConfigureOptions(ValidatorOptions options)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(options);

        Options.SuppressAnnotationValidation = options.SuppressAnnotationValidation;
        Options.ValidateAllProperties = options.ValidateAllProperties;

        return this;
    }

    /// <summary>
    ///     设置验证条件
    /// </summary>
    /// <remarks>当条件满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public virtual ObjectValidator<T> When(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        WhenCondition = (instance, _) => condition(instance);

        return this;
    }

    /// <summary>
    ///     设置验证条件
    /// </summary>
    /// <remarks>当条件满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public virtual ObjectValidator<T> When(Func<T, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        WhenCondition = condition;

        return this;
    }

    /// <summary>
    ///     设置对象验证器
    /// </summary>
    /// <param name="validatorFactory">
    ///     <see cref="ObjectValidator{T}" /> 工厂委托
    /// </param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual ObjectValidator<T> SetValidator(
        Func<IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<T>?> validatorFactory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorFactory);

        // 空检查
        if (_objectValidator is not null)
        {
            throw new InvalidOperationException(
                "An object validator has already been assigned to this object. Only one object validator is allowed per object.");
        }

        // 调用工厂方法，传入当前 Items 和 Options
        _objectValidator = validatorFactory(Items, Options);

        // 空检查
        if (_objectValidator is null)
        {
            return this;
        }

        // 继承当前规则集
        _objectValidator.SetInheritedRuleSetsIfNotSet(InheritedRuleSets);

        // 同步 IServiceProvider 委托
        _objectValidator.InitializeServiceProvider(_serviceProvider);

        return this;
    }

    /// <summary>
    ///     设置对象验证器
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ObjectValidator{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual ObjectValidator<T> SetValidator(ObjectValidator<T>? validator) =>
        SetValidator((_, _) => validator);

    /// <summary>
    ///     配置是否启用对象属性验证特性验证
    /// </summary>
    /// <param name="enabled">是否启用</param>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public virtual ObjectValidator<T> UseAnnotationValidation(bool enabled)
    {
        Options.SuppressAnnotationValidation = !enabled;

        return this;
    }

    /// <summary>
    ///     配置启用对象属性验证特性验证
    /// </summary>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public virtual ObjectValidator<T> UseAnnotationValidation() => UseAnnotationValidation(true);

    /// <summary>
    ///     配置跳过对象属性验证特性验证
    /// </summary>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public virtual ObjectValidator<T> SkipAnnotationValidation() => UseAnnotationValidation(false);

    /// <summary>
    ///     配置跳过对象属性验证特性验证
    /// </summary>
    /// <remarks>仅验证自定义规则。</remarks>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public virtual ObjectValidator<T> CustomOnly() => UseAnnotationValidation(false);

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="disposeAfterValidation">是否在验证完成后自动释放当前实例。默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public virtual List<ValidationResult> ToResults(bool disposeAfterValidation = true)
    {
        // 查找共享数据中是否包含 ValidationContextsKey 键数据
        if (Items.TryGetValue(ValidationContextsKey, out var validationContextObject) &&
            validationContextObject is ValidationContext validationContext)
        {
            return ToResults(validationContext, disposeAfterValidation);
        }

        throw new InvalidOperationException(
            "The parameterless 'ToResults()' method can only be used when the validator is created via 'ValidationContext.With<T>()'. Ensure you are calling it inside 'IValidatableObject.Validate' and have used 'With' to configure inline validation rules.");
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

        // 移除 ValidatorOptions 属性变更事件
        Options.PropertyChanged -= OptionsOnPropertyChanged;

        // 清空共享数据
        Items.Clear();

        // 释放所有属性验证器资源
        foreach (var propertyValidator in Validators)
        {
            propertyValidator.Dispose();
        }

        // 释放对象级别验证器资源
        _objectValidator?.Dispose();
    }

    /// <summary>
    ///     检查是否应该对该对象执行验证
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldValidate(T instance, ValidationContext<T> validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(validationContext);

        // 检查正向条件（When）
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (WhenCondition is not null && !WhenCondition(instance, validationContext))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     检查是否启用对象属性验证特性验证
    /// </summary>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldRunAnnotationValidation() => !Options.SuppressAnnotationValidation;

    /// <summary>
    ///     解析验证时使用的规则集
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    /// <returns><see cref="string" />列表</returns>
    internal string?[]? ResolveValidationRuleSets(string?[]? ruleSets) =>
        // 优先使用显式传入的规则集，否则从验证数据上下文中解析
        ruleSets ?? (_serviceProvider?.Invoke(typeof(IValidationDataContext)) as IValidationDataContext)
        ?.GetValidationOptions()?.RuleSets;

    /// <summary>
    ///     订阅 <see cref="Options" /> 属性变更事件
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="arg">
    ///     <see cref="PropertyChangedEventArgs" />
    /// </param>
    internal void OptionsOnPropertyChanged(object? sender, PropertyChangedEventArgs arg)
    {
        // 同步 ValidatorOptions.ValidateAllProperties 属性值到 _annotationValidator.ValidateAllProperties
        if (arg.PropertyName == nameof(ValidatorOptions.ValidateAllProperties))
        {
            _annotationValidator.ValidateAllProperties = Options.ValidateAllProperties;
        }
    }

    /// <inheritdoc cref="IRuleSetContextProvider.GetCurrentRuleSets" />
    internal string?[]? GetCurrentRuleSets() =>
        _ruleSetStack is { Count: > 0 } ? [_ruleSetStack.Peek()] : InheritedRuleSets;

    /// <summary>
    ///     设置从父级继承的规则集
    /// </summary>
    /// <remarks>仅当尚未设置时。</remarks>
    /// <param name="inheritedRuleSets">从父级继承的规则集</param>
    internal void SetInheritedRuleSetsIfNotSet(string?[]? inheritedRuleSets) => InheritedRuleSets ??= inheritedRuleSets;

    /// <inheritdoc cref="IMemberPathRepairable.RepairMemberPaths" />
    internal virtual void RepairMemberPaths(string? memberPath)
    {
        _memberPath = memberPath;

        // 递归修复所有子属性验证器
        foreach (var propertyValidator in Validators)
        {
            propertyValidator.RepairMemberPaths(memberPath);
        }

        _objectValidator?.RepairMemberPaths(memberPath);
    }

    /// <summary>
    ///     创建 <see cref="ValidationContext{T}" /> 实例
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="ValidationContext{T}" />
    /// </returns>
    internal ValidationContext<T> CreateValidationContext(T instance, string?[]? ruleSets) =>
        new(instance, _serviceProvider, Items) { RuleSets = ruleSets };
}