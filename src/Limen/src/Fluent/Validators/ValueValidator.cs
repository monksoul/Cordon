// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     单个值验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ValueValidator<T> : FluentValidatorBuilder<T, ValueValidator<T>>
{
    /// <summary>
    ///     <inheritdoc cref="ValueValidator{T}" />
    /// </summary>
    public ValueValidator()
        : this(null, null)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValueValidator{T}" />
    /// </summary>
    /// <param name="items">验证上下文数据</param>
    public ValueValidator(IDictionary<object, object?>? items)
        : this(null, items)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValueValidator{T}" />
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">验证上下文数据</param>
    public ValueValidator(IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
        : base(serviceProvider, items)
    {
    }

    /// <summary>
    ///     显示名称
    /// </summary>
    internal string? DisplayName { get; private set; }

    /// <summary>
    ///     验证条件
    /// </summary>
    /// <remarks>当条件满足时才进行验证。</remarks>
    internal Func<T, bool>? WhenCondition { get; private set; }

    /// <summary>
    ///     逆向验证条件
    /// </summary>
    /// <remarks>当条件不满足时才进行验证。</remarks>
    internal Func<T, bool>? UnlessCondition { get; private set; }

    /// <summary>
    ///     检查对象合法性
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public bool IsValid(T? value) => !ShouldValidate(value) || Validators.All(u => u.IsValid(value));

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public List<ValidationResult>? GetValidationResults(T? value)
    {
        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(value))
        {
            return null;
        }

        // 获取显示名称
        var displayName = GetDisplayName();

        // 获取所有验证器验证结果集合
        return Validators.SelectMany(u => u.GetValidationResults(value, displayName) ?? []).ToResults();
    }

    /// <summary>
    ///     验证指定的对象
    /// </summary>
    /// <param name="value">对象</param>
    public void Validate(T? value)
    {
        // 检查是否应该对该对象执行验证
        if (!ShouldValidate(value))
        {
            return;
        }

        // 获取显示名称
        var displayName = GetDisplayName();

        // 遍历验证器集合
        foreach (var validator in Validators)
        {
            validator.Validate(value, displayName);
        }
    }

    /// <summary>
    ///     设置验证条件
    /// </summary>
    /// <remarks>当条件满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> When(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        WhenCondition = condition;

        return this;
    }

    /// <summary>
    ///     设置逆向验证条件
    /// </summary>
    /// <remarks>当条件不满足时才验证。</remarks>
    /// <param name="condition">条件委托</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> Unless(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        UnlessCondition = condition;

        return this;
    }

    /// <summary>
    ///     设置显示名称
    /// </summary>
    /// <param name="displayName">显示名称</param>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public ValueValidator<T> WithDisplayName(string? displayName)
    {
        DisplayName = displayName;

        return this;
    }

    /// <summary>
    ///     检查是否应该对该对象执行验证
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ShouldValidate(T? value)
    {
        // 检查正向条件（When）
        if (WhenCondition is not null && !WhenCondition(value!))
        {
            return false;
        }

        // 检查逆向条件（Unless）
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (UnlessCondition is not null && UnlessCondition(value!))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     获取显示名称
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetDisplayName() => DisplayName ?? "Value";
}