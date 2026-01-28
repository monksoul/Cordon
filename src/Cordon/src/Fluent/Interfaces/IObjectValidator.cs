// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     对象验证器服务
/// </summary>
public interface IObjectValidator : IValidatorInitializer, IDisposable
{
    /// <summary>
    ///     检查对象是否合法
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool IsValid(object? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     获取对象验证结果列表
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    List<ValidationResult>? GetValidationResults(object? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     执行验证
    /// </summary>
    /// <remarks>验证失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <exception cref="ValidationException"></exception>
    void Validate(object? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     尝试执行验证
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="ValidatorResult" />
    /// </returns>
    ValidatorResult TryValidate(object? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     获取对象验证结果列表
    /// </summary>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext" />
    /// </param>
    /// <param name="disposeAfterValidation">是否在验证完成后自动释放当前实例。默认值为：<c>true</c></param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    List<ValidationResult> ToResults(ValidationContext validationContext, bool disposeAfterValidation = true);
}

/// <summary>
///     <inheritdoc cref="IObjectValidator" />
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public interface IObjectValidator<T> : IObjectValidator
{
    /// <summary>
    ///     检查对象是否合法
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool IsValid(T? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     获取对象验证结果列表
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    List<ValidationResult>? GetValidationResults(T? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     执行验证
    /// </summary>
    /// <remarks>验证失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <exception cref="ValidationException"></exception>
    void Validate(T? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     尝试执行验证
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="ValidatorResult{T}" />
    /// </returns>
    ValidatorResult<T> TryValidate(T? instance, string?[]? ruleSets = null);
}