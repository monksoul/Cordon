// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <inheritdoc cref="IValidationContext" />
/// <typeparam name="T">对象类型</typeparam>
public sealed class ValidationContext<T> : IValidationContext, IValidatorInitializer
{
    /// <summary>
    ///     <see cref="IServiceProvider" /> 委托
    /// </summary>
    internal Func<Type, object?>? _serviceProvider;

    /// <summary>
    ///     <inheritdoc cref="ValidationContext{T}" />
    /// </summary>
    /// <param name="instance">对象</param>
    public ValidationContext(T instance)
        // ReSharper disable once IntroduceOptionalParameters.Global
        : this(instance, (Func<Type, object?>?)null, null)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValidationContext{T}" />
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="items">共享数据</param>
    public ValidationContext(T instance, IDictionary<object, object?>? items)
        : this(instance, (Func<Type, object?>?)null, items)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValidationContext{T}" />
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    public ValidationContext(T instance, IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
        : this(instance, serviceProvider is null ? null : serviceProvider.GetService, items)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValidationContext{T}" />
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    internal ValidationContext(T instance, Func<Type, object?>? serviceProvider, IDictionary<object, object?>? items)
    {
        Instance = instance;
        _serviceProvider = serviceProvider;
        Items = items is not null ? new Dictionary<object, object?>(items) : new Dictionary<object, object?>();
    }

    /// <inheritdoc cref="IValidationContext.Instance" />
    public T Instance { get; }

    /// <inheritdoc />
    object? IValidationContext.Instance => Instance;

    /// <inheritdoc />
    public string DisplayName { get; init; } = null!;

    /// <inheritdoc />
    public IEnumerable<string>? MemberNames { get; init; }

    /// <inheritdoc />
    public string?[]? RuleSets { get; init; }

    /// <inheritdoc />
    public IDictionary<object, object?> Items { get; }

    /// <inheritdoc />
    public object? GetService(Type serviceType) => _serviceProvider?.Invoke(serviceType);

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider) => _serviceProvider = serviceProvider;
}