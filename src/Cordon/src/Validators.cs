// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     数据验证模块静态类
/// </summary>
public static class Validators
{
    /// <summary>
    ///     默认的 <see cref="IValidationService" /> 实例
    /// </summary>
    /// <remarks>避免重复创建实例。</remarks>
    internal static readonly IValidationService _defaultValidationService = new ValidationService();

    /// <summary>
    ///     创建对象验证器
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public static ObjectValidator<T> Object<T>()
        where T : class => new();

    /// <summary>
    ///     创建对象验证器
    /// </summary>
    /// <param name="items">共享数据</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public static ObjectValidator<T> Object<T>(IDictionary<object, object?>? items)
        where T : class => new(items);

    /// <summary>
    ///     创建对象验证器
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ObjectValidator{T}" />
    /// </returns>
    public static ObjectValidator<T> Object<T>(IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
        where T : class => new(serviceProvider, items);

    /// <summary>
    ///     创建单值验证器
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public static ValueValidator<T> Value<T>() => new();

    /// <summary>
    ///     创建单值验证器
    /// </summary>
    /// <param name="items">共享数据</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public static ValueValidator<T> Value<T>(IDictionary<object, object?>? items) => new(items);

    /// <summary>
    ///     创建单值验证器
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ValueValidator{T}" />
    /// </returns>
    public static ValueValidator<T> Value<T>(IServiceProvider? serviceProvider, IDictionary<object, object?>? items) =>
        new(serviceProvider, items);

    /// <summary>
    ///     创建集合验证器
    /// </summary>
    /// <param name="elementValidator">
    ///     <see cref="IObjectValidator{T}" />
    /// </param>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <returns>
    ///     <see cref="CollectionValidator{TElement}" />
    /// </returns>
    public static CollectionValidator<TElement> Collection<TElement>(IObjectValidator<TElement> elementValidator) =>
        new(elementValidator);

    /// <summary>
    ///     创建年龄（0-120 岁）验证器
    /// </summary>
    /// <param name="isAdultOnly">是否仅允许成年人（18 岁及以上），默认值为：<c>false</c></param>
    /// <param name="allowStringValues">是否允许字符串数值，默认值为：<c>false</c></param>
    /// <returns>
    ///     <see cref="AgeValidator" />
    /// </returns>
    public static AgeValidator Age(bool isAdultOnly = false, bool allowStringValues = false) =>
        new() { IsAdultOnly = isAdultOnly, AllowStringValues = allowStringValues };

    /// <summary>
    ///     创建允许的值列表验证器
    /// </summary>
    /// <param name="values">允许的值列表</param>
    /// <returns>
    ///     <see cref="AllowedValuesValidator" />
    /// </returns>
    public static AllowedValuesValidator AllowedValues(params object?[] values) => new(values);

    /// <summary>
    ///     创建银行卡号验证器（Luhn 算法）
    /// </summary>
    /// <returns>
    ///     <see cref="BankCardValidator" />
    /// </returns>
    public static BankCardValidator BankCard() => new();

    /// <summary>
    ///     创建 Base64 字符串验证器
    /// </summary>
    /// <returns>
    ///     <see cref="Base64StringValidator" />
    /// </returns>
    public static Base64StringValidator Base64String() => new();

    /// <summary>
    ///     创建中文姓名验证器
    /// </summary>
    /// <returns>
    ///     <see cref="ChineseNameValidator" />
    /// </returns>
    public static ChineseNameValidator ChineseName() => new();

    /// <summary>
    ///     创建中文/汉字验证器
    /// </summary>
    /// <returns>
    ///     <see cref="ChineseValidator" />
    /// </returns>
    public static ChineseValidator Chinese() => new();

    /// <summary>
    ///     创建颜色值验证器
    /// </summary>
    /// <param name="fullMode">
    ///     是否启用完整模式。在完整模式下，支持的颜色格式包括：十六进制颜色、RGB、RGBA、HSL 和 HSLA；若未启用，则仅支持十六进制颜色、RGB 和 RGBA。默认值为：<c>false</c>
    /// </param>
    /// <returns>
    ///     <see cref="ColorValueValidator" />
    /// </returns>
    public static ColorValueValidator ColorValue(bool fullMode = false) => new() { FullMode = fullMode };

    /// <summary>
    ///     创建组合验证器
    /// </summary>
    /// <param name="configure">验证器配置委托</param>
    /// <param name="mode"><see cref="CompositeMode" />，默认值为：<see cref="CompositeMode.FailFast" /></param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="CompositeValidator{T}" />
    /// </returns>
    public static CompositeValidator<T> Composite<T>(Action<FluentValidatorBuilder<T>> configure,
        CompositeMode mode = CompositeMode.FailFast) => new(configure) { Mode = mode };

    /// <summary>
    ///     创建条件验证器
    /// </summary>
    /// <param name="buildConditions">条件构建器配置委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ConditionalValidator{T}" />
    /// </returns>
    public static ConditionalValidator<T> Conditional<T>(Action<ConditionBuilder<T>> buildConditions) =>
        new(buildConditions);

    /// <summary>
    ///     创建条件验证器
    /// </summary>
    /// <remarks>定义满足指定条件时执行的验证规则。</remarks>
    /// <param name="condition">条件委托</param>
    /// <param name="thenConfigure">验证器配置委托</param>
    /// <param name="otherwiseConfigure">验证器配置委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ConditionalValidator{T}" />
    /// </returns>
    public static ConditionalValidator<T> WhenMatch<T>(Func<T, bool> condition,
        Action<FluentValidatorBuilder<T>> thenConfigure,
        Action<FluentValidatorBuilder<T>>? otherwiseConfigure = null) =>
        Conditional<T>(builder =>
        {
            // 构建 ConditionBuilder<T> 实例
            var conditionBuilder = builder.When(condition).Then(thenConfigure);

            // 空检查
            if (otherwiseConfigure is not null)
            {
                conditionBuilder.Otherwise(otherwiseConfigure);
            }
        });

    /// <summary>
    ///     创建条件验证器
    /// </summary>
    /// <remarks>定义满足指定条件时返回指定的错误消息。</remarks>
    /// <param name="condition">条件委托</param>
    /// <param name="errorMessage">错误消息</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ConditionalValidator{T}" />
    /// </returns>
    public static ConditionalValidator<T> WhenMatch<T>(Func<T, bool> condition, string? errorMessage) =>
        Conditional<T>(builder => builder.When(condition).ThenMessage(errorMessage));

    /// <summary>
    ///     创建条件验证器
    /// </summary>
    /// <remarks>定义满足指定条件时返回指定的错误消息。</remarks>
    /// <param name="condition">条件委托</param>
    /// <param name="resourceType">错误信息资源类型</param>
    /// <param name="resourceName">错误信息资源名称</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="ConditionalValidator{T}" />
    /// </returns>
    public static ConditionalValidator<T> WhenMatch<T>(Func<T, bool> condition,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type resourceType, string resourceName) =>
        Conditional<T>(builder => builder.When(condition).ThenMessage(resourceType, resourceName));

    /// <summary>
    ///     创建自定义验证特性验证器
    /// </summary>
    /// <param name="validatorType">执行自定义验证的类型</param>
    /// <param name="method">验证方法</param>
    /// <returns>
    ///     <see cref="CustomValidationValidator" />
    /// </returns>
    public static CustomValidationValidator CustomValidation(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
        Type validatorType, string method) =>
        new(validatorType, method);

    /// <summary>
    ///     创建 <see cref="System.DateOnly" /> 验证器
    /// </summary>
    /// <param name="formats">允许的日期格式（如 "yyyy-MM-dd"）</param>
    /// <returns>
    ///     <see cref="DateOnlyValidator" />
    /// </returns>
    public static DateOnlyValidator DateOnly(params string[] formats) => new(formats);

    /// <summary>
    ///     创建 <see cref="System.DateOnly" /> 验证器
    /// </summary>
    /// <param name="formats">允许的日期格式（如 "yyyy-MM-dd"）</param>
    /// <param name="provider">格式提供器</param>
    /// <param name="style">日期解析样式，需与 <paramref name="provider" /> 搭配使用。默认值为：<see cref="DateTimeStyles.None" /></param>
    /// <returns>
    ///     <see cref="DateOnlyValidator" />
    /// </returns>
    public static DateOnlyValidator DateOnly(string[] formats, IFormatProvider? provider,
        DateTimeStyles style = DateTimeStyles.None) => new(formats) { Provider = provider, Style = style };

    /// <summary>
    ///     创建 <see cref="System.DateTime" /> 验证器
    /// </summary>
    /// <param name="formats">允许的日期格式（如 "yyyy-MM-dd HH:mm:ss"）</param>
    /// <returns>
    ///     <see cref="DateTimeValidator" />
    /// </returns>
    public static DateTimeValidator DateTime(params string[] formats) => new(formats);

    /// <summary>
    ///     创建 <see cref="System.DateTime" /> 验证器
    /// </summary>
    /// <param name="formats">允许的日期格式（如 "yyyy-MM-dd HH:mm:ss"）</param>
    /// <param name="provider">格式提供器</param>
    /// <param name="style">日期解析样式，需与 <paramref name="provider" /> 搭配使用。默认值为：<see cref="DateTimeStyles.None" /></param>
    /// <returns>
    ///     <see cref="DateTimeValidator" />
    /// </returns>
    public static DateTimeValidator DateTime(string[] formats, IFormatProvider? provider,
        DateTimeStyles style = DateTimeStyles.None) => new(formats) { Provider = provider, Style = style };

    /// <summary>
    ///     创建验证数值的小数位数验证器
    /// </summary>
    /// <param name="maxDecimalPlaces">允许的最大有效小数位数</param>
    /// <param name="allowStringValues">是否允许字符串数值，默认值为：<c>false</c></param>
    /// <returns>
    ///     <see cref="DecimalPlacesValidator" />
    /// </returns>
    public static DecimalPlacesValidator DecimalPlaces(int maxDecimalPlaces, bool allowStringValues = false) =>
        new(maxDecimalPlaces) { AllowStringValues = allowStringValues };

    /// <summary>
    ///     创建不允许的值列表验证器
    /// </summary>
    /// <param name="values">不允许的值列表</param>
    /// <returns>
    ///     <see cref="DeniedValuesValidator" />
    /// </returns>
    public static DeniedValuesValidator DeniedValues(params object?[] values) => new(values);

    /// <summary>
    ///     创建域名验证器
    /// </summary>
    /// <returns>
    ///     <see cref="DomainValidator" />
    /// </returns>
    public static DomainValidator Domain() => new();

    /// <summary>
    ///     创建邮箱地址验证器
    /// </summary>
    /// <returns>
    ///     <see cref="EmailAddressValidator" />
    /// </returns>
    public static EmailAddressValidator EmailAddress() => new();

    /// <summary>
    ///     创建空集合、数组和字符串验证器
    /// </summary>
    /// <returns>
    ///     <see cref="EmptyValidator" />
    /// </returns>
    public static EmptyValidator Empty() => new();

    /// <summary>
    ///     创建以特定字符/字符串结尾的验证器
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    /// <param name="comparison"><see cref="StringComparison" />，默认值为：<see cref="StringComparison.Ordinal" /></param>
    /// <returns>
    ///     <see cref="EndsWithValidator" />
    /// </returns>
    public static EndsWithValidator
        EndsWith(string searchValue, StringComparison comparison = StringComparison.Ordinal) =>
        new(searchValue) { Comparison = comparison };

    /// <summary>
    ///     创建枚举验证器
    /// </summary>
    /// <param name="supportFlags">是否支持 Flags 模式。默认值为：<c>false</c>。</param>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <returns>
    ///     <see cref="EnumValidator{TEnum}" />
    /// </returns>
    public static EnumValidator Enum<TEnum>(bool supportFlags = false)
        where TEnum : struct, Enum => new EnumValidator<TEnum> { SupportFlags = supportFlags };

    /// <summary>
    ///     创建枚举验证器
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    /// <param name="supportFlags">是否支持 Flags 模式。默认值为：<c>false</c>。</param>
    /// <returns>
    ///     <see cref="EnumValidator" />
    /// </returns>
    public static EnumValidator Enum(Type enumType, bool supportFlags = false) =>
        new(enumType) { SupportFlags = supportFlags };

    /// <summary>
    ///     创建相等验证器
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <returns>
    ///     <see cref="EqualToValidator" />
    /// </returns>
    public static EqualToValidator EqualTo(object? compareValue) => new(compareValue);

    /// <summary>
    ///     创建固定失败验证器
    /// </summary>
    /// <returns>
    ///     <see cref="FailureValidator" />
    /// </returns>
    public static FailureValidator Failure() => new();

    /// <summary>
    ///     创建文件拓展名验证器
    /// </summary>
    /// <param name="extensions">文件拓展名</param>
    /// <returns>
    ///     <see cref="FileExtensionsValidator" />
    /// </returns>
    public static FileExtensionsValidator FileExtensions(string extensions) => new(extensions);

    /// <summary>
    ///     创建大于等于验证器
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <returns>
    ///     <see cref="GreaterThanOrEqualToValidator" />
    /// </returns>
    public static GreaterThanOrEqualToValidator GreaterThanOrEqualTo(IComparable compareValue) => new(compareValue);

    /// <summary>
    ///     创建大于验证器
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <returns>
    ///     <see cref="GreaterThanValidator" />
    /// </returns>
    public static GreaterThanValidator GreaterThan(IComparable compareValue) => new(compareValue);

    /// <summary>
    ///     创建固定长度验证器
    /// </summary>
    /// <param name="length">长度</param>
    /// <param name="allowEmpty">是否允许空集合、数组和字符串，默认值为：<c>false</c>。</param>
    /// <returns>
    ///     <see cref="HaveLengthValidator" />
    /// </returns>
    public static HaveLengthValidator HaveLength(int length, bool allowEmpty = false) =>
        new(length) { AllowEmpty = allowEmpty };

    /// <summary>
    ///     创建身份证号验证器
    /// </summary>
    /// <returns>
    ///     <see cref="IDCardValidator" />
    /// </returns>
    public static IDCardValidator IDCard() => new();

    /// <summary>
    ///     创建 IP 地址验证器
    /// </summary>
    /// <param name="allowIPv6">是否允许 IPv6 地址，默认值为：<c>false</c></param>
    /// <returns>
    ///     <see cref="IpAddressValidator" />
    /// </returns>
    public static IpAddressValidator IpAddress(bool allowIPv6 = false) => new() { AllowIPv6 = allowIPv6 };

    /// <summary>
    ///     创建 JSON 格式验证器
    /// </summary>
    /// <param name="allowTrailingCommas">是否允许末尾多余逗号，默认值为：<c>false</c></param>
    /// <returns>
    ///     <see cref="JsonValidator" />
    /// </returns>
    public static JsonValidator Json(bool allowTrailingCommas = false) =>
        new() { AllowTrailingCommas = allowTrailingCommas };

    /// <summary>
    ///     创建长度验证器
    /// </summary>
    /// <param name="minimumLength">最小允许长度</param>
    /// <param name="maximumLength">最大允许长度</param>
    /// <returns>
    ///     <see cref="LengthValidator" />
    /// </returns>
    public static LengthValidator Length(int minimumLength, int maximumLength) => new(minimumLength, maximumLength);

    /// <summary>
    ///     创建小于等于验证器
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <returns>
    ///     <see cref="LessThanOrEqualToValidator" />
    /// </returns>
    public static LessThanOrEqualToValidator LessThanOrEqualTo(IComparable compareValue) => new(compareValue);

    /// <summary>
    ///     创建小于验证器
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <returns>
    ///     <see cref="LessThanValidator" />
    /// </returns>
    public static LessThanValidator LessThan(IComparable compareValue) => new(compareValue);

    /// <summary>
    ///     创建最大长度验证器
    /// </summary>
    /// <param name="length">最大允许长度</param>
    /// <returns>
    ///     <see cref="MaxLengthValidator" />
    /// </returns>
    public static MaxLengthValidator MaxLength(int length) => new(length);

    /// <summary>
    ///     创建最大值验证器
    /// </summary>
    /// <param name="maximum">允许的最大字段值</param>
    /// <returns>
    ///     MaxValidator
    /// </returns>
    public static MaxValidator Max(IComparable maximum) => new(maximum);

    /// <summary>
    ///     创建 MD5 字符串验证器
    /// </summary>
    /// <param name="allowShortFormat">是否允许截断的 128 位哈希值（16 字节的十六进制字符串，共 32 字符），默认值为：<c>false</c></param>
    /// <returns>
    ///     <see cref="MD5StringValidator" />
    /// </returns>
    public static MD5StringValidator MD5String(bool allowShortFormat = false) =>
        new() { AllowShortFormat = allowShortFormat };

    /// <summary>
    ///     创建最小长度验证器
    /// </summary>
    /// <param name="length">最小允许长度</param>
    /// <returns>
    ///     <see cref="MinLengthValidator" />
    /// </returns>
    public static MinLengthValidator MinLength(int length) => new(length);

    /// <summary>
    ///     创建最小值验证器
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    /// <returns>
    ///     <see cref="MinValidator" />
    /// </returns>
    public static MinValidator Min(IComparable minimum) => new(minimum);

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> Must<T>(Func<T, bool> condition) => new(condition);

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> Must<T>(Func<T, ValidationContext<T>, bool> condition) => new(condition);

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <remarks>仅当被验证的值不为 <c>null</c> 时才执行验证逻辑。</remarks>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> MustIfNotNull<T>(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return new MustValidator<T>(u => u is null || condition(u));
    }

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <remarks>仅当被验证的值不为 <c>null</c> 时才执行验证逻辑。</remarks>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> MustIfNotNull<T>(Func<T, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return new MustValidator<T>((u, c) => u is null || condition(u, c));
    }

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <remarks>仅当被验证的值不为 <c>null</c>、非空集合、数组和字符串时才执行验证逻辑。</remarks>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> MustIfNotNullOrEmpty<T>(Func<T, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return new MustValidator<T>(u => u is null || (u.TryGetCount(out var count) && count == 0) || condition(u));
    }

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <remarks>仅当被验证的值不为 <c>null</c>、非空集合、数组和字符串时才执行验证逻辑。</remarks>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> MustIfNotNullOrEmpty<T>(Func<T, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(condition);

        return new MustValidator<T>((u, c) =>
            u is null || (u.TryGetCount(out var count) && count == 0) || condition(u, c));
    }

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <param name="enumerable">集合</param>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> MustAny<T, TElement>(IEnumerable<TElement> enumerable,
        Func<T, TElement, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(condition);

        return new MustValidator<T>(u => enumerable.Any(x => condition(u, x)));
    }

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <param name="enumerable">集合</param>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> MustAny<T, TElement>(IEnumerable<TElement> enumerable,
        Func<T, TElement, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(condition);

        return new MustValidator<T>((u, c) => enumerable.Any(x => condition(u, x, c)));
    }

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <param name="enumerable">集合</param>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> MustAll<T, TElement>(IEnumerable<TElement> enumerable,
        Func<T, TElement, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(condition);

        return new MustValidator<T>(u => enumerable.All(x => condition(u, x)));
    }

    /// <summary>
    ///     创建自定义条件成立时委托验证器
    /// </summary>
    /// <param name="enumerable">集合</param>
    /// <param name="condition">条件委托</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <returns>
    ///     <see cref="MustValidator{T}" />
    /// </returns>
    public static MustValidator<T> MustAll<T, TElement>(IEnumerable<TElement> enumerable,
        Func<T, TElement, ValidationContext<T>, bool> condition)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(condition);

        return new MustValidator<T>((u, c) => enumerable.All(x => condition(u, x, c)));
    }

