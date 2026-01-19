// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     座机（电话）验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class TelephoneAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="TelephoneValidator" />
    internal readonly TelephoneValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="TelephoneAttribute" />
    /// </summary>
    public TelephoneAttribute()
    {
        _validator = new TelephoneValidator();

        UseResourceKey(() => nameof(ValidationMessages.TelephoneValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}