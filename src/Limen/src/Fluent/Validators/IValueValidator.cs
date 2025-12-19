// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     单个值验证器服务
/// </summary>
public interface IValueValidator : IValidatorInitializer;

/// <summary>
///     <inheritdoc cref="IValueValidator" />
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public interface IValueValidator<in T> : IValueValidator
{
    /// <summary>
    ///     检查对象合法性
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool IsValid(T? value);

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    List<ValidationResult>? GetValidationResults(T? value);

    /// <summary>
    ///     验证指定的对象
    /// </summary>
    /// <param name="value">对象</param>
    void Validate(T? value);
}