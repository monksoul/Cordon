// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     定义可配置是否启用基于特性的验证行为
/// </summary>
public interface IValidationAttributeConfigurable
{
    /// <summary>
    ///     配置是否启用对象属性验证特性验证
    /// </summary>
    /// <param name="enabled">是否启用</param>
    void UseAttributeValidation(bool enabled);
}