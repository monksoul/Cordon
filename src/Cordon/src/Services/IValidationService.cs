// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     数据验证服务
/// </summary>
public interface IValidationService
{
    /// <summary>
    ///     检查对象合法性
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool IsValid(object? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    List<ValidationResult>? GetValidationResults(object? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     验证对象
    /// </summary>
    /// <remarks>失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <exception cref="ValidationException"></exception>
    void Validate(object? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     检查多个对象合法性
    /// </summary>
    /// <param name="instances">对象集合</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool IsValid(IEnumerable<object?> instances, string?[]? ruleSets = null);

    /// <summary>
    ///     获取多个对象验证结果集合
    /// </summary>
    /// <param name="instances">对象集合</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    List<ValidationResult>? GetValidationResults(IEnumerable<object?> instances, string?[]? ruleSets = null);

    /// <summary>
    ///     验证多个对象
    /// </summary>
    /// <remarks>失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="instances">对象集合</param>
    /// <param name="ruleSets">规则集</param>
    /// <exception cref="ValidationException"></exception>
    void Validate(IEnumerable<object?> instances, string?[]? ruleSets = null);
}