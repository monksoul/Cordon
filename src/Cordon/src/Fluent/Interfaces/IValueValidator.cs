// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     单个值验证器服务
/// </summary>
public interface IValueValidator;

/// <summary>
///     <inheritdoc cref="IValueValidator" />
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public interface IValueValidator<in T> : IValueValidator, IObjectValidator<T>, IRuleSetContextProvider;