    /// <summary>
    ///     创建非空白字符串验证器
    /// </summary>
    /// <returns>
    ///     <see cref="NotBlankValidator" />
    /// </returns>
    public static NotBlankValidator NotBlank() => new();

    /// <summary>
    ///     创建非空集合、数组和字符串验证器
    /// </summary>
    /// <returns>
    ///     <see cref="NotEmptyValidator" />
    /// </returns>
    public static NotEmptyValidator NotEmpty() => new();

    /// <summary>
    ///     创建不相等验证器
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    /// <returns>
    ///     <see cref="NotEqualToValidator" />
    /// </returns>
    public static NotEqualToValidator NotEqualTo(object? compareValue) => new(compareValue);

    /// <summary>
    ///     创建非 null 验证器
    /// </summary>
    /// <returns>
    ///     <see cref="NotNullValidator" />
    /// </returns>
    public static NotNullValidator NotNull() => new();

    /// <summary>
    ///     创建 null 验证器
    /// </summary>
    /// <returns>
    ///     <see cref="NullValidator" />
    /// </returns>
    public static NullValidator Null() => new();

    /// <summary>
    ///     创建对象验证特性验证器
    /// </summary>
    /// <remarks>支持使用 <c>[ValidateNever]</c> 特性来跳过对特定属性的验证，仅限于 ASP.NET Core 应用项目。</remarks>
    /// <returns>
    ///     <see cref="AttributeObjectValidator" />
    /// </returns>
    public static AttributeObjectValidator AttributeObject() => new();

