// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     最小长度验证器
/// </summary>
public class MinLengthValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    internal readonly ValueAnnotationValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="MinLengthValidator" />
    /// </summary>
    /// <param name="length">最小允许长度</param>
    public MinLengthValidator(int length)
    {
        Length = length;
        _validator = new ValueAnnotationValidator(new MinLengthAttribute(Length));

        UseResourceKey(() => nameof(ValidationMessages.MinLengthValidator_ValidationError));
    }

    /// <summary>
    ///     最小允许长度
    /// </summary>
    public int Length { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        _validator.IsValid(value, validationContext);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Length);
}