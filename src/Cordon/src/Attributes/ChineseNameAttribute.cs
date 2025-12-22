// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     中文姓名验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ChineseNameAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="ChineseNameAttribute" />
    /// </summary>
    public ChineseNameAttribute()
    {
        Validator = new ChineseNameValidator();

        UseResourceKey(() => nameof(ValidationMessages.ChineseNameValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="ChineseNameValidator" />
    /// </summary>
    protected ChineseNameValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}