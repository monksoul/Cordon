// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Core.Extensions;

/// <summary>
///     <see cref="IDictionary{TKey, TValue}" /> 拓展类
/// </summary>
internal static class IDictionaryExtensions
{
    /// <summary>
    ///     添加或更新
    /// </summary>
    /// <typeparam name="TKey">字典键类型</typeparam>
    /// <typeparam name="TValue">字典值类型</typeparam>
    /// <param name="dictionary">
    ///     <see cref="IDictionary{TKey, TValue}" />
    /// </param>
    /// <param name="key">
    ///     <typeparamref name="TKey" />
    /// </param>
    /// <param name="value">
    ///     <typeparamref name="TValue" />
    /// </param>
    internal static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TKey : notnull =>
        dictionary[key] = value;
}