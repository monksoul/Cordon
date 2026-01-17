// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     提取验证选项元数据（<see cref="ValidationOptionsAttribute" />）的 MVC 验证器
/// </summary>
/// <remarks>
///     为实现了 <see cref="IValidatableObject" /> 接口的模型和 <see cref="ValidateWithAttribute{TValidator}" />
///     特性提供验证选项（如规则集）支持。
/// </remarks>
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
        var validationOptionsMetadata = ExtractFromMethod(actionDescriptor.MethodInfo) ??
                                        ExtractFromDeclaredType(actionDescriptor.ControllerTypeInfo);

        // 设置当前验证选项（单次请求仅解析并设置一次，支持 null 值）
        validationDataContext.SetValidationOptions(validationOptionsMetadata);
    }

    /// <summary>
    ///     从操作方法（Action/Handler）提取验证选项
    /// </summary>
    /// <param name="methodInfo">
    ///     <see cref="MethodInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="ValidationOptionsMetadata" />
    /// </returns>
    internal static ValidationOptionsMetadata? ExtractFromMethod(MethodInfo methodInfo) =>
        CreateMetadata(methodInfo.GetCustomAttribute<ValidationOptionsAttribute>(true));

    /// <summary>
    ///     从声明类（Controller/PageModel）提取验证选项
    /// </summary>
    /// <param name="typeInfo">
    ///     <see cref="TypeInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="ValidationOptionsMetadata" />
    /// </returns>
    internal static ValidationOptionsMetadata? ExtractFromDeclaredType(TypeInfo typeInfo) =>
        CreateMetadata(typeInfo.GetCustomAttribute<ValidationOptionsAttribute>(true));

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