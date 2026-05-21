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
    /// <remarks><c>null</c> 或空字符串表示默认实例。</remarks>
    internal static readonly ConcurrentDictionary<string, Lazy<SensitiveWordSanitizer>> _instances = new();

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="words">敏感词集合</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public static SensitiveWordSanitizer GetOrCreate(IEnumerable<string> words, bool ignoreCase = true,
        bool ignoreSymbol = true) =>
        GetOrCreate(null, () => SensitiveWordSanitizer.Build(words, ignoreCase, ignoreSymbol));

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public static SensitiveWordSanitizer GetOrCreateFromPath(string filePath, bool ignoreCase = true,
        bool ignoreSymbol = true)
    {
        // 规范化文件路径作为缓存键
        var name = NormalizeFilePath(filePath);

        return GetOrCreate(name, () => SensitiveWordSanitizer.CreateFromPath(filePath, ignoreCase, ignoreSymbol));
    }

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="stream">输入流</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </returns>
    public static SensitiveWordSanitizer GetOrCreateFromStream(Stream stream, bool ignoreCase = true,
        bool ignoreSymbol = true) => GetOrCreate(null,
        () => SensitiveWordSanitizer.CreateFromStream(stream, ignoreCase, ignoreSymbol));

    /// <summary>
    ///     获取或创建 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">
    ///     实例名称，<c>null</c> 或空字符串表示默认实例
    /// </param>
    /// <param name="factory">构建敏感词清理器的工厂委托</param>
    /// <remarks>
    ///     <see cref="SensitiveWordSanitizer" />
    /// </remarks>
    public static SensitiveWordSanitizer GetOrCreate(string? name, Func<SensitiveWordSanitizer> factory)
    {
        name ??= string.Empty;

        return _instances.GetOrAdd(name,
            _ => new Lazy<SensitiveWordSanitizer>(factory, LazyThreadSafetyMode.ExecutionAndPublication)).Value;
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="words">敏感词集合</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    public static void Refresh(IEnumerable<string> words, bool ignoreCase = true,
        bool ignoreSymbol = true) =>
        Refresh(null, () => SensitiveWordSanitizer.Build(words, ignoreCase, ignoreSymbol));

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    public static void RefreshFromPath(string filePath, bool ignoreCase = true, bool ignoreSymbol = true)
    {
        // 规范化文件路径作为缓存键
        var name = NormalizeFilePath(filePath);

        Refresh(name, () => SensitiveWordSanitizer.CreateFromPath(filePath, ignoreCase, ignoreSymbol));
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="stream">输入流</param>
    /// <param name="ignoreCase">是否忽略大小写，默认值为：<c>true</c></param>
    /// <param name="ignoreSymbol">是否跳过符号匹配，默认值为：<c>true</c></param>
    public static void RefreshFromStream(Stream stream, bool ignoreCase = true, bool ignoreSymbol = true) =>
        Refresh(null, () => SensitiveWordSanitizer.CreateFromStream(stream, ignoreCase, ignoreSymbol));

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">
    ///     实例名称，<c>null</c> 或空字符串表示默认实例
    /// </param>
    /// <param name="factory">构建敏感词清理器的工厂委托</param>
    public static void Refresh(string? name, Func<SensitiveWordSanitizer> factory)
    {
        name ??= string.Empty;

        // 初始化新的惰性 SensitiveWordSanitizer 实例
        var newLazy = new Lazy<SensitiveWordSanitizer>(factory, LazyThreadSafetyMode.ExecutionAndPublication);

        _instances.AddOrUpdate(name, newLazy, (_, _) => newLazy);
    }

    /// <summary>
    ///     刷新 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    /// <param name="name">
    ///     实例名称，<c>null</c> 或空字符串表示默认实例
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool TryRemove(string? name)
    {
        name ??= string.Empty;

        return _instances.TryRemove(name, out _);
    }

    /// <summary>
    ///     清除所有缓存的 <see cref="SensitiveWordSanitizer" /> 实例
    /// </summary>
    public static void Clear() => _instances.Clear();

    /// <summary>
    ///     规范化文件路径作为缓存键
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string NormalizeFilePath(string filePath)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        // 获取完整的文件路径
        var fullPath = Path.GetFullPath(filePath);

        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? fullPath.ToLowerInvariant()
            : fullPath;
    }
}