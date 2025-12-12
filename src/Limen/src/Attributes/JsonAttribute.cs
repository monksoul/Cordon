// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     JSON 格式验证特性
/// </summary>
/// <remarks>验证输入是否为有效的 JSON 对象（{...}）或数组（[...]）。</remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class JsonAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="JsonAttribute" />
    /// </summary>
    public JsonAttribute()
    {
        Validator = new JsonValidator();

        UseResourceKey(() => nameof(ValidationMessages.JsonValidator_ValidationError));
    }

    /// <summary>
    ///     是否允许末尾多余逗号
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowTrailingCommas
    {
        get;
        set
        {
            field = value;
            Validator.AllowTrailingCommas = value;
        }
    }

    /// <summary>
    ///     <inheritdoc cref="JsonValidator" />
    /// </summary>
    protected JsonValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}