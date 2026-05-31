// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     敏感词清理器工厂
/// </summary>
public static class SensitiveWordSanitizerFactory
{
    /// <summary>
    ///     默认字典名称
    /// </summary>
    public const string DefaultName = "SensitiveWords:Default";

    /// <summary>
    ///     敏感词清理器缓存字典
    /// </summary>
    internal static readonly ConcurrentDictionary<string, SanitizerEntry> _instances =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     获取已缓存的默认 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <remarks>使用默认字典名称：<see cref="SensitiveWordSanitizerFactory.DefaultName" />。</remarks>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public static SensitiveWordSanitizer Get() => Get(DefaultName);

    /// <summary>
    ///     获取已缓存的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="dictionaryName">字典名称，不区分大小写</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static SensitiveWordSanitizer Get(string dictionaryName)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(dictionaryName);

        // 尝试获取敏感词清理器缓存条目
        if (_instances.TryGetValue(dictionaryName, out var entry))
        {
            return entry.LazyInstance.Value;
        }

        throw new InvalidOperationException(
            $"The sensitive word dictionary '{dictionaryName}' has not been registered. Please register it using `SensitiveWordSanitizerFactory.GetOrCreate` at application startup.");
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <remarks>使用默认字典名称：<see cref="SensitiveWordSanitizerFactory.DefaultName" />。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public static SensitiveWordSanitizer GetOrCreate(Action<SensitiveWordSanitizerBuilder> configure) =>
        GetOrCreate(DefaultName, configure);

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="dictionaryName">字典名称，不区分大小写</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer GetOrCreate(string dictionaryName,
        Action<SensitiveWordSanitizerBuilder> configure)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(dictionaryName);
        ArgumentNullException.ThrowIfNull(configure);

        return GetOrCreate(dictionaryName, () =>
        {
            // 初始化 SensitiveWordSanitizerBuilder 实例
            var builder = new SensitiveWordSanitizerBuilder();

            // 调用自定义配置委托
            configure(builder);

            // 构建 SensitiveWordSanitizer 实例
            return builder.Build();
        });
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="dictionaryName">字典名称，不区分大小写</param>
    /// <param name="factory">构建 <see cref="SensitiveWordSanitizer" /> 的工厂委托</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public static SensitiveWordSanitizer GetOrCreate(string dictionaryName, Func<SensitiveWordSanitizer> factory)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(dictionaryName);
        ArgumentNullException.ThrowIfNull(factory);

        // 获取或添加敏感词清理器缓存条目
        var entry = _instances.GetOrAdd(dictionaryName,
            _ => new SanitizerEntry(factory,
                new Lazy<SensitiveWordSanitizer>(factory, LazyThreadSafetyMode.ExecutionAndPublication)));

        try
        {
            return entry.LazyInstance.Value;
        }
        catch
        {
            // 构建失败时移除缓存条目，避免 Lazy 缓存异常
            _instances.TryRemove(dictionaryName, out _);

            throw;
        }
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <remarks>
    ///     <para>使用默认字典名称：<see cref="SensitiveWordSanitizerFactory.DefaultName" />。</para>
    ///     <para>当 <paramref name="configure" /> 为 <c>null</c> 时，使用注册时的配置重新构建实例（热更新）。</para>
    ///     <para>当 <paramref name="configure" /> 不为 <c>null</c> 时，将用新配置替换原有构建逻辑并立即生效。</para>
    /// </remarks>
    /// <param name="configure">自定义配置委托</param>
    public static void Refresh(Action<SensitiveWordSanitizerBuilder>? configure = null) =>
        Refresh(DefaultName, configure);

    /// <summary>
    ///     刷新指定名称的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <remarks>
    ///     <para>当 <paramref name="configure" /> 为 <c>null</c> 时，使用注册时的配置重新构建实例（热更新）。</para>
    ///     <para>当 <paramref name="configure" /> 不为 <c>null</c> 时，将用新配置替换原有构建逻辑并立即生效。</para>
    /// </remarks>
    /// <param name="dictionaryName">字典名称，不区分大小写</param>
    /// <param name="configure">自定义配置委托</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static void Refresh(string dictionaryName, Action<SensitiveWordSanitizerBuilder>? configure = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(dictionaryName);

        // 空检查
        if (configure is not null)
        {
            Refresh(dictionaryName, () =>
            {
                // 初始化 SensitiveWordSanitizerBuilder 实例
                var builder = new SensitiveWordSanitizerBuilder();

                // 调用自定义配置委托
                configure(builder);

                // 构建 SensitiveWordSanitizer 实例
                return builder.Build();
            });
        }
        else
        {
            // 尝试获取敏感词清理器缓存条目
            if (!_instances.TryGetValue(dictionaryName, out var oldEntry))
            {
                throw new InvalidOperationException(
                    $"The sensitive word dictionary '{dictionaryName}' has not been registered. Cannot refresh an unregistered dictionary.");
            }

            Refresh(dictionaryName, oldEntry.Factory);
        }
    }

    /// <summary>
    ///     刷新指定名称的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="dictionaryName">字典名称，不区分大小写</param>
    /// <param name="factory">构建 <see cref="SensitiveWordSanitizer" /> 的工厂委托</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Refresh(string dictionaryName, Func<SensitiveWordSanitizer> factory)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(dictionaryName);
        ArgumentNullException.ThrowIfNull(factory);

        // 先构建实例，确保成功后再替换缓存
        var instance = factory();
        var newLazy = new Lazy<SensitiveWordSanitizer>(() => instance, LazyThreadSafetyMode.PublicationOnly);
        var newEntry = new SanitizerEntry(factory, newLazy);

        _instances.AddOrUpdate(dictionaryName, newEntry, (_, _) => newEntry);
    }

    /// <summary>
    ///     获取所有已注册的字典名称
    /// </summary>
    /// <returns>
    ///     <see cref="ICollection{T}" />
    /// </returns>
    public static ICollection<string> GetNames() => _instances.Keys;

    /// <summary>
    ///     移除指定名称的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="dictionaryName">字典名称，不区分大小写</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="ArgumentException">字典名称为空</exception>
    public static bool TryRemove(string dictionaryName)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(dictionaryName);

        return _instances.TryRemove(dictionaryName, out _);
    }

    /// <summary>
    ///     清除所有缓存的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    public static void Clear() => _instances.Clear();

    /// <summary>
    ///     敏感词清理器缓存条目
    /// </summary>
    internal sealed class SanitizerEntry
    {
        /// <summary>
        ///     <inheritdoc cref="SanitizerEntry" />
        /// </summary>
        /// <param name="factory">原始的构建委托</param>
        /// <param name="lazyInstance">线程安全的延迟初始化实例</param>
        internal SanitizerEntry(Func<SensitiveWordSanitizer> factory, Lazy<SensitiveWordSanitizer> lazyInstance)
        {
            Factory = factory;
            LazyInstance = lazyInstance;
        }

        /// <summary>
        ///     原始的构建委托
        /// </summary>
        internal Func<SensitiveWordSanitizer> Factory { get; }

        /// <summary>
        ///     线程安全的延迟初始化实例
        /// </summary>
        internal Lazy<SensitiveWordSanitizer> LazyInstance { get; }
    }
}