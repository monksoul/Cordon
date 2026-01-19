// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证器抽象基类
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public abstract class ValidatorBase<T> : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="ValidatorBase{T}" />
    /// </summary>
    protected ValidatorBase()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValidatorBase{T}" />
    /// </summary>
    /// <param name="errorMessage">错误信息</param>
    protected ValidatorBase(string errorMessage)
        : base(errorMessage)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValidatorBase{T}" />
    /// </summary>
    /// <param name="errorMessageResourceAccessor">错误信息资源访问器</param>
    protected ValidatorBase(Func<string> errorMessageResourceAccessor)
        : base(errorMessageResourceAccessor)
    {
    }

    /// <summary>
    ///     检查对象是否合法
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public abstract bool IsValid(T? instance, ValidationContext<T> validationContext);

    /// <summary>
    ///     获取对象验证结果列表
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public virtual List<ValidationResult>? GetValidationResults(T? instance, ValidationContext<T> validationContext) =>
        IsValid(instance, validationContext)
            ? null
            : [new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.MemberNames)];

    /// <summary>
    ///     执行验证
    /// </summary>
    /// <remarks>失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="instance">对象</param>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <exception cref="ValidationException"></exception>
    public virtual void Validate(T? instance, ValidationContext<T> validationContext)
    {
        // 检查对象是否合法
        if (!IsValid(instance, validationContext))
        {
            throw new ValidationException(
                new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.MemberNames),
                null, instance);
        }
    }

    /// <inheritdoc />
    public sealed override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 将 value 转换为 T
        var instance = ConvertValue(value);

        // 检查 validationContext 是否是 ValidationContext<T> 类型
        if (validationContext is ValidationContext<T> typedValidationContext)
        {
            return IsValid(instance, typedValidationContext);
        }

        return IsValid(instance, CreateValidationContext(instance, validationContext));
    }

    /// <inheritdoc />
    public sealed override List<ValidationResult>? GetValidationResults(object? value,
        IValidationContext? validationContext)
    {
        // 将 value 转换为 T
        var instance = ConvertValue(value);

        // 检查 validationContext 是否是 ValidationContext<T> 类型
        if (validationContext is ValidationContext<T> typedValidationContext)
        {
            return GetValidationResults(instance, typedValidationContext);
        }

        return GetValidationResults(instance, CreateValidationContext(instance, validationContext));
    }

    /// <inheritdoc />
    public sealed override void Validate(object? value, IValidationContext? validationContext)
    {
        // 将 value 转换为 T
        var instance = ConvertValue(value);

        // 检查 validationContext 是否是 ValidationContext<T> 类型
        if (validationContext is ValidationContext<T> typedValidationContext)
        {
            Validate(instance, typedValidationContext);
        }

        Validate(instance, CreateValidationContext(instance, validationContext));
    }

    /// <summary>
    ///     将 <see cref="object" /> 对象转换为 <typeparamref name="T" /> 对象
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <typeparamref name="T" />
    /// </returns>
    internal static T? ConvertValue(object? value) => (T?)value;

    /// <summary>
    ///     创建 <see cref="ValidationContext{T}" /> 实例
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="validationContext">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <returns>
    ///     <see cref="ValidationContext{T}" />
    /// </returns>
    internal static ValidationContext<T> CreateValidationContext(T? instance, IValidationContext? validationContext) =>
        new(instance!, validationContext is null ? null : validationContext.GetService,
            validationContext?.Items)
        {
            DisplayName = validationContext?.DisplayName!,
            MemberNames = validationContext?.MemberNames,
            RuleSets = validationContext?.RuleSets
        };
}

/// <summary>
///     验证器抽象基类
/// </summary>
public abstract class ValidatorBase
{
    /// <summary>
    ///     外部程序集用于覆盖默认验证消息的【强制约定类型全名】
    /// </summary>
    internal const string ExternalValidationMessagesFullTypeName = "Cordon.Resources.Overrides.ValidationMessages";

    /// <summary>
    ///     错误信息
    /// </summary>
    internal string? _errorMessage;

    /// <summary>
    ///     错误信息资源访问器
    /// </summary>
    internal Func<string>? _errorMessageResourceAccessor;

    /// <summary>
    ///     错误信息资源名称
    /// </summary>
    internal string? _errorMessageResourceName;

