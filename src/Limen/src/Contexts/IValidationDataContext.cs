// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     验证数据上下文服务
/// </summary>
public interface IValidationDataContext
{
    /// <summary>
    ///     存储验证数据的字典
    /// </summary>
    IDictionary<object, object?> Items { get; }

    /// <summary>
    ///     设置或更新验证数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    void SetValue(object key, object? value);

    /// <summary>
    ///     尝试获取验证数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool TryGetValue(object key, out object? value);

    /// <summary>
    ///     判断键是否定义
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    bool ContainsKey(object key);

    /// <summary>
    ///     获取当前验证选项
    /// </summary>
    /// <remarks>结合 <see cref="ValidationOptionsAttribute" /> 使用。</remarks>
    /// <returns>
    ///     <see cref="ValidationOptionsMetadata" />
    /// </returns>
    ValidationOptionsMetadata? GetValidationOptions();

    /// <summary>
    ///     设置当前验证选项
    /// </summary>
    /// <remarks>结合 <see cref="ValidationOptionsAttribute" /> 使用。</remarks>
    /// <param name="metadata">
    ///     <see cref="ValidationOptionsMetadata" />
    /// </param>
    void SetValidationOptions(ValidationOptionsMetadata metadata);

    /// <summary>
    ///     检查是否已设置验证选项
    /// </summary>
    /// <remarks>结合 <see cref="ValidationOptionsAttribute" /> 使用。</remarks>
    /// <remarks>
    ///     <see cref="bool" />
    /// </remarks>
    bool HasValidationOptions();
}