// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     文件扩展名验证器
/// </summary>
public class FileExtensionsValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="FileExtensionsValidator" />
    /// </summary>
    /// <remarks>默认文件扩展名为：<c>png,jpg,jpeg,gif</c>。</remarks>
    public FileExtensionsValidator() =>
        UseResourceKey(() => nameof(ValidationMessages.FileExtensionsValidator_ValidationError));

    /// <summary>
    ///     文件扩展名
    /// </summary>
    public string Extensions
    {
        get => string.IsNullOrWhiteSpace(field) ? "png,jpg,jpeg,gif" : field;
        set;
    }

    /// <summary>
    ///     格式化后的文件扩展名列表
    /// </summary>
    internal string ExtensionsFormatted => ExtensionsParsed.Aggregate((left, right) => left + ", " + right);

    /// <summary>
    ///     标准化后的文件扩展名字符串
    /// </summary>
    internal string ExtensionsNormalized =>
        Extensions.Replace(" ", string.Empty).Replace(".", string.Empty).ToLowerInvariant();

    /// <summary>
    ///     解析后的文件扩展名集合
    /// </summary>
    internal IEnumerable<string> ExtensionsParsed => ExtensionsNormalized.Split(',').Select(e => "." + e);

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value is null || (value is string valueAsString && ValidateExtension(valueAsString));

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, ExtensionsFormatted);

    /// <summary>
    ///     验证文件扩展名是否在允许的列表中
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ValidateExtension(string fileName) =>
        ExtensionsParsed.Contains(Path.GetExtension(fileName).ToLowerInvariant());
}