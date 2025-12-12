// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     数据验证构建器
/// </summary>
public sealed class ValidationBuilder
{
    /// <summary>
    ///     <see cref="AbstractValidator{T}" /> 类型集合
    /// </summary>
    internal Dictionary<Type, Type>? _validatorTypes;

    /// <summary>
    ///     添加 <see cref="AbstractValidator{T}" /> 对象验证器
    /// </summary>
    /// <param name="validatorType">
    ///     <see cref="AbstractValidator{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public ValidationBuilder AddValidator(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        Type validatorType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorType);

        // 检查类型是否可实例化
        if (!validatorType.IsInstantiable())
        {
            throw new ArgumentException(
                // ReSharper disable once LocalizableElement
                $"Type `{validatorType}` must be a non-abstract, non-static class to be registered as a validator.",
                nameof(validatorType));
        }

        // 检查是否继承自 AbstractValidator<T> 抽象基类
        if (!TryGetValidatedType(validatorType, out var modelType))
        {
            throw new ArgumentException(
                // ReSharper disable once LocalizableElement
                $"Type `{validatorType}` is not a valid validator; it does not derive from `AbstractValidator<T>`.",
                nameof(validatorType));
        }

        _validatorTypes ??= new Dictionary<Type, Type>();

        _validatorTypes[validatorType] = modelType;

        return this;
    }

    /// <summary>
    ///     添加 <see cref="AbstractValidator{T}" /> 对象验证器
    /// </summary>
    /// <param name="validatorTypes">
    ///     <see cref="AbstractValidator{T}" /> 集合
    /// </param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    public ValidationBuilder AddValidators(params IEnumerable<Type> validatorTypes)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorTypes);

        foreach (var validatorType in validatorTypes)
        {
            AddValidator(validatorType);
        }

        return this;
    }

    /// <summary>
    ///     扫描程序集并添加 <see cref="AbstractValidator{T}" /> 对象验证器
    /// </summary>
    /// <param name="assemblies"><see cref="Assembly" /> 集合</param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddValidatorFromAssemblies(params IEnumerable<Assembly?> assemblies)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assemblies);

        return AddValidators(assemblies.SelectMany(ass =>
            (ass?.GetTypes() ?? Enumerable.Empty<Type>()).Where(t =>
                t.IsInstantiable() && TryGetValidatedType(t, out _))));
    }

    /// <summary>
    ///     构建模块服务
    /// </summary>
    /// <remarks>多个实例会导致重复注册。</remarks>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    internal void Build(IServiceCollection services)
    {
        /*
         * 防止重复注册验证服务：ValidationBuilder 可能被多次构建，
         * 而 BuildObjectValidatorServices 内部会注册 IValidationDataContext 服务，
         * 多次注册会导致验证器重复创建和 ServiceProvider 同步冲突。
         */
        if (services.Any(u => u.ServiceType == typeof(IValidationDataContext)))
        {
            return;
        }

        // 注册验证数据上下文服务
        services.TryAddScoped<IValidationDataContext, ValidationDataContext>();

        // 构建对象验证器服务
        BuildObjectValidatorServices(services);
    }

    /// <summary>
    ///     构建对象验证器服务
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    internal void BuildObjectValidatorServices(IServiceCollection services)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(services);

        // 空检查
        if (_validatorTypes is null)
        {
            return;
        }

        // 遍历所有对象验证器类型并注册为服务
        foreach (var (validatorType, modelType) in _validatorTypes)
        {
            // 注册 IObjectValidator<T> 泛型接口
            services.Add(ServiceDescriptor.Transient(typeof(IObjectValidator<>).MakeGenericType(modelType),
                provider => CreateObjectValidator(provider, validatorType)));

            // 注册 AbstractValidator<T> 基类
            // services.Add(ServiceDescriptor.Transient(typeof(AbstractValidator<>).MakeGenericType(modelType),
            //     provider => CreateObjectValidator(provider, validatorType)));

            // 注册 IObjectValidator 非泛型接口
            services.Add(ServiceDescriptor.Transient(typeof(IObjectValidator),
                provider => CreateObjectValidator(provider, validatorType)));
        }
    }

    /// <summary>
    ///     创建对象验证器实例
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="validatorType">对象验证器类型</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    internal static object CreateObjectValidator(IServiceProvider serviceProvider, Type validatorType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(validatorType);

        // 创建对象验证器实例
        var validatorObject = ActivatorUtilities.CreateInstance(serviceProvider, validatorType);

        // 检查验证器是否实现 IValidatorInitializer 接口
        if (validatorObject is IValidatorInitializer initializer)
        {
            // 同步 IServiceProvider 委托
            initializer.InitializeServiceProvider(serviceProvider.GetService);
        }

        return validatorObject;
    }

    /// <summary>
    ///     检查是否继承自 <see cref="AbstractValidator{T}" /> 抽象基类
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="modelType">被验证的模型类型</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool TryGetValidatedType(Type type, [NotNullWhen(true)] out Type? modelType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(type);

        // 初始化被验证的模型类型
        modelType = null;

        // 必须是类且不能是抽象类
        if (!type.IsClass || type.IsAbstract)
        {
            return false;
        }

        // 沿着继承链向上遍历
        for (var current = type; current != null; current = current.BaseType)
        {
            // 检查当前类型是否是泛型并且泛型定义为 AbstractValidator<>
            if (!current.IsGenericType || current.GetGenericTypeDefinition() != typeof(AbstractValidator<>))
            {
                continue;
            }

            // 提取泛型参数 T（即被验证的模型类型）
            modelType = current.GetGenericArguments()[0];

            return true;
        }

        return false;
    }
}