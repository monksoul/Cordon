// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     邮政编码（中国）验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class PostalCodeAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="PostalCodeAttribute" />
    /// </summary>
    public PostalCodeAttribute()
    {
        Validator = new PostalCodeValidator();

        UseResourceKey(() => nameof(ValidationMessages.PostalCodeValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="PostalCodeValidator" />
    /// </summary>
    protected PostalCodeValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}