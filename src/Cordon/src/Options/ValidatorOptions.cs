// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证器选项
/// </summary>
public sealed class ValidatorOptions : INotifyPropertyChanged
{
    /// <summary>
    ///     是否禁用属性验证特性验证
    /// </summary>
    /// <remarks>默认值为：<c>false</c>，即启用。</remarks>
    public bool SuppressAttributeValidation { get; set => SetField(ref field, value); }

    /// <summary>
    ///     是否验证所有属性的验证特性
    /// </summary>
    /// <remarks>
    ///     该属性用于控制是否执行属性级别的验证逻辑，默认值为 <c>true</c>。
    ///     若设置为 <c>true</c>，则会同时验证所有属性以及 <see cref="IValidatableObject.Validate" /> 方法；
    ///     若设置为 <c>false</c>，则仅验证 <see cref="IValidatableObject.Validate" /> 方法。
    /// </remarks>
    public bool ValidateAllProperties { get; set => SetField(ref field, value); } = true;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///     触发属性变更事件
    /// </summary>
    /// <param name="propertyName">属性名称</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    ///     设置字段值
    /// </summary>
    /// <param name="field">属性关联的字段</param>
    /// <param name="value">已更改属性的值</param>
    /// <param name="propertyName">已更改属性的名称</param>
    /// <typeparam name="T">属性类型</typeparam>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        // 检查值是否发生变更
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;

        // 触发属性变更事件
        OnPropertyChanged(propertyName);

        return true;
    }
}