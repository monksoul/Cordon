// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     单项验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class SingleAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="SingleAttribute" />
    /// </summary>
    public SingleAttribute()
    {
        Validator = new SingleValidator();

        UseResourceKey(() => nameof(ValidationMessages.SingleValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="SingleValidator" />
    /// </summary>
    protected SingleValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}