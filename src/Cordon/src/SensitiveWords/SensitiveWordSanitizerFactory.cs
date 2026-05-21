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
    internal static readonly ConcurrentDictionary<string, Lazy<SensitiveWordSanitizer>> _instances = new();

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="words">敏感词集合</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer GetOrCreate(string name, IEnumerable<string> words, bool ignoreCase = true,
        bool ignoreSymbol = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(words);

        return GetOrCreate(name, () => SensitiveWordSanitizer.Build(words, ignoreCase, ignoreSymbol));
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static SensitiveWordSanitizer GetOrCreateFromPath(string name, string filePath, bool ignoreCase = true,
        bool ignoreSymbol = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        return GetOrCreate(name, () => SensitiveWordSanitizer.CreateFromPath(filePath, ignoreCase, ignoreSymbol));
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="stream">输入流</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer GetOrCreateFromStream(string name, Stream stream, bool ignoreCase = true,
        bool ignoreSymbol = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(stream);

        return GetOrCreate(name, () => SensitiveWordSanitizer.CreateFromStream(stream, ignoreCase, ignoreSymbol));
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例（核心方法）
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="factory">构建敏感词清理器的工厂委托</param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static SensitiveWordSanitizer GetOrCreate(string name, Func<SensitiveWordSanitizer> factory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(factory);

        return _instances.GetOrAdd(name,
            _ => new Lazy<SensitiveWordSanitizer>(factory, LazyThreadSafetyMode.ExecutionAndPublication)).Value;
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="words">敏感词集合</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Refresh(string name, IEnumerable<string> words, bool ignoreCase = true,
        bool ignoreSymbol = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(words);

        Refresh(name, () => SensitiveWordSanitizer.Build(words, ignoreCase, ignoreSymbol));
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="filePath">文件路径</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static void RefreshFromPath(string name, string filePath, bool ignoreCase = true, bool ignoreSymbol = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        Refresh(name, () => SensitiveWordSanitizer.CreateFromPath(filePath, ignoreCase, ignoreSymbol));
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="stream">输入流</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void RefreshFromStream(string name, Stream stream, bool ignoreCase = true, bool ignoreSymbol = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(stream);

        Refresh(name, () => SensitiveWordSanitizer.CreateFromStream(stream, ignoreCase, ignoreSymbol));
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例（核心方法）
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <param name="factory">构建敏感词清理器的工厂委托</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Refresh(string name, Func<SensitiveWordSanitizer> factory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(factory);

        // 提前执行工厂，确保新实例构建成功后才替换缓存
        var instance = factory();
        var newLazy = new Lazy<SensitiveWordSanitizer>(() => instance, LazyThreadSafetyMode.ExecutionAndPublication);

        _instances.AddOrUpdate(name, newLazy, (_, _) => newLazy);
    }

    /// <summary>
    ///     移除指定名称的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">实例名称</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool TryRemove(string name)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(name);

        return _instances.TryRemove(name, out _);
    }

    /// <summary>
    ///     清除所有缓存的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    public static void Clear() => _instances.Clear();
}