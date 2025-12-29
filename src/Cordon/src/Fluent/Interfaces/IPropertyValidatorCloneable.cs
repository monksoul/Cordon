// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     定义属性验证器可克隆行为
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public interface IPropertyValidatorCloneable<T>
{
    /// <summary>
    ///     克隆
    /// </summary>
    /// <param name="objectValidator">
    ///     <see cref="ObjectValidator{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="IPropertyValidator{T}" />
    /// </returns>
    IPropertyValidator<T> Clone(ObjectValidator<T> objectValidator);
}