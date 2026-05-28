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
    /// <remarks>忽略大小写、忽略符号、忽略全角/半角差异、忽略 Unicode 字母变体。</remarks>
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

    /// <summary>
    ///     是否忽略全角与半角字符的差异
    /// </summary>
    /// <remarks>
    ///     <para>启用后，全角字母、数字、符号将视为与对应半角字符相同。例如 <c>ｆｕｃｋ</c> 等同于 <c>fuck</c>，<c>１２３</c> 等同于 <c>123</c>。</para>
    ///     <para>默认值为：<c>true</c>。</para>
    /// </remarks>
    public bool IgnoreFullwidth { get; init; } = true;

    /// <summary>
    ///     是否忽略 Unicode 字母变体
    /// </summary>
    /// <remarks>
    ///     <para>启用后，（带圈、带括号、数学粗体等），统一视为普通字母。例如 <c>Ⓕⓤc⒦</c>、<c>𝐟𝐮𝐜𝐤</c> 等变体均等同于 <c>fuck</c>。</para>
    ///     <para>默认值为：<c>true</c>。</para>
    /// </remarks>
    public bool IgnoreUnicodeVariants { get; init; } = true;
}