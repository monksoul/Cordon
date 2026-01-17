// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     提取验证选项元数据（<see cref="ValidationOptionsAttribute" />）的模型验证器提供器
/// </summary>
/// <remarks>
///     为实现了 <see cref="IValidatableObject" /> 接口的模型和 <see cref="ValidateWithAttribute{TValidator}" />
///     特性提供验证选项（如规则集）支持。
/// </remarks>
internal sealed class ValidationOptionsModelValidatorProvider : IModelValidatorProvider
{
    /// <inheritdoc />
    public void CreateValidators(ModelValidatorProviderContext context) =>
        context.Results.Add(new ValidatorItem
        {
            Validator = new ValidationOptionsModelValidator(), IsReusable = true /*确保验证器实例可以被重用*/
        });
}