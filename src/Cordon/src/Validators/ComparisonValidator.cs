// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     比较验证器抽象基类
/// </summary>
public abstract class ComparisonValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="ComparisonValidator" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <param name="resourceKey">资源属性名</param>
    protected ComparisonValidator(IComparable compareValue, string resourceKey)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValue);
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);

        CompareValue = compareValue;

        UseResourceKey(() => resourceKey);
    }

    /// <summary>
    ///     <inheritdoc cref="ComparisonValidator" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <param name="errorMessageResourceAccessor">错误信息资源访问器</param>
    protected ComparisonValidator(IComparable compareValue, Func<string> errorMessageResourceAccessor)
        : base(errorMessageResourceAccessor)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareValue);

        CompareValue = compareValue;
    }

    /// <summary>
    ///     比较的值
    /// </summary>
    public IComparable CompareValue { get; }

    /// <summary>
    ///     执行验证的值的转换委托
    /// </summary>
    /// <remarks>用于将执行验证的值转换为 <see cref="CompareValue" /> 同等类型。</remarks>
    internal Func<object, object>? Conversion { get; private set; }

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
    protected abstract bool IsValid(IComparable value, IValidationContext? validationContext);

    /// <inheritdoc />
    public sealed override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 初始化执行验证的值的转换委托
        SetupConversion();

        // 可检查
        if (value is null or string { Length: 0 })
        {
            return true;
        }

        // 将执行验证的值转换为 CompareValue 同等类型
        object? convertedValue;
        try
        {
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
        catch (OverflowException)
        {
            return false;
        }

        return convertedValue is IComparable comparable && IsValid(comparable, validationContext);
    }

    /// <inheritdoc />
    public override string FormatErrorMessage(string name)
    {
        // 初始化执行验证的值的转换委托
        SetupConversion();

        return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, CompareValue);
    }

    /// <summary>
    ///     初始化执行验证的值的转换委托
    /// </summary>
    internal void SetupConversion()
    {
        // 空检查
        if (Conversion is not null)
        {
            return;
        }

        // 获取数据字段值的类型
        var operandType = CompareValue.GetType();

        // 检查是否是 int 类型
        if (operandType == typeof(int))
        {
            Conversion = u => Convert.ToInt32(u, CultureInfo.InvariantCulture);
        }
        // 检查是否是 double 类型
        else if (operandType == typeof(double))
        {
            Conversion = u => Convert.ToDouble(u, CultureInfo.InvariantCulture);
        }
        // 其他类型创建默认的类型转换委托
        else
        {
            Conversion = CreateDefaultConversion(operandType);
        }
    }

    /// <summary>
    ///     为指定类型创建默认的类型转换委托
    /// </summary>
    /// <remarks>此方法可能触发反射，在 AOT 或裁剪（trimming）环境下需确保类型元数据保留。</remarks>
    /// <param name="operandType">数据字段值的类型</param>
    /// <returns>
    ///     <see cref="Func{T,TResult}" />
    /// </returns>
    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode",
        Justification = "The ctor that allows this code to be called is marked with RequiresUnreferencedCode.")]
    protected virtual Func<object, object> CreateDefaultConversion(Type operandType)
    {
        // 若要支持更多类型，可使用: value => Convert.ChangeType(value, operandType, CultureInfo.InvariantCulture);

        // 获取目标类型的 TypeConverter（主要用于字符串解析，对于数值类型间的转换可能失败）
        var typeConverter = TypeDescriptor.GetConverter(operandType);

        return value => value.GetType() == operandType ? value : typeConverter.ConvertFrom(value)!;
    }
}