// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     单个值验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ValueValidator<T> : FluentValidatorBuilder<T, ValueValidator<T>>, IObjectValidator<T>,
    IRuleSetContextProvider
{
    /// <summary>
    ///     当前规则集上下文栈
    /// </summary>
    internal readonly Stack<string?> _ruleSetStack;

    /// <summary>
    ///     值验证前的预处理器
    /// </summary>
    /// <remarks>该预处理器仅用于验证，不会修改原始的值。</remarks>
    internal Func<T, T>? _preProcessor;

    /// <inheritdoc cref="ValueValidator{T}" />
    /// <remarks>单个值级别验证器。</remarks>
    internal ValueValidator<T>? _valueValidator;

    /// <summary>
    ///     <inheritdoc cref="ValueValidator{T}" />
    /// </summary>
    public ValueValidator()
        : this(null, null)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValueValidator{T}" />
    /// </summary>
    /// <param name="items">验证上下文数据</param>
    public ValueValidator(IDictionary<object, object?>? items)
        : this(null, items)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValueValidator{T}" />
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">验证上下文数据</param>
    public ValueValidator(IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
        : base(serviceProvider, items) =>
        _ruleSetStack = new Stack<string?>();

    /// <summary>
    ///     显示名称
    /// </summary>
    internal string? DisplayName { get; private set; }

    /// <summary>
    ///     验证条件
    /// </summary>
    /// <remarks>当条件满足时才进行验证。</remarks>
    internal Func<T, bool>? WhenCondition { get; private set; }

    /// <summary>
    ///     逆向验证条件
    /// </summary>
    /// <remarks>当条件不满足时才进行验证。</remarks>
    internal Func<T, bool>? UnlessCondition { get; private set; }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public virtual bool IsValid(T? value, string?[]? ruleSets = null)
    {
        // 获取用于验证的值
        var resolvedValue = GetValueForValidation(value!);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(resolvedValue))
        {
            return true;
        }

        // 解析验证时使用的规则集
        var resolvedRuleSets = ResolveValidationRuleSets(ruleSets);

        // 检查是否设置单个值级别验证器
        if (_valueValidator is not null && !_valueValidator.IsValid(resolvedValue, resolvedRuleSets))
        {
            return false;
        }

        return Validators.Where(u => RuleSetMatcher.Matches(u.RuleSets, resolvedRuleSets))
            .All(u => u.IsValid(resolvedValue));
    }

    /// <inheritdoc />
    public virtual List<ValidationResult>? GetValidationResults(T? value, string?[]? ruleSets = null)
    {
        // 获取用于验证的值
        var resolvedValue = GetValueForValidation(value!);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(resolvedValue))
        {
            return null;
        }

        // 解析验证时使用的规则集
        var resolvedRuleSets = ResolveValidationRuleSets(ruleSets);

        // 获取显示名称和初始化验证结果集合
        var displayName = GetDisplayName();
        var validationResults = new List<ValidationResult>();

        // 检查是否设置单个值级别验证器
        if (_valueValidator is not null)
        {
            validationResults.AddRange(_valueValidator.GetValidationResults(resolvedValue, resolvedRuleSets) ?? []);
        }

        // 获取所有验证器验证结果集合
        validationResults.AddRange(Validators.Where(u => RuleSetMatcher.Matches(u.RuleSets, resolvedRuleSets))
            .SelectMany(u => u.GetValidationResults(resolvedValue, displayName) ?? []));

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public virtual void Validate(T? value, string?[]? ruleSets = null)
    {
        // 获取用于验证的值
        var resolvedValue = GetValueForValidation(value!);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(resolvedValue))
        {
            return;
        }

        // 解析验证时使用的规则集
        var resolvedRuleSets = ResolveValidationRuleSets(ruleSets);

        // 获取显示名称
        var displayName = GetDisplayName();

        // 检查是否设置单个值级别验证器
        // ReSharper disable once UseNullPropagation
        if (_valueValidator is not null)
        {
            _valueValidator.Validate(resolvedValue, resolvedRuleSets);
        }

        // 遍历验证器集合
        foreach (var validator in Validators.Where(u => RuleSetMatcher.Matches(u.RuleSets, resolvedRuleSets)))
        {
            validator.Validate(resolvedValue, displayName);
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
    public List<ValidationResult> ToResults(ValidationContext validationContext, bool disposeAfterValidation = true)
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
            // 获取用于验证的值（解决 null 值时 ObjectInstance 为 object 实例问题）
            var instance = validationContext.ObjectInstance is T value ? value : default;

            // 获取对象验证结果集合
            return WithDisplayName(validationContext.DisplayName).GetValidationResults(instance, ruleSets) ?? [];
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

    /// <summary>
    ///     为当前值自身配置验证规则
    /// </summary>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> Rule() => this;

    /// <summary>
    ///     在指定规则集上下文中配置验证规则
    /// </summary>
    /// <param name="ruleSet">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> RuleSet(string? ruleSet, Action setAction)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(setAction);

        return RuleSet(ruleSet, _ => setAction());
    }

    /// <summary>
    ///     在指定规则集上下文中配置验证规则
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> RuleSet(string?[]? ruleSets, Action setAction)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(setAction);

        return RuleSet(ruleSets, _ => setAction());
    }

    /// <summary>
    ///     在指定规则集上下文中配置验证规则
    /// </summary>
    /// <param name="ruleSet">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> RuleSet(string? ruleSet, Action<ValueValidator<T>> setAction) =>
        RuleSet(ruleSet is null ? null : [ruleSet], setAction);

    /// <summary>
    ///     在指定规则集上下文中配置验证规则
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    /// <param name="setAction">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> RuleSet(string?[]? ruleSets, Action<ValueValidator<T>> setAction)
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
            // 将当前规则集压入上下文栈，使后续的 Rule() 调用能感知到该规则集
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
    ///     设置验证条件
    /// </summary>
    /// <remarks>当条件满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> When(Func<T, bool> condition)
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
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> Unless(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        UnlessCondition = condition;

        return this;
    }

    /// <summary>
    ///     设置值验证前的预处理器
    /// </summary>
    /// <remarks>该预处理器仅用于验证，不会修改原始的值。</remarks>
    /// <param name="preProcess">预处理器（函数）</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> PreProcess(Func<T, T>? preProcess)
    {
        _preProcessor = preProcess;

        return this;
    }

    /// <summary>
    ///     设置单个值验证器
    /// </summary>
    /// <param name="validatorFactory">
    ///     <see cref="ValueValidator{T}" /> 工厂委托
    /// </param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ValueValidator<T> SetValidator(Func<IDictionary<object, object?>?, ValueValidator<T>?> validatorFactory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorFactory);

        // 空检查
        if (_valueValidator is not null)
        {
            throw new InvalidOperationException(
                "An value validator has already been assigned to this value. Only one value validator is allowed per value.");
        }

        // 调用工厂方法，传入当前 _items
        _valueValidator = validatorFactory(_items);

        // 空检查
        if (_valueValidator is null)
        {
            return this;
        }

        // 同步 IServiceProvider 委托
        _valueValidator.InitializeServiceProvider(_serviceProvider);

        return this;
    }

    /// <summary>
    ///     设置单个值验证器
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ValueValidator{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public ValueValidator<T> SetValidator(ValueValidator<T>? validator) =>
        SetValidator(_ => validator);

    /// <summary>
    ///     设置显示名称
    /// </summary>
    /// <param name="displayName">显示名称</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> WithDisplayName(string? displayName)
    {
        DisplayName = displayName;

        return this;
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

        // 释放单个值级别验证器资源
        if (_valueValidator is IDisposable valueValidatorDisposable)
        {
            valueValidatorDisposable.Dispose();
        }
    }

    /// <summary>
    ///     检查是否应该对该对象执行验证
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldValidate(T? value)
    {
        // 检查正向条件（When）
        if (WhenCondition is not null && !WhenCondition(value!))
        {
            return false;
        }

        // 检查逆向条件（Unless）
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (UnlessCondition is not null && UnlessCondition(value!))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     获取用于验证的值
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <typeparamref name="T" />
    /// </returns>
    internal T GetValueForValidation(T value) => _preProcessor is not null ? _preProcessor(value) : value;

    /// <summary>
    ///     获取显示名称
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetDisplayName() => DisplayName ?? "Value";

    /// <summary>
    ///     解析验证时使用的规则集
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    /// <returns><see cref="string" />列表</returns>
    internal string?[]? ResolveValidationRuleSets(string?[]? ruleSets) =>
        // 优先使用显式传入的规则集，否则从验证数据上下文中解析
        ruleSets ?? (_serviceProvider?.Invoke(typeof(IValidationDataContext)) as IValidationDataContext)
        ?.GetValidationOptions()?.RuleSets;

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal new void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 同步基类 IServiceProvider 委托
        base.InitializeServiceProvider(serviceProvider);

        // 同步 _valueValidator 实例 IServiceProvider 委托
        _valueValidator?.InitializeServiceProvider(serviceProvider);
    }

    /// <inheritdoc cref="IRuleSetContextProvider.GetCurrentRuleSets" />
    internal string?[]? GetCurrentRuleSets() => _ruleSetStack is { Count: > 0 } ? [_ruleSetStack.Peek()] : null;
}