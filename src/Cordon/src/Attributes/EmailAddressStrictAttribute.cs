// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     邮箱地址增强版验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class EmailAddressStrictAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="EmailAddressValidator" />
    internal readonly EmailAddressValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="EmailAddressStrictAttribute" />
    /// </summary>
    public EmailAddressStrictAttribute()
    {
        _validator = new EmailAddressValidator();

        UseResourceKey(() => nameof(ValidationMessages.EmailAddressValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}