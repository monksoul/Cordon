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
    /// <remarks>
    ///     <para>使用默认字典名称：<see cref="SensitiveWordSanitizerFactory.DefaultName" />。</para>
    ///     <para>若该名称已注册，则不会执行配置委托，也不会覆盖已有实例。</para>
    /// </remarks>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddSensitiveWords(Action<SensitiveWordSanitizerBuilder> configure)
    {
        SensitiveWordSanitizerFactory.Register(configure);

        return this;
    }

    /// <summary>
    ///     添加敏感词词库
    /// </summary>
    /// <remarks>若该名称已注册，则不会执行配置委托，也不会覆盖已有实例。</remarks>
    /// <param name="dictionaryName">字典名称，不区分大小写</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddSensitiveWords(string dictionaryName, Action<SensitiveWordSanitizerBuilder> configure)
    {
        SensitiveWordSanitizerFactory.Register(dictionaryName, configure);

        return this;
    }

    /// <summary>
    ///     添加敏感词词库
    /// </summary>
    /// <remarks>若该名称已注册，则不会执行配置委托，也不会覆盖已有实例。</remarks>
    /// <param name="dictionaryName">字典名称，不区分大小写</param>
    /// <param name="factory">构建 <see cref="SensitiveWordSanitizer" /> 的工厂委托</param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddSensitiveWords(string dictionaryName, Func<SensitiveWordSanitizer> factory)
    {
        SensitiveWordSanitizerFactory.Register(dictionaryName, factory);

        return this;
    }
}