// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     文件拓展名验证器
/// </summary>
public class FileExtensionsValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="AttributeValueValidator" />
    /// </summary>
    internal readonly AttributeValueValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="FileExtensionsValidator" />
    /// </summary>
    /// <param name="extensions">文件拓展名</param>
    public FileExtensionsValidator(string extensions)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(extensions);

        Extensions = extensions;

        _validator = new AttributeValueValidator(new FileExtensionsAttribute { Extensions = extensions });

        UseResourceKey(() => nameof(ValidationMessages.FileExtensionsValidator_ValidationError));
    }

    /// <summary>
    ///     文件拓展名
    /// </summary>
    public string Extensions { get; }

    /// <summary>
    ///     格式化后的文件拓展名列表
    /// </summary>
    internal string ExtensionsFormatted => ExtensionsParsed.Aggregate((left, right) => left + ", " + right);

    /// <summary>
    ///     标准化后的文件拓展名字符串
    /// </summary>
    internal string ExtensionsNormalized =>
        Extensions.Replace(" ", string.Empty).Replace(".", string.Empty).ToLowerInvariant();

    /// <summary>
    ///     解析后的文件拓展名集合
    /// </summary>
    internal IEnumerable<string> ExtensionsParsed => ExtensionsNormalized.Split(',').Select(e => "." + e);

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        _validator.IsValid(value, validationContext);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, ExtensionsFormatted);
}