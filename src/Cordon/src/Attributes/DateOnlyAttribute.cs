// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     <see cref="DateOnly" /> 验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DateOnlyAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="DateOnlyValidator" />
    internal readonly DateOnlyValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="DateOnlyAttribute" />
    /// </summary>
    /// <param name="formats">允许的日期格式（如 "yyyy-MM-dd"）</param>
    public DateOnlyAttribute(params string[] formats)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(formats);

        Formats = formats;
        _validator = new DateOnlyValidator(formats);

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     允许的日期格式（如 "yyyy-MM-dd"）
    /// </summary>
    public string[] Formats { get; }

    /// <summary>
    ///     格式提供器
    /// </summary>
    /// <remarks>默认值为：<see cref="CultureInfo.InvariantCulture" /></remarks>
    public IFormatProvider? Provider
    {
        get;
        set
        {
            field = value;
            _validator.Provider = value;
        }
    } = CultureInfo.InvariantCulture;

    /// <summary>
    ///     日期解析样式
    /// </summary>
    /// <remarks>需与 <see cref="Provider" /> 搭配使用。默认值为：<see cref="DateTimeStyles.None" />。</remarks>
    public DateTimeStyles Style
    {
        get;
        set
        {
            field = value;
            _validator.Style = value;
        }
    } = DateTimeStyles.None;

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) => string.Format(CultureInfo.CurrentCulture,
        ErrorMessageString, name, string.Join(", ", Formats.Select(u => $"'{u}'")));

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        Formats.Length == 0
            ? nameof(ValidationMessages.DateOnlyValidator_ValidationError)
            : nameof(ValidationMessages.DateOnlyValidator_ValidationError_Formats);
}