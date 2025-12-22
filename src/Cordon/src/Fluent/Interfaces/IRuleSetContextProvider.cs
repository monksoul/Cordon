// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     规则集上下文提供器
/// </summary>
public interface IRuleSetContextProvider
{
    /// <summary>
    ///     获取当前上下文中的规则集
    /// </summary>
    /// <returns><see cref="string" />[]</returns>
    string?[]? GetCurrentRuleSets();
}