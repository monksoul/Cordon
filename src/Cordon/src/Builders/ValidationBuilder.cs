// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     数据验证构建器
/// </summary>
public sealed class ValidationBuilder
{
    /// <summary>
    ///     <see cref="AbstractValidator{T}" /> 泛型定义
    /// </summary>
    internal static readonly Type AbstractValidatorDefinition = typeof(AbstractValidator<>);

    /// <summary>
    ///     <see cref="AbstractValueValidator{T}" /> 泛型定义
    /// </summary>
    internal static readonly Type AbstractValueValidatorDefinition = typeof(AbstractValueValidator<>);

    /// <summary>
    ///     <see cref="IObjectValidator{T}" /> 类型集合
    /// </summary>
    internal Dictionary<Type, Type>? _validatorTypes;

    /// <summary>
    ///     添加 <see cref="IObjectValidator{T}" /> 验证器
    /// </summary>
    /// <typeparam name="TValidator">
    ///     <see cref="IObjectValidator{T}" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddValidator<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TValidator>()
        where TValidator : class, IObjectValidator =>
        AddValidator(typeof(TValidator));

    /// <summary>
    ///     添加 <see cref="IObjectValidator{T}" /> 验证器
    /// </summary>
    /// <param name="validatorType">
    ///     <see cref="IObjectValidator{T}" />
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

        // 检查验证器是否已添加
        if (_validatorTypes is not null && _validatorTypes.ContainsKey(validatorType))
        {
            return this;
        }

        // 检查类型是否可实例化
        if (!validatorType.IsInstantiable())
        {
            throw new ArgumentException(
                // ReSharper disable once LocalizableElement
                $"Type `{validatorType}` must be a non-abstract, non-static class to be registered as a validator.",
                nameof(validatorType));
        }

        // 检查是否继承自 AbstractValidator<T> 或 AbstractValueValidator<T> 抽象基类
        if (!TryGetValidatedType(validatorType, AbstractValidatorDefinition, out var modelType) &&
            !TryGetValidatedType(validatorType, AbstractValueValidatorDefinition, out modelType))
        {
            throw new ArgumentException(
                // ReSharper disable once LocalizableElement
                $"Type `{validatorType}` is not a valid validator; it does not derive from `AbstractValidator<>` or `AbstractValueValidator<>`.",
                nameof(validatorType));
        }

        _validatorTypes ??= new Dictionary<Type, Type>();
        _validatorTypes[validatorType] = modelType;

        return this;
    }

    /// <summary>
    ///     添加 <see cref="IObjectValidator{T}" /> 验证器
    /// </summary>
    /// <param name="validatorTypes">
    ///     <see cref="IObjectValidator{T}" /> 集合
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
    ///     扫描程序集并添加 <see cref="IObjectValidator{T}" /> 验证器
    /// </summary>
    /// <param name="assemblies"><see cref="Assembly" /> 集合</param>
    /// <returns>
    ///     <see cref="ValidationBuilder" />
    /// </returns>
    public ValidationBuilder AddValidatorsFromAssemblies(params IEnumerable<Assembly?> assemblies)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assemblies);

        // 扫描程序集中所有继承自 AbstractValidator<T> 或 AbstractValueValidator<T> 抽象基类的类型
        var candidateTypes = assemblies.SelectMany(ass => (ass?.GetTypes() ?? Enumerable.Empty<Type>()).Where(t =>
            t.IsInstantiable() && (TryGetValidatedType(t, AbstractValidatorDefinition, out _) ||
                                   TryGetValidatedType(t, AbstractValueValidatorDefinition, out _))));

        return AddValidators(candidateTypes);
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
         * 而 BuildValidatorServices 内部会注册 IValidationDataContext 服务，
         * 多次注册会导致验证器重复创建和 ServiceProvider 同步冲突。
         */
        if (services.Any(u => u.ServiceType == typeof(IValidationDataContext)))
        {
            return;
        }

        // 注册验证数据上下文服务
        services.TryAddScoped<IValidationDataContext, ValidationDataContext>();

        // 注册数据验证服务
        services.TryAddTransient<IValidationService, ValidationService>();

        // 构建验证器服务
        BuildValidatorServices(services);
    }

    /// <summary>
    ///     构建验证器服务
    /// </summary>
    /// <param name="services">
    ///     <see cref="IServiceCollection" />
    /// </param>
    internal void BuildValidatorServices(IServiceCollection services)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(services);

        // 空检查
        if (_validatorTypes is null)
        {
            return;
        }

        // 遍历所有验证器类型并注册为服务
        foreach (var (validatorType, modelType) in _validatorTypes)
        {
            // 注册 IObjectValidator<T> 泛型接口
            services.Add(ServiceDescriptor.Transient(typeof(IObjectValidator<>).MakeGenericType(modelType),
                provider => CreateValidator(provider, validatorType)));

            // 注册 IObjectValidator 非泛型接口
            services.Add(ServiceDescriptor.Transient(typeof(IObjectValidator),
                provider => CreateValidator(provider, validatorType)));

            // 注册验证器自身
            services.Add(ServiceDescriptor.Transient(validatorType,
                provider => CreateValidator(provider, validatorType)));

            // 检查是否是单值验证器
            // ReSharper disable once InvertIf
            if (typeof(IValueValidator).IsAssignableFrom(validatorType))
            {
                // 注册 IValueValidator<T> 泛型接口
                services.Add(ServiceDescriptor.Transient(typeof(IValueValidator<>).MakeGenericType(modelType),
                    provider => CreateValidator(provider, validatorType)));

                // 注册 IValueValidator 非泛型接口
                services.Add(ServiceDescriptor.Transient(typeof(IValueValidator),
                    provider => CreateValidator(provider, validatorType)));
            }
        }
    }

    /// <summary>
    ///     创建验证器实例
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="validatorType">验证器类型</param>
    /// <returns>
    ///     <see cref="object" />
    /// </returns>
    internal static object CreateValidator(IServiceProvider serviceProvider, Type validatorType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(validatorType);

        // 创建验证器实例
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
    ///     检查是否继承自 <paramref name="genericTypeDefinition" /> 抽象基类
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="genericTypeDefinition">泛型基类定义类型</param>
    /// <param name="modelType">被验证的模型类型</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    /// <exception cref="ArgumentException"></exception>
    internal static bool TryGetValidatedType(Type type, Type genericTypeDefinition,
        [NotNullWhen(true)] out Type? modelType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(genericTypeDefinition);

        // 泛型定义检查
        if (!genericTypeDefinition.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                // ReSharper disable once LocalizableElement
                $"The type '{genericTypeDefinition}' is not a generic type definition; expected an open generic such as `AbstractValidator<>` or `AbstractValueValidator<>`.",
                nameof(genericTypeDefinition));
        }

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
            // 检查当前类型是否是泛型并且泛型定义为 genericTypeDefinition
            if (!current.IsGenericType || current.GetGenericTypeDefinition() != genericTypeDefinition)
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