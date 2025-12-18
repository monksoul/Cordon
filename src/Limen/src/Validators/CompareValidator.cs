// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     比较两个属性验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TProperty">属性类型</typeparam>
public class CompareValidator<T, TProperty> : ValidatorBase<T>
{
    /// <summary>
    ///     属性值访问器
    /// </summary>
    internal readonly Func<T, object?> _otherPropertyGetter;

    /// <summary>
    ///     其他属性值访问器
    /// </summary>
    internal readonly Func<T, TProperty> _propertyGetter;

    /// <summary>
    ///     <inheritdoc cref="CompareValidator{T,TProperty}" />
    /// </summary>
    /// <param name="propertySelector">属性选择器</param>
    /// <param name="otherPropertyName">其他属性的名称</param>
    public CompareValidator(Expression<Func<T, TProperty>> propertySelector, string otherPropertyName)
        : this(propertySelector, CreatePropertySelector(otherPropertyName))
    {
    }

    /// <summary>
    ///     <inheritdoc cref="CompareValidator{T,TProperty}" />
    /// </summary>
    /// <param name="propertySelector">属性选择器</param>
    /// <param name="otherPropertySelector">其他属性选择器</param>
    public CompareValidator(Expression<Func<T, TProperty>> propertySelector,
        Expression<Func<T, object?>> otherPropertySelector)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertySelector);
        ArgumentNullException.ThrowIfNull(otherPropertySelector);

        Property = propertySelector.GetProperty();
        _propertyGetter = propertySelector.Compile();

        OtherProperty = otherPropertySelector.GetProperty();
        _otherPropertyGetter = otherPropertySelector.Compile();

        UseResourceKey(() => nameof(ValidationMessages.CompareValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="PropertyInfo" />
    /// </summary>
    public PropertyInfo Property { get; }

    /// <summary>
    ///     <inheritdoc cref="PropertyInfo" />
    /// </summary>
    public PropertyInfo OtherProperty { get; }

    /// <inheritdoc />
    public override bool IsValid(T? instance)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        return Equals(_propertyGetter(instance), _otherPropertyGetter(instance));
    }

    /// <inheritdoc />
    public override string? FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, GetDisplayNameForProperty(OtherProperty));

    /// <summary>
    ///     获取属性显示名称
    /// </summary>
    /// <param name="property">
    ///     <see cref="PropertyInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string GetDisplayNameForProperty(PropertyInfo property)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(property);

        return property.GetCustomAttribute<DisplayAttribute>(false)?.GetName() ??
               property.GetCustomAttribute<DisplayNameAttribute>(false)?.DisplayName ?? property.Name;
    }

    /// <summary>
    ///     根据属性名创建 <![CDATA[Expression<Func<T, object?>>]]> 表达式
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <returns>
    ///     <see cref="Expression{TDelegate}" />
    /// </returns>
    internal static Expression<Func<T, object?>> CreatePropertySelector(string propertyName)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var parameter = Expression.Parameter(typeof(T), "u");

        // 尝试获取公共实例属性
        var property = Expression.Property(parameter, propertyName);

        // 将属性值统一转换为 object?
        var converted = Expression.Convert(property, typeof(object));

        return Expression.Lambda<Func<T, object?>>(converted, parameter);
    }
}