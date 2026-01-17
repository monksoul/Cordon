// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     数据验证模块常量配置
/// </summary>
internal static class Constants
{
    /// <summary>
    ///     验证选项键
    /// </summary>
    /// <remarks>
    ///     用于 <see cref="ValidationContext" />、<c>ValidationOptionsModelValidator</c> 和
    ///     <c>ValidationOptionsAsyncPageFilter</c> 中写入规则集配置。
    /// </remarks>
    internal static readonly object ValidationOptionsKey = new();

    /// <summary>
    ///     验证上下文键
    /// </summary>
    /// <remarks>
    ///     用于使用 <![CDATA[ValidationContext.With<T>()]]> 时设置。
    /// </remarks>
    internal static readonly object ValidationContextKey = new();
}