// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     用户名验证特性
/// </summary>
/// <remarks>
///     长度 4-16 位，以字母开头，支持字母、数字、下划线、减号组合。
///     不允许包含空格或其他特殊字符，禁止连续特殊字符（如 __）。
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class UserNameAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="UserNameValidator" />
    internal readonly UserNameValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="UserNameAttribute" />
    /// </summary>
    public UserNameAttribute()
    {
        _validator = new UserNameValidator();

        UseResourceKey(() => nameof(ValidationMessages.UserNameValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}