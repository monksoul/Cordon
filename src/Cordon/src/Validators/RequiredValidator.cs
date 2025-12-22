// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     必填验证器
/// </summary>
public class RequiredValidator : ValidatorBase, IHighPriorityValidator, IDisposable
{
    /// <summary>
    ///     需要监听属性变更的属性名集合
    /// </summary>
    internal readonly string[] _observedPropertyNames = [nameof(AllowEmptyStrings)];

    /// <summary>
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    internal readonly ValueAnnotationValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="RequiredValidator" />
    /// </summary>
    public RequiredValidator()
    {
        _validator = new ValueAnnotationValidator(new RequiredAttribute());

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        UseResourceKey(() => nameof(ValidationMessages.RequiredValidator_ValidationError));
    }

    /// <summary>
    ///     是否允许空字符串
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowEmptyStrings
    {
        get;
        set
        {
            field = value;

            // 触发属性变更事件
            OnPropertyChanged(value);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    /// <remarks>默认值为：10。</remarks>
    public int Priority => 10;

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <summary>
    ///     释放资源
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 移除属性变更事件
            PropertyChanged -= OnPropertyChanged;
        }
    }

    /// <summary>
    ///     订阅属性变更事件
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="eventArgs">
    ///     <see cref="ValidationPropertyChangedEventArgs" />
    /// </param>
    internal void OnPropertyChanged(object? sender, ValidationPropertyChangedEventArgs eventArgs)
    {
        // 检查是否是需要同步的属性名
        if (!_observedPropertyNames.Contains(eventArgs.PropertyName))
        {
            return;
        }

        // 应用属性变更到 RequiredAttribute 对应的属性中
        typeof(RequiredAttribute).GetProperty(eventArgs.PropertyName!)
            ?.SetValue(_validator.Attributes[0], eventArgs.PropertyValue);
    }
}