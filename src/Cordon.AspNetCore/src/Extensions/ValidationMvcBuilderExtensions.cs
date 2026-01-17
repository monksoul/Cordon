// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     数据验证模块 <see cref="IMvcBuilder" /> 拓展类
/// </summary>
public static class ValidationMvcBuilderExtensions
{
    /// <summary>
    ///     添加数据验证服务
    /// </summary>
    /// <param name="mvcBuilder">
    ///     <see cref="IMvcBuilder" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="IMvcBuilder" />
    /// </returns>
    public static IMvcBuilder AddValidationCore(this IMvcBuilder mvcBuilder,
        Action<ValidationBuilder>? configure = null)
    {
        // 添加数据验证服务
        mvcBuilder.Services.AddValidationCore(configure);

        // 添加验证选项模型验证器提供器
        mvcBuilder.AddMvcOptions(options =>
        {
            if (!options.ModelValidatorProviders.OfType<ValidationOptionsModelValidatorProvider>().Any())
            {
                options.ModelValidatorProviders.Insert(0, new ValidationOptionsModelValidatorProvider());
            }

            if (!options.Filters.OfType<ValidationOptionsAsyncPageFilter>().Any())
            {
                options.Filters.Add(new ValidationOptionsAsyncPageFilter());
            }
        });

        return mvcBuilder;
    }
}