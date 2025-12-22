// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证选项模型验证器提供程序
/// </summary>
internal sealed class ValidationOptionsModelValidatorProvider : IModelValidatorProvider
{
    /// <inheritdoc />
    public void CreateValidators(ModelValidatorProviderContext context)
    {
        // 检查是否实现了 IValidatableObject 接口
        if (typeof(IValidatableObject).IsAssignableFrom(context.ModelMetadata.ModelType))
        {
            context.Results.Add(new ValidatorItem
            {
                Validator = new ValidationOptionsModelValidator(), IsReusable = true
            });
        }
    }
}