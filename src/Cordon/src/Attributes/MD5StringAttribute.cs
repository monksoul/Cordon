// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     MD5 字符串验证特性
/// </summary>
/// <remarks>支持 32 位标准格式，可选 16 字符截断格式。</remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class MD5StringAttribute : ValidationBaseAttribute
{
    /// <summary>
    ///     <inheritdoc cref="MD5StringAttribute" />
    /// </summary>
    public MD5StringAttribute()
    {
        Validator = new MD5StringValidator();

        UseResourceKey(() => nameof(ValidationMessages.MD5StringValidator_ValidationError));
    }

    /// <summary>
    ///     是否允许截断的 128 位哈希值（16 字节的十六进制字符串，共 32 字符）
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowShortFormat
    {
        get;
        set
        {
            field = value;
            Validator.AllowShortFormat = value;
        }
    }

    /// <summary>
    ///     <inheritdoc cref="MD5StringValidator" />
    /// </summary>
    protected MD5StringValidator Validator { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => Validator.IsValid(value);
}