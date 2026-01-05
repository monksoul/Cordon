// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     支持成员路径修复的验证器
/// </summary>
public interface IMemberPathRepairable
{
    /// <summary>
    ///     修复验证器及其子验证器的成员路径
    /// </summary>
    /// <param name="memberPath">对象图中的属性路径</param>
    void RepairMemberPaths(string? memberPath);
}