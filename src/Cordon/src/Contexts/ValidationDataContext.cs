// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <inheritdoc />
internal sealed class ValidationDataContext : IValidationDataContext
{
    /// <summary>
    ///     验证选项键
    /// </summary>
    /// <remarks>用于 <see cref="ValidationContext" /> 或 <c>ValidationOptionsModelValidator</c> 中写入规则集配置。</remarks>
    internal static readonly object ValidationOptionsKey = new();

    /// <inheritdoc />
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

    /// <inheritdoc />
    public void SetValue(object key, object? value)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(key);

        Items[key] = value;
    }

    /// <inheritdoc />
    public bool TryGetValue(object key, out object? value)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(key);

        return Items.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    public bool ContainsKey(object key)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(key);

        return Items.ContainsKey(key);
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