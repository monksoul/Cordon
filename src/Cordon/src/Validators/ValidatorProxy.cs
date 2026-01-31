// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证器代理
/// </summary>
/// <typeparam name="TValidator">
///     <see cref="ValidatorBase" />
/// </typeparam>
public class ValidatorProxy<TValidator> : ValidatorBase, IValidatorInitializer, IDisposable
    where TValidator : ValidatorBase
{
    /// <summary>
    ///     <typeparamref name="TValidator" /> 实例
    /// </summary>
    internal readonly TValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="ValidatorProxy{TValidator}" />
    /// </summary>
    /// <param name="constructorArgs"><typeparamref name="TValidator" /> 构造函数参数列表</param>
    public ValidatorProxy(params object?[]? constructorArgs)
    {
        _validator = (TValidator)Activator.CreateInstance(typeof(TValidator), constructorArgs)!;

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <summary>
    ///     配置验证器实例
    /// </summary>
    /// <param name="predicate">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValidatorProxy{TValidator}" />
    /// </returns>
    public ValidatorProxy<TValidator> Configure(Action<TValidator> predicate)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(predicate);

        predicate(_validator);

        return this;
    }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        _validator.IsValid(value, validationContext);

    /// <inheritdoc />
    public override List<ValidationResult>?
        GetValidationResults(object? value, IValidationContext? validationContext) =>
        _validator.GetValidationResults(value, validationContext);

    /// <inheritdoc />
    public override void Validate(object? value, IValidationContext? validationContext) =>
        _validator.Validate(value, validationContext);

    /// <inheritdoc />
    public override string? FormatErrorMessage(string name) => _validator.FormatErrorMessage(name);

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

        // 移除属性变更事件
        PropertyChanged -= OnPropertyChanged;

        // 释放验证器资源
        if (_validator is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>
    ///     订阅属性变更事件
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="eventArgs">
    ///     <see cref="ValidationPropertyChangedEventArgs" />
    /// </param>
    internal void OnPropertyChanged(object? sender, ValidationPropertyChangedEventArgs eventArgs)
    {
        // 根据变更的属性名查找对应的验证器实例属性
        var validatorProperty =
            typeof(TValidator).GetProperty(eventArgs.PropertyName!, BindingFlags.Instance | BindingFlags.Public);

        // 检查验证器实例属性是否可写
        if (validatorProperty is not { CanWrite: true })
        {
            return;
        }

        // 设置验证器实例属性值
        validatorProperty.SetValue(_validator, eventArgs.PropertyValue);
    }

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 检查验证器是否实现 IValidatorInitializer 接口
        if (_validator is IValidatorInitializer initializer)
        {
            // 同步 IServiceProvider 委托
            initializer.InitializeServiceProvider(serviceProvider);
        }
    }
}

