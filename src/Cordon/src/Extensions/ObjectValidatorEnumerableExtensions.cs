// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <![CDATA[IEnumerable<IObjectValidator<T>>]]> 扩展类
/// </summary>
public static class ObjectValidatorEnumerableExtensions
{
    /// <summary>
    ///     检查对象是否合法
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

        return validators.All(u => u.IsValid(instance, ruleSets));
    }

    /// <summary>
    ///     获取对象验证结果列表
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
    ///     执行验证
    /// </summary>
    /// <remarks>验证失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="validators"><see cref="IObjectValidator{T}" /> 集合</param>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <exception cref="ValidationException"></exception>
    public static void Validate<T>(this IEnumerable<IObjectValidator<T>> validators, T? instance,
        string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validators);

        // 遍历验证器列表
        foreach (var validator in validators)
        {
            validator.Validate(instance, ruleSets);
        }
    }
}