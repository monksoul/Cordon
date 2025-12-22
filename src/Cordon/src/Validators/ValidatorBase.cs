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
    ///     检查对象合法性
    /// </summary>
    /// <param name="instance">对象</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public abstract bool IsValid(T? instance);

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="name">显示名称</param>
    /// <param name="memberNames">成员名称列表</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public virtual List<ValidationResult>? GetValidationResults(T? instance, string name,
        IEnumerable<string>? memberNames = null) =>
        base.GetValidationResults(instance, name, memberNames);

    /// <summary>
    ///     验证指定的对象
    /// </summary>
    /// <param name="instance">对象</param>
    /// <param name="name">显示名称</param>
    /// <param name="memberNames">成员名称列表</param>
    /// <exception cref="ValidationException"></exception>
    public virtual void Validate(T? instance, string name, IEnumerable<string>? memberNames = null) =>
        base.Validate(instance, name, memberNames);

    /// <inheritdoc />
    public sealed override bool IsValid(object? value) => IsValid(ConvertValue(value));

    /// <inheritdoc />
    public sealed override List<ValidationResult>? GetValidationResults(object? value, string name,
        IEnumerable<string>? memberNames = null) =>
        GetValidationResults(ConvertValue(value), name, memberNames);

    /// <inheritdoc />
    public sealed override void Validate(object? value, string name, IEnumerable<string>? memberNames = null) =>
        Validate(ConvertValue(value), name, memberNames);

    /// <summary>
    ///     将 <see cref="object" /> 类型对象转换为 <typeparamref name="T" /> 类型对象
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <typeparamref name="T" />
    /// </returns>
    internal static T? ConvertValue(object? value) => (T?)value;
}

/// <summary>
///     验证器抽象基类
/// </summary>
public abstract class ValidatorBase
{
    /// <summary>
    ///     外部程序集用于覆盖默认验证消息的【强制约定类型全名】
    /// </summary>
    /// <remarks>TODO: 未来可考虑使用 <see cref="AppContext" /> 配置。</remarks>
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
    protected ValidatorBase(Func<string> errorMessageResourceAccessor)
    {
        _errorMessageResourceAccessor = errorMessageResourceAccessor;

        // ReSharper disable once SuspiciousTypeConversion.Global
        SupportsAsync = this is IAsyncValidator;
    }

    /// <summary>
    ///     规则集
    /// </summary>
    /// <remarks>实现内部规则集功能。</remarks>
    internal string?[]? RuleSets
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
    internal bool SupportsAsync { get; }

    /// <summary>
    ///     属性变更事件
    /// </summary>
    protected event EventHandler<ValidationPropertyChangedEventArgs>? PropertyChanged;

    /// <summary>
    ///     检查对象合法性
    /// </summary>
    /// <param name="value">对象</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    public abstract bool IsValid(object? value);

    /// <summary>
    ///     获取对象验证结果集合
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="name">显示名称</param>
    /// <param name="memberNames">成员名称列表</param>
    /// <returns>
    ///     <see cref="List{T}" />
    /// </returns>
    public virtual List<ValidationResult>? GetValidationResults(object? value, string name,
        IEnumerable<string>? memberNames = null) =>
        IsValid(value) ? null : [new ValidationResult(FormatErrorMessage(name), memberNames)];

    /// <summary>
    ///     验证指定的对象
    /// </summary>
    /// <param name="value">对象</param>
    /// <param name="name">显示名称</param>
    /// <param name="memberNames">成员名称列表</param>
    /// <exception cref="ValidationException"></exception>
    public virtual void Validate(object? value, string name, IEnumerable<string>? memberNames = null)
    {
        // 检查对象合法性
        if (!IsValid(value))
        {
            throw new ValidationException(new ValidationResult(FormatErrorMessage(name), memberNames), null, value);
        }
    }

    /// <summary>
    ///     错误信息格式化设置
    /// </summary>
    /// <param name="name">显示名称</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    public virtual string? FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);

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

            return GetResourceString(resourceKey) ?? $"[{resourceKey}]";
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

        // 获取 ValidationMessages 静态属性
        var property = GetValidationMessagesProperty(resourceKey);

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
                "Either ErrorMessageString or ErrorMessageResourceName must be set, but not both.");
        }

        // 必须同时设置或都不设置 ErrorMessageResourceType 和 ErrorMessageResourceName 属性
        if (resourceTypeSet != resourceNameSet)
        {
            throw new InvalidOperationException(
                "Both ErrorMessageResourceType and ErrorMessageResourceName need to be set on this validator.");
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
        PropertyInfo? property = null;
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