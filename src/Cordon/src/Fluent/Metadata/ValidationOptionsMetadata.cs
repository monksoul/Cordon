// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证选项元数据
/// </summary>
/// <remarks>用于在验证管道中传递解析后的 <see cref="ValidationOptionsAttribute" /> 信息。</remarks>
public sealed class ValidationOptionsMetadata
{
    /// <summary>
    ///     <inheritdoc cref="ValidationOptionsMetadata" />
    /// </summary>
    /// <param name="ruleSets">规则集</param>
    internal ValidationOptionsMetadata(string?[]? ruleSets) => RuleSets = ruleSets;

    /// <summary>
    ///     规则集
    /// </summary>
    public string?[]? RuleSets { get; }
}