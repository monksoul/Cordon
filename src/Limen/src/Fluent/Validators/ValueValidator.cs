// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     单个值验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ValueValidator<T> : FluentValidatorBuilder<T, ValueValidator<T>>, IObjectValidator<T>, IDisposable
{
    /// <summary>
    ///     值验证前的预处理器
    /// </summary>
    /// <remarks>该预处理器仅用于验证，不会修改原始的值。</remarks>
    internal Func<T, T>? _preProcessor;

    /// <inheritdoc cref="ValueValidator{T}" />
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
        : base(serviceProvider, items)
    {
    }

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
        if (!ShouldValidate(resolvedValue, ruleSets))
        {
            return true;
        }

        // 检查是否设置单个值验证器
        if (_valueValidator is not null && !_valueValidator.IsValid(resolvedValue))
        {
            return false;
        }

        return Validators.All(u => u.IsValid(resolvedValue));
    }

    /// <inheritdoc />
    public virtual List<ValidationResult>? GetValidationResults(T? value, string?[]? ruleSets = null)
    {
        // 获取用于验证的值
        var resolvedValue = GetValueForValidation(value!);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(resolvedValue, ruleSets))
        {
            return null;
        }

        // 获取显示名称和初始化验证结果集合
        var displayName = GetDisplayName();
        var validationResults = new List<ValidationResult>();

        // 检查是否设置单个值验证器
        if (_valueValidator is not null)
        {
            validationResults.AddRange(_valueValidator.GetValidationResults(resolvedValue) ?? []);
        }

        // 获取所有验证器验证结果集合
        validationResults.AddRange(Validators.SelectMany(u =>
            u.GetValidationResults(resolvedValue, displayName) ?? []));

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public virtual void Validate(T? value, string?[]? ruleSets = null)
    {
        // 获取用于验证的值
        var resolvedValue = GetValueForValidation(value!);

        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(resolvedValue, ruleSets))
        {
            return;
        }

        // 获取显示名称
        var displayName = GetDisplayName();

        // 检查是否设置单个值验证器
        // ReSharper disable once UseNullPropagation
        if (_valueValidator is not null)
        {
            _valueValidator.Validate(resolvedValue);
        }

        // 遍历验证器集合
        foreach (var validator in Validators)
        {
            validator.Validate(resolvedValue, displayName);
        }
    }

    /// <summary>
    ///     为当前值自身配置验证规则
    /// </summary>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> Rule() => this;

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

        // 释放单个值验证器资源
        if (_valueValidator is IDisposable valueValidatorDisposable)
        {
            valueValidatorDisposable.Dispose();
        }
    }

    /// <summary>
    ///     检查是否应该对该对象执行验证
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldValidate(T? value, string?[]? ruleSets = null)
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
}