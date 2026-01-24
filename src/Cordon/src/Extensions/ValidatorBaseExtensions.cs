// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <see cref="ValidatorBase" /> 扩展类
/// </summary>
public static class ValidatorBaseExtensions
{
    /// <summary>
    ///     设置错误信息
    /// </summary>
    /// <typeparam name="TValidator">
    ///     <see cref="ValidatorBase" />
    /// </typeparam>
    /// <param name="validator">
    ///     <typeparamref name="TValidator" />
    /// </param>
    /// <param name="errorMessage">错误信息</param>
    /// <returns>
    ///     <typeparamref name="TValidator" />
    /// </returns>
    public static TValidator WithMessage<TValidator>(this TValidator validator, string? errorMessage)
        where TValidator : ValidatorBase
    {
        validator.ErrorMessage = errorMessage;

        return validator;
    }

    /// <summary>
    ///     设置错误信息资源
    /// </summary>
    /// <typeparam name="TValidator">
    ///     <see cref="ValidatorBase" />
    /// </typeparam>
    /// <param name="validator">
    ///     <typeparamref name="TValidator" />
    /// </param>
    /// <param name="resourceType">错误信息资源类型</param>
    /// <param name="resourceName">错误信息资源名称</param>
    /// <returns>
    ///     <typeparamref name="TValidator" />
    /// </returns>
    public static TValidator WithMessage<TValidator>(this TValidator validator,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type resourceType, string resourceName)
        where TValidator : ValidatorBase
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(resourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);

        validator.ErrorMessageResourceType = resourceType;
        validator.ErrorMessageResourceName = resourceName;

        return validator;
    }
}