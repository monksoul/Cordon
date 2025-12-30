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

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     长度
    /// </summary>
    public int Length { get; }

    /// <summary>
    ///     是否允许空集合、数组和字符串
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowEmpty { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value is null || (value.TryGetCount(out var count) && ((AllowEmpty && count == 0) || count == Length));

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Length);

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        AllowEmpty
            ? nameof(ValidationMessages.HaveLengthValidator_ValidationError_AllowEmpty)
            : nameof(ValidationMessages.HaveLengthValidator_ValidationError);
}