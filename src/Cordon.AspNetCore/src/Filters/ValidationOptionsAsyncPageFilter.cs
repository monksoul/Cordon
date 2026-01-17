// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     提取验证选项元数据（<see cref="ValidationOptionsAttribute" />）的 Razor Pages 筛选器
/// </summary>
/// <remarks>
///     为实现了 <see cref="IValidatableObject" /> 接口的模型和 <see cref="ValidateWithAttribute{TValidator}" />
///     特性提供验证选项（如规则集）支持。
/// </remarks>
internal sealed class ValidationOptionsAsyncPageFilter : IAsyncPageFilter
{
    /// <inheritdoc />
    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        // 尝试获取验证数据上下文服务
        if (context.HttpContext.RequestServices.GetService<IValidationDataContext>() is not
            ValidationDataContext validationDataContext)
        {
            return Task.CompletedTask;
        }

        // 检查是否已设置验证选项（避免重复提取）
        if (validationDataContext.HasValidationOptions())
        {
            return Task.CompletedTask;
        }

        // 空检查
        if (context.HandlerMethod is not { } handlerMethod ||
            context.ActionDescriptor.ModelTypeInfo is not { } modelType)
        {
            return Task.CompletedTask;
        }

        // 提取验证选项
        var validationOptionsMetadata = ValidationOptionsModelValidator.ExtractFromMethod(handlerMethod.MethodInfo) ??
                                        ValidationOptionsModelValidator.ExtractFromDeclaredType(modelType);

        // 设置当前验证选项（单次请求仅解析并设置一次，支持 null 值）
        validationDataContext.SetValidationOptions(validationOptionsMetadata);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
        PageHandlerExecutionDelegate next) => await next.Invoke();
}