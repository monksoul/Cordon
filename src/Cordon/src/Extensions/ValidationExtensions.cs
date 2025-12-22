// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     数据验证模块拓展类
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    ///     若 <see cref="ValidationResult" /> 列表为空则返回 <c>null</c>，否则返回列表本身
    /// </summary>
    /// <param name="validationResults"><see cref="ValidationResult" /> 列表</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public static List<ValidationResult>? ToResults(this List<ValidationResult>? validationResults) =>
        validationResults is { Count: > 0 } ? validationResults : null;

    /// <summary>
    ///     若 <see cref="ValidationResult" /> 列表为空则返回 <c>null</c>，否则返回列表本身
    /// </summary>
    /// <param name="validationResults"><see cref="ValidationResult" /> 列表</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public static List<ValidationResult>? ToResults(this IEnumerable<ValidationResult>? validationResults) =>
        validationResults?.ToList().ToResults();

    /// <summary>
    ///     设置规则集
    /// </summary>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext" />
    /// </param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="ValidationContext" />
    /// </returns>
    public static ValidationContext WithRuleSets(this ValidationContext validationContext, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);

        validationContext.Items[ValidationDataContext.ValidationOptionsKey] = new ValidationOptionsMetadata(ruleSets);

        return validationContext;
    }

    /// <summary>
    ///     创建对象验证器用于在 <see cref="IValidatableObject.Validate" /> 方法中内联配置验证规则
    /// </summary>
    /// <remarks>配置完成后，请调用无参的 <see cref="ObjectValidator{T}.ToResults(bool)" /> 方法获取验证结果。</remarks>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext" />
    /// </param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public static ObjectValidator<T> ContinueWith<T>(this ValidationContext validationContext)
        where T : class
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);

        // 拷贝一份验证上下文数据配置并追加自身实例
        var items = new Dictionary<object, object?>(validationContext.Items)
        {
            [ObjectValidator<T>.ValidationContextsKey] = validationContext
        };

        // 初始化 ObjectValidator<T> 实例并跳过属性验证特性验证，避免死循环
        var objectValidator = new ObjectValidator<T>(items).SkipAnnotationValidation();

        // 同步 IServiceProvider 委托
        objectValidator.InitializeServiceProvider(validationContext.GetService);

        return objectValidator;
    }

    /// <summary>
    ///     创建对象验证器验证当前实例并返回验证结果集合
    /// </summary>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext" />
    /// </param>
    /// <param name="configure">自定义配置委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public static IEnumerable<ValidationResult> ValidateUsing<T>(this ValidationContext validationContext,
        Action<ObjectValidator<T>>? configure = null)
        where T : class
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);

        // 初始化 ObjectValidator<T> 实例并跳过属性验证特性验证，避免死循环
        var objectValidator = new ObjectValidator<T>(new Dictionary<object, object?>(validationContext.Items))
            .SkipAnnotationValidation();

        // 调用自定义配置委托
        configure?.Invoke(objectValidator);

        return validationContext.ValidateWith(objectValidator);
    }

    /// <summary>
    ///     使用指定对象验证器验证当前实例并返回验证结果集合
    /// </summary>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext" />
    /// </param>
    /// <param name="objectValidator">
    ///     <see cref="AbstractValidator{T}" />
    /// </param>
    /// <param name="disposeAfterValidation">
    ///     是否在验证完成后自动释放 <paramref name="objectValidator" />。默认值为：<c>true</c>
    /// </param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    public static IEnumerable<ValidationResult> ValidateWith<T>(this ValidationContext validationContext,
        ObjectValidator<T> objectValidator, bool disposeAfterValidation = true) where T : class
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);
        ArgumentNullException.ThrowIfNull(objectValidator);

        // 禁用属性验证特性验证，避免死循环（TODO: 这里存在线程安全问题）
        var needsRestoreSuppressAnnotationValidation = false;
        if (!objectValidator.Options.SuppressAnnotationValidation)
        {
            objectValidator.Options.SuppressAnnotationValidation = true;
            needsRestoreSuppressAnnotationValidation = true;
        }

        // 同步 IServiceProvider 委托
        objectValidator.InitializeServiceProvider(validationContext.GetService);

        try
        {
            // 获取对象验证结果集合
            return objectValidator.ToResults(validationContext, disposeAfterValidation);
        }
        finally
        {
            // 恢复禁用属性验证特性验证状态
            if (needsRestoreSuppressAnnotationValidation)
            {
                objectValidator.Options.SuppressAnnotationValidation = false;
            }
        }
    }
}