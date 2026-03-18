// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证规则的执行聚合模式
/// </summary>
public enum RuleMode
{
    /// <summary>
    ///     所有验证器都会执行，且全部必须通过
    /// </summary>
    All = 0,

    /// <summary>
    ///     遇到首个验证失败即停止后续验证
    /// </summary>
    FailFast,

    /// <summary>
    ///     任一验证器验证成功，即视为整体验证通过
    /// </summary>
    Any
}