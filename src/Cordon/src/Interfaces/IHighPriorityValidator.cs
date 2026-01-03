// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     高优先级的验证器接口
/// </summary>
/// <remarks>会在验证流程中优先执行。</remarks>
public interface IHighPriorityValidator
{
    /// <summary>
    ///     验证器的优先级值
    /// </summary>
    /// <remarks>数值越小，优先级越高（越先执行）。</remarks>
    int Priority { get; }
}