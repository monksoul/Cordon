// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     最大长度验证器
/// </summary>
public class MaxLengthValidator : ValidatorBase
{
    /// <summary>
    ///     最大有效的长度值
    /// </summary>
    internal const int MaxAllowableLength = -1;

    /// <summary>
    ///     <inheritdoc cref="MaxLengthValidator" />
    /// </summary>
    /// <param name="length">最大允许长度</param>
    public MaxLengthValidator(int length)
    {
        Length = length;

        UseResourceKey(() => nameof(ValidationMessages.MaxLengthValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="MaxLengthValidator" />
    /// </summary>
    public MaxLengthValidator()
        : this(MaxAllowableLength)
    {
    }

    /// <summary>
    ///     最大允许长度
    /// </summary>
    public int Length { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 验证长度参数的合法性
        EnsureLegalLengths();

        // 空检查
        if (value == null)
        {
            return true;
        }

        // 尝试获取对象长度
        if (!value.TryGetCount(out var length))
        {
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture,
                "The field of type {0} must be a string, array or ICollection type.", value.GetType()));
        }

        return MaxAllowableLength == Length || length <= Length;
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Length);

    /// <summary>
    ///     验证长度参数的合法性
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    internal void EnsureLegalLengths()
    {
        if (Length is 0 or < -1)
        {
            throw new InvalidOperationException(
                "MaxLengthValidator must have a Length value that is greater than zero. Use MaxLength() without parameters to indicate that the string or array can have the maximum allowable length.");
        }
    }
}