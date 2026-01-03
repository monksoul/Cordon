// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     自定义条件成立时委托验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class PredicateValidator<T> : ValidatorBase<T>
{
    /// <summary>
    ///     <inheritdoc cref="PredicateValidator{T}" />
    /// </summary>
    /// <param name="condition">条件委托</param>
    public PredicateValidator(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        Condition = (instance, _) => condition(instance);
    }

    /// <summary>
    ///     <inheritdoc cref="PredicateValidator{T}" />
    /// </summary>
    /// <param name="condition">条件委托</param>
    public PredicateValidator(Func<T, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        Condition = condition;
    }

    /// <summary>
    ///     条件委托
    /// </summary>
    public Func<T, ValidationContext<T>, bool> Condition { get; }

    /// <inheritdoc />
    public override bool IsValid(T? instance, ValidationContext<T> validationContext) =>
        Condition(instance!, validationContext);
}