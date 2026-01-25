// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     单值验证特性验证器
/// </summary>
public class AttributeValueValidator : ValidatorBase, IDisposable
{
    /// <summary>
    ///     用于值验证的 <see cref="ValidationContext" /> 占位对象
    /// </summary>
    internal static readonly object _sentinel = new();

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
    /// <param name="attributes">验证特性列表</param>
    public AttributeValueValidator(params ValidationAttribute[] attributes)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(attributes);

        // 确保数组元素不存在 null 值
        if (attributes.Any(u => (ValidationAttribute?)u is null))
        {
            // ReSharper disable once LocalizableElement
            throw new ArgumentException("Attributes cannot contain null elements.", nameof(attributes));
        }

        Attributes = attributes;

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <summary>
    ///     验证特性列表
    /// </summary>
    public ValidationAttribute[] Attributes { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        Validator.TryValidateValue(value, CreateValidationContext(value, validationContext), null, Attributes);

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(object? value, IValidationContext? validationContext)
    {
        // 初始化验证结果列表和成员名称列表
        var validationResults = new List<ValidationResult>();

        Validator.TryValidateValue(value, CreateValidationContext(value, validationContext), validationResults,
            Attributes);

        // 如果验证未通过且配置了自定义错误信息，则在首部添加自定义错误信息
        // 注意：当验证特性列表有且只有一个时，跳过以下操作
        if (validationResults.Count > 0 && Attributes.Length != 1 && (string?)ErrorMessageString is not null)
        {
            validationResults.Insert(0,
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!),
                    validationContext?.MemberNames));
        }

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public override void Validate(object? value, IValidationContext? validationContext)
    {
        try
        {
            Validator.ValidateValue(value, CreateValidationContext(value, validationContext), Attributes);
        }
        // 如果验证未通过且配置了自定义错误信息，则重新抛出异常
        // 注意：当验证特性列表有且只有一个时，跳过以下操作
        catch (ValidationException e) when (Attributes.Length != 1 && ErrorMessageString is not null)
        {
            throw new ValidationException(
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!),
                    validationContext?.MemberNames),
                e.ValidationAttribute, e.Value) { Source = e.Source };
        }
    }

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
        // 注意：当验证特性列表存在多个时，跳过以下操作
        if (Attributes.Length != 1)
        {
            return;
        }

        // 检查是否是需要同步的属性名
        if (!_observedPropertyNames.Contains(eventArgs.PropertyName))
        {
            return;
        }

        // 获取单个验证特性
        var attribute = Attributes.Single();

        // 应用属性变更到验证特性对应的属性中
        attribute.GetType().GetProperty(eventArgs.PropertyName!)?.SetValue(attribute, eventArgs.PropertyValue);
    }

    /// <summary>
    ///     创建 <see cref="ValidationContext" /> 实例
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="context">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <returns>
    ///     <see cref="ValidationContext" />
    /// </returns>
    internal static ValidationContext CreateValidationContext(object? value, IValidationContext? context)
    {
        // 初始化 ValidationContext 实例
        var validationContext =
            new ValidationContext(value ?? _sentinel, context, context?.Items)
            {
                MemberName = context?.MemberNames?.FirstOrDefault()
            };

        // 空检查
        if (context?.DisplayName is not null)
        {
            validationContext.DisplayName = context.DisplayName;
        }

        // 空检查
        if (context?.RuleSets is not null)
        {
            // 设置规则集
            validationContext.WithRuleSets(context.RuleSets);
        }

        return validationContext;
    }
}