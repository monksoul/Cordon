// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证器的初始化行为接口
/// </summary>
public interface IValidatorInitializer
{
    /// <summary>
    ///     同步 <see cref="IServiceProvider" /> 委托
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider" /> 委托</param>
    void InitializeServiceProvider(Func<Type, object?>? serviceProvider);
}