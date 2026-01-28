// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <inheritdoc cref="ValidatorResult" />
/// <typeparam name="T">对象类型</typeparam>
public sealed class ValidatorResult<T> : ValidatorResult
{
    /// <summary>
    ///     <inheritdoc cref="ValidatorResult{T}" />
    /// </summary>
    /// <param name="isValid">验证是否通过</param>
    /// <param name="validationResults">验证结果列表</param>
    /// <param name="instance">验证的对象</param>
    internal ValidatorResult(bool isValid, List<ValidationResult>? validationResults, T? instance)
        : base(isValid, validationResults, instance)
    {
    }

    /// <summary>
    ///     验证的对象
    /// </summary>
    public new T? Instance => (T?)base.Instance;
}

/// <summary>
///     验证器执行结果
/// </summary>
public class ValidatorResult
{
    /// <summary>
    ///     <inheritdoc cref="ValidatorResult" />
    /// </summary>
    /// <param name="isValid">验证是否通过</param>
    /// <param name="validationResults">验证结果列表</param>
    /// <param name="instance">验证的对象</param>
    internal ValidatorResult(bool isValid, List<ValidationResult>? validationResults, object? instance)
    {
        IsValid = isValid;
        ValidationResults = validationResults;
        Instance = instance;
    }

    /// <summary>
    ///     验证是否通过
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    ///     验证结果列表
    /// </summary>
    public IReadOnlyList<ValidationResult>? ValidationResults { get; }

    /// <summary>
    ///     验证的对象
    /// </summary>
    public object? Instance { get; }

    /// <summary>
    ///     验证验证失败时抛出 <see cref="ValidationException" /> 异常
    /// </summary>
    /// <exception cref="ValidationException">
    ///     当 <see cref="IsValid" /> 为 <see langword="false" /> 时。
    /// </exception>
    public void ThrowIfInvalid()
    {
        if (!IsValid)
        {
            throw new ValidationException(ValidationResults![0], null, Instance);
        }
    }
}