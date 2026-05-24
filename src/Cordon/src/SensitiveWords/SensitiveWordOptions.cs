// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     敏感词清理器配置选项
/// </summary>
public sealed record SensitiveWordOptions
{
    /// <summary>
    ///     默认敏感词字典名称
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         当 <see cref="SensitiveWordValidator.DictionaryName" /> 和 <see cref="SensitiveWordValidator.FilePath" />
    ///         均未配置时，将使用此名称从工厂获取实例。
    ///     </para>
    ///     <para>用户需在应用启动时通过 <c>SensitiveWordSanitizerFactory.GetOrCreateFromPath</c> 等方式预注册该名称的实例。</para>
    /// </remarks>
    public const string DefaultDictionaryName = "SensitiveWords:Default";

    /// <summary>
    ///     默认选项
    /// </summary>
    /// <remarks>忽略大小写、忽略符号。</remarks>
    public static readonly SensitiveWordOptions Default = new();

    /// <summary>
    ///     是否忽略大小写
    /// </summary>
    /// <remarks>默认值为：<c>true</c>。</remarks>
    public bool IgnoreCase { get; init; } = true;

    /// <summary>
    ///     是否跳过标点/空格/符号进行匹配
    /// </summary>
    /// <remarks>默认值为：<c>true</c>。</remarks>
    public bool IgnoreSymbol { get; init; } = true;
}