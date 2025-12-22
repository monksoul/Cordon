// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     强密码模式验证器
/// </summary>
/// <remarks>
///     密码长度为 12-64 位，必须包含大小写字母、数字、特殊字符（如 <![CDATA[!@#$%^&*]]>）。
/// </remarks>
public class StrongPasswordValidator : PasswordValidator
{
    /// <summary>
    ///     <inheritdoc cref="StrongPasswordValidator" />
    /// </summary>
    public StrongPasswordValidator() => Strong = true;
}