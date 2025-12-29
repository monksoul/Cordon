// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     固定长度验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class HaveLengthAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="HaveLengthAttribute" />
    /// </summary>
    /// <param name="length">长度</param>
    public HaveLengthAttribute(int length)
    {
        Length = length;
        Validator = new HaveLengthValidator(length);

        UseResourceKey(() => nameof(ValidationMessages.HaveLengthValidator_ValidationError));
    }

    /// <summary>
    ///     长度
    /// </summary>
    public int Length { get; }

    /// <summary>
    ///     <inheritdoc cref="HaveLengthValidator" />
    /// </summary>
    protected HaveLengthValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Length);
}