// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     条件验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ConditionalValidator<T> : ValidatorBase<T>, IValidatorInitializer, IDisposable
{
    /// <summary>
    ///     <see cref="ConditionResult{T}" /> 构建结果
    /// </summary>
    internal readonly ConditionResult<T> _conditionResult;

    /// <summary>
    ///     <inheritdoc cref="ConditionalValidator{T}" />
    /// </summary>
    /// <param name="buildConditions">条件验证构建器配置委托</param>
    public ConditionalValidator(Action<ConditionBuilder<T>> buildConditions)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(buildConditions);

        _conditionResult = new ConditionBuilder<T>().Build(buildConditions);

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    void IValidatorInitializer.InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        InitializeServiceProvider(serviceProvider);

    /// <inheritdoc />
    public override bool IsValid(T? instance, ValidationContext<T> validationContext)
    {
        // 获取匹配到的验证器集合
        var matchedValidators = GetMatchedValidators(instance);

        return matchedValidators is null or { Count: 0 } ||
               matchedValidators.All(u => u.IsValid(instance, validationContext));
    }

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext)
    {
        // 获取匹配到的验证器集合和成员名称列表
        var matchedValidators = GetMatchedValidators(instance);

        // 空检查
        if (matchedValidators is null or { Count: 0 })
        {
            return null;
        }

        // 获取验证结果集合
        var validationResults = matchedValidators
            .SelectMany(u => u.GetValidationResults(instance, validationContext) ?? []).ToList();

        // 如果验证未通过且配置了自定义错误信息，则在首部添加自定义错误信息
        if (validationResults.Count > 0 && (string?)ErrorMessageString is not null)
        {
            validationResults.Insert(0,
                new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.MemberNames));
        }

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public override void Validate(T? instance, ValidationContext<T> validationContext)
    {
        // 获取匹配到的验证器集合
        var matchedValidators = GetMatchedValidators(instance);

        // 空检查
        if (matchedValidators is null or { Count: 0 })
        {
            return;
        }

        // 遍历验证器集合
        foreach (var validator in matchedValidators)
        {
            // 检查对象合法性
            if (!validator.IsValid(instance, validationContext))
            {
                ThrowValidationException(instance, validator, validationContext);
            }
        }
    }

    /// <summary>
    ///     抛出验证异常
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="validator">
    ///     <see cref="ValidatorBase" />
    /// </param>
    /// <param name="validationContext">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <exception cref="ValidationException"></exception>
    internal void ThrowValidationException(T? instance, ValidatorBase validator,
        IValidationContext? validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validator);

        // 检查是否配置了自定义错误信息
        if ((string?)ErrorMessageString is null)
        {
            validator.Validate(instance, validationContext);
        }
        else
        {
            throw new ValidationException(
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!),
                    validationContext?.MemberNames), null, instance);
        }
    }

    /// <summary>
    ///     释放资源
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        // 释放所有验证器资源
        foreach (var validator in (_conditionResult.DefaultRules ?? []).Concat(
                     _conditionResult.ConditionalRules.SelectMany(u => u.Validators)))
        {
            if (validator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    ///     获取匹配到的验证器集合
    /// </summary>
    /// <param name="instance">对象</param>
    /// <returns>
    ///     <see cref="IReadOnlyList{T}" />
    /// </returns>
    internal IReadOnlyList<ValidatorBase>? GetMatchedValidators(T? instance)
    {
        // 初始化匹配到的验证器集合
        IReadOnlyList<ValidatorBase>? matchedValidators = null;

        // 遍历并查找第一个条件匹配的验证器集合
        foreach (var (condition, validators) in _conditionResult.ConditionalRules)
        {
            // ReSharper disable once InvertIf
            if (condition(instance!))
            {
                matchedValidators = validators;
                break;
            }
        }

        // 没有匹配条件时使用默认验证器集合
        matchedValidators ??= _conditionResult.DefaultRules;

        return matchedValidators;
    }

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 遍历所有验证器并尝试同步 IServiceProvider 委托
        foreach (var validator in (_conditionResult.DefaultRules ?? []).Concat(
                     _conditionResult.ConditionalRules.SelectMany(u => u.Validators)))
        {
            // 检查验证器是否实现 IValidatorInitializer 接口
            if (validator is IValidatorInitializer initializer)
            {
                // 同步 IServiceProvider 委托
                initializer.InitializeServiceProvider(serviceProvider);
            }
        }
    }
}