// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <inheritdoc />
/// <remarks>用于兼容旧版本 <see cref="ValidatorBase" /> 相关方法。</remarks>
internal sealed class LegacyValidationContext : IValidationContext
{
    /// <summary>
    ///     <inheritdoc cref="LegacyValidationContext" />
    /// </summary>
    internal LegacyValidationContext()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="LegacyValidationContext" />
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="displayName">显示名称</param>
    /// <param name="memberNames">成员名称列表</param>
    internal LegacyValidationContext(object? instance, string? displayName, IEnumerable<string>? memberNames)
    {
        Instance = instance;
        DisplayName = displayName!;
        MemberNames = memberNames;
    }

    /// <inheritdoc />
    public object? Instance { get; set; }

    /// <inheritdoc />
    public string DisplayName { get => field ?? MemberNames?.FirstOrDefault() ?? Instance?.GetType().Name!; set; }

    /// <inheritdoc />
    public IEnumerable<string>? MemberNames { get; set; }

    /// <inheritdoc />
    public string?[]? RuleSets { get; set; }

    /// <inheritdoc />
    public IDictionary<object, object?> Items { get; set; } = new Dictionary<object, object?>();

    /// <inheritdoc />
    public object? GetService(Type serviceType) => null;
}