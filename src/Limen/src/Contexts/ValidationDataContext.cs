// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <inheritdoc />
internal sealed class ValidationDataContext : IValidationDataContext
{
    /// <summary>
    ///     验证选项键
    /// </summary>
    /// <remarks>用于 <see cref="ValidationContext" /> 或 <c>ValidationOptionsModelValidator</c> 中写入规则集配置。</remarks>
    internal static readonly object ValidationOptionsKey = new();

    /// <summary>
    ///     存储验证数据的内部字典
    /// </summary>
    internal readonly Dictionary<object, object?> _items = new();

    /// <inheritdoc />
    public IDictionary<object, object?> Items => _items;

    /// <inheritdoc />
    public void SetValue(object key, object? value)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(key);

        _items[key] = value;
    }

    /// <inheritdoc />
    public bool TryGetValue(object key, out object? value)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(key);

        return _items.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    public bool ContainsKey(object key)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(key);

        return _items.ContainsKey(key);
    }

    /// <inheritdoc />
    public ValidationOptionsMetadata? GetValidationOptions() =>
        TryGetValue(ValidationOptionsKey, out var metadataObj) && metadataObj is ValidationOptionsMetadata metadata
            ? metadata
            : null;

    /// <inheritdoc />
    public void SetValidationOptions(ValidationOptionsMetadata metadata)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(metadata);

        SetValue(ValidationOptionsKey, metadata);
    }

    /// <inheritdoc />
    public bool HasValidationOptions() => ContainsKey(ValidationOptionsKey);
}