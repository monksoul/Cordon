// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     组合验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class CompositeValidator<T> : ValidatorBase<T>, IValidatorInitializer, IDisposable
{
    /// <summary>
    ///     <see cref="ValidatorBase" /> 集合
    /// </summary>
    internal readonly IReadOnlyList<ValidatorBase> _validators;

    /// <summary>
    ///     <inheritdoc cref="CompositeValidator{T}" />
    /// </summary>
    /// <param name="configure">验证器配置委托</param>
    public CompositeValidator(Action<FluentValidatorBuilder<T>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        _validators = new FluentValidatorBuilder<T>().Build(configure);

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <summary>
    ///     <inheritdoc cref="CompositeMode" />
    /// </summary>
    /// <remarks>默认值为：<see cref="CompositeMode.FailFast" />。</remarks>
    public CompositeMode Mode { get; set; } = CompositeMode.FailFast;

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
    public override bool IsValid(T? instance, ValidationContext<T> validationContext) =>
        Mode switch
        {
            CompositeMode.FailFast or CompositeMode.All => _validators.All(u => u.IsValid(instance, validationContext)),
            CompositeMode.Any => _validators.Any(u => u.IsValid(instance, validationContext)),
            _ => throw new NotSupportedException()
        };

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext)
    {
        // 初始化验证结果集合
        var validationResults = new List<ValidationResult>();

        // 遍历验证器集合
        foreach (var validator in _validators)
        {
            // 获取对象验证结果集合
            if (validator.GetValidationResults(instance, validationContext) is { Count: > 0 } results)
            {
                // 追加验证结果集合
                validationResults.AddRange(results);

                // 检查验证器模式是否是遇到首个验证失败即停止后续验证
                if (Mode is CompositeMode.FailFast)
                {
                    break;
                }
            }
            // 检查验证器模式是否是任一验证器验证成功，即视为整体验证通过
            else if (Mode is CompositeMode.Any)
            {
                // 清空验证结果集合
                validationResults.Clear();

                break;
            }
        }

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
        // 初始化首个验证无效的验证器
        ValidatorBase? firstFailedValidator = null;

        // 遍历验证器集合
        foreach (var validator in _validators)
        {
            // 检查对象合法性
            if (!validator.IsValid(instance, validationContext))
            {
                // 缓存首个验证无效的验证器
                firstFailedValidator ??= validator;

                // 检查验证器模式是否是遇到首个验证失败即停止后续验证
                if (Mode is CompositeMode.FailFast or CompositeMode.All)
                {
                    ThrowValidationException(instance, validator, validationContext);
                }
            }
            // 检查验证器模式是否是任一验证器验证成功，即视为整体验证通过
            else if (Mode is CompositeMode.Any)
            {
                return;
            }
        }

        // 空检查
        if (firstFailedValidator is not null)
        {
            ThrowValidationException(instance, firstFailedValidator, validationContext);
        }
    }

    /// <summary>
    ///     设置验证模式
    /// </summary>
    /// <param name="mode">
    ///     <see cref="CompositeMode" />
    /// </param>
    /// <returns>
    ///     <see cref="CompositeValidator{T}" />
    /// </returns>
    public CompositeValidator<T> UseMode(CompositeMode mode)
    {
        Mode = mode;

        return this;
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
        foreach (var validator in _validators)
        {
            if (validator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 遍历所有验证器并尝试同步 IServiceProvider 委托
        foreach (var validator in _validators)
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