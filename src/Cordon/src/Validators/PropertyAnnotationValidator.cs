// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     属性验证特性验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TProperty">属性类型</typeparam>
public class PropertyAnnotationValidator<T, TProperty> : PropertyAnnotationValidator<T>
{
    /// <summary>
    ///     <inheritdoc cref="PropertyAnnotationValidator{T,TProperty}" />
    /// </summary>
    /// <param name="selector">属性选择器</param>
    public PropertyAnnotationValidator(Expression<Func<T, TProperty>> selector)
        : base(ConvertExpression(selector))
    {
    }

    /// <summary>
    ///     <inheritdoc cref="PropertyAnnotationValidator{T,TProperty}" />
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="items">共享数据</param>
    public PropertyAnnotationValidator(Expression<Func<T, TProperty>> selector, IDictionary<object, object?>? items)
        : base(ConvertExpression(selector), items)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="PropertyAnnotationValidator{T,TProperty}" />
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    public PropertyAnnotationValidator(Expression<Func<T, TProperty>> selector, IServiceProvider? serviceProvider,
        IDictionary<object, object?>? items)
        : base(ConvertExpression(selector), serviceProvider, items)
    {
    }

    /// <summary>
    ///     将属性表达式转换为 <![CDATA[Func<T, object?>]]> 类型
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <returns>
    ///     <see cref="Expression{TDelegate}" />
    /// </returns>
    internal static Expression<Func<T, object?>> ConvertExpression(Expression<Func<T, TProperty>> selector)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(selector);

        return Expression.Lambda<Func<T, object?>>(Expression.Convert(selector.Body, typeof(object)),
            selector.Parameters);
    }

    /// <summary>
    ///     获取属性值
    /// </summary>
    /// <param name="instance">对象</param>
    /// <returns>
    ///     <typeparamref name="TProperty" />
    /// </returns>
    public new TProperty GetValue(T instance)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        return (TProperty)base.GetValue(instance)!;
    }
}

/// <summary>
///     属性验证特性验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class PropertyAnnotationValidator<T> : ValidatorBase<T>, IValidatorInitializer
{
    /// <summary>
    ///     属性值访问器
    /// </summary>
    internal readonly Func<T, object?> _getter;

    /// <summary>
    ///     <see cref="IServiceProvider" /> 委托
    /// </summary>
    internal Func<Type, object?>? _serviceProvider;

    /// <summary>
    ///     <inheritdoc cref="PropertyAnnotationValidator{T}" />
    /// </summary>
    /// <param name="selector">属性选择器</param>
    public PropertyAnnotationValidator(Expression<Func<T, object?>> selector)
        // ReSharper disable once IntroduceOptionalParameters.Global
        : this(selector, null, null)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="PropertyAnnotationValidator{T}" />
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="items">共享数据</param>
    public PropertyAnnotationValidator(Expression<Func<T, object?>> selector, IDictionary<object, object?>? items)
        : this(selector, null, items)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="PropertyAnnotationValidator{T}" />
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    public PropertyAnnotationValidator(Expression<Func<T, object?>> selector, IServiceProvider? serviceProvider,
        IDictionary<object, object?>? items)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(selector);

        Property = selector.GetProperty();
        _getter = selector.Compile();

        // 空检查
        if (serviceProvider is not null)
        {
            _serviceProvider = serviceProvider.GetService;
        }

        Items = items is not null ? new Dictionary<object, object?>(items) : new Dictionary<object, object?>();

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <summary>
    ///     <inheritdoc cref="PropertyInfo" />
    /// </summary>
    public PropertyInfo Property { get; }

    /// <summary>
    ///     共享数据
    /// </summary>
    public IDictionary<object, object?> Items { get; }

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <inheritdoc />
    public override bool IsValid(T? instance, ValidationContext<T> validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 获取显示名称
        var displayName = GetDisplayName(validationContext.DisplayName);

        return Validator.TryValidateProperty(GetValue(instance), CreateValidationContext(instance, displayName), null);
    }

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 获取显示名称
        var displayName = GetDisplayName(validationContext.DisplayName);

        // 初始化验证结果集合
        var validationResults = new List<ValidationResult>();

        Validator.TryValidateProperty(GetValue(instance), CreateValidationContext(instance, displayName),
            validationResults);

        // 如果验证未通过且配置了自定义错误信息，则在首部添加自定义错误信息
        if (validationResults.Count > 0 && (string?)ErrorMessageString is not null)
        {
            validationResults.Insert(0,
                new ValidationResult(FormatErrorMessage(displayName),
                    validationContext.MemberNames ?? [Property.Name]));
        }

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public override void Validate(T? instance, ValidationContext<T> validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 获取显示名称
        var displayName = GetDisplayName(validationContext.DisplayName);

        try
        {
            Validator.ValidateProperty(GetValue(instance), CreateValidationContext(instance, displayName));
        }
        // 如果验证未通过且配置了自定义错误信息，则重新抛出异常
        catch (ValidationException e) when (ErrorMessageString is not null)
        {
            throw new ValidationException(
                new ValidationResult(FormatErrorMessage(displayName),
                    validationContext.MemberNames ?? e.ValidationResult.MemberNames), e.ValidationAttribute, e.Value)
            {
                Source = e.Source
            };
        }
    }

    /// <summary>
    ///     获取属性值
    /// </summary>
    /// <param name="instance">对象</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    public object? GetValue(T instance)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        return _getter(instance);
    }

    /// <summary>
    ///     获取显示名称
    /// </summary>
    /// <param name="name">显示名称</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public string GetDisplayName(string? name) =>
        name ?? Property.GetCustomAttribute<DisplayAttribute>(false)?.GetName() ??
        Property.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName ?? GetMemberName();

    /// <summary>
    ///     获取成员名称
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public string GetMemberName() => Property.Name;

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
        var validationContext = new ValidationContext(value, Items) { MemberName = Property.Name };

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