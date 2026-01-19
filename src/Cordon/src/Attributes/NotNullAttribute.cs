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
    /// <inheritdoc cref="NotNullValidator" />
    internal readonly NotNullValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="NotNullAttribute" />
    /// </summary>
    public NotNullAttribute()
    {
        _validator = new NotNullValidator();

        UseResourceKey(() => nameof(ValidationMessages.NotNullValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}