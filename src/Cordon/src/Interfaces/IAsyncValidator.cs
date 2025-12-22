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
    ///     检查对象合法性
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    Task<bool> IsValidAsync(object? value);

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="name">显示名称</param>
    /// <param name="memberNames">成员名称列表</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    Task<List<ValidationResult>?> GetValidationResultsAsync(object? value, string name,
        IEnumerable<string>? memberNames = null);

    /// <summary>
    ///     验证指定的对象
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="name">显示名称</param>
    /// <param name="memberNames">成员名称列表</param>
    /// <exception cref="ValidationException"></exception>
    Task Validate(object? value, string name, IEnumerable<string>? memberNames = null);
}