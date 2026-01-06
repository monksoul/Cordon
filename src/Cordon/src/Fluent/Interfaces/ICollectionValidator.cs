// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     集合验证器服务
/// </summary>
/// <typeparam name="TElement">元素类型</typeparam>
public interface ICollectionValidator<in TElement> : IObjectValidator<IEnumerable<TElement>>, IMemberPathRepairable;