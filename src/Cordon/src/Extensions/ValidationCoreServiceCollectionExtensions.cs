// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     数据验证模块 <see cref="IServiceCollection" /> 拓展类
/// </summary>
public static class ValidationCoreServiceCollectionExtensions
{
    /// <summary>
    ///     添加数据验证服务
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="IServiceCollection" />
    /// </returns>
    public static IServiceCollection AddValidationCore(this IServiceCollection services,
        Action<ValidationBuilder>? configure = null)
    {
        // 初始化数据验证构建器
        var validationBuilder = new ValidationBuilder();

        // 调用自定义配置委托
        configure?.Invoke(validationBuilder);

        return services.AddValidationCore(validationBuilder);
    }

    /// <summary>
    ///     添加数据验证服务
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    /// <param name="validationBuilder">
    ///     <see cref="ValidationBuilder" />
    /// </param>
    /// <returns>
    ///     <see cref="IServiceCollection" />
    /// </returns>
    public static IServiceCollection AddValidationCore(this IServiceCollection services,
        ValidationBuilder validationBuilder)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationBuilder);

        // 构建模块服务
        validationBuilder.Build(services);

        return services;
    }
}