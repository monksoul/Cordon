// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     JSON 格式验证器
/// </summary>
/// <remarks>验证输入是否为有效的 JSON 对象（{...}）或数组（[...]）。</remarks>
public class JsonValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="JsonValidator" />
    /// </summary>
    public JsonValidator() => UseResourceKey(() => nameof(ValidationMessages.JsonValidator_ValidationError));

    /// <summary>
    ///     是否允许末尾多余逗号
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowTrailingCommas { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 空检查
        if (value is null)
        {
            return true;
        }

        // 检查值是否为字符串类型，且字符串不是由空白字符组成
        if (value is not string str || string.IsNullOrWhiteSpace(str))
        {
            return false;
        }

        // 去除字符串两端空格
        var text = str.Trim();

        // 检查字符串是否以 '{' 开头和 '}' 结尾，或者以 '[' 开头和 ']' 结尾
        if ((!text.StartsWith('{') || !text.EndsWith('}')) && (!text.StartsWith('[') || !text.EndsWith(']')))
        {
            return false;
        }

        try
        {
            // 使用 JsonDocument 解析字符串，若解析成功，说明是一个有效的 JSON 格式
            using var jsonDocument = JsonDocument.Parse(text,
                new JsonDocumentOptions { AllowTrailingCommas = AllowTrailingCommas });

            return jsonDocument.RootElement.ValueKind is JsonValueKind.Object or JsonValueKind.Array;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}