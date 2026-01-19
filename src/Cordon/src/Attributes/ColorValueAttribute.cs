// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     颜色值验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class ColorValueAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="ColorValueValidator" />
    internal readonly ColorValueValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="ColorValueAttribute" />
    /// </summary>
    public ColorValueAttribute()
    {
        _validator = new ColorValueValidator();

        UseResourceKey(() => nameof(ValidationMessages.ColorValueValidator_ValidationError));
    }

    /// <summary>
    ///     是否启用完整模式
    /// </summary>
    /// <remarks>在完整模式下，支持的颜色格式包括：十六进制颜色、RGB、RGBA、HSL 和 HSLA；若未启用，则仅支持十六进制颜色、RGB 和 RGBA。默认值为：<c>false</c>。</remarks>
    public bool FullMode
    {
        get;
        set
        {
            field = value;
            _validator.FullMode = value;
        }
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}