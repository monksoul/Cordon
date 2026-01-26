// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     属性验证特性验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class AttributePropertyValidator<T> : ValidatorBase<T>
{
    /// <summary>
    ///     属性值访问器
    /// </summary>
    internal readonly Func<T, object?> _getter;

    /// <summary>
    ///     <inheritdoc cref="AttributePropertyValidator{T}" />
    /// </summary>
    /// <param name="selector">属性选择器</param>
    public AttributePropertyValidator(Expression<Func<T, object?>> selector)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(selector);

        Property = selector.GetProperty();
        _getter = selector.Compile();

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <summary>
    ///     <inheritdoc cref="PropertyInfo" />
    /// </summary>
    public PropertyInfo Property { get; }

    /// <inheritdoc />
    public override bool IsValid(T? instance, ValidationContext<T> validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 获取显示名称
        var displayName = GetDisplayName(validationContext.DisplayName);

        return Validator.TryValidateProperty(GetValue(instance),
            CreateValidationContext(instance, displayName, validationContext), null);
    }

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 获取显示名称
        var displayName = GetDisplayName(validationContext.DisplayName);

        // 初始化验证结果列表
        var validationResults = new List<ValidationResult>();

        Validator.TryValidateProperty(GetValue(instance),
            CreateValidationContext(instance, displayName, validationContext), validationResults);

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
            Validator.ValidateProperty(GetValue(instance),
                CreateValidationContext(instance, displayName, validationContext));
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
        name ?? Property.GetCustomAttribute<DisplayAttribute>(true)?.GetName() ??
        Property.GetCustomAttribute<DisplayNameAttribute>(true)?.DisplayName ?? GetMemberName();

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
    /// <param name="instance">对象</param>
    /// <param name="name">显示名称</param>
    /// <param name="context">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="ValidationContext" />
    /// </returns>
    internal ValidationContext CreateValidationContext(object instance, string? name, ValidationContext<T> context)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 初始化 ValidationContext 实例
        var validationContext = new ValidationContext(instance, context, context.Items) { MemberName = Property.Name };

        // 空检查
        if (name is not null)
        {
            validationContext.DisplayName = name;
        }

        // 空检查
        if (context.RuleSets is not null)
        {
            // 设置规则集
            validationContext.WithRuleSets(context.RuleSets);
        }

        return validationContext;
    }
}