    /// <summary>
    ///     创建对象验证特性验证器
    /// </summary>
    /// <remarks>支持使用 <c>[ValidateNever]</c> 特性来跳过对特定属性的验证，仅限于 ASP.NET Core 应用项目。</remarks>
    /// <param name="items">共享数据</param>
    /// <returns>
    ///     <see cref="AttributeObjectValidator" />
    /// </returns>
    public static AttributeObjectValidator AttributeObject(IDictionary<object, object?>? items) => new(items);

    /// <summary>
    ///     创建对象验证特性验证器
    /// </summary>
    /// <remarks>支持使用 <c>[ValidateNever]</c> 特性来跳过对特定属性的验证，仅限于 ASP.NET Core 应用项目。</remarks>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    /// <returns>
    ///     <see cref="AttributeObjectValidator" />
    /// </returns>
    public static AttributeObjectValidator AttributeObject(IServiceProvider? serviceProvider,
        IDictionary<object, object?>? items) => new(serviceProvider, items);

    /// <summary>
    ///     创建密码验证器
    /// </summary>
    /// <param name="strong">是否启用强密码验证模式，默认值为：<c>false</c></param>
    /// <returns>
    ///     <see cref="PasswordValidator" />
    /// </returns>
    public static PasswordValidator Password(bool strong = false) => new() { Strong = strong };

