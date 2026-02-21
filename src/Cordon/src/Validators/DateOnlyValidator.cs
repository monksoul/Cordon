// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <see cref="DateOnly" /> 验证器
/// </summary>
public class DateOnlyValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="DateOnlyValidator" />
    /// </summary>
    /// <param name="formats">允许的日期格式列表（如 "yyyy-MM-dd"）</param>
    public DateOnlyValidator(params string[] formats)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(formats);

        Formats = formats;

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     允许的日期格式列表（如 "yyyy-MM-dd"）
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
    /// <remarks>需与 <see cref="Formats" /> 搭配使用。默认值为：<see cref="DateTimeStyles.None" />。</remarks>
    public DateTimeStyles Style { get; set; } = DateTimeStyles.None;

    /// <summary>
    ///     格式化后的允许的日期格式列表
    /// </summary>
    internal string FormatsFormatted => string.Join(", ", Formats.Select(u => $"'{u}'"));

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value switch
        {
            null => true,
            DateOnly => true,
            string text => ValidateDate(text),
            _ => false
        };

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, FormatsFormatted);

    /// <summary>
    ///     验证日期有效性
    /// </summary>
    /// <param name="text">文本</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ValidateDate(string text) =>
        Formats.Length == 0
            ? DateOnly.TryParse(text, Provider, Style, out _)
            : DateOnly.TryParseExact(text, Formats, Provider, Style, out _);

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