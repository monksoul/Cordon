// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     单个值验证特性验证器
/// </summary>
public class ValueAnnotationValidator : ValidatorBase, IValidatorInitializer
{
    /// <summary>
    ///     用于值验证的 <see cref="ValidationContext" /> 占位对象
    /// </summary>
    internal static readonly object _sentinel = new();

    /// <summary>
    ///     <see cref="IServiceProvider" /> 委托
    /// </summary>
    internal Func<Type, object?>? _serviceProvider;

    /// <summary>
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    /// <param name="attributes">验证特性列表</param>
    /// <exception cref="ArgumentException"></exception>
    public ValueAnnotationValidator(params ValidationAttribute[] attributes)
        : this(attributes, null, null)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    /// <param name="attributes">验证特性列表</param>
    /// <param name="items">共享数据</param>
    public ValueAnnotationValidator(ValidationAttribute[] attributes, IDictionary<object, object?>? items)
        : this(attributes, null, items)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    /// <param name="attributes">验证特性列表</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    public ValueAnnotationValidator(ValidationAttribute[] attributes, IServiceProvider? serviceProvider,
        IDictionary<object, object?>? items)
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

        // 空检查
        if (serviceProvider is not null)
        {
            _serviceProvider = serviceProvider.GetService;
        }

        Items = items is not null ? new Dictionary<object, object?>(items) : new Dictionary<object, object?>();

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <summary>
    ///     验证特性列表
    /// </summary>
    public ValidationAttribute[] Attributes { get; }

    /// <summary>
    ///     共享数据
    /// </summary>
    public IDictionary<object, object?> Items { get; }

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        Validator.TryValidateValue(value, CreateValidationContext(value, validationContext?.DisplayName), null,
            Attributes);

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(object? value, IValidationContext? validationContext)
    {
        // 初始化验证结果集合和成员名称列表
        var validationResults = new List<ValidationResult>();

        Validator.TryValidateValue(value, CreateValidationContext(value, validationContext?.DisplayName),
            validationResults, Attributes);

        // 如果验证未通过且配置了自定义错误信息，则在首部添加自定义错误信息
        if (validationResults.Count > 0 && (string?)ErrorMessageString is not null)
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
            Validator.ValidateValue(value, CreateValidationContext(value, validationContext?.DisplayName), Attributes);
        }
        // 如果验证未通过且配置了自定义错误信息，则重新抛出异常
        catch (ValidationException e) when (ErrorMessageString is not null)
        {
            throw new ValidationException(
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!),
                    validationContext?.MemberNames),
                e.ValidationAttribute, e.Value) { Source = e.Source };
        }
    }

    /// <summary>
    ///     创建 <see cref="ValidationContext" /> 实例
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="name">显示名称</param>
    /// <returns>
    ///     <see cref="ValidationContext" />
    /// </returns>
    internal ValidationContext CreateValidationContext(object? value, string? name)
    {
        // 初始化 ValidationContext 实例
        var validationContext = new ValidationContext(value ?? _sentinel, Items);

        // 空检查
        if (name is not null)
        {
            validationContext.DisplayName = name;
        }

        // 同步 IServiceProvider 委托
        validationContext.InitializeServiceProvider(_serviceProvider!);

        return validationContext;
    }

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider) => _serviceProvider = serviceProvider;
}