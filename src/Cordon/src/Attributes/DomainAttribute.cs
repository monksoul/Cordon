// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     域名验证特性
/// </summary>
/// <remarks>不含协议（如 https/http）。</remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class DomainAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="DomainAttribute" />
    /// </summary>
    public DomainAttribute()
    {
        Validator = new DomainValidator();

        UseResourceKey(() => nameof(ValidationMessages.DomainValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="DomainValidator" />
    /// </summary>
    protected DomainValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}