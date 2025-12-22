// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     密码验证器
/// </summary>
/// <remarks>
///     支持普通和强密码两种模式：
///     普通模式：密码长度为 8-64 位，包含至少一个字母和一个数字。
///     强密码模式：密码长度为 12-64 位，必须包含大小写字母、数字、特殊字符（如 <![CDATA[!@#$%^&*]]>）。
/// </remarks>
public partial class PasswordValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="PasswordValidator" />
    /// </summary>
    public PasswordValidator() => UseResourceKey(GetResourceKey);

    /// <summary>
    ///     是否启用强密码验证模式
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool Strong { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value) =>
        value switch
        {
            null => true,
            string text => !string.IsNullOrWhiteSpace(text) &&
                           (Strong ? StrongRegex().IsMatch(text) : Regex().IsMatch(text)),
            _ => false
        };

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

    /// <summary>
    ///     普通密码正则表达式
    /// </summary>
    /// <remarks>至少包含一个字母和一个数字，长度 8-64 位。</remarks>
    [GeneratedRegex(@"\A(?=.*[a-zA-Z])(?=.*\d).{8,64}\z")]
    private static partial Regex Regex();

    /// <summary>
    ///     强密码正则表达式
    /// </summary>
    /// <remarks>必须包含大小写、数字、特殊字符，长度 12-64 位。</remarks>
    [GeneratedRegex(@"\A(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).{12,64}\z")]
    private static partial Regex StrongRegex();
}