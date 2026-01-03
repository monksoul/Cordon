// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证上下文
/// </summary>
public interface IValidationContext : IServiceProvider
{
    /// <summary>
    ///     对象
    /// </summary>
    object? Instance { get; }

    /// <summary>
    ///     显示名称
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    ///     成员名称列表
    /// </summary>
    IEnumerable<string>? MemberNames { get; }

    /// <summary>
    ///     规则集
    /// </summary>
    string?[]? RuleSets { get; }

    /// <summary>
    ///     共享数据
    /// </summary>
    IDictionary<object, object?> Items { get; }
}