// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     年龄（0-120 岁）验证器
/// </summary>
public class AgeValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="AgeValidator" />
    /// </summary>
    public AgeValidator() => UseResourceKey(GetResourceKey);

    /// <summary>
    ///     是否仅允许成年人（18 岁及以上）
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool IsAdultOnly { get; set; }

    /// <summary>
    ///     是否允许字符串数值
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowStringValues { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 空检查
        if (value is null)
        {
            return true;
        }

        // 尝试将值解析为合法年龄（0-120）
        int? parsedAge = value switch
        {
            int i and >= 0 and <= 120 => i,
            uint u and <= 120 => (int)u,
            long l and >= 0 and <= 120 => (int)l,
            ulong ul and <= 120 => (int)ul,
            short s and >= 0 and <= 120 => s,
            ushort us and <= 120 => us,
            byte b => b,
            sbyte sb and >= 0 and <= 120 => sb,
            string str when AllowStringValues &&
                            int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var age) &&
                            age is >= 0 and <= 120 => age,
            _ => null
        };

        // 空检查
        if (parsedAge is null)
        {
            return false;
        }

        return !IsAdultOnly || parsedAge.Value >= 18;
    }

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