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
    ///     创建指定对象类型的数据验证服务
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="IValidationService{T}" />
    /// </returns>
    IValidationService<T> For<T>() where T : class;
}

/// <summary>
///     数据验证服务
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public interface IValidationService<in T>
    where T : class
{
    /// <summary>
    ///     检查对象合法性
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool IsValid(T? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    List<ValidationResult>? GetValidationResults(T? instance, string?[]? ruleSets = null);

    /// <summary>
    ///     验证对象
    /// </summary>
    /// <remarks>失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <exception cref="ValidationException"></exception>
    void Validate(T? instance, string?[]? ruleSets = null);
}