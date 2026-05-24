// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     敏感词验证特性
/// </summary>
/// <remarks>
///     <para>支持通过 <see cref="DictionaryName" /> 或 <see cref="FilePath" /> 配置词库来源。</para>
///     <para>注意：<see cref="DictionaryName" /> 与 <see cref="FilePath" /> 互斥，只能设置其中一个。</para>
///     <para>若需注入 <see cref="SensitiveWordSanitizer" /> 实例，请通过 <see cref="SensitiveWordSanitizerFactory" /> 注册或手动构建。</para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class SensitiveWordAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="SensitiveWordValidator" />
    internal readonly SensitiveWordValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="SensitiveWordAttribute" />
    /// </summary>
    public SensitiveWordAttribute()
    {
        _validator = new SensitiveWordValidator();

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     敏感词字典名称
    /// </summary>
    /// <remarks>对应 <see cref="SensitiveWordSanitizerFactory" /> 中缓存的实例名称。与 <see cref="FilePath" /> 互斥，只能设置其中一个。</remarks>
    public string? DictionaryName
    {
        get;
        set
        {
            field = value;
            _validator.DictionaryName = value;
        }
    }

    /// <summary>
    ///     敏感词文件路径
    /// </summary>
    /// <remarks>
    ///     与 <see cref="DictionaryName" /> 互斥，只能设置其中一个。支持后续通过
    ///     <see cref="SensitiveWordSanitizerFactory.Refresh(string)" /> 进行刷新（热更新）。
    /// </remarks>
    public string? FilePath
    {
        get;
        set
        {
            field = value;
            _validator.FilePath = value;
        }
    }

    /// <summary>
    ///     是否在错误信息中显示命中的敏感词详情
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool ShowMatchedWords
    {
        get;
        set
        {
            field = value;
            _validator.ShowMatchedWords = value;
        }
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        ShowMatchedWords
            ? nameof(ValidationMessages.SensitiveWordValidator_ValidationError_ShowMatchedWords)
            : nameof(ValidationMessages.SensitiveWordValidator_ValidationError);
}