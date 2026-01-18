// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     .NET 内置验证特性验证消息覆盖提供器
/// </summary>
/// <remarks>用于在运行时替换 .NET 内置验证特性默认验证错误消息。</remarks>
public static class DataAnnotationMessageProvider
{
    /// <summary>
    ///     线程同步锁对象
    /// </summary>
    internal static readonly object _lock = new();

    /// <summary>
    ///     原始的 .NET 内置 ResourceManager 实例
    /// </summary>
    internal static ResourceManager? _originalResourceManager;

    /// <summary>
    ///     指向 <c>System.SR.s_resourceManager</c> 字段的反射引用
    /// </summary>
    internal static FieldInfo? _resourceManagerField;

    /// <summary>
    ///     存储资源键到自定义消息的映射
    /// </summary>
    internal static readonly ConcurrentDictionary<string, string> _overrides = new();

    /// <summary>
    ///     注册一个验证消息覆盖项
    /// </summary>
    /// <param name="resourceKey">资源属性名</param>
    /// <param name="message">消息模板，支持 {0} 占位符</param>
    public static void AddOverride(string resourceKey, string message)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);
        ArgumentNullException.ThrowIfNull(message);

        _overrides[resourceKey] = message;

        // 应用当前所有覆盖消息到 .NET 内部资源管理器
        ApplyOverrides();
    }

    /// <summary>
    ///     批量注册多个验证消息覆盖项
    /// </summary>
    /// <param name="overrides">包含资源键到消息模板映射的字典</param>
    public static void AddOverrides(IDictionary<string, string> overrides)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(overrides);

        foreach (var (resourceKey, message) in overrides)
        {
            AddOverride(resourceKey, message);
        }
    }

    /// <summary>
    ///     使用标准中文验证消息替换 .NET 内置验证特性默认验证错误消息
    /// </summary>
    public static void UseChineseMessages()
    {
        // 更多可替换项参考：https://github.com/dotnet/dotnet/blob/main/src/runtime/src/libraries/System.ComponentModel.Annotations/src/Resources/Strings.resx
        var chineseMessages = new Dictionary<string, string>
        {
            { "RequiredAttribute_ValidationError", "字段 {0} 是必填项。" },
            { "StringLengthAttribute_ValidationError", "字段 {0} 必须是字符串，且最大长度为 {1}。" },
            { "StringLengthAttribute_ValidationErrorIncludingMinimum", "字段 {0} 必须是字符串，且长度介于 {2} 到 {1} 之间。" },
            { "RangeAttribute_ValidationError", "字段 {0} 的值必须介于 {1} 到 {2} 之间。" },
            { "RangeAttribute_ValidationError_MinExclusive", "字段 {0} 的值必须大于 {1} 且小于或等于 {2}。" },
            { "RangeAttribute_ValidationError_MaxExclusive", "字段 {0} 的值必须大于或等于 {1} 且小于 {2}。" },
            { "RangeAttribute_ValidationError_MinExclusive_MaxExclusive", "字段 {0} 的值必须大于 {1} 且小于 {2}。" },
            { "EmailAddressAttribute_Invalid", "字段 {0} 不是有效的电子邮件地址。" },
            { "RegularExpressionAttribute_ValidationError", "字段 {0} 必须匹配正则表达式“{1}”。" },
            { "MaxLengthAttribute_ValidationError", "字段 {0} 必须是字符串或数组类型，且最大长度为“{1}”。" },
            { "MinLengthAttribute_ValidationError", "字段 {0} 必须是字符串或数组类型，且最小长度为“{1}”。" },
            { "LengthAttribute_ValidationError", "字段 {0} 必须是字符串或集合类型，且长度介于“{1}”到“{2}”之间。" },
            { "CompareAttribute_MustMatch", "“{0}”和“{1}”不匹配。" },
            { "DataTypeAttribute_Invalid", "字段 {0} 的数据类型无效。" },
            { "EnumDataTypeAttribute_Invalid", "字段 {0} 的值不是枚举 {1} 中的有效值。" },
            { "UrlAttribute_Invalid", "字段 {0} 不是有效的完整 http、https 或 ftp URL。" },
            { "PhoneAttribute_Invalid", "字段 {0} 不是有效的电话号码。" },
            { "CreditCardAttribute_Invalid", "字段 {0} 不是有效的信用卡号。" },
            { "FileExtensionsAttribute_Invalid", "字段 {0} 仅接受以下扩展名的文件：{1}。" },
            { "AllowedValuesAttribute_Invalid", "字段 {0} 的值不在 AllowedValuesAttribute 指定的允许值列表中。" },
            { "DeniedValuesAttribute_Invalid", "字段 {0} 的值属于 DeniedValuesValidator 指定的禁止值之一。" },
            { "Base64StringAttribute_Invalid", "字段 {0} 不是有效的 Base64 编码。" },
            { "CustomValidationAttribute_ValidationError", "{0} 无效。" },
            { "ValidationAttribute_ValidationError", "字段 {0} 无效。" }
        };

        AddOverrides(chineseMessages);
    }

    /// <summary>
    ///     清除所有已注册的验证消息覆盖项
    /// </summary>
    /// <remarks>恢复 .NET 内置验证特性默认验证错误消息。</remarks>
    public static void ClearOverrides()
    {
        lock (_lock)
        {
            // 空检查
            if (_resourceManagerField is not null && _originalResourceManager is not null)
            {
                // 恢复原始 ResourceManager
                _resourceManagerField.SetValue(null, _originalResourceManager);
            }

            // 重置所有状态
            _overrides.Clear();
            _originalResourceManager = null;
            _resourceManagerField = null;
        }
    }

    /// <summary>
    ///     尝试根据资源键获取已注册的覆盖消息
    /// </summary>
    /// <param name="resourceKey">资源属性名</param>
    /// <returns>
    ///     <see cref="string" />
    /// </returns>
    internal static string? TryGetOverride(string resourceKey)
    {
        // 空检查
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceKey);

        return _overrides.GetValueOrDefault(resourceKey);
    }

    /// <summary>
    ///     应用当前所有覆盖消息到 .NET 内部资源管理器
    /// </summary>
    /// <remarks>仅在首次调用时执行注入操作，确保线程安全和幂等性。</remarks>
    internal static void ApplyOverrides()
    {
        lock (_lock)
        {
            // 无覆盖项则直接返回
            if (_overrides.IsEmpty)
            {
                return;
            }

            // 空检查
            if (_resourceManagerField is not null)
            {
                return;
            }

            // 获取 System.SR 类型
            var srType = typeof(ValidationAttribute).Assembly.GetType("System.SR");
            // 获取 System.SR 类型内部 s_resourceManager 字段
            var sResourceManagerField =
                srType?.GetField("s_resourceManager", BindingFlags.NonPublic | BindingFlags.Static);

            // 空检查
            ArgumentNullException.ThrowIfNull(sResourceManagerField);

            // 缓存 sResourceManagerField 字段
            _resourceManagerField = sResourceManagerField;

            // 缓存原始 ResourceManager 实例
            _originalResourceManager = (ResourceManager?)_resourceManagerField.GetValue(null);

            // 注入自定义 ResourceManager
            _resourceManagerField.SetValue(null, new OverrideResourceManager(_overrides));
        }
    }

    /// <summary>
    ///     自定义 ResourceManager
    /// </summary>
    /// <remarks>用于返回覆盖 .NET 内置验证特性验证错误消息。</remarks>
    internal sealed class OverrideResourceManager(ConcurrentDictionary<string, string> overrides) : ResourceManager
    {
        /// <inheritdoc />
        public override string? GetString(string name, CultureInfo? culture) => overrides.GetValueOrDefault(name);
    }
}