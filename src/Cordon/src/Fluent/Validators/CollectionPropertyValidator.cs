// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     集合类型属性验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TElement">元素类型</typeparam>
public sealed class
    CollectionPropertyValidator<T, TElement> : PropertyValidator<T, IEnumerable<TElement>?,
    CollectionPropertyValidator<T, TElement>>
{
    /// <summary>
    ///     元素过滤委托
    /// </summary>
    internal Func<TElement, ValidationContext<T>, bool>? _elementFilter;

    /// <inheritdoc cref="ObjectValidator{T}" />
    /// <remarks>集合元素对象验证器。</remarks>
    internal ObjectValidator<TElement>? _elementValidator;

    /// <inheritdoc cref="ObjectValidator{T}" />
    /// <remarks>集合元素值验证器。</remarks>
    internal ValueValidator<TElement>? _valueValidator;

    /// <inheritdoc />
    internal CollectionPropertyValidator(Expression<Func<T, IEnumerable<TElement?>?>> selector,
        ObjectValidator<T> objectValidator) : base(selector!, objectValidator)
    {
    }

    /// <inheritdoc />
    public override bool IsValid(T? instance, string?[]? ruleSets = null)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);

        // 调用基类 IsValid 方法
        return base.IsValid(instance, ruleSets) &&
               ForEachValidatedElement(instance, element =>
               {
                   // 检查是否设置了集合元素对象验证器
                   if (_elementValidator is not null)
                   {
                       return _elementValidator.IsValid(element, ruleSets);
                   }

                   // 检查是否设置了集合元素值验证器
                   // ReSharper disable once ConvertIfStatementToReturnStatement
                   if (_valueValidator is not null)
                   {
                       return _valueValidator.IsValid(element, ruleSets);
                   }

                   return true;
               }, ruleSets);
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
            // 检查是否设置了集合元素对象验证器
            if (_elementValidator is not null)
            {
                validationResults.AddRange(_elementValidator.GetValidationResults(element, ruleSets) ?? []);
            }
            // 检查是否设置了集合元素值验证器
            else if (_valueValidator is not null)
            {
                validationResults.AddRange(_valueValidator.GetValidationResults(element, ruleSets) ?? []);
            }

            return true;
        }, ruleSets);

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
            // 检查是否设置了集合元素对象验证器
            if (_elementValidator is not null)
            {
                _elementValidator.Validate(element, ruleSets);
            }
            // 检查是否设置了集合元素值验证器
            // ReSharper disable once UseNullPropagation
            else if (_valueValidator is not null)
            {
                _valueValidator.Validate(element, ruleSets);
            }

            return true;
        }, ruleSets);
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

        _elementFilter = (element, _) => filter(element);

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

        _elementFilter = filter;

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

        // 检查集合元素是否是类类型
        if (!typeof(TElement).IsClass || typeof(TElement) == typeof(string))
        {
            throw new InvalidOperationException(
                $"Collection element type '{typeof(TElement)}' is not a reference type. `{nameof(SetValidator)}` (object validator) and `{nameof(ChildRules)}` are only supported for class types. For value types, use `{nameof(EachRules)}` or `{nameof(SetValidator)}` (value validator) instead.");
        }

        // 空检查
        if (_elementValidator is not null)
        {
            throw new InvalidOperationException(
                $"An object validator has already been assigned to this element. Only one object validator is allowed per element. To define nested rules, use `{nameof(ChildRules)}` within a single validator.");
        }

        // 互斥检查
        if (_valueValidator is not null)
        {
            throw new InvalidOperationException(
                $"Cannot configure object validator because a value validator has already been configured via `{nameof(EachRules)}` or `{nameof(SetValidator)}` (value validator).");
        }

        // 调用工厂方法，传入当前 RuleSets、Items 和 Options
        _elementValidator = validatorFactory(RuleSets, _objectValidator.Items, _objectValidator.Options);

        // 空检查
        if (_elementValidator is null)
        {
            return this;
        }

        // 继承当前规则集
        _elementValidator.SetInheritedRuleSetsIfNotSet(RuleSets);

        // 同步 IServiceProvider 委托
        _elementValidator.InitializeServiceProvider(_serviceProvider);

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
                $"An object validator has already been assigned to this element. `{nameof(ChildRules)}` cannot be applied after `{nameof(SetValidator)}` (object validator) or another `{nameof(ChildRules)}` call.");
        }

        return SetValidator((ruleSets, items, options) =>
        {
            // 初始化集合元素对象验证器实例
            var elementValidator =
                new ObjectValidator<TElement>(items) { InheritedRuleSets = ruleSets }.ConfigureOptions(options);

            // 调用自定义配置委托
            configure(elementValidator);

            return elementValidator;
        });
    }

    /// <summary>
    ///     设置集合元素值验证器
    /// </summary>
    /// <param name="validatorFactory">
    ///     <see cref="ValueValidator{T}" /> 工厂委托
    /// </param>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public CollectionPropertyValidator<T, TElement> SetValidator(
        Func<IDictionary<object, object?>?, ValueValidator<TElement>?> validatorFactory)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorFactory);

        // 空检查
        if (_valueValidator is not null)
        {
            throw new InvalidOperationException(
                $"A value validator has already been assigned to this element. Only one value validator is allowed per element. To define custom rules, use `{nameof(EachRules)}` within a single validator.");
        }

        // 互斥检查
        if (_elementValidator is not null)
        {
            throw new InvalidOperationException(
                $"Cannot configure value validator because an object validator has already been configured via `{nameof(ChildRules)}` or `{nameof(SetValidator)}` (object validator).");
        }

        // 调用工厂方法，传入当前 Items 
        _valueValidator = validatorFactory(_objectValidator.Items);

        // 空检查
        if (_valueValidator is null)
        {
            return this;
        }

        // 同步 IServiceProvider 委托
        _valueValidator.InitializeServiceProvider(_serviceProvider);

        return this;
    }

    /// <summary>
    ///     设置集合元素值验证器
    /// </summary>
    /// <param name="validator">
    ///     <see cref="ValueValidator{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public CollectionPropertyValidator<T, TElement> SetValidator(ValueValidator<TElement>? validator) =>
        SetValidator(_ => validator);

    /// <summary>
    ///     为集合元素继续配置规则
    /// </summary>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="CollectionPropertyValidator{T,TElement}" />
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public CollectionPropertyValidator<T, TElement> EachRules(Action<ValueValidator<TElement>> configure)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(configure);

        // 空检查
        if (_valueValidator is not null)
        {
            throw new InvalidOperationException(
                $"A value validator has already been assigned to this element. `{nameof(EachRules)}` cannot be applied after `{nameof(SetValidator)}` (value validator) or another `{nameof(EachRules)}` call.");
        }

        return SetValidator(items =>
        {
            // 初始化集合元素值验证器实例
            var valueValidator = new ValueValidator<TElement>(items);

            // 调用自定义配置委托
            configure(valueValidator);

            return valueValidator;
        });
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        // 调用基类 Dispose 方法
        base.Dispose(disposing);

        // 释放集合元素对象验证器资源
        _elementValidator?.Dispose();

        // 释放集合元素值验证器资源
        _valueValidator?.Dispose();
    }

    /// <summary>
    ///     筛选用于验证的集合元素
    /// </summary>
    /// <param name="elements">
    ///     <see cref="IEnumerable{T}" />
    /// </param>
    /// <param name="validationContext">
    ///     <see cref="ValidationContext{T}" />
    /// </param>
    /// <returns>
    ///     <see cref="IEnumerable{T}" />
    /// </returns>
    internal IEnumerable<TElement> GetValidatedElements(IEnumerable<TElement> elements,
        ValidationContext<T> validationContext)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validationContext);

        // 空检查
        return _elementFilter is null
            ? elements
            : elements.Where(element => _elementFilter(element, validationContext));
    }

    /// <summary>
    ///     获取集合并遍历用于验证的元素
    /// </summary>
    /// <remarks>内部为每个元素设置带索引的成员路径（如 "Addresses[0]"），然后执行验证操作。支持短路退出：若操作返回 <c>false</c>，则立即终止遍历。</remarks>
    /// <param name="instance">对象</param>
    /// <param name="action">元素处理委托</param>
    /// <param name="ruleSets">规则集</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal bool ForEachValidatedElement(T instance, Func<TElement, bool> action, string?[]? ruleSets)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(action);

        // 检查是否设置了任一类型的元素验证器
        if (_elementValidator is null && _valueValidator is null)
        {
            return true;
        }

        // 获取用于验证的属性值
        var propertyValue = GetValueForValidation(instance);
        if (propertyValue is null)
        {
            return true;
        }

        // 获取基础成员名称
        var baseMemberName = GetEffectiveMemberName();

        // 遍历用于验证的集合元素
        var index = 0;
        foreach (var element in GetValidatedElements(propertyValue, CreateValidationContext(instance, ruleSets)))
        {
            // 检查是否设置了集合元素对象验证器
            if (_elementValidator is not null)
            {
                // 获取原始属性路径
                var originalPath = _elementValidator._memberPath;

                // 设置当前属性路径
                _elementValidator._memberPath = $"{baseMemberName}[{index}]";

                try
                {
                    // 调用元素处理委托
                    if (!action(element))
                    {
                        return false;
                    }
                }
                finally
                {
                    // 恢复原始属性路径
                    _elementValidator._memberPath = originalPath;
                }
            }
            // 检查是否设置了集合元素值验证器
            else if (_valueValidator is not null)
            {
                // 获取原始属性路径
                var originalPath = _valueValidator._memberPath;

                // 设置当前属性路径
                _valueValidator._memberPath = $"{baseMemberName}[{index}]";

                try
                {
                    // 调用元素处理委托
                    if (!action(element))
                    {
                        return false;
                    }
                }
                finally
                {
                    // 恢复原始属性路径
                    _valueValidator._memberPath = originalPath;
                }
            }

            index++;
        }

        return true;
    }

    /// <inheritdoc cref="IValidatorInitializer.InitializeServiceProvider" />
    internal override void InitializeServiceProvider(Func<Type, object?>? serviceProvider)
    {
        // 同步基类 IServiceProvider 委托
        base.InitializeServiceProvider(serviceProvider);

        // 同步 _elementValidator 实例 IServiceProvider 委托
        _elementValidator?.InitializeServiceProvider(serviceProvider);

        // 同步 _valueValidator 实例 IServiceProvider 委托
        _valueValidator?.InitializeServiceProvider(serviceProvider);
    }

    /// <inheritdoc cref="IMemberPathRepairable.RepairMemberPaths" />
    internal override void RepairMemberPaths(string? memberPath)
    {
        // 调用基类的修复验证器及其子验证器的成员路径
        base.RepairMemberPaths(memberPath);

        _elementValidator?.RepairMemberPaths(memberPath);
        _valueValidator?.RepairMemberPaths(memberPath);
    }

    /// <inheritdoc cref="IPropertyValidatorCloneable{T}.Clone" />
    internal override IPropertyValidator<T> Clone(ObjectValidator<T> objectValidator)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(objectValidator);

        // 初始化 PropertyValidator 实例
        var propertyValidator = new CollectionPropertyValidator<T, TElement>(_selector!, objectValidator)
        {
            RuleSets = RuleSets,
            SuppressAnnotationValidation = SuppressAnnotationValidation,
            DisplayName = DisplayName,
            MemberName = MemberName,
            WhenCondition = WhenCondition
        };

        // 同步字段
        propertyValidator._allowEmptyStrings = _allowEmptyStrings;
        propertyValidator._preProcessor = _preProcessor;

        propertyValidator._elementValidator = _elementValidator;
        propertyValidator._valueValidator = _valueValidator;
        propertyValidator._elementFilter = _elementFilter;

        // 同步已设置的验证器
        propertyValidator.AddValidators(Validators);

        // 同步 IServiceProvider 委托
        propertyValidator.InitializeServiceProvider(objectValidator._serviceProvider);

        return propertyValidator;
    }
}