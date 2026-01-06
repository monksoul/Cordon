// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     对象验证器代理
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class ObjectValidatorProxy<T> : ValidatorBase<T>, IValidatorInitializer, IMemberPathRepairable,
    IDisposable
{
    /// <inheritdoc cref="IObjectValidator{T}" />
    internal readonly IObjectValidator<T> _objectValidator;

    /// <summary>
    ///     <inheritdoc cref="ObjectValidatorProxy{T}" />
    /// </summary>
    /// <param name="objectValidator">
    ///     <see cref="IObjectValidator{T}" />
    /// </param>
    public ObjectValidatorProxy(IObjectValidator<T> objectValidator)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(objectValidator);

        _objectValidator = objectValidator;

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    string? IMemberPathRepairable.MemberPath { get; set; }

    /// <inheritdoc />
    void IMemberPathRepairable.RepairMemberPaths(string? memberPath) => RepairMemberPaths(memberPath);

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

        _objectValidator.Dispose();
    }

    /// <inheritdoc />
    public override bool IsValid(T? instance, ValidationContext<T> validationContext) =>
        _objectValidator.IsValid(instance, validationContext.RuleSets);

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext) =>
        _objectValidator.GetValidationResults(instance, validationContext.RuleSets);

    /// <inheritdoc />
    public override void Validate(T? instance, ValidationContext<T> validationContext) =>
        _objectValidator.Validate(instance, validationContext.RuleSets);

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal void InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        _objectValidator.InitializeServiceProvider(serviceProvider);

    /// <inheritdoc cref="IMemberPathRepairable.RepairMemberPaths" />
    internal virtual void RepairMemberPaths(string? memberPath)
    {
        if (_objectValidator is IMemberPathRepairable memberPathRepairable)
        {
            memberPathRepairable.RepairMemberPaths(memberPath);
        }
    }
}