// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     年龄（0-120 岁）验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class AgeAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="AgeValidator" />
    internal readonly AgeValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="AgeAttribute" />
    /// </summary>
    public AgeAttribute()
    {
        _validator = new AgeValidator();

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     是否仅允许成年人（18 岁及以上）
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool IsAdultOnly
    {
        get;
        set
        {
            field = value;
            _validator.IsAdultOnly = value;
        }
    }

    /// <summary>
    ///     是否允许字符串数值
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowStringValues
    {
        get;
        set
        {
            field = value;
            _validator.AllowStringValues = value;
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
        IsAdultOnly
            ? nameof(ValidationMessages.AgeValidator_ValidationError_IsAdultOnly)
            : nameof(ValidationMessages.AgeValidator_ValidationError);
}