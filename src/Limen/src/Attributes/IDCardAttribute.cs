// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     身份证号验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class IDCardAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="IDCardAttribute" />
    /// </summary>
    public IDCardAttribute()
    {
        Validator = new IDCardValidator();

        UseResourceKey(() => nameof(ValidationMessages.IDCardValidator_ValidationError));
    }

    /// <summary>
    ///     <inheritdoc cref="IDCardValidator" />
    /// </summary>
    protected IDCardValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}