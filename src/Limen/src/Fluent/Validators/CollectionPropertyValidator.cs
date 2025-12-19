// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     集合类型属性验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TElement">元素类型</typeparam>
public sealed class CollectionPropertyValidator<T, TElement> : PropertyValidator<T, IEnumerable<TElement>?>
    where TElement : class
{
    /// <inheritdoc cref="ObjectValidator{T}" />
    /// <remarks>集合元素对象验证器。</remarks>
    internal ObjectValidator<TElement>? _elementValidator;

    /// <inheritdoc />
    internal CollectionPropertyValidator(Expression<Func<T, IEnumerable<TElement>?>> selector,
        ObjectValidator<T> objectValidator) : base(selector, objectValidator)
    {
    }

    /// <summary>
    ///     元素过滤委托
    /// </summary>
    internal Func<TElement, ValidationContext<T>, bool>? ElementFilter { get; private set; }

    /// <inheritdoc />
    public override bool IsValid(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 调用基类 IsValid 方法
        return base.IsValid(instance, ruleSets) &&
               ForEachValidatedElement(instance, element => _elementValidator!.IsValid(element, ruleSets));
    }

    /// <inheritdoc />
    public override List<ValidationResult>? GetValidationResults(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 初始化验证结果集合
        var validationResults = new List<ValidationResult>();

        // 调用基类 GetValidationResults 方法
        validationResults.AddRange(base.GetValidationResults(instance, ruleSets) ?? []);

        // 获取集合并遍历用于验证的元素
        ForEachValidatedElement(instance, element =>
        {
            validationResults.AddRange(_elementValidator!.GetValidationResults(element, ruleSets) ?? []);
            return true;
        });

        return validationResults.ToResults();
    }

    /// <inheritdoc />
    public override void Validate(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 调用基类 Validate 方法
        base.Validate(instance, ruleSets);

        // 获取集合并遍历用于验证的元素
        ForEachValidatedElement(instance, element =>
        {
            _elementValidator!.Validate(element, ruleSets);
            return true;
        });
    }

    /// <summary>
    ///     筛选用于验证的集合元素
    /// </summary>
    /// <param name="filter">过滤委托</param>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    public CollectionPropertyValidator<T, TElement> Where(Func<TElement, bool> filter)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(filter);

        ElementFilter = (element, _) => filter(element);

        return this;
    }

    /// <summary>
    ///     筛选用于验证的集合元素
    /// </summary>
    /// <param name="filter">过滤委托</param>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    public CollectionPropertyValidator<T, TElement> Where(Func<TElement, ValidationContext<T>, bool> filter)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(filter);

        ElementFilter = filter;

        return this;
    }

    /// <summary>
    ///     设置集合元素对象验证器
    /// </summary>
    /// <param name="validatorFactory">
    ///     <see cref="ObjectValidator{T}" /> 工厂委托
    /// </param>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public CollectionPropertyValidator<T, TElement> SetValidator(
        Func<string?[]?, IDictionary<object, object?>?, ValidatorOptions, ObjectValidator<TElement>?> validatorFactory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorFactory);

        // 空检查
        if (_elementValidator is not null)
        {
            throw new InvalidOperationException(
                $"An object validator has already been assigned to this element. Only one object validator is allowed per element. To define nested rules, use `{nameof(ChildRules)}` within a single validator.");
        }

        // 调用工厂方法，传入当前 RuleSets、_items 和 Options
        _elementValidator = validatorFactory(RuleSets, _objectValidator._items, _objectValidator.Options);

        // 空检查
        if (_elementValidator is null)
        {
            return this;
        }

        // 继承当前规则集
        _elementValidator.SetInheritedRuleSetsIfNotSet(RuleSets);

        // 同步 IServiceProvider 委托
        _elementValidator.InitializeServiceProvider(_serviceProvider);

        // 修复整个子验证器树的成员路径
        RepairMemberPaths();

        return this;
    }

    /// <summary>
    ///     设置集合元素对象验证器
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ObjectValidator{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public CollectionPropertyValidator<T, TElement> SetValidator(ObjectValidator<TElement>? validator) =>
        SetValidator((_, _, _) => validator);

    /// <summary>
    ///     为集合元素继续配置规则
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public CollectionPropertyValidator<T, TElement> ChildRules(Action<ObjectValidator<TElement>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 空检查
        if (_elementValidator is not null)
        {
            throw new InvalidOperationException(
                $"An object validator has already been assigned to this element. `{nameof(ChildRules)}` cannot be applied after `{nameof(SetValidator)}` or another `{nameof(ChildRules)}` call.");
        }

        return SetValidator((ruleSets, items, options) =>
        {
            // 初始化集合元素对象验证器实例
            var elementValidator = new ObjectValidator<TElement>(options, null, items) { InheritedRuleSets = ruleSets };

            // 调用自定义配置委托
            configure(elementValidator);

            return elementValidator;
        });
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        // 调用基类 Dispose 方法
        base.Dispose(disposing);

        // 释放集合元素对象验证器资源
        if (_elementValidator is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>
    ///     筛选用于验证的集合元素
    /// </summary>
    /// <param name="elements">
    ///     <see cref="IEnumerable{T}" />
    /// </param>
    /// <param name="instance">对象</param>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    internal IEnumerable<TElement> GetValidatedElements(IEnumerable<TElement> elements, T instance)
    {
        // 空检查
        if (ElementFilter is null)
        {
            return elements;
        }

        // 创建 ValidationContext<T> 实例
        var context = CreateValidationContext(instance);

        return elements.Where(element => ElementFilter(element, context));
    }

    /// <summary>
    ///     获取集合并遍历用于验证的元素
    /// </summary>
    /// <remarks>内部为每个元素设置带索引的成员路径（如 "Addresses[0]"），然后执行验证操作。支持短路退出：若操作返回 <c>false</c>，则立即终止遍历。</remarks>
    /// <param name="instance">对象</param>
    /// <param name="action">元素处理委托</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ForEachValidatedElement(T instance, Func<TElement, bool> action)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(action);

        // 检查是否设置了集合元素对象验证器
        if (_elementValidator is null)
        {
            return true;
        }

        // 获取用于验证的属性值
        var propertyValue = GetValueForValidation(instance);
        if (propertyValue is null)
        {
            return true;
        }

        // 获取原始属性路径
        var originalPath = _elementValidator.MemberPath;
        var baseMemberName = originalPath ?? GetEffectiveMemberName();

        try
        {
            // 遍历用于验证的集合元素
            var index = 0;
            foreach (var element in GetValidatedElements(propertyValue, instance))
            {
                // 设置当前属性路径
                _elementValidator.MemberPath = $"{baseMemberName}[{index}]";

                // 调用元素处理委托
                if (!action(element))
                {
                    return false;
                }

                index++;
            }
        }
        finally
        {
            // 恢复原始属性路径
            _elementValidator.MemberPath = originalPath;
        }

        return true;
    }

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal new void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 同步基类 IServiceProvider 委托
        base.InitializeServiceProvider(serviceProvider);

        // 同步 _elementValidator 实例 IServiceProvider 委托
        _elementValidator?.InitializeServiceProvider(serviceProvider);
    }

    /// <inheritdoc cref="IMemberPathRepairable.RepairMemberPaths" />
    internal new void RepairMemberPaths()
    {
        // 空检查
        if (_elementValidator is null)
        {
            return;
        }

        // 设置元素验证器的基础路径
        _elementValidator.MemberPath = GetEffectiveMemberName();

        // 递归修复元素验证器内部的所有子验证器
        foreach (var childValidator in _elementValidator.Validators)
        {
            // 检查验证器是否实现 IMemberPathRepairable 接口
            if (childValidator is IMemberPathRepairable repairable)
            {
                // 修复验证器及其子验证器的成员路径
                repairable.RepairMemberPaths();
            }
        }
    }
}