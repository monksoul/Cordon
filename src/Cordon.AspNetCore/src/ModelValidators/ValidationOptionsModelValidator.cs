// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证选项元数据提取验证器
/// </summary>
internal sealed class ValidationOptionsModelValidator : IModelValidator
{
    /// <inheritdoc />
    public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
    {
        // 检查 context.ActionContext.ActionDescriptor 是否是 ControllerActionDescriptor（MVC）
        if (context.ActionContext.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
        {
            yield break;
        }

        // 尝试获取验证数据上下文服务
        if (context.ActionContext.HttpContext.RequestServices.GetService<IValidationDataContext>() is not
            ValidationDataContext validationDataContext)
        {
            yield break;
        }

        // 检查是否已设置验证选项（避免重复提取）
        if (validationDataContext.HasValidationOptions())
        {
            yield break;
        }

        // 提取验证选项
        var validationOptionsMetadata = ExtractFromAction(actionDescriptor.MethodInfo) ??
                                        ExtractFromController(actionDescriptor.ControllerTypeInfo);

        // 空检查
        if (validationOptionsMetadata is not null)
        {
            // 设置当前验证选项
            validationDataContext.SetValidationOptions(validationOptionsMetadata);
        }
    }

    /// <summary>
    ///     从 Action 方法提取验证选项
    /// </summary>
    /// <param name="methodInfo">
    ///     <see cref="MethodInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="ValidationOptionsMetadata" />
    /// </returns>
    internal static ValidationOptionsMetadata? ExtractFromAction(MethodInfo methodInfo) =>
        CreateMetadata(methodInfo.GetCustomAttribute<ValidationOptionsAttribute>(true));

    /// <summary>
    ///     从 Controller 类提取验证选项
    /// </summary>
    /// <param name="declaredTypeInfo">
    ///     <see cref="TypeInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="ValidationOptionsMetadata" />
    /// </returns>
    internal static ValidationOptionsMetadata? ExtractFromController(TypeInfo declaredTypeInfo) =>
        CreateMetadata(declaredTypeInfo.GetCustomAttribute<ValidationOptionsAttribute>(true));

    /// <summary>
    ///     从 <see cref="ValidationOptionsAttribute" /> 中创建 <see cref="ValidationOptionsMetadata" /> 实例
    /// </summary>
    /// <param name="attribute">
    ///     <see cref="ValidationOptionsAttribute" />
    /// </param>
    /// <returns>
    ///     <see cref="ValidationOptionsMetadata" />
    /// </returns>
    internal static ValidationOptionsMetadata? CreateMetadata(ValidationOptionsAttribute? attribute) =>
        attribute is null ? null : new ValidationOptionsMetadata(attribute.RuleSets);
}