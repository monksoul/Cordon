// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     异步验证器接口
/// </summary>
public interface IAsyncValidator
{
    /// <summary>
    ///     检查对象是否合法
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="validationContext">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    Task<bool> IsValidAsync(object? value, IValidationContext? validationContext);

    /// <summary>
    ///     获取对象验证结果列表
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="validationContext">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    Task<List<ValidationResult>?> GetValidationResultsAsync(object? value, IValidationContext? validationContext);

    /// <summary>
    ///     执行验证
    /// </summary>
    /// <remarks>失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="value">对象</param>
    /// <param name="validationContext">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <exception cref="ValidationException"></exception>
    Task ValidateAsync(object? value, IValidationContext? validationContext);
}