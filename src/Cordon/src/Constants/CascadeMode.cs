// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     属性验证级联模式
/// </summary>
public enum CascadeMode
{
    /// <summary>
    ///     所有属性都会验证，收集全部错误信息
    /// </summary>
    All = 0,

    /// <summary>
    ///     遇到第一个属性验证失败即停止验证后续属性
    /// </summary>
    FailFast
}