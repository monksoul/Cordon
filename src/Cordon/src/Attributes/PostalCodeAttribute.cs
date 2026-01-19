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
    /// <inheritdoc cref="PostalCodeValidator" />
    internal readonly PostalCodeValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="PostalCodeAttribute" />
    /// </summary>
    public PostalCodeAttribute()
    {
        _validator = new PostalCodeValidator();

        UseResourceKey(() => nameof(ValidationMessages.PostalCodeValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}