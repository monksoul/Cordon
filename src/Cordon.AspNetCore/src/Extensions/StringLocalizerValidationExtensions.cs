// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <see cref="IStringLocalizer" /> 扩展类
/// </summary>
public static class StringLocalizerValidationExtensions
{
    /// <summary>
    ///     获取具有给定名称的字符串资源
    /// </summary>
    /// <param name="localizer">
    ///     <see cref="IStringLocalizer" />
    /// </param>
    /// <param name="name">字符串资源的名称</param>
    /// <returns>
    ///     <see cref="LocalizedString" />
    /// </returns>
    public static LocalizedString GetString(this IStringLocalizer? localizer, string name) =>
        localizer is null ? new LocalizedString(name, name) : localizer[name];

    /// <summary>
    ///     获取具有给定名称的字符串资源
    /// </summary>
    /// <param name="localizer">
    ///     <see cref="IStringLocalizer" />
    /// </param>
    /// <param name="name">字符串资源的名称</param>
    /// <param name="arguments">用于设置字符串格式的值</param>
    /// <returns>
    ///     <see cref="LocalizedString" />
    /// </returns>
    public static LocalizedString GetString(this IStringLocalizer? localizer, string name, params object[] arguments) =>
        localizer is null ? new LocalizedString(name, string.Format(name, arguments)) : localizer[name, arguments];
}