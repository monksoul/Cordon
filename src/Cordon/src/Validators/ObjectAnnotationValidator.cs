// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     对象验证特性验证器
/// </summary>
/// <remarks>支持使用 <c>[ValidateNever]</c> 特性来跳过对特定属性的验证，仅限于 ASP.NET Core 应用项目。</remarks>
public class ObjectAnnotationValidator : ValidatorBase, IValidatorInitializer
{
    /// <summary>
    ///     <see cref="IServiceProvider" /> 委托
    /// </summary>
    internal Func<Type, object?>? _serviceProvider;

    /// <summary>
    ///     <inheritdoc cref="ObjectAnnotationValidator" />
    /// </summary>
    public ObjectAnnotationValidator()
        : this(null, null)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ObjectAnnotationValidator" />
    /// </summary>
    /// <param name="items">共享数据</param>
    public ObjectAnnotationValidator(IDictionary<object, object?>? items)
        : this(null, items)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ObjectAnnotationValidator" />
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    public ObjectAnnotationValidator(IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
    {
        // 空检查
        if (serviceProvider is not null)
        {
            _serviceProvider = serviceProvider.GetService;
        }

        Items = items is not null ? new Dictionary<object, object?>(items) : new Dictionary<object, object?>();

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <summary>
    ///     是否验证所有属性的验证特性
    /// </summary>
    /// <remarks>
    ///     该属性用于控制是否执行属性级别的验证逻辑，默认值为 <c>true</c>。
    ///     若设置为 <c>true</c>，则会同时验证所有属性以及 <see cref="IValidatableObject.Validate" /> 方法；
    ///     若设置为 <c>false</c>，则仅验证 <see cref="IValidatableObject.Validate" /> 方法。
    /// </remarks>
    public bool ValidateAllProperties { get; set; } = true;

    /// <summary>
    ///     共享数据
    /// </summary>
    public IDictionary<object, object?> Items { get; }

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(value);

        return Validator.TryValidateObject(value, CreateValidationContext(value, validationContext?.DisplayName), null,
            ValidateAllProperties);
    }

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(object? value, IValidationContext? validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(value);

        // 初始化验证结果集合
        var validationResults = new List<ValidationResult>();

        /*
         * 只有在所有属性级验证均失败的情况下，才会执行 IValidatableObject.Validate 方法的验证。
         * 此时，验证结果才会包含该方法返回的错误信息；否则，结果中仅包含属性级验证失败的信息。
         *
         * 参考源码：
         * https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/src/libraries/System.ComponentModel.Annotations/src/System/ComponentModel/DataAnnotations/Validator.cs#L423-L430
         */
        Validator.TryValidateObject(value, CreateValidationContext(value, validationContext?.DisplayName),
            validationResults, ValidateAllProperties);

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
        // 空检查
        ArgumentNullException.ThrowIfNull(value);

        try
        {
            Validator.ValidateObject(value, CreateValidationContext(value, validationContext?.DisplayName),
                ValidateAllProperties);
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
    internal ValidationContext CreateValidationContext(object value, string? name)
    {
        // 初始化 ValidationContext 实例
        var validationContext = new ValidationContext(value, Items);

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