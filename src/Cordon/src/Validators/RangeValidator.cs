// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     指定数值范围约束验证器
/// </summary>
/// <remarks>
///     <see
///         href="https://github.com/dotnet/dotnet/blob/main/src/runtime/src/libraries/System.ComponentModel.Annotations/src/System/ComponentModel/DataAnnotations/RangeAttribute.cs">
///         参考代码
///     </see>
/// </remarks>
public class RangeValidator : ValidatorBase
{
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

        UseResourceKey(GetResourceKey);
    }

    /// <summary>
    ///     允许的最小字段值
    /// </summary>
    public object Minimum { get; private set; }

    /// <summary>
    ///     允许的最大字段值
    /// </summary>
    public object Maximum { get; private set; }

    /// <summary>
    ///     是否当值等于 <see cref="Minimum" /> 时验证失败
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool MinimumIsExclusive { get; set; }

    /// <summary>
    ///     是否当值等于 <see cref="Maximum" /> 时验证失败
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool MaximumIsExclusive { get; set; }

    /// <summary>
    ///     数据字段值的类型
    /// </summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public Type OperandType { get; }

    /// <summary>
    ///     判断 <see cref="Minimum" /> 和 <see cref="Maximum" /> 的字符串值是否依据固定区域性而非当前区域性进行解析
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool ParseLimitsInInvariantCulture { get; set; }

    /// <summary>
    ///     验证由构造函数参数 <c>RangeValidator(Type, String, String)</c> 设置的 <c>type</c> 的 <see cref="OperandType" />
    ///     值在进行任何转换时，是否采用的是固定区域性而非当前区域性
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool ConvertValueInInvariantCulture { get; set; }

    /// <summary>
    ///     内部缓存的值转换委托
    /// </summary>
    /// <remarks>用于将任意输入对象转换为目标类型（<see cref="OperandType" />）。</remarks>
    internal Func<object, object?>? Conversion { get; set; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 确保转换逻辑已初始化
        SetupConversion();

        // 空检查
        if (value is null or string { Length: 0 })
        {
            return true;
        }

        object? convertedValue;

        try
        {
            // 执行类型转换
            convertedValue = Conversion!(value);
        }
        catch (FormatException)
        {
            return false;
        }
        catch (InvalidCastException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }

        // 将边界值转为 IComparable 以便通用比较
        var min = (IComparable)Minimum;
        var max = (IComparable)Maximum;

        return
            (MinimumIsExclusive ? min.CompareTo(convertedValue) < 0 : min.CompareTo(convertedValue) <= 0) &&
            (MaximumIsExclusive ? max.CompareTo(convertedValue) > 0 : max.CompareTo(convertedValue) >= 0);
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        // 确保转换逻辑已初始化
        SetupConversion();

        return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Minimum, Maximum);
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
    ///     初始化内部状态
    /// </summary>
    /// <remarks>验证边界有效性，并创建值转换委托。</remarks>
    /// <param name="minimum">已解析的最小字段值</param>
    /// <param name="maximum">已解析的最大字段值</param>
    /// <param name="conversion">将输入值转换为目标类型的委托</param>
    /// <exception cref="InvalidOperationException"></exception>
    internal void Initialize(IComparable minimum, IComparable maximum, Func<object, object?> conversion)
    {
        // 获取最小字段值和最大字段值比较结果
        var cmp = minimum.CompareTo(maximum);

        switch (cmp)
        {
            case > 0:
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "The maximum value '{0}' must be greater than or equal to the minimum value '{1}'.", maximum,
                    minimum));
            case 0 when MinimumIsExclusive || MaximumIsExclusive:
                throw new InvalidOperationException(
                    "Cannot use exclusive bounds when the maximum value is equal to the minimum value.");
        }

        // 保存解析后的边界值和转换委托
        Minimum = minimum;
        Maximum = maximum;
        Conversion = conversion;
    }

    /// <summary>
    ///     初始化转换逻辑
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    internal void SetupConversion()
    {
        // 已初始化则直接返回
        if (Conversion is not null)
        {
            return;
        }

        var minimum = Minimum;
        var maximum = Maximum;

        // 空检查
        if (minimum == null || maximum == null)
        {
            throw new InvalidOperationException("The minimum and maximum values must be set.");
        }

        // 根据数据字段值的类型进行初始化
        var operandType = minimum.GetType();
        if (operandType == typeof(int))
        {
            Initialize((int)minimum, (int)maximum, u => Convert.ToInt32(u, CultureInfo.InvariantCulture));
        }
        else if (operandType == typeof(double))
        {
            Initialize((double)minimum, (double)maximum, u => Convert.ToDouble(u, CultureInfo.InvariantCulture));
        }
        else
        {
            var type = OperandType;
            if (type is null)
            {
                throw new InvalidOperationException(
                    "The OperandType must be set when strings are used for minimum and maximum values.");
            }

            // 检查目标类型是否可以比较
            var comparableType = typeof(IComparable);
            if (!comparableType.IsAssignableFrom(type))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "The type {0} must implement {1}.", type.FullName, comparableType.FullName));
            }

            // 获取目标类型的 TypeConverter
            var converter = GetOperandTypeConverter();

            // 解析字符串边界值为目标类型实例
            var min = (IComparable)(ParseLimitsInInvariantCulture
                ? converter.ConvertFromInvariantString((string)minimum)!
                : converter.ConvertFromString((string)minimum))!;
            var max = (IComparable)(ParseLimitsInInvariantCulture
                ? converter.ConvertFromInvariantString((string)maximum)!
                : converter.ConvertFromString((string)maximum))!;

            // 构建输入值转换委托
            Func<object, object?> conversion;
            if (ConvertValueInInvariantCulture)
            {
                conversion = value => value.GetType() == type
                    ? value
                    : converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
            }
            else
            {
                conversion = value => value.GetType() == type ? value : converter.ConvertFrom(value);
            }

            Initialize(min, max, conversion);
        }
    }

    /// <summary>
    ///     获取与 <see cref="OperandType" /> 关联的 <see cref="TypeConverter" /> 实例
    /// </summary>
    /// <remarks>此方法可能触发反射，在 AOT 或裁剪（trimming）环境下需确保类型元数据保留。</remarks>
    /// <returns>
    ///     <see cref="TypeConverter" />
    /// </returns>
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode",
        Justification = "The ctor that allows this code to be called is marked with RequiresUnreferencedCode.")]
    internal TypeConverter GetOperandTypeConverter() => TypeDescriptor.GetConverter(OperandType);
}