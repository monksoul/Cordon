// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     规则集匹配器
/// </summary>
internal static class RuleSetMatcher
{
    /// <summary>
    ///     检查传入的规则集是否与指定的规则集匹配
    /// </summary>
    /// <param name="ruleSets">预设规则集</param>
    /// <param name="activeRuleSets">传入的激活规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool Matches(string?[]? ruleSets, string?[]? activeRuleSets)
    {
        // 规范化规则集
        var current = (ruleSets ?? []).Select(NormalizeRuleSet).ToArray();
        var input = (activeRuleSets ?? []).Select(NormalizeRuleSet).ToArray();

        // 当前实例未定义规则集时
        if (current is { Length: 0 })
        {
            return input is { Length: 0 } || input.Contains("*") || input.Contains(null);
        }

        // 当前实例有规则集但无传入规则集时
        if (input is { Length: 0 })
        {
            return current.Contains("*") || current.Contains(null);
        }

        // 当双方均有规则集时
        return input.Contains("*") || current.Contains("*") || current.Intersect(input).Any();

        // 规范化规则集：去除前后空格
        static string? NormalizeRuleSet(string? ruleSet) => ruleSet?.Trim();
    }
}