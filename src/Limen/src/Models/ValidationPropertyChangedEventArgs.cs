// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     验证器属性变更事件参数
/// </summary>
public sealed class ValidationPropertyChangedEventArgs : PropertyChangedEventArgs
{
    /// <summary>
    ///     <inheritdoc cref="ValidationPropertyChangedEventArgs" />
    /// </summary>
    /// <param name="propertyName">已更改属性的名称</param>
    /// <param name="propertyValue">已更改属性的值</param>
    public ValidationPropertyChangedEventArgs(string? propertyName, object? propertyValue)
        : base(propertyName) => PropertyValue = propertyValue;

    /// <summary>
    ///     已更改属性的值
    /// </summary>
    public object? PropertyValue { get; }
}