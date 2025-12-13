// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     验证选项特性
/// </summary>
/// <remarks>
///     要在项目中启用 <see cref="ValidationOptionsAttribute" /> 支持，需在配置服务时调用：
///     <c>services.AddControllers().AddValidationOptions()</c>。
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ValidationOptionsAttribute : Attribute
{
    /// <summary>
    ///     <inheritdoc cref="ValidationOptionsAttribute" />
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    public ValidationOptionsAttribute(string?[]? ruleSets = null) => RuleSets = ruleSets;

    /// <summary>
    ///     规则集
    /// </summary>
    public string?[]? RuleSets { get; }
}