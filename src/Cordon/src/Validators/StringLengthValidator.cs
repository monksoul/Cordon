// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     字符串长度验证器
/// </summary>
public class StringLengthValidator : ValidatorBase, IDisposable
{
    /// <summary>
    ///     需要监听属性变更的属性名集合
    /// </summary>
    internal readonly string[] _observedPropertyNames = [nameof(MinimumLength)];

    /// <summary>
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    internal readonly ValueAnnotationValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="StringLengthValidator" />
    /// </summary>
    /// <param name="maximumLength">最大允许长度</param>
    public StringLengthValidator(int maximumLength)
    {
        MaximumLength = maximumLength;

        _validator = new ValueAnnotationValidator(new StringLengthAttribute(maximumLength));

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        UseResourceKey(() => nameof(ValidationMessages.StringLengthValidator_ValidationError));
    }

    /// <summary>
    ///     最大允许长度
    /// </summary>
    public int MaximumLength { get; }

    /// <summary>
    ///     最小允许长度
    /// </summary>
    public int MinimumLength
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
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        _validator.IsValid(value, validationContext);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) => string.Format(CultureInfo.CurrentCulture,
        (MinimumLength == 0 ? 0 : !CustomErrorMessageSet ? 1 : 0) != 0
            ? ValidationMessages.StringLengthValidator_ValidationError_MinimumLength
            : ErrorMessageString, name, MaximumLength, MinimumLength);

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

        // 应用属性变更到 StringLengthAttribute 对应的属性中
        typeof(StringLengthAttribute).GetProperty(eventArgs.PropertyName!)
            ?.SetValue(_validator.Attributes[0], eventArgs.PropertyValue);
    }
}