// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <see cref="MustValidator{T}" /> 内部静态类
/// </summary>
/// <remarks>可通过 <see cref="Must.WithMessage(string)" /> 或 <see cref="Must.WithMessage(Type,string)" /> 设置不满足条件时的异常消息。</remarks>
public static class Must
{
    /// <summary>
    ///     设置错误信息
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>Never Return</returns>
    [DoesNotReturn]
    public static bool WithMessage(string message)
    {
        ValidatorException.Throw(message);
        return false;
    }

    /// <summary>
    ///     设置错误信息资源
    /// </summary>
    /// <param name="resourceType">错误信息资源类型</param>
    /// <param name="resourceName">错误信息资源名称</param>
    /// <returns>Never Return</returns>
    [DoesNotReturn]
    public static bool WithMessage(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type resourceType, string resourceName) =>
        WithMessage(ValidatorBase.GetResourceString(resourceType, resourceName) ?? resourceName);
}

/// <summary>
///     自定义条件成立时委托验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class MustValidator<T> : ValidatorBase<T>
{
    /// <summary>
    ///     <inheritdoc cref="MustValidator{T}" />
    /// </summary>
    /// <param name="condition">条件委托</param>
    public MustValidator(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        Condition = (instance, _) => condition(instance);
    }

    /// <summary>
    ///     <inheritdoc cref="MustValidator{T}" />
    /// </summary>
    /// <param name="condition">条件委托</param>
    public MustValidator(Func<T, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        Condition = condition;
    }

    /// <summary>
    ///     条件委托
    /// </summary>
    public Func<T, ValidationContext<T>, bool> Condition { get; }

    /// <inheritdoc />
    public override bool IsValid(T? instance, ValidationContext<T> validationContext)
    {
        try
        {
            return Condition(instance!, validationContext);
        }
        // 检查是否是 ValidatorException 异常
        catch (ValidatorException)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext)
    {
        try
        {
            // 检查条件是否成立
            if (Condition(instance!, validationContext))
            {
                return null;
            }

            return
            [
                new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.MemberNames)
            ];
        }
        // 检查是否是 ValidatorException 异常
        catch (ValidatorException e)
        {
            return
            [
                new ValidationResult(
                    string.Format(CultureInfo.CurrentCulture, e.Message, validationContext.DisplayName),
                    validationContext.MemberNames)
            ];
        }
    }

    /// <inheritdoc />
    public override void Validate(T? instance, ValidationContext<T> validationContext)
    {
        try
        {
            // 检查条件是否成立
            if (!Condition(instance!, validationContext))
            {
                throw new ValidationException(
                    new ValidationResult(FormatErrorMessage(validationContext.DisplayName),
                        validationContext.MemberNames), null, instance);
            }
        }
        // 检查是否是 ValidatorException 异常
        catch (ValidatorException e)
        {
            throw new ValidationException(
                new ValidationResult(
                    string.Format(CultureInfo.CurrentCulture, e.Message, validationContext.DisplayName),
                    validationContext.MemberNames), null, instance);
        }
    }
}