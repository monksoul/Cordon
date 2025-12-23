// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     <c>null</c> 验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class NullAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="NullAttribute" />
    /// </summary>
    public NullAttribute()
    {
        Validator = new NullValidator();

        UseResourceKey(() => nameof(ValidationMessages.NullValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="NullValidator" />
    /// </summary>
    protected NullValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}