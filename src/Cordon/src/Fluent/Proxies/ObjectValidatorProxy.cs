// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     对象验证器代理
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ObjectValidatorProxy<T> : ValidatorBase<T>, IValidatorInitializer, IDisposable
{
    /// <inheritdoc cref="ObjectValidator{T}" />
    internal readonly ObjectValidator<T> _validator;

    /// <summary>
    ///     <inheritdoc cref="ObjectValidatorProxy{T}" />
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ObjectValidator{T}" />
    /// </param>
    public ObjectValidatorProxy(ObjectValidator<T> validator)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validator);

        _validator = validator;

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

        // 释放验证器资源
        if (_validator is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <inheritdoc />
    public override bool IsValid(T? instance, ValidationContext<T> validationContext) =>
        _validator.IsValid(instance, validationContext.RuleSets);

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext) =>
        _validator.GetValidationResults(instance, validationContext.RuleSets);

    /// <inheritdoc />
    public override void Validate(T? instance, ValidationContext<T> validationContext) =>
        _validator.Validate(instance, validationContext.RuleSets);

    /// <inheritdoc />
    public override string? FormatErrorMessage(string name) =>
        (string?)ErrorMessageString is null ? null : base.FormatErrorMessage(name);

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 检查验证器是否实现 IValidatorInitializer 接口
        if (_validator is IValidatorInitializer initializer)
        {
            // 同步 IServiceProvider 委托
            initializer.InitializeServiceProvider(serviceProvider);
        }
    }
}