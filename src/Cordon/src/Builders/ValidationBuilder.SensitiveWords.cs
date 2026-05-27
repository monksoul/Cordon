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
    /// <param name="filePath">文件路径</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddSensitiveWords(string filePath, SensitiveWordOptions? options = null) =>
        AddSensitiveWords(filePath, SensitiveWordOptions.DefaultDictionaryName, options);

    /// <summary>
    ///     添加敏感词词库
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="dictionaryName">字典名称</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddSensitiveWords(string filePath, string dictionaryName,
        SensitiveWordOptions? options = null)
    {
        SensitiveWordSanitizerFactory.GetOrCreateFromPath(dictionaryName, filePath, options);

        return this;
    }

    /// <summary>
    ///     添加敏感词词库
    /// </summary>
    /// <param name="words">敏感词集合</param>
    /// <param name="dictionaryName">字典名称</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddSensitiveWords(IEnumerable<string> words, string dictionaryName,
        SensitiveWordOptions? options = null)
    {
        SensitiveWordSanitizerFactory.GetOrCreate(dictionaryName, words, options);

        return this;
    }

    /// <summary>
    ///     添加敏感词词库
    /// </summary>
    /// <param name="stream">输入流</param>
    /// <param name="dictionaryName">字典名称</param>
    /// <param name="options"><see cref="SensitiveWordOptions" />，默认值为：<see cref="SensitiveWordOptions.Default" /></param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddSensitiveWords(Stream stream, string dictionaryName,
        SensitiveWordOptions? options = null)
    {
        SensitiveWordSanitizerFactory.GetOrCreateFromStream(dictionaryName, stream, options);

        return this;
    }
}