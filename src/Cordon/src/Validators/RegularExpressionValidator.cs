// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     正则表达式验证器
/// </summary>
public class RegularExpressionValidator : ValidatorBase, IDisposable
{
    /// <summary>
    ///     需要监听属性变更的属性名集合
    /// </summary>
    internal readonly string[] _observedPropertyNames = [nameof(MatchTimeoutInMilliseconds)];

    /// <summary>
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    internal readonly ValueAnnotationValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="RegularExpressionValidator" />
    /// </summary>
    /// <param name="pattern">正则表达式模式</param>
    public RegularExpressionValidator(string pattern)
    {
        Pattern = pattern;
        MatchTimeoutInMilliseconds = 2000;

        _validator = new ValueAnnotationValidator(new RegularExpressionAttribute(pattern));

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        UseResourceKey(() => nameof(ValidationMessages.RegularExpressionValidator_ValidationError));
    }

    /// <summary>
    ///     正则表达式模式
    /// </summary>
    public string Pattern { get; }

    /// <summary>
    ///     用于在操作超时前执行单个匹配操作的时间量
    /// </summary>
    /// <remarks>以毫秒为单位，默认值为：2000。</remarks>
    public int MatchTimeoutInMilliseconds
    {
        get;
        set
        {
            field = value;

            // 触发属性变更事件
            OnPropertyChanged(value);
        }
    }

    /// <summary>
    ///     匹配正则表达式模式时要使用的超时值
    /// </summary>
    public TimeSpan MatchTimeout => TimeSpan.FromMilliseconds(MatchTimeoutInMilliseconds);

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Pattern);

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

        // 应用属性变更到 RegularExpressionAttribute 对应的属性中
        typeof(RegularExpressionAttribute).GetProperty(eventArgs.PropertyName!)
            ?.SetValue(_validator.Attributes[0], eventArgs.PropertyValue);
    }
}