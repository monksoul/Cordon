// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     <![CDATA[IEnumerable<IObjectValidator<T>>]]> 拓展类
/// </summary>
public static class ObjectValidatorEnumerableExtensions
{
    /// <summary>
    ///     检查对象合法性
    /// </summary>
    /// <param name="validators"><see cref="IObjectValidator{T}" /> 集合</param>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public static bool IsValid<T>(this IEnumerable<IObjectValidator<T>> validators, T? instance,
        string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validators);

        return validators.All(validator => validator.IsValid(instance, ruleSets));
    }

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="validators"><see cref="IObjectValidator{T}" /> 集合</param>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public static List<ValidationResult>? GetValidationResults<T>(this IEnumerable<IObjectValidator<T>> validators,
        T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validators);

        return validators.SelectMany(u => u.GetValidationResults(instance, ruleSets) ?? []).ToResults();
    }

    /// <summary>
    ///     验证指定的对象
    /// </summary>
    /// <param name="validators"><see cref="IObjectValidator{T}" /> 集合</param>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    public static void Validate<T>(this IEnumerable<IObjectValidator<T>> validators, T? instance,
        string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validators);

        // 遍历验证器集合
        foreach (var validator in validators)
        {
            validator.Validate(instance, ruleSets);
        }
    }
}