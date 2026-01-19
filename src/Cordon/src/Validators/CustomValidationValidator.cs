// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     自定义验证特性验证器
/// </summary>
public class CustomValidationValidator : ValidatorBase, IDisposable
{
    /// <summary>
    ///     需要监听属性变更的属性名集合
    /// </summary>
    internal readonly string[] _observedPropertyNames =
    [
        nameof(ErrorMessage), nameof(ErrorMessageResourceType), nameof(ErrorMessageResourceName)
    ];

    /// <summary>
    ///     <inheritdoc cref="AttributeValueValidator" />
    /// </summary>
    internal readonly AttributeValueValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="CustomValidationValidator" />
    /// </summary>
    /// <param name="validatorType">执行自定义验证的类型</param>
    /// <param name="method">验证方法</param>
    public CustomValidationValidator(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        Type validatorType, string method)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorType);
        ArgumentException.ThrowIfNullOrWhiteSpace(method);

        ValidatorType = validatorType;
        Method = method;

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        _validator = new AttributeValueValidator(new CustomValidationAttribute(validatorType, method));

        UseResourceKey(() => nameof(ValidationMessages.CustomValidationValidator_ValidationError));
    }

    /// <summary>
    ///     执行自定义验证的类型
    /// </summary>
    public Type ValidatorType { get; }

    /// <summary>
    ///     验证方法
    /// </summary>
    public string Method { get; }

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
    public override List<ValidationResult>?
        GetValidationResults(object? value, IValidationContext? validationContext) =>
        _validator.GetValidationResults(value, validationContext);

    /// <inheritdoc />
    public override void Validate(object? value, IValidationContext? validationContext) =>
        _validator.Validate(value, validationContext);

    /// <inheritdoc />
    public override string? FormatErrorMessage(string name) => _validator.FormatErrorMessage(name);

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

        // 应用属性变更到 CustomValidationAttribute 对应的属性中
        typeof(CustomValidationAttribute).GetProperty(eventArgs.PropertyName!)
            ?.SetValue(_validator.Attributes[0], eventArgs.PropertyValue);
    }
}