// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <inheritdoc cref="ValidationBuilder" />
public sealed partial class ValidationBuilder
{
    /// <summary>
    ///     添加敏感词词库
    /// </summary>
    /// <remarks>使用默认字典名称：<see cref="SensitiveWordOptions.DefaultDictionaryName" />。</remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddSensitiveWords(Action<SensitiveWordSanitizerBuilder> configure)
    {
        SensitiveWordSanitizerFactory.GetOrCreate(configure);

        return this;
    }

    /// <summary>
    ///     添加敏感词词库
    /// </summary>
    /// <param name="dictionaryName">字典名称</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddSensitiveWords(string dictionaryName, Action<SensitiveWordSanitizerBuilder> configure)
    {
        SensitiveWordSanitizerFactory.GetOrCreate(dictionaryName, configure);

        return this;
    }
}