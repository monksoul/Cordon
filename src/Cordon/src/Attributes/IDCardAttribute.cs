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
    /// <inheritdoc cref="IDCardValidator" />
    internal readonly IDCardValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="IDCardAttribute" />
    /// </summary>
    public IDCardAttribute()
    {
        _validator = new IDCardValidator();

        UseResourceKey(() => nameof(ValidationMessages.IDCardValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}