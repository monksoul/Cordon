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
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    internal readonly ValueAnnotationValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="LengthValidator" />
    /// </summary>
    /// <param name="minimumLength">最小允许长度</param>
    /// <param name="maximumLength">最大允许长度</param>
    public LengthValidator(int minimumLength, int maximumLength)
    {
        MinimumLength = minimumLength;
        MaximumLength = maximumLength;

        _validator = new ValueAnnotationValidator(new LengthAttribute(minimumLength, maximumLength));

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
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        _validator.IsValid(value, validationContext);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, MinimumLength, MaximumLength);
}