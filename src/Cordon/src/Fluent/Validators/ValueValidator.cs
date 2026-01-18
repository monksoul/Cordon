// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     单值验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ValueValidator<T> : FluentValidatorBuilder<T, ValueValidator<T>>, IValueValidator<T>
{
    /// <summary>
    ///     当前规则集上下文栈
    /// </summary>
    internal readonly Stack<string?> _ruleSetStack;

    /// <summary>
    ///     是否允许空字符串
    /// </summary>
    internal bool? _allowEmptyStrings;

    /// <summary>
    ///     对象图中的属性路径
    /// </summary>
    internal string? _memberPath;

    /// <summary>
    ///     值验证前的预处理器
    /// </summary>
    /// <remarks>该预处理器仅用于验证，不会修改原始的值。</remarks>
    internal Func<T, T>? _preProcessor;

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
    /// <param name="items">共享数据</param>
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
    /// <param name="items">共享数据</param>
    public ValueValidator(IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
        : base(serviceProvider, items) =>
        _ruleSetStack = new Stack<string?>();

    /// <summary>
    ///     显示名称
    /// </summary>
    internal string? DisplayName { get; private set; }

    /// <summary>
    ///     成员名称
    /// </summary>
    internal string? MemberName { get; set; }

    /// <summary>
    ///     验证条件
    /// </summary>
    /// <remarks>当条件满足时才进行验证。</remarks>
    internal Func<T, ValidationContext<T>, bool>? WhenCondition { get; private set; }

    /// <inheritdoc />
    string? IMemberPathRepairable.MemberPath
    {
        get => _memberPath;
        set => _memberPath = value;
    }

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

        // 解析验证时使用的规则集
        var resolvedRuleSets = ResolveValidationRuleSets(ruleSets);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(resolvedValue, resolvedRuleSets);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(resolvedValue, validationContext))
        {
            return true;
        }

        return Validators.Where(u => RuleSetMatcher.Matches(u.RuleSets, resolvedRuleSets))
            .All(u => u.IsValid(resolvedValue, validationContext));
    }

    /// <inheritdoc />
    public virtual List<ValidationResult>? GetValidationResults(T? value, string?[]? ruleSets = null)
    {
        // 获取用于验证的值
        var resolvedValue = GetValueForValidation(value!);

        // 解析验证时使用的规则集
        var resolvedRuleSets = ResolveValidationRuleSets(ruleSets);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(resolvedValue, resolvedRuleSets);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(resolvedValue, validationContext))
        {
            return null;
        }

        return Validators.Where(u => RuleSetMatcher.Matches(u.RuleSets, resolvedRuleSets))
            .SelectMany(u => u.GetValidationResults(resolvedValue, validationContext) ?? []).ToResults();
    }

    /// <inheritdoc />
    public virtual void Validate(T? value, string?[]? ruleSets = null)
    {
        // 获取用于验证的值
        var resolvedValue = GetValueForValidation(value!);

        // 解析验证时使用的规则集
        var resolvedRuleSets = ResolveValidationRuleSets(ruleSets);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(resolvedValue, resolvedRuleSets);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(resolvedValue, validationContext))
        {
            return;
        }

        // 遍历验证器集合
        foreach (var validator in Validators.Where(u => RuleSetMatcher.Matches(u.RuleSets, resolvedRuleSets)))
        {
            validator.Validate(resolvedValue, validationContext);
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
    public virtual List<ValidationResult> ToResults(ValidationContext validationContext,
        bool disposeAfterValidation = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);

        // 同步 IServiceProvider 委托
        InitializeServiceProvider(validationContext.GetService);

        // 尝试从 ValidationContext.Items 中解析验证选项中的规则集
        string?[]? ruleSets = null;
        if (validationContext.Items.TryGetValue(Constants.ValidationOptionsKey, out var metadataObj) &&
            metadataObj is ValidationOptionsMetadata metadata)
        {
            ruleSets = metadata.RuleSets;
        }

        try
        {
            // 获取用于验证的值（解决 null 值时 ObjectInstance 为 object 实例问题）
            var instance = validationContext.ObjectInstance is T value ? value : default;

            // 设置显示名称并返回对象验证结果集合
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

    /// <inheritdoc />
    void IMemberPathRepairable.RepairMemberPaths(string? memberPath) => RepairMemberPaths(memberPath);

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
    public virtual ValueValidator<T> When(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        WhenCondition = (u, _) => condition(u);

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
    public virtual ValueValidator<T> When(Func<T, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        WhenCondition = condition;

        return this;
    }

    /// <summary>
    ///     设置值验证前的预处理器
    /// </summary>
    /// <remarks>该预处理器仅用于验证，不会修改原始的值。</remarks>
    /// <param name="preProcess">预处理器</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public virtual ValueValidator<T> PreProcess(Func<T, T>? preProcess)
    {
        _preProcessor = preProcess;

        return this;
    }

    /// <summary>
    ///     设置单值验证器
    /// </summary>
    /// <param name="validatorFactory">
    ///     <see cref="ValueValidator{T}" /> 工厂委托
    /// </param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual ValueValidator<T> SetValidator(
        Func<IDictionary<object, object?>?, ValueValidator<T>?> validatorFactory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorFactory);

        // 空检查
        if (Validators.OfType<ObjectValidatorProxy<T>>().Any())
        {
            throw new InvalidOperationException(
                "An value validator has already been assigned to this value. Only one value validator is allowed per value.");
        }

        // 调用工厂方法，传入当前 Items
        var valueValidator = validatorFactory(Items);

        // 空检查
        if (valueValidator is null)
        {
            return this;
        }

        // 同步 IServiceProvider 委托
        valueValidator.InitializeServiceProvider(_serviceProvider);

        return AddValidator(new ObjectValidatorProxy<T>(valueValidator));
    }

    /// <summary>
    ///     设置单值验证器
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ValueValidator{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual ValueValidator<T> SetValidator(ValueValidator<T>? validator) =>
        SetValidator(_ => validator);

    /// <summary>
    ///     设置显示名称
    /// </summary>
    /// <param name="displayName">显示名称</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public virtual ValueValidator<T> WithDisplayName(string? displayName)
    {
        DisplayName = displayName;

        return this;
    }

    /// <summary>
    ///     设置成员名称
    /// </summary>
    /// <param name="memberName">成员名称</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public virtual ValueValidator<T> WithName(string? memberName)
    {
        MemberName = memberName;

        return This;
    }

    /// <summary>
    ///     配置是否允许空字符串
    /// </summary>
    /// <param name="allowEmptyStrings">是否允许空字符串，默认值为：<c>true</c>。</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public virtual ValueValidator<T> AllowEmptyStrings(bool allowEmptyStrings = true)
    {
        _allowEmptyStrings = allowEmptyStrings;

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
    }

    /// <summary>
    ///     检查是否应该对该对象执行验证
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldValidate(T? value, ValidationContext<T> validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);

        // 检查 When 条件
        if (WhenCondition is not null && !WhenCondition(value!, validationContext))
        {
            return false;
        }

        // 处理空字符串验证问题
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_allowEmptyStrings == true && value is string { Length: 0 })
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
    internal string GetDisplayName() => DisplayName ?? MemberName ?? typeof(T).Name;

    /// <summary>
    ///     获取用于 <see cref="ValidationResult.MemberNames" /> 的最终成员名称
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string? GetEffectiveMemberName() => MemberName ?? _memberPath;

    /// <summary>
    ///     解析验证时使用的规则集
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    /// <returns><see cref="string" />列表</returns>
    internal string?[]? ResolveValidationRuleSets(string?[]? ruleSets) =>
        // 优先使用显式传入的规则集，否则从验证数据上下文中解析
        ruleSets ?? (_serviceProvider?.Invoke(typeof(IValidationDataContext)) as IValidationDataContext)
        ?.GetValidationOptions()?.RuleSets;

    /// <inheritdoc cref="IRuleSetContextProvider.GetCurrentRuleSets" />
    internal string?[]? GetCurrentRuleSets() => _ruleSetStack is { Count: > 0 } ? [_ruleSetStack.Peek()] : null;

    /// <inheritdoc cref="IMemberPathRepairable.RepairMemberPaths" />
    internal virtual void RepairMemberPaths(string? memberPath)
    {
        _memberPath = memberPath;

        // 遍历所有验证器
        foreach (var validator in Validators)
        {
            if (validator is IMemberPathRepairable memberPathRepairable)
            {
                memberPathRepairable.RepairMemberPaths(memberPath);
            }
        }
    }

    /// <summary>
    ///     创建 <see cref="ValidationContext{T}" /> 实例
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="ValidationContext{T}" />
    /// </returns>
    internal ValidationContext<T> CreateValidationContext(T value, string?[]? ruleSets)
    {
        // 获取显示名称和成员名称
        var displayName = GetDisplayName();
        var memberPath = GetEffectiveMemberName();

        return new ValidationContext<T>(value, _serviceProvider, Items)
        {
            DisplayName = displayName, MemberNames = memberPath is null ? null : [memberPath], RuleSets = ruleSets
        };
    }
}