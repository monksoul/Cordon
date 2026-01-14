// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     指定数值范围约束验证器
/// </summary>
public class RangeValidator : ValidatorBase, IDisposable
{
    /// <summary>
    ///     需要监听属性变更的属性名集合
    /// </summary>
    internal readonly string[] _observedPropertyNames =
    [
        nameof(MinimumIsExclusive), nameof(MaximumIsExclusive), nameof(ParseLimitsInInvariantCulture),
        nameof(ConvertValueInInvariantCulture)
    ];

    /// <summary>
    ///     <inheritdoc cref="AttributeValueValidator" />
    /// </summary>
    internal readonly AttributeValueValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="RangeValidator" />
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    /// <param name="maximum">允许的最大字段值</param>
    public RangeValidator(int minimum, int maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
        OperandType = typeof(int);

        _validator = new AttributeValueValidator(new RangeAttribute(minimum, maximum));

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     <inheritdoc cref="RangeValidator" />
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    /// <param name="maximum">允许的最大字段值</param>
    public RangeValidator(double minimum, double maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
        OperandType = typeof(double);

        _validator = new AttributeValueValidator(new RangeAttribute(minimum, maximum));

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     <inheritdoc cref="RangeValidator" />
    /// </summary>
    /// <param name="type">数据字段值的类型</param>
    /// <param name="minimum">允许的最小字段值</param>
    /// <param name="maximum">允许的最大字段值</param>
    public RangeValidator([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, string minimum,
        string maximum)
    {
        OperandType = type;
        Minimum = minimum;
        Maximum = maximum;

        _validator = new AttributeValueValidator(new RangeAttribute(type, minimum, maximum));

        // 订阅属性变更事件
        PropertyChanged += OnPropertyChanged;

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     允许的最小字段值
    /// </summary>
    public object Minimum { get; }

    /// <summary>
    ///     允许的最大字段值
    /// </summary>
    public object Maximum { get; }

    /// <summary>
    ///     是否当值等于 <see cref="Minimum" /> 时验证失败
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool MinimumIsExclusive
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
    ///     是否当值等于 <see cref="Maximum" /> 时验证失败
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool MaximumIsExclusive
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
    ///     数据字段值的类型
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public Type OperandType { get; }

    /// <summary>
    ///     判断 <see cref="Minimum" /> 和 <see cref="Maximum" /> 的字符串值是否依据固定区域性而非当前区域性进行解析
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool ParseLimitsInInvariantCulture
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
    ///     验证由构造函数参数 <c>RangeValidator(Type, String, String)</c> 设置的 <c>type</c> 的 <see cref="OperandType" />
    ///     值在进行任何转换时，是否采用的是固定区域性而非当前区域性
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool ConvertValueInInvariantCulture
    {
        get;
        set
        {
            field = value;

            // 触发属性变更事件
            OnPropertyChanged(value);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        _validator.IsValid(value, validationContext);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Minimum, Maximum);

    /// <summary>
    ///     释放资源
    /// </summary>
    /// <param name="disposing">是否释放托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 移除属性变更事件
            PropertyChanged -= OnPropertyChanged;
        }
    }

    /// <summary>
    ///     获取错误信息对应的资源键
    /// </summary>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal string GetResourceKey() =>
        MinimumIsExclusive switch
        {
            true when MaximumIsExclusive => nameof(ValidationMessages
                .RangeValidator_ValidationError_MinExclusive_MaxExclusive),
            true => nameof(ValidationMessages.RangeValidator_ValidationError_MinExclusive),
            _ => MaximumIsExclusive
                ? nameof(ValidationMessages.RangeValidator_ValidationError_MaxExclusive)
                : nameof(ValidationMessages.RangeValidator_ValidationError)
        };

    /// <summary>
    ///     订阅属性变更事件
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="eventArgs">
    ///     <see cref="ValidationPropertyChangedEventArgs" />
    /// </param>
    internal void OnPropertyChanged(object? sender, ValidationPropertyChangedEventArgs eventArgs)
    {
        // 检查是否是需要同步的属性名
        if (!_observedPropertyNames.Contains(eventArgs.PropertyName))
        {
            return;
        }

        // 应用属性变更到 RangeAttribute 对应的属性中
        typeof(RangeAttribute).GetProperty(eventArgs.PropertyName!)
            ?.SetValue(_validator.Attributes[0], eventArgs.PropertyValue);
    }
}