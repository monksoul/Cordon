// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     强密码模式验证特性
/// </summary>
/// <remarks>
///     密码长度为 12-64 位，必须包含大小写字母、数字、特殊字符（如 <![CDATA[!@#$%^&*]]>）。
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class StrongPasswordAttribute : PasswordAttribute
{
    /// <summary>
    ///     <inheritdoc cref="StrongPasswordAttribute" />
    /// </summary>
    public StrongPasswordAttribute() => Strong = true;
}