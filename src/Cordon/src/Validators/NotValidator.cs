// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     取反验证器
/// </summary>
/// <remarks>只要内部配置的验证规则中【有任何一个】验证失败，则整体取反验证通过。</remarks>
/// <typeparam name="T">对象类型</typeparam>
public class NotValidator<T> : ValidatorBase<T>, IValidatorInitializer, IDisposable
{
    /// <summary>
    ///     <see cref="ValidatorBase" /> 集合
    /// </summary>
    internal readonly IReadOnlyList<ValidatorBase> _validators;

    /// <summary>
    ///     <inheritdoc cref="NotValidator{T}" />
    /// </summary>
    /// <param name="validators">验证器列表</param>
    public NotValidator(ValidatorBase[] validators)
        : this(u => u.AddValidators(validators))
    {
    }

    /// <summary>
    ///     <inheritdoc cref="NotValidator{T}" />
    /// </summary>
    /// <param name="configure">验证器配置委托</param>
    public NotValidator(Action<FluentValidatorBuilder<T>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        _validators = new FluentValidatorBuilder<T>().Build(configure);
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
    public override bool IsValid(T? instance, ValidationContext<T> validationContext) =>
        !_validators.All(u => u.IsValid(instance, validationContext));

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