// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     组合验证器模式
/// </summary>
public enum ValidationMode
{
    /// <summary>
    ///     验证所有
    /// </summary>
    ValidateAll = 0,

    /// <summary>
    ///     首个验证成功则视为通过
    /// </summary>
    BreakOnFirstSuccess,

    /// <summary>
    ///     首个验证失败则停止验证
    /// </summary>
    BreakOnFirstError
}