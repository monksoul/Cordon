// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     字符串长度验证器
/// </summary>
public class StringLengthValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="StringLengthValidator" />
    /// </summary>
    /// <param name="maximumLength">最大允许长度</param>
    public StringLengthValidator(int maximumLength)
    {
        MaximumLength = maximumLength;

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     最大允许长度
    /// </summary>
    public int MaximumLength { get; }

    /// <summary>
    ///     最小允许长度
    /// </summary>
    public int MinimumLength { get; set; }

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

        var length = ((string)value).Length;
        return length >= MinimumLength && length <= MaximumLength;
    }

    /// <inheritdoc />
    public override string? FormatErrorMessage(string name)
    {
        // 验证长度参数的合法性
        EnsureLegalLengths();

        return string.Format(CultureInfo.CurrentCulture,
            ErrorMessageString, name, MaximumLength, MinimumLength);
    }

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        MinimumLength != 0 && !CustomErrorMessageSet
            ? nameof(ValidationMessages.StringLengthValidator_ValidationError_MinimumLength)
            : nameof(ValidationMessages.StringLengthValidator_ValidationError);

    /// <summary>
    ///     验证长度参数的合法性
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    internal void EnsureLegalLengths()
    {
        if (MaximumLength < 0)
        {
            throw new InvalidOperationException("The maximum length must be a nonnegative integer.");
        }

        if (MaximumLength < MinimumLength)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                "The maximum value '{0}' must be greater than or equal to the minimum value '{1}'.", MaximumLength,
                MinimumLength));
        }
    }
}