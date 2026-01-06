// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     集合验证器
/// </summary>
/// <typeparam name="TElement">元素类型</typeparam>
public class CollectionValidator<TElement> : ValidatorBase<IEnumerable<TElement>>,
    IObjectValidator<IEnumerable<TElement>>, IMemberPathRepairable
{
    /// <inheritdoc cref="ObjectValidator{T}" />
    internal readonly IObjectValidator<TElement> _elementValidator;

    /// <inheritdoc cref="IMemberPathRepairable" />
    internal readonly IMemberPathRepairable? _repairable;

    /// <summary>
    ///     元素过滤委托
    /// </summary>
    internal Func<TElement, bool>? _elementFilter;

    /// <summary>
    ///     对象图中的属性路径
    /// </summary>
    internal string? _memberPath;

    /// <summary>
    ///     <inheritdoc cref="CollectionValidator{TElement}" />
    /// </summary>
    /// <param name="elementValidator">
    ///     <see cref="IObjectValidator{T}" />
    /// </param>
    public CollectionValidator(IObjectValidator<TElement> elementValidator)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(elementValidator);

        _elementValidator = elementValidator;
        _repairable = elementValidator as IMemberPathRepairable;

        ErrorMessageResourceAccessor = () => null!;
    }

    /// <inheritdoc />
    string? IMemberPathRepairable.MemberPath
    {
        get => _memberPath;
        set => _memberPath = value;
    }

    /// <inheritdoc />
    void IMemberPathRepairable.RepairMemberPaths(string? memberPath) => RepairMemberPaths(memberPath);

    /// <inheritdoc />
    public bool IsValid(IEnumerable<TElement>? instance, string?[]? ruleSets = null) => instance is null ||
        GetValidatedElements(instance).All(u => _elementValidator.IsValid(u, ruleSets));

    /// <inheritdoc />
    public List<ValidationResult>? GetValidationResults(IEnumerable<TElement>? instance, string?[]? ruleSets = null)
    {
        // 空检查
        if (instance is null)
        {
            return null;
        }

        // 初始化验证结果集合
        var validationResults = new List<ValidationResult>();

        // 遍历用于验证的集合元素
        var index = 0;
        foreach (var element in GetValidatedElements(instance))
        {
            // 获取原始属性路径
            var originalPath = _repairable?.MemberPath;

            // 设置当前属性路径
            _repairable?.MemberPath = $"{_memberPath}[{index}]";

            try
            {
                validationResults.AddRange(_elementValidator.GetValidationResults(element, ruleSets) ?? []);
            }
            finally
            {
                // 恢复原始属性路径
                _repairable?.MemberPath = originalPath;
            }

            index++;
        }

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public void Validate(IEnumerable<TElement>? instance, string?[]? ruleSets = null)
    {
        // 空检查
        if (instance is null)
        {
            return;
        }

        // 遍历用于验证的集合元素
        var index = 0;
        foreach (var element in GetValidatedElements(instance))
        {
            // 获取原始属性路径
            var originalPath = _repairable?.MemberPath;

            // 设置当前属性路径
            _repairable?.MemberPath = $"{_memberPath}[{index}]";

            try
            {
                _elementValidator.Validate(element, ruleSets);
            }
            finally
            {
                // 恢复原始属性路径
                _repairable?.MemberPath = originalPath;
            }

            index++;
        }
    }

    /// <inheritdoc />
    public virtual void InitializeServiceProvider(Func<Type, object?>? serviceProvider) =>
        _elementValidator.InitializeServiceProvider(serviceProvider);

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    bool IObjectValidator.IsValid(object? instance, string?[]? ruleSets) =>
        IsValid((IEnumerable<TElement>?)instance, ruleSets);

    /// <inheritdoc />
    List<ValidationResult>? IObjectValidator.GetValidationResults(object? instance, string?[]? ruleSets) =>
        GetValidationResults((IEnumerable<TElement>?)instance, ruleSets);

    /// <inheritdoc />
    void IObjectValidator.Validate(object? instance, string?[]? ruleSets) =>
        Validate((IEnumerable<TElement>?)instance, ruleSets);

    /// <inheritdoc />
    public List<ValidationResult> ToResults(ValidationContext validationContext, bool disposeAfterValidation = true)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);

        // 同步 IServiceProvider 委托
        InitializeServiceProvider(validationContext.GetService);

        // 尝试从 ValidationContext.Items 中解析验证选项中的规则集
        string?[]? ruleSets = null;
        if (validationContext.Items.TryGetValue(ValidationDataContext.ValidationOptionsKey, out var metadataObj) &&
            metadataObj is ValidationOptionsMetadata metadata)
        {
            ruleSets = metadata.RuleSets;
        }

        try
        {
            // 获取对象验证结果集合
            return GetValidationResults((IEnumerable<TElement>)validationContext.ObjectInstance, ruleSets) ?? [];
        }
        finally
        {
            // 自动释放资源
            if (disposeAfterValidation)
            {
                Dispose();
            }
        }
    }

    /// <inheritdoc />
    public override bool IsValid(IEnumerable<TElement>? instance,
        ValidationContext<IEnumerable<TElement>> validationContext) =>
        instance is null || IsValid(instance, validationContext.RuleSets);

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(IEnumerable<TElement>? instance,
        ValidationContext<IEnumerable<TElement>> validationContext) =>
        instance is null ? null : GetValidationResults(instance, validationContext.RuleSets);

    /// <inheritdoc />
    public override void Validate(IEnumerable<TElement>? instance,
        ValidationContext<IEnumerable<TElement>> validationContext)
    {
        // 空检查
        if (instance is not null)
        {
            Validate(instance, validationContext.RuleSets);
        }
    }

    /// <inheritdoc />
    public override string? FormatErrorMessage(string name) =>
        (string?)ErrorMessageString is null ? null : base.FormatErrorMessage(name);

    /// <summary>
    ///     筛选用于验证的集合元素
    /// </summary>
    /// <param name="elementFilter">元素过滤委托</param>
    /// <returns>
    ///     <see cref="CollectionValidator{TElement}" />
    /// </returns>
    public CollectionValidator<TElement> Where(Func<TElement, bool>? elementFilter)
    {
        _elementFilter = elementFilter;

        return this;
    }

    /// <summary>
    ///     释放资源
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _elementValidator.Dispose();
    }

    /// <summary>
    ///     筛选用于验证的集合元素
    /// </summary>
    /// <param name="elements">
    ///     <see cref="IEnumerable{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    internal IEnumerable<TElement> GetValidatedElements(IEnumerable<TElement> elements) =>
        _elementFilter is null ? elements : elements.Where(element => _elementFilter(element));

    /// <inheritdoc cref="IMemberPathRepairable.RepairMemberPaths" />
    internal virtual void RepairMemberPaths(string? memberPath) => _memberPath = memberPath;
}