// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     固定长度验证器
/// </summary>
public class HaveLengthValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="HaveLengthValidator" />
    /// </summary>
    /// <param name="length">长度</param>
    public HaveLengthValidator(int length)
    {
        Length = length;

        UseResourceKey(() => nameof(ValidationMessages.HaveLengthValidator_ValidationError));
    }

    /// <summary>
    ///     长度
    /// </summary>
    public int Length { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value is null || (value.TryGetCount(out var count) && count == Length);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Length);
}