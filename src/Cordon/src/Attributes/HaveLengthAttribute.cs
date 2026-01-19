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
    /// <inheritdoc cref="HaveLengthValidator" />
    internal readonly HaveLengthValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="HaveLengthAttribute" />
    /// </summary>
    /// <param name="length">长度</param>
    public HaveLengthAttribute(int length)
    {
        Length = length;
        _validator = new HaveLengthValidator(length);

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
    public bool AllowEmpty
    {
        get;
        set
        {
            field = value;
            _validator.AllowEmpty = value;
        }
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

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