    /// <summary>
    ///     创建手机号（中国）验证器
    /// </summary>
    /// <returns>
    ///     <see cref="PhoneNumberValidator" />
    /// </returns>
    public static PhoneNumberValidator PhoneNumber() => new();

    /// <summary>
    ///     创建邮政编码（中国）验证器
    /// </summary>
    /// <returns>
    ///     <see cref="PostalCodeValidator" />
    /// </returns>
    public static PostalCodeValidator PostalCode() => new();

    /// <summary>
    ///     创建属性验证特性验证器
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="AttributePropertyValidator{T}" />
    /// </returns>
    public static AttributePropertyValidator<T> AttributeProperty<T>(Expression<Func<T, object?>> selector)
        where T : class => new(selector);

    /// <summary>
    ///     创建属性验证特性验证器
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="items">共享数据</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="AttributePropertyValidator{T}" />
    /// </returns>
    public static AttributePropertyValidator<T> AttributeProperty<T>(Expression<Func<T, object?>> selector,
        IDictionary<object, object?>? items)
        where T : class => new(selector, items);

    /// <summary>
    ///     创建属性验证特性验证器
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <returns>
    ///     <see cref="AttributePropertyValidator{T}" />
    /// </returns>
    public static AttributePropertyValidator<T> AttributeProperty<T>(Expression<Func<T, object?>> selector,
        IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
        where T : class => new(selector, serviceProvider, items);

    /// <summary>
    ///     创建属性验证特性验证器
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TProperty">属性类型</typeparam>
    /// <returns>
    ///     <see cref="AttributePropertyValidator{T,TProperty}" />
    /// </returns>
    public static AttributePropertyValidator<T, TProperty> AttributeProperty<T, TProperty>(
        Expression<Func<T, TProperty>> selector) where T : class =>
        new(selector);

    /// <summary>
    ///     创建属性验证特性验证器
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="items">共享数据</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TProperty">属性类型</typeparam>
    /// <returns>
    ///     <see cref="AttributePropertyValidator{T,TProperty}" />
    /// </returns>
    public static AttributePropertyValidator<T, TProperty> AttributeProperty<T, TProperty>(
        Expression<Func<T, TProperty>> selector, IDictionary<object, object?>? items) where T : class =>
        new(selector, items);

    /// <summary>
    ///     创建属性验证特性验证器
    /// </summary>
    /// <param name="selector">属性选择器</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TProperty">属性类型</typeparam>
    /// <returns>
    ///     <see cref="AttributePropertyValidator{T,TProperty}" />
    /// </returns>
    public static AttributePropertyValidator<T, TProperty> AttributeProperty<T, TProperty>(
        Expression<Func<T, TProperty>> selector, IServiceProvider? serviceProvider,
        IDictionary<object, object?>? items) where T : class => new(selector, serviceProvider, items);

    /// <summary>
    ///     创建指定数值范围约束验证器
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    /// <param name="maximum">允许的最大字段值</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="RangeValidator" />
    /// </returns>
    public static RangeValidator Range(int minimum, int maximum, Action<RangeValidator>? configure = null)
    {
        // 初始化 RangeValidator 实例
        var validator = new RangeValidator(minimum, maximum);

        // 调用自定义配置委托
        configure?.Invoke(validator);

        return validator;
    }

    /// <summary>
    ///     创建指定数值范围约束验证器
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    /// <param name="maximum">允许的最大字段值</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="RangeValidator" />
    /// </returns>
    public static RangeValidator Range(double minimum, double maximum, Action<RangeValidator>? configure = null)
    {
        // 初始化 RangeValidator 实例
        var validator = new RangeValidator(minimum, maximum);

        // 调用自定义配置委托
        configure?.Invoke(validator);

        return validator;
    }

    /// <summary>
    ///     创建指定数值范围约束验证器
    /// </summary>
    /// <param name="type">数据字段值的类型</param>
    /// <param name="minimum">允许的最小字段值</param>
    /// <param name="maximum">允许的最大字段值</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="RangeValidator" />
    /// </returns>
    public static RangeValidator Range([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type,
        string minimum, string maximum, Action<RangeValidator>? configure = null)
    {
        // 初始化 RangeValidator 实例
        var validator = new RangeValidator(type, minimum, maximum);

        // 调用自定义配置委托
        configure?.Invoke(validator);

        return validator;
    }

    /// <summary>
    ///     创建指定数值范围约束验证器
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    /// <param name="maximum">允许的最大字段值</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="RangeValidator" />
    /// </returns>
    public static RangeValidator Between(int minimum, int maximum, Action<RangeValidator>? configure = null)
    {
        // 初始化 RangeValidator 实例
        var validator = new RangeValidator(minimum, maximum);

        // 调用自定义配置委托
        configure?.Invoke(validator);

        return validator;
    }

    /// <summary>
    ///     创建指定数值范围约束验证器
    /// </summary>
    /// <param name="minimum">允许的最小字段值</param>
    /// <param name="maximum">允许的最大字段值</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="RangeValidator" />
    /// </returns>
    public static RangeValidator Between(double minimum, double maximum, Action<RangeValidator>? configure = null)
    {
        // 初始化 RangeValidator 实例
        var validator = new RangeValidator(minimum, maximum);

        // 调用自定义配置委托
        configure?.Invoke(validator);

        return validator;
    }

    /// <summary>
    ///     创建指定数值范围约束验证器
    /// </summary>
    /// <param name="type">数据字段值的类型</param>
    /// <param name="minimum">允许的最小字段值</param>
    /// <param name="maximum">允许的最大字段值</param>
    /// <param name="configure">自定义配置委托</param>
    /// <returns>
    ///     <see cref="RangeValidator" />
    /// </returns>
    public static RangeValidator Between([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type,
        string minimum, string maximum, Action<RangeValidator>? configure = null)
    {
        // 初始化 RangeValidator 实例
        var validator = new RangeValidator(type, minimum, maximum);

        // 调用自定义配置委托
        configure?.Invoke(validator);

        return validator;
    }

    /// <summary>
    ///     创建正则表达式验证器
    /// </summary>
    /// <param name="pattern">正则表达式模式</param>
    /// <param name="matchTimeoutInMilliseconds">用于在操作超时前执行单个匹配操作的时间量。以毫秒为单位，默认值为：2000。</param>
    /// <returns>
    ///     <see cref="RegularExpressionValidator" />
    /// </returns>
    public static RegularExpressionValidator RegularExpression(string pattern, int matchTimeoutInMilliseconds = 2000) =>
        new(pattern) { MatchTimeoutInMilliseconds = matchTimeoutInMilliseconds };

    /// <summary>
    ///     创建正则表达式验证器
    /// </summary>
    /// <param name="pattern">正则表达式模式</param>
    /// <param name="matchTimeoutInMilliseconds">用于在操作超时前执行单个匹配操作的时间量。以毫秒为单位，默认值为：2000。</param>
    /// <returns>
    ///     <see cref="RegularExpressionValidator" />
    /// </returns>
    public static RegularExpressionValidator Matches(string pattern, int matchTimeoutInMilliseconds = 2000) =>
        new(pattern) { MatchTimeoutInMilliseconds = matchTimeoutInMilliseconds };

    /// <summary>
    ///     创建必填验证器
    /// </summary>
    /// <param name="allowEmptyStrings">是否允许空字符串。默认值为：<c>false</c>。</param>
    /// <returns>
    ///     <see cref="RequiredValidator" />
    /// </returns>
    public static RequiredValidator Required(bool allowEmptyStrings = false) =>
        new() { AllowEmptyStrings = allowEmptyStrings };

    /// <summary>
    ///     创建单项验证器
    /// </summary>
    /// <returns>
    ///     <see cref="SingleValidator" />
    /// </returns>
    public static SingleValidator Single() => new();

    /// <summary>
    ///     创建以特定字符/字符串开头的验证器
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    /// <param name="comparison"><see cref="StringComparison" />，默认值为：<see cref="StringComparison.Ordinal" /></param>
    /// <returns>
    ///     <see cref="StartsWithValidator" />
    /// </returns>
    public static StartsWithValidator StartsWith(string searchValue,
        StringComparison comparison = StringComparison.Ordinal) => new(searchValue) { Comparison = comparison };

    /// <summary>
    ///     创建包含特定字符/字符串的验证器
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    /// <param name="comparison"><see cref="StringComparison" />，默认值为：<see cref="StringComparison.Ordinal" /></param>
    /// <returns>
    ///     <see cref="StringContainsValidator" />
    /// </returns>
    public static StringContainsValidator StringContains(string searchValue,
        StringComparison comparison = StringComparison.Ordinal) => new(searchValue) { Comparison = comparison };

    /// <summary>
    ///     创建字符串长度验证器
    /// </summary>
    /// <param name="maximumLength">最大允许长度</param>
    /// <returns>
    ///     <see cref="StringLengthValidator" />
    /// </returns>
    public static StringLengthValidator StringLength(int maximumLength) => new(maximumLength);

    /// <summary>
    ///     创建字符串长度验证器
    /// </summary>
    /// <param name="minimumLength">最小允许长度</param>
    /// <param name="maximumLength">最大允许长度</param>
    /// <returns>
    ///     <see cref="StringLengthValidator" />
    /// </returns>
    public static StringLengthValidator StringLength(int minimumLength, int maximumLength) =>
        new(maximumLength) { MinimumLength = minimumLength };

    /// <summary>
    ///     创建不包含特定字符/字符串的验证器
    /// </summary>
    /// <param name="searchValue">检索的值</param>
    /// <param name="comparison"><see cref="StringComparison" />，默认值为：<see cref="StringComparison.Ordinal" /></param>
    /// <returns>
    ///     <see cref="StringNotContainsValidator" />
    /// </returns>
    public static StringNotContainsValidator StringNotContains(string searchValue,
        StringComparison comparison = StringComparison.Ordinal) => new(searchValue) { Comparison = comparison };

    /// <summary>
    ///     创建强密码模式验证器
    /// </summary>
    /// <returns>
    ///     <see cref="StrongPasswordValidator" />
    /// </returns>
    public static StrongPasswordValidator StrongPassword() => new();

    /// <summary>
    ///     创建座机（电话）验证器
    /// </summary>
    /// <returns>
    ///     <see cref="TelephoneValidator" />
    /// </returns>
    public static TelephoneValidator Telephone() => new();

    /// <summary>
    ///     创建时间格式 <see cref="System.TimeOnly" /> 验证器
    /// </summary>
    /// <param name="formats">允许的时间格式（如 "HH:mm:ss"）</param>
    /// <returns>
    ///     <see cref="TimeOnlyValidator" />
    /// </returns>
    public static TimeOnlyValidator TimeOnly(params string[] formats) => new(formats);

    /// <summary>
    ///     创建时间格式 <see cref="System.TimeOnly" /> 验证器
    /// </summary>
    /// <param name="formats">允许的时间格式（如 "HH:mm:ss"）</param>
    /// <param name="provider">格式提供器</param>
    /// <param name="style">日期解析样式，需与 <paramref name="provider" /> 搭配使用。默认值为：<see cref="DateTimeStyles.None" /></param>
    /// <returns>
    ///     <see cref="TimeOnlyValidator" />
    /// </returns>
    public static TimeOnlyValidator TimeOnly(string[] formats, IFormatProvider? provider,
        DateTimeStyles style = DateTimeStyles.None) => new(formats) { Provider = provider, Style = style };

    /// <summary>
    ///     创建 URL 地址验证器
    /// </summary>
    /// <param name="supportsFtp">是否支持 FTP 协议。默认值为：<c>false</c>。</param>
    /// <returns>
    ///     <see cref="UrlValidator" />
    /// </returns>
    public static UrlValidator Url(bool supportsFtp = false) => new() { SupportsFtp = supportsFtp };

    /// <summary>
    ///     创建用户名验证器
    /// </summary>
    /// <returns>
    ///     <see cref="UserNameValidator" />
    /// </returns>
    public static UserNameValidator UserName() => new();

    /// <summary>
    ///     创建验证器代理
    /// </summary>
    /// <param name="constructorArgs"><typeparamref name="TValidator" /> 构造函数参数列表</param>
    /// <typeparam name="TValidator">
    ///     <see cref="ValidatorBase" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="ValidatorProxy{T}" />
    /// </returns>
    public static ValidatorProxy<TValidator> ValidatorProxy<TValidator>(params object?[]? constructorArgs)
        where TValidator : ValidatorBase =>
        new(constructorArgs);

    /// <summary>
    ///     创建验证器代理
    /// </summary>
    /// <param name="validatingObjectFactory">用于执行验证的对象工厂</param>
    /// <param name="constructorArgsFactory"><typeparamref name="TValidator" /> 构造函数参数工厂</param>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TValidator">
    ///     <see cref="ValidatorBase" />
    /// </typeparam>
    /// <returns>
    ///     <see cref="ValidatorProxy{T1,T2}" />
    /// </returns>
    public static ValidatorProxy<T, TValidator> ValidatorProxy<T, TValidator>(Func<T, object?> validatingObjectFactory,
        Func<T, ValidationContext<T>, object?[]?>? constructorArgsFactory = null)
        where TValidator : ValidatorBase =>
        new(validatingObjectFactory, constructorArgsFactory);

    /// <summary>
    ///     创建单值验证特性验证器
    /// </summary>
    /// <param name="attributes">验证特性列表</param>
    /// <returns>
    ///     <see cref="AttributeValueValidator" />
    /// </returns>
    public static AttributeValueValidator AttributeValue(params ValidationAttribute[] attributes) => new(attributes);

    /// <summary>
    ///     创建单值验证特性验证器
    /// </summary>
    /// <param name="attributes">验证特性列表</param>
    /// <param name="items">共享数据</param>
    /// <returns>
    ///     <see cref="AttributeValueValidator" />
    /// </returns>
    public static AttributeValueValidator AttributeValue(ValidationAttribute[] attributes,
        IDictionary<object, object?>? items) => new(attributes, items);

    /// <summary>
    ///     创建单值验证特性验证器
    /// </summary>
    /// <param name="attributes">验证特性列表</param>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <param name="items">共享数据</param>
    /// <returns>
    ///     <see cref="AttributeValueValidator" />
    /// </returns>
    public static AttributeValueValidator AttributeValue(ValidationAttribute[] attributes,
        IServiceProvider? serviceProvider, IDictionary<object, object?>? items) =>
        new(attributes, serviceProvider, items);

    /// <summary>
    ///     创建指定对象类型的数据验证服务
    /// </summary>
    /// <param name="serviceProvider">
    ///     <see cref="IServiceProvider" />
    /// </param>
    /// <returns>
    ///     <see cref="IValidationService" />
    /// </returns>
    public static IValidationService Service(IServiceProvider? serviceProvider = null) =>
        serviceProvider is null ? _defaultValidationService : new ValidationService(serviceProvider);
}