/// <summary>
///     验证器代理
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TValidator">
///     <see cref="ValidatorBase" />
/// </typeparam>
public class ValidatorProxy<T, TValidator> : ValidatorBase<T>, IValidatorInitializer, IDisposable
    where TValidator : ValidatorBase
{
    /// <summary>
    ///     <typeparamref name="TValidator" /> 构造函数参数工厂
    /// </summary>
    internal readonly Func<T, ValidationContext<T>, object?[]?>? _constructorArgsFactory;

    /// <summary>
    ///     属性变更字典
    /// </summary>
    internal readonly ConcurrentDictionary<string, object?> _propertyChanges = new();

    /// <summary>
    ///     用于执行验证的对象工厂
    /// </summary>
    internal readonly Func<T, object?> _validatingObjectFactory;

    /// <summary>
    ///     验证器实例缓存字典
    /// </summary>
    internal readonly ConcurrentDictionary<int, TValidator> _validatorCache = new();

    /// <summary>
    ///     验证器实例配置委托集合
    /// </summary>
    internal readonly List<Action<TValidator>> _validatorConfigurations = [];

    /// <summary>
    ///     <inheritdoc cref="ValidatorProxy{TValidator}" />
    /// </summary>
    /// <param name="validatingObjectFactory">用于执行验证的对象工厂</param>
    /// <param name="constructorArgsFactory"><typeparamref name="TValidator" /> 构造函数参数工厂</param>
    public ValidatorProxy(Func<T, object?> validatingObjectFactory,
        Func<T, ValidationContext<T>, object?[]?>? constructorArgsFactory = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatingObjectFactory);

        _validatingObjectFactory = validatingObjectFactory;
        _constructorArgsFactory = constructorArgsFactory;

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <summary>
    ///     配置验证器实例
    /// </summary>
    /// <param name="predicate">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValidatorProxy{TValidator}" />
    /// </returns>
    public ValidatorProxy<T, TValidator> Configure(Action<TValidator> predicate)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(predicate);

        _validatorConfigurations.Add(predicate);

        // 清除缓存以确保新实例获取最新属性
        _validatorCache.Clear();

        return this;
    }

    /// <inheritdoc />
    public override bool IsValid(T? instance, ValidationContext<T> validationContext) =>
        GetValidator(instance, validationContext).IsValid(GetValidatingObject(instance), validationContext);

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext) =>
        GetValidator(instance, validationContext)
            .GetValidationResults(GetValidatingObject(instance), validationContext);

    /// <inheritdoc />
    public override void Validate(T? instance, ValidationContext<T> validationContext) =>
        GetValidator(instance, validationContext).Validate(GetValidatingObject(instance), validationContext);

    /// <inheritdoc />
    /// <exception cref="NotSupportedException"></exception>
    public sealed override string FormatErrorMessage(string name) =>
        throw new NotSupportedException("Use FormatErrorMessage(string name, T? instance) instead.");

    /// <summary>
    ///     格式化错误信息
    /// </summary>
    /// <param name="name">显示名称</param>
    /// <param name="instance">对象</param>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public virtual string? FormatErrorMessage(string name, T? instance, ValidationContext<T> validationContext) =>
        GetValidator(instance, validationContext).FormatErrorMessage(name);

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

        // 移除属性变更事件
        PropertyChanged -= OnPropertyChanged;

        // 释放所有验证器资源
        foreach (var validator in _validatorCache.Values)
        {
            if (validator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    ///     获取或创建被代理的验证器实例
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <returns>
    ///     <typeparamref name="TValidator" />
    /// </returns>
    internal TValidator GetValidator(T? instance, ValidationContext<T> validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(validationContext);

        return _validatorCache.GetOrAdd(RuntimeHelpers.GetHashCode(instance), _ =>
        {
            // 反射创建验证器实例
            var validator = _constructorArgsFactory is null
                ? Activator.CreateInstance<TValidator>()
                : (TValidator)Activator.CreateInstance(typeof(TValidator),
                    _constructorArgsFactory.Invoke(instance, validationContext))!;

            // 应用属性变更到验证器
            ApplyPropertyChanges(validator);

            // 应用验证器实例配置
            foreach (var configuredValidator in _validatorConfigurations)
            {
                configuredValidator.Invoke(validator);
            }

            return validator;
        });
    }

    /// <summary>
    ///     获取用于执行验证的对象
    /// </summary>
    /// <remarks>用于确定在 <see cref="ValidatorBase" /> 中实际被验证的对象（即验证的主体）。</remarks>
    /// <param name="instance">对象</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    internal object? GetValidatingObject(T? instance)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        return _validatingObjectFactory.Invoke(instance);
    }

    /// <summary>
    ///     应用属性变更到验证器
    /// </summary>
    /// <param name="validator">
    ///     <typeparamref name="TValidator" />
    /// </param>
    internal void ApplyPropertyChanges(TValidator validator)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validator);

        // 遍历所有变更的属性
        foreach (var (propertyName, propertyValue) in _propertyChanges)
        {
            // 根据变更的属性名查找对应的验证器实例属性
            var validatorProperty =
                typeof(TValidator).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            // 检查验证器实例属性是否可写
            if (validatorProperty is not { CanWrite: true })
            {
                continue;
            }

            validatorProperty.SetValue(validator, propertyValue);
        }
    }


    /// <summary>
    ///     订阅属性变更事件
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="eventArgs">
    ///     <see cref="ValidationPropertyChangedEventArgs" />
    /// </param>
    internal void OnPropertyChanged(object? sender, ValidationPropertyChangedEventArgs eventArgs)
    {
        _propertyChanges[eventArgs.PropertyName!] = eventArgs.PropertyValue;

        // 清除缓存以确保新实例获取最新属性
        _validatorCache.Clear();
    }

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 遍历所有验证器并尝试同步 IServiceProvider 委托
        foreach (var validator in _validatorCache.Values)
        {
            // 检查验证器是否实现 IValidatorInitializer 接口
            if (validator is IValidatorInitializer initializer)
            {
                // 同步 IServiceProvider 委托
                initializer.InitializeServiceProvider(serviceProvider);
            }
        }
    }
}