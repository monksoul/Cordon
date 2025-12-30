// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <inheritdoc />
internal sealed class ValidationService<T> : IValidationService<T>
    where T : class
{
    /// <inheritdoc cref="IServiceProvider" />
    internal readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     <inheritdoc cref="ValidationService{T}" />
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    public ValidationService(IServiceProvider serviceProvider)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public bool IsValid(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(instance, ruleSets);

        return Validator.TryValidateObject(instance, validationContext, null, true);
    }

    /// <inheritdoc />
    public List<ValidationResult>? GetValidationResults(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(instance, ruleSets);

        // 初始化验证结果集合
        var validationResults = new List<ValidationResult>();

        /*
         * 只有在所有属性级验证均失败的情况下，才会执行 IValidatableObject.Validate 方法的验证。
         * 此时，验证结果才会包含该方法返回的错误信息；否则，结果中仅包含属性级验证失败的信息。
         *
         * 参考源码：
         * https://github.com/dotnet/runtime/blob/5535e31a712343a63f5d7d796cd874e563e5ac14/src/libraries/System.ComponentModel.Annotations/src/System/ComponentModel/DataAnnotations/Validator.cs#L423-L430
         */
        Validator.TryValidateObject(instance, validationContext, validationResults, true);

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public void Validate(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 创建 ValidationContext 实例
        var validationContext = CreateValidationContext(instance, ruleSets);

        Validator.ValidateObject(instance, validationContext, true);
    }

    /// <summary>
    ///     创建 <see cref="ValidationContext" /> 实例
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="ValidationContext" />
    /// </returns>
    internal ValidationContext CreateValidationContext(T instance, string?[]? ruleSets)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 初始化 ValidationContext 实例
        var validationContext = new ValidationContext(instance, _serviceProvider, null);

        // 空检查
        if (ruleSets is not null)
        {
            // 设置规则集
            validationContext.WithRuleSets(ruleSets);
        }

        return validationContext;
    }
}