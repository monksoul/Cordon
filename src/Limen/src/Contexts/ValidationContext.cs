// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     验证上下文
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public sealed class ValidationContext<T> : IValidatorInitializer, IServiceProvider
{
    /// <summary>
    ///     <see cref="IServiceProvider" /> 委托
    /// </summary>
    internal Func<Type, object?>? _serviceProvider;

    /// <summary>
    ///     <inheritdoc cref="ValidationContext{T}" />
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">验证上下文数据</param>
    internal ValidationContext(T instance, IServiceProvider? serviceProvider,
        IReadOnlyDictionary<object, object?>? items)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        Instance = instance;

        // 空检查
        if (serviceProvider is not null)
        {
            _serviceProvider = serviceProvider.GetService;
        }

        Items = items is not null ? new Dictionary<object, object?>(items) : new Dictionary<object, object?>();
    }

    /// <summary>
    ///     对象
    /// </summary>
    public T Instance { get; }

    /// <summary>
    ///     验证上下文数据
    /// </summary>
    public IReadOnlyDictionary<object, object?> Items { get; }

    /// <summary>
    ///     解析服务
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    public object? GetService(Type serviceType) => _serviceProvider?.Invoke(serviceType);

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider) => _serviceProvider = serviceProvider;
}