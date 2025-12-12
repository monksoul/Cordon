// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     最大长度验证器
/// </summary>
public class MaxLengthValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    internal readonly ValueAnnotationValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="MaxLengthValidator" />
    /// </summary>
    /// <param name="length">最大允许长度</param>
    public MaxLengthValidator(int length)
    {
        Length = length;
        _validator = new ValueAnnotationValidator(new MaxLengthAttribute(Length));

        UseResourceKey(() => nameof(ValidationMessages.MaxLengthValidator_ValidationError));
    }

    /// <summary>
    ///     最大允许长度
    /// </summary>
    public int Length { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Length);
}