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
    ///     敏感词清理器缓存字典
    /// </summary>
    internal static readonly ConcurrentDictionary<string, SanitizerEntry> _instances =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     获取已缓存的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static SensitiveWordSanitizer Get(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 尝试获取敏感词清理器缓存条目
        if (_instances.TryGetValue(name, out var entry))
        {
            return entry.LazyInstance.Value;
        }

        throw new InvalidOperationException(
            $"The sensitive word dictionary '{name}' has not been registered. Please register it using `SensitiveWordSanitizerFactory.GetOrCreate` at application startup.");
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="words">敏感词集合</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer GetOrCreate(string name, IEnumerable<string> words,
        SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(words);

        return GetOrCreate(name, () => SensitiveWordSanitizer.Build(words, options));
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <remarks>以规范化的文件路径作为缓存的键。</remarks>
    /// <param name="filePath">文件路径</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public static SensitiveWordSanitizer GetOrCreateFromPath(string filePath, SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 解析（规范化）文件路径
        var resolvedPath = ResolveFilePath(filePath);

        return GetOrCreate(resolvedPath,
            () => SensitiveWordSanitizer.CreateFromPath(resolvedPath, options));
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer GetOrCreateFromPath(string name, string filePath,
        SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 解析（规范化）文件路径
        var resolvedPath = ResolveFilePath(filePath);

        return GetOrCreate(name, () => SensitiveWordSanitizer.CreateFromPath(resolvedPath, options));
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="stream">输入流</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer GetOrCreateFromStream(string name, Stream stream,
        SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(stream);

        return GetOrCreate(name, () => SensitiveWordSanitizer.CreateFromStream(stream, options));
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例（核心方法）
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="factory">构建敏感词清理器的工厂委托</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer GetOrCreate(string name, Func<SensitiveWordSanitizer> factory)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(factory);

        // 获取或添加敏感词清理器缓存条目
        var entry = _instances.GetOrAdd(name,
            _ => new SanitizerEntry(factory,
                new Lazy<SensitiveWordSanitizer>(factory, LazyThreadSafetyMode.ExecutionAndPublication)));

        try
        {
            return entry.LazyInstance.Value;
        }
        catch
        {
            // 处理 Lazy<T> 会缓存异常问题
            ((ICollection<KeyValuePair<string, SanitizerEntry>>)_instances).Remove(
                new KeyValuePair<string, SanitizerEntry>(name, entry));

            throw;
        }
    }

    /// <summary>
    ///     使用原始配置重新刷新 <see cref="SensitiveWordSanitizer" /> 实例（热更新）
    /// </summary>
    /// <remarks>注意：若实例最初是通过 <see cref="GetOrCreateFromStream" /> 注册的，传入的流必须支持 <c>CanSeek</c>（如 FileStream），否则刷新时将读取到空数据。</remarks>
    /// <param name="name">实例名称</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static void Refresh(string name)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        // 尝试获取敏感词清理器缓存条目
        if (!_instances.TryGetValue(name, out var oldEntry))
        {
            throw new InvalidOperationException(
                $"The sensitive word dictionary '{name}' has not been registered. Cannot refresh an unregistered dictionary.");
        }

        // 提前执行原始 Factory，确保新实例构建成功后才替换缓存
        var instance = oldEntry.Factory();
        var newLazy = new Lazy<SensitiveWordSanitizer>(() => instance, LazyThreadSafetyMode.PublicationOnly);
        var newEntry = new SanitizerEntry(oldEntry.Factory, newLazy);

        _instances.AddOrUpdate(name, newEntry, (_, _) => newEntry);
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="words">敏感词集合</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Refresh(string name, IEnumerable<string> words, SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(words);

        Refresh(name, () => SensitiveWordSanitizer.Build(words, options));
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <remarks>以规范化的文件路径作为缓存的键。</remarks>
    /// <param name="filePath">文件路径</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <exception cref="ArgumentException"></exception>
    public static void RefreshFromPath(string filePath, SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 解析（规范化）文件路径
        var resolvedPath = ResolveFilePath(filePath);

        Refresh(resolvedPath, () => SensitiveWordSanitizer.CreateFromPath(resolvedPath, options));
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RefreshFromPath(string name, string filePath, SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 解析（规范化）文件路径
        var resolvedPath = ResolveFilePath(filePath);

        Refresh(name, () => SensitiveWordSanitizer.CreateFromPath(resolvedPath, options));
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="stream">输入流</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RefreshFromStream(string name, Stream stream, SensitiveWordOptions? options = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(stream);

        Refresh(name, () => SensitiveWordSanitizer.CreateFromStream(stream, options));
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例（核心方法）
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="factory">构建敏感词清理器的工厂委托</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static void Refresh(string name, Func<SensitiveWordSanitizer> factory)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(factory);

        // 提前执行工厂，确保新实例构建成功后才替换缓存
        var instance = factory();
        var newLazy = new Lazy<SensitiveWordSanitizer>(() => instance, LazyThreadSafetyMode.PublicationOnly);
        var newEntry = new SanitizerEntry(factory, newLazy);

        _instances.AddOrUpdate(name, newEntry, (_, _) => newEntry);
    }

    /// <summary>
    ///     移除指定名称的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <remarks>注意：若使用文件路径作为缓存的键，那么请使用 <c>Path.GetFullPath(filePath)</c> 来标准化文件路径。</remarks>
    /// <param name="name">实例名称</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool TryRemove(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return _instances.TryRemove(name, out _);
    }

    /// <summary>
    ///     清除所有缓存的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    public static void Clear() => _instances.Clear();

    /// <summary>
    ///     解析文件路径
    /// </summary>
    /// <remarks>如果已是绝对路径，直接返回。如果是相对路径，基于 <see cref="AppContext.BaseDirectory" /> 解析。</remarks>
    /// <param name="filePath">文件路径</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string ResolveFilePath(string filePath)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 处理绝对和相对路径问题
        var basePath = Path.IsPathRooted(filePath)
            ? filePath
            : Path.Combine(AppContext.BaseDirectory, filePath);

        return Path.GetFullPath(basePath);
    }

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