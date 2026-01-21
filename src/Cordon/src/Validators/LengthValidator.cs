// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     长度验证器
/// </summary>
public class LengthValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="LengthValidator" />
    /// </summary>
    /// <param name="minimumLength">最小允许长度</param>
    /// <param name="maximumLength">最大允许长度</param>
    public LengthValidator(int minimumLength, int maximumLength)
    {
        MinimumLength = minimumLength;
        MaximumLength = maximumLength;

        UseResourceKey(() => nameof(ValidationMessages.LengthValidator_ValidationError));
    }

    /// <summary>
    ///     最小允许长度
    /// </summary>
    public int MinimumLength { get; }

    /// <summary>
    ///     最大允许长度
    /// </summary>
    public int MaximumLength { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 验证长度参数的合法性
        EnsureLegalLengths();

        // 空检查
        if (value is null)
        {
            return true;
        }

        // 尝试获取对象长度
        if (!value.TryGetCount(out var length))
        {
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture,
                "The field of type {0} must be a string, array or ICollection type.", value.GetType()));
        }

        return (uint)(length - MinimumLength) <= (uint)(MaximumLength - MinimumLength);
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) => string.Format(CultureInfo.CurrentCulture,
        ErrorMessageString, name, MinimumLength, MaximumLength);

    /// <summary>
    ///     验证长度参数的合法性
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    internal void EnsureLegalLengths()
    {
        if (MinimumLength < 0)
        {
            throw new InvalidOperationException(
                "LengthValidator must have a MinimumLength value that is zero or greater.");
        }

        if (MaximumLength < MinimumLength)
        {
            throw new InvalidOperationException(
                "LengthValidator must have a MaximumLength value that is greater than or equal to MinimumLength.");
        }
    }
}