// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     空集合、数组和字符串验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class EmptyAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="EmptyValidator" />
    internal readonly EmptyValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="EmptyAttribute" />
    /// </summary>
    public EmptyAttribute()
    {
        _validator = new EmptyValidator();

        UseResourceKey(() => nameof(ValidationMessages.EmptyValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}