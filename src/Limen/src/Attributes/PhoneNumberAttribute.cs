// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     手机号（中国）验证特性
/// </summary>
/// <remarks>支持国际区号（如：13800138000 或 +8613800138000 或 008613800138000）。</remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class PhoneNumberAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="PhoneNumberAttribute" />
    /// </summary>
    public PhoneNumberAttribute()
    {
        Validator = new PhoneNumberValidator();

        UseResourceKey(() => nameof(ValidationMessages.PhoneNumberValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="PhoneNumberValidator" />
    /// </summary>
    protected PhoneNumberValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}