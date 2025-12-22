// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证特性抽象基类
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public abstract class ValidationBaseAttribute : ValidationAttribute
{
    /// <summary>
    ///     ErrorMessageResourceAccessor 属性设置器
    /// </summary>
    internal static readonly Lazy<Action<object, object?>> _errorMessageResourceAccessorSetter = new(() =>
        typeof(ValidationAttribute).CreatePropertySetter(
            typeof(ValidationAttribute).GetProperty("ErrorMessageResourceAccessor",
                BindingFlags.Instance | BindingFlags.NonPublic)!));

    /// <inheritdoc />
    protected ValidationBaseAttribute()
    {
    }

    /// <inheritdoc />
    protected ValidationBaseAttribute(string errorMessage)
        : base(errorMessage)
    {
    }

    /// <inheritdoc />
    protected ValidationBaseAttribute(Func<string> errorMessageAccessor)
        : base(errorMessageAccessor)
    {
    }

    /// <summary>
    ///     使用指定资源键设置验证错误消息
    /// </summary>
    /// <remarks>支持入口程序集覆盖框架内部资源，若未找到则返回占位符。</remarks>
    /// <param name="resourceKeyResolver">返回 <see cref="ValidationMessages" /> 中属性名的委托</param>
    protected void UseResourceKey(Func<string> resourceKeyResolver)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(resourceKeyResolver);

        // 创建委托的本地副本
        var capturedResourceKeyResolver = resourceKeyResolver;

        _errorMessageResourceAccessorSetter.Value(this, () =>
        {
            // 获取 ValidationMessages 中的属性名
            var resourceKey = capturedResourceKeyResolver();

            return ValidatorBase.GetResourceString(resourceKey) ?? $"[{resourceKey}]";
        });
    }
}