    /// <summary>
    ///     错误信息资源类型
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                DynamicallyAccessedMemberTypes.NonPublicProperties)]
    internal Type? _errorMessageResourceType;

    /// <summary>
    ///     <inheritdoc cref="ValidatorBase" />
    /// </summary>
    protected ValidatorBase() => UseResourceKey(() => nameof(ValidationMessages.ValidatorBase_ValidationError));

    /// <summary>
    ///     <inheritdoc cref="ValidatorBase" />
    /// </summary>
    /// <param name="errorMessage">错误信息</param>
    protected ValidatorBase(string errorMessage)
        : this(() => errorMessage)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValidatorBase" />
    /// </summary>
    /// <param name="errorMessageResourceAccessor">错误信息资源访问器</param>
    protected ValidatorBase(Func<string> errorMessageResourceAccessor) =>
        _errorMessageResourceAccessor = errorMessageResourceAccessor;

    /// <summary>
    ///     错误信息
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            _errorMessageResourceAccessor = null;
            CustomErrorMessageSet = true;

            // 触发属性变更事件
            OnPropertyChanged(value);
        }
    }

    /// <summary>
    ///     错误信息资源名称
    /// </summary>
    public string? ErrorMessageResourceName
    {
        get => _errorMessageResourceName;
        set
        {
            _errorMessageResourceName = value;
            _errorMessageResourceAccessor = null;
            CustomErrorMessageSet = true;

            // 触发属性变更事件
            OnPropertyChanged(value);
        }
    }

    /// <summary>
    ///     错误信息资源类型
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                DynamicallyAccessedMemberTypes.NonPublicProperties)]
    public Type? ErrorMessageResourceType
    {
        get => _errorMessageResourceType;
        set
        {
            _errorMessageResourceType = value;
            _errorMessageResourceAccessor = null;
            CustomErrorMessageSet = true;

            // 触发属性变更事件
            OnPropertyChanged(value);
        }
    }

    /// <summary>
    ///     规则集
    /// </summary>
    /// <remarks>实现内部规则集功能。</remarks>
    public string?[]? RuleSets
    {
        get;
        set
        {
            field = value;

            // 触发属性变更事件
            OnPropertyChanged(value);
        }
    }

    /// <summary>
    ///     错误信息资源访问器
    /// </summary>
    private protected Func<string> ErrorMessageResourceAccessor
    {
        init => _errorMessageResourceAccessor = value;
    }

    /// <summary>
    ///     错误信息字符串
    /// </summary>
    protected string ErrorMessageString
    {
        get
        {
            // 设置错误信息资源访问器
            SetupResourceAccessor();

            return _errorMessageResourceAccessor!();
        }
    }

    /// <summary>
    ///     是否设置了错误信息
    /// </summary>
    internal bool CustomErrorMessageSet { get; private set; }

    /// <summary>
    ///     是否支持异步操作
    /// </summary>
    /// <remarks>实现 <see cref="IAsyncValidator" /> 接口。</remarks>
    // ReSharper disable once SuspiciousTypeConversion.Global
    internal bool SupportsAsync => this is IAsyncValidator;

    /// <summary>
    ///     是否是验证器代理类型
    /// </summary>
    /// <remarks>验证器类型定义与 <see cref="ValidatorProxy{T,TValidator}" /> 泛型定义一致。</remarks>
    internal bool IsTypedProxy =>
        GetType().IsGenericType && GetType().GetGenericTypeDefinition() == typeof(ValidatorProxy<,>);

    /// <summary>
    ///     属性变更事件
    /// </summary>
    protected event EventHandler<ValidationPropertyChangedEventArgs>? PropertyChanged;

    /// <summary>
    ///     检查对象是否合法
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public virtual bool IsValid(object? value) => IsValid(value, null);

    /// <summary>
    ///     检查对象是否合法
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="validationContext">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public abstract bool IsValid(object? value, IValidationContext? validationContext);

    /// <summary>
    ///     获取对象验证结果列表
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="name">显示名称</param>
    /// <param name="memberNames">成员名称列表</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public virtual List<ValidationResult>? GetValidationResults(object? value, string name,
        IEnumerable<string>? memberNames = null) =>
        GetValidationResults(value, new LegacyValidationContext(value, name, memberNames));

    /// <summary>
    ///     获取对象验证结果列表
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="validationContext">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public virtual List<ValidationResult>? GetValidationResults(object? value, IValidationContext? validationContext) =>
        IsValid(value, validationContext)
            ? null
            :
            [
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!),
                    validationContext?.MemberNames)
            ];

    /// <summary>
    ///     执行验证
    /// </summary>
    /// <remarks>失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="value">对象</param>
    /// <param name="name">显示名称</param>
    /// <param name="memberNames">成员名称列表</param>
    /// <exception cref="ValidationException"></exception>
    public virtual void Validate(object? value, string name, IEnumerable<string>? memberNames = null) =>
        Validate(value, new LegacyValidationContext(value, name, memberNames));

    /// <summary>
    ///     执行验证
    /// </summary>
    /// <remarks>失败时抛出 <see cref="ValidationException" /> 异常。</remarks>
    /// <param name="value">对象</param>
    /// <param name="validationContext">
    ///     <see cref="IValidationContext" />
    /// </param>
    /// <exception cref="ValidationException"></exception>
    public virtual void Validate(object? value, IValidationContext? validationContext)
    {
        // 检查对象是否合法
        if (!IsValid(value, validationContext))
        {
            throw new ValidationException(
                new ValidationResult(FormatErrorMessage(validationContext?.DisplayName!),
                    validationContext?.MemberNames), null, value);
        }
    }

    /// <summary>
    ///     格式化错误消息
    /// </summary>
    /// <param name="name">显示名称</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public virtual string? FormatErrorMessage(string name) =>
        (string?)ErrorMessageString is null
            ? null
            : string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);

    /// <summary>
    ///     使用指定资源键设置验证错误消息
    /// </summary>
    /// <remarks>支持入口程序集覆盖框架内部资源，若未找到则返回占位符。</remarks>
    /// <param name="resourceKeyResolver">返回 <see cref="ValidationMessages" /> 中属性名的委托</param>
    protected void UseResourceKey(Func<string> resourceKeyResolver)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(resourceKeyResolver);

        _errorMessageResourceAccessor = () =>
        {
            // 获取 ValidationMessages 中的属性名
            var resourceKey = resourceKeyResolver();

            return GetResourceString(resourceKey) ?? resourceKey;
        };
    }

    /// <summary>
    ///     获取支持外部覆盖的资源字符串
    /// </summary>
    /// <remarks>支持入口程序集覆盖框架内部资源，若未找到则返回占位符。</remarks>
    /// <param name="resourceKey">资源属性名</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? GetResourceString(string resourceKey)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);

        // 优先使用：尝试根据资源键获取已注册的覆盖消息
        var overrideMessage = ValidationMessageProvider.TryGetOverride(resourceKey);

        // 空检查
        if (overrideMessage is not null)
        {
            return overrideMessage;
        }

        // 获取 ValidationMessages 静态属性
        var property = GetValidationMessagesProperty(resourceKey);

        return property?.GetValue(null, null) as string;
    }

    /// <summary>
    ///     获取资源字符串
    /// </summary>
    /// <param name="resourceType">资源类型</param>
    /// <param name="resourceKey">资源属性名</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? GetResourceString(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type resourceType, string resourceKey)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(resourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);

        // 查找用户自定义资源类型
        var property = TryGetPropertyFromAssembly(resourceType.Assembly, resourceType.FullName!, resourceKey);

        return property?.GetValue(null, null) as string;
    }

    /// <summary>
    ///     触发属性变更事件
    /// </summary>
    /// <param name="propertyValue">已更改属性的值</param>
    /// <param name="propertyName">已更改属性的名称</param>
    protected void OnPropertyChanged(object? propertyValue, [CallerMemberName] string? propertyName = null)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        PropertyChanged?.Invoke(this, new ValidationPropertyChangedEventArgs(propertyName, propertyValue));
    }

    /// <summary>
    ///     设置错误信息资源访问器
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    internal void SetupResourceAccessor()
    {
        // 空检查
        if (_errorMessageResourceAccessor is not null)
        {
            return;
        }

        var localErrorMessage = ErrorMessage;
        var resourceNameSet = !string.IsNullOrEmpty(_errorMessageResourceName);
        var errorMessageSet = !string.IsNullOrEmpty(_errorMessage);
        var resourceTypeSet = _errorMessageResourceType is not null;

        // 以下组合是非法的，会抛出 InvalidOperationException：
        //   1) 同时设置了 ErrorMessage 和 ErrorMessageResourceName 属性
        //   2) 或者 ErrorMessage、ErrorMessageResourceName 属性均未设置
        if ((resourceNameSet && errorMessageSet) || !(resourceNameSet || errorMessageSet))
        {
            throw new InvalidOperationException(
                $"Either {nameof(ErrorMessageString)} or {nameof(ErrorMessageResourceName)} must be set, but not both.");
        }

        // 必须同时设置或都不设置 ErrorMessageResourceType 和 ErrorMessageResourceName 属性
        if (resourceTypeSet != resourceNameSet)
        {
            throw new InvalidOperationException(
                $"Both {nameof(ErrorMessageResourceType)} and {nameof(ErrorMessageResourceName)} need to be set on this validator.");
        }

        // 如果设置了错误信息资源类型及其资源名称，那么就去查找该资源对应的值并设置错误信息资源访问器
        if (resourceNameSet)
        {
            SetResourceAccessorByPropertyLookup();
        }
        // 否则将错误信息设置给错误信息资源访问器
        else
        {
            _errorMessageResourceAccessor = () => localErrorMessage!;
        }
    }

    /// <summary>
    ///     通过错误信息资源查找并设置错误信息资源访问器
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    internal void SetResourceAccessorByPropertyLookup()
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(_errorMessageResourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(_errorMessageResourceName);

        // 初始化属性对象
        PropertyInfo? property;
        var propertyName = _errorMessageResourceName;

        // 判断是否使用的是默认的 ValidationMessages 类型
        if (_errorMessageResourceType == typeof(ValidationMessages))
        {
            // 获取 ValidationMessages 静态属性
            property = GetValidationMessagesProperty(propertyName);
        }
        else
        {
            // 查找用户自定义资源类型
            property = TryGetPropertyFromAssembly(_errorMessageResourceType.Assembly,
                _errorMessageResourceType.FullName!, propertyName);
        }

        // 检查属性是否只对同一程序集中的其他类型可见，而对该程序集以外的派生类型则不可见（顾名思义，使用 internal 声明的属性）
        // https://learn.microsoft.com/zh-cn/dotnet/api/system.reflection.methodbase.isassembly?view=net-9.0
        if (property?.GetMethod is null or { IsAssembly: false, IsPublic: false })
        {
            property = null;
        }

        // 空检查
        if (property is null)
        {
            throw new InvalidOperationException(
                $"The resource type `{_errorMessageResourceType.FullName}` does not have an accessible static property named `{_errorMessageResourceName}`.");
        }

        // 检查 ErrorMessageResourceName 属性类型是否是 string 类型
        if (property.PropertyType != typeof(string))
        {
            throw new InvalidOperationException(
                $"The property `{property.Name}` on resource type `{_errorMessageResourceType.FullName}` is not a string type.");
        }

        _errorMessageResourceAccessor = () => (string)property.GetValue(null, null)!;
    }

    /// <summary>
    ///     获取 <see cref="ValidationMessages" /> 静态属性
    /// </summary>
    /// <remarks>支持入口程序集覆盖框架内部资源。</remarks>
    /// <param name="resourceKey">资源属性名</param>
    /// <returns>
    ///     <see cref="PropertyInfo" />
    /// </returns>
    internal static PropertyInfo? GetValidationMessagesProperty(string resourceKey)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);

        // 初始化属性对象
        PropertyInfo? property = null;

        // 获取入口程序集和内部程序集
        var entryAssembly = Assembly.GetEntryAssembly();
        var internalAssembly = typeof(ValidationMessages).Assembly; // 固定为内部资源程序集

        // 先查询入口程序集（命名空间必须为 Cordon.Resources.Overrides）
        if (entryAssembly is not null && entryAssembly != internalAssembly)
        {
            property = TryGetPropertyFromAssembly(entryAssembly, ExternalValidationMessagesFullTypeName, resourceKey);
        }

        // 回退到框架内置资源
        property ??= TryGetPropertyFromAssembly(internalAssembly, typeof(ValidationMessages).FullName!, resourceKey);

        return property;
    }

    /// <summary>
    ///     尝试从指定程序集中获取资源类型的静态字符串属性
    /// </summary>
    /// <param name="assembly">目标程序集</param>
    /// <param name="fullTypeName">
    ///     完整类型名（如 <c>Cordon.Resources.ValidationMessages</c> 或
    ///     <c>Cordon.Resources.Overrides.ValidationMessages</c>）
    /// </param>
    /// <param name="propertyName">属性名</param>
    /// <returns>
    ///     <see cref="PropertyInfo" />
    /// </returns>
    internal static PropertyInfo? TryGetPropertyFromAssembly(Assembly assembly, string fullTypeName,
        string propertyName)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullTypeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        return assembly.GetType(fullTypeName)?.GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
    }
}