// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     中文/汉字验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ChineseAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="ChineseAttribute" />
    /// </summary>
    public ChineseAttribute()
    {
        Validator = new ChineseValidator();

        UseResourceKey(() => nameof(ValidationMessages.ChineseValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="ChineseValidator" />
    /// </summary>
    protected ChineseValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}