// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     非 <c>null</c> 验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class NotNullAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="NotNullAttribute" />
    /// </summary>
    public NotNullAttribute()
    {
        Validator = new NotNullValidator();

        UseResourceKey(() => nameof(ValidationMessages.NotNullValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="NotNullValidator" />
    /// </summary>
    protected NotNullValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}