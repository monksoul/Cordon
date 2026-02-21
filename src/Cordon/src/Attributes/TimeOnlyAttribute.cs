// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     时间格式 <see cref="System.TimeOnly" /> 验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class TimeOnlyAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="TimeOnlyValidator" />
    internal readonly TimeOnlyValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="TimeOnlyAttribute" />
    /// </summary>
    /// <param name="formats">允许的时间格式（如 "HH:mm:ss"）</param>
    public TimeOnlyAttribute(params string[] formats)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(formats);

        Formats = formats;
        _validator = new TimeOnlyValidator(formats);

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     允许的时间格式（如 "HH:mm:ss"）
    /// </summary>
    public string[] Formats { get; }

    /// <summary>
    ///     区域性（格式提供器）
    /// </summary>
    public string? Culture
    {
        get;
        set
        {
            field = value;
            _validator.Provider = string.IsNullOrEmpty(value) ? null : CultureInfo.GetCultureInfo(value);
        }
    }

    /// <summary>
    ///     格式提供器
    /// </summary>
    /// <remarks>使用 <see cref="Culture" /> 进行指定。</remarks>
    public IFormatProvider? Provider => _validator.Provider;

    /// <summary>
    ///     日期解析样式
    /// </summary>
    /// <remarks>需与 <see cref="Formats" /> 搭配使用。默认值为：<see cref="DateTimeStyles.None" />。</remarks>
    public DateTimeStyles Style
    {
        get;
        set
        {
            field = value;
            _validator.Style = value;
        }
    } = DateTimeStyles.None;

    /// <summary>
    ///     格式化后的允许的日期格式列表
    /// </summary>
    internal string FormatsFormatted => string.Join(", ", Formats.Select(u => $"'{u}'"));

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, FormatsFormatted);

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        Formats.Length == 0
            ? nameof(ValidationMessages.TimeOnlyValidator_ValidationError)
            : nameof(ValidationMessages.TimeOnlyValidator_ValidationError_Formats);
}