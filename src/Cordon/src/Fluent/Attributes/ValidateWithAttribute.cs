// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     指定参数使用指定的验证器进行验证
/// </summary>
/// <typeparam name="TValidator">
///     <see cref="IObjectValidator" />
/// </typeparam>
[AttributeUsage(AttributeTargets.Parameter)]
public class ValidateWithAttribute<TValidator> : ValidationAttribute
    where TValidator : IObjectValidator
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // 创建 TValidator 实例
        var validator = validationContext.GetService<IServiceProvider>() is null
            ? Activator.CreateInstance<TValidator>()
            : ActivatorUtilities.CreateInstance<TValidator>(validationContext);

        // 获取对象验证结果集合
        var validationResults = validator.ToResults(validationContext);

        return validationResults is { Count: > 0 } ? validationResults[0] : ValidationResult.Success;
    }
}