// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     密码验证特性
/// </summary>
/// <remarks>
///     支持普通和强密码两种模式：
///     普通模式：密码长度为 8-64 位，包含至少一个字母和一个数字。
///     强密码模式：密码长度为 12-64 位，必须包含大小写字母、数字、特殊字符（如 <![CDATA[!@#$%^&*]]>）。
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class PasswordAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="PasswordValidator" />
    internal readonly PasswordValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="PasswordAttribute" />
    /// </summary>
    public PasswordAttribute()
    {
        _validator = new PasswordValidator();

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     是否启用强密码验证模式
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool Strong
    {
        get;
        set
        {
            field = value;
            _validator.Strong = value;
        }
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        Strong
            ? nameof(ValidationMessages.PasswordValidator_ValidationError_Strong)
            : nameof(ValidationMessages.PasswordValidator_ValidationError);
}