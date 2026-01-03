// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <see cref="DateTime" /> 验证器
/// </summary>
public class DateTimeValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="DateTimeValidator" />
    /// </summary>
    /// <param name="formats">允许的日期格式（如 "yyyy-MM-dd HH:mm:ss"）</param>
    public DateTimeValidator(params string[] formats)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(formats);

        Formats = formats;

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     允许的日期格式（如 "yyyy-MM-dd HH:mm:ss"）
    /// </summary>
    public string[] Formats { get; }

    /// <summary>
    ///     格式提供器
    /// </summary>
    /// <remarks>默认值为：<see cref="CultureInfo.InvariantCulture" /></remarks>
    public IFormatProvider? Provider { get; set; } = CultureInfo.InvariantCulture;

    /// <summary>
    ///     日期解析样式
    /// </summary>
    /// <remarks>需与 <see cref="Provider" /> 搭配使用。默认值为：<see cref="DateTimeStyles.None" />。</remarks>
    public DateTimeStyles Style { get; set; } = DateTimeStyles.None;

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value switch
        {
            null => true,
            DateTime or DateTimeOffset or DateOnly => true,
            string text => ValidateDateTime(text),
            _ => false
        };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) => string.Format(CultureInfo.CurrentCulture,
        ErrorMessageString, name, string.Join(", ", Formats.Select(u => $"'{u}'")));

    /// <summary>
    ///     验证日期时间有效性
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ValidateDateTime(string text) =>
        Formats.Length == 0
            ? DateTime.TryParse(text, Provider, Style, out _)
            : DateTime.TryParseExact(text, Formats, Provider, Style, out _);

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        Formats.Length == 0
            ? nameof(ValidationMessages.DateTimeValidator_ValidationError)
            : nameof(ValidationMessages.DateTimeValidator_ValidationError_Formats);
}