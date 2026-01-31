// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <inheritdoc />
public sealed class ValidationService : IValidationService
{
    /// <inheritdoc cref="AttributeObjectValidator" />
    internal readonly AttributeObjectValidator _attributeValidator;

    /// <inheritdoc cref="IServiceProvider" />
    internal readonly IServiceProvider? _serviceProvider;

    /// <summary>
    ///     <inheritdoc cref="ValidationService" />
    /// </summary>
    public ValidationService()
    {
        _attributeValidator = new AttributeObjectValidator();

        Items = new Dictionary<object, object?>();
    }

    /// <summary>
    ///     <inheritdoc cref="ValidationService" />
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    [ActivatorUtilitiesConstructor]
    public ValidationService(IServiceProvider serviceProvider)
        : this()
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     共享数据
    /// </summary>
    public IDictionary<object, object?> Items { get; }

    /// <inheritdoc />
    public bool IsValid(object? instance, string?[]? ruleSets = null) =>
        _attributeValidator.IsValid(instance, CreateValidationContext(instance, ruleSets));

    /// <inheritdoc />
    public List<ValidationResult>? GetValidationResults(object? instance, string?[]? ruleSets = null) =>
        _attributeValidator.GetValidationResults(instance, CreateValidationContext(instance, ruleSets));

    /// <inheritdoc />
    public void Validate(object? instance, string?[]? ruleSets = null) =>
        _attributeValidator.Validate(instance, CreateValidationContext(instance, ruleSets));

    /// <inheritdoc />
    public ValidatorResult TryValidate(object? instance, string?[]? ruleSets = null) =>
        _attributeValidator.TryValidate(instance, CreateValidationContext(instance, ruleSets));

    /// <inheritdoc />
    public bool IsValid(IEnumerable<object?> instances, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instances);

        return instances.All(instance => IsValid(instance, ruleSets));
    }

    /// <inheritdoc />
    public List<ValidationResult>? GetValidationResults(IEnumerable<object?> instances, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instances);

        return instances.SelectMany(instance => GetValidationResults(instance, ruleSets) ?? []).ToResults();
    }

    /// <inheritdoc />
    public void Validate(IEnumerable<object?> instances, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instances);

        // 遍历对象集合
        foreach (var instance in instances)
        {
            Validate(instance, ruleSets);
        }
    }

    /// <inheritdoc />
    public List<ValidatorResult> TryValidate(IEnumerable<object?> instances, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instances);

        return instances.Select(instance => TryValidate(instance, ruleSets)).ToList();
    }

    /// <summary>
    ///     创建 <see cref="ValidationContext{T}" /> 实例
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="ValidationContext{T}" />
    /// </returns>
    internal ValidationContext<object> CreateValidationContext(object? instance, string?[]? ruleSets)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        return new ValidationContext<object>(instance, _serviceProvider, Items) { RuleSets = ruleSets };
    }
}