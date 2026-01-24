// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     全局验证消息覆盖提供器
/// </summary>
/// <remarks>用于在运行时替换框架内置的默认验证错误信息。</remarks>
public static class ValidationMessageProvider
{
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
    ///     使用标准中文验证消息替换框架内置的默认验证错误信息
    /// </summary>
    public static void UseChineseMessages()
    {
        // 更多可替换项参考：https://gitee.com/dotnetchina/Cordon/blob/master/src/Cordon/src/Resources/ValidationMessages.resx
        var chineseMessages = new Dictionary<string, string>
        {
            { "AgeValidator_ValidationError", "字段 {0} 不是有效的年龄。" },
            { "BankCardValidator_ValidationError", "字段 {0} 不是有效的银行卡号。" },
            { "ChineseNameValidator_ValidationError", "字段 {0} 不是有效的中文姓名。" },
            { "ChineseValidator_ValidationError", "字段 {0} 包含无效的中文字符。" },
            { "ColorValueValidator_ValidationError", "字段 {0} 不是有效的颜色值。" },
            { "DateTimeValidator_ValidationError", "字段 {0} 必须是有效的日期时间。" },
            { "DomainValidator_ValidationError", "字段 {0} 不是有效的域名。" },
            { "EndsWithValidator_ValidationError", "字段 {0} 未以字符串“{1}”结尾。" },
            { "EqualToValidator_ValidationError", "字段 {0} 必须等于“{1}”。" },
            { "GreaterThanOrEqualToValidator_ValidationError", "字段 {0} 必须大于或等于“{1}”。" },
            { "GreaterThanValidator_ValidationError", "字段 {0} 必须大于“{1}”。" },
            { "IDCardValidator_ValidationError", "字段 {0} 不是有效的身份证号码格式。" },
            { "LessThanOrEqualToValidator_ValidationError", "字段 {0} 必须小于或等于“{1}”。" },
            { "LessThanValidator_ValidationError", "字段 {0} 必须小于“{1}”。" },
            { "NotEmptyValidator_ValidationError", "字段 {0} 不允许为空值。" },
            { "NotEqualToValidator_ValidationError", "字段 {0} 不能等于“{1}”。" },
            { "PasswordValidator_ValidationError", "字段 {0} 的密码格式无效。密码长度必须为 8 到 64 个字符，且至少包含一个字母和一个数字。" },
            { "PhoneNumberValidator_ValidationError", "字段 {0} 不是有效的手机号码。" },
            { "PostalCodeValidator_ValidationError", "字段 {0} 不是有效的邮政编码。" },
            { "SingleValidator_ValidationError", "字段 {0} 仅允许单个项。" },
            { "StartsWithValidator_ValidationError", "字段 {0} 未以字符串“{1}”开头。" },
            { "StringContainsValidator_ValidationError", "字段 {0} 不包含字符串“{1}”。" },
            { "TelephoneValidator_ValidationError", "字段 {0} 不是有效的电话号码。" },
            { "UserNameValidator_ValidationError", "字段 {0} 不是有效的用户名。" },
            { "ValidatorBase_ValidationError", "字段 {0} 无效。" },
            { "DecimalPlacesValidator_ValidationError", "字段 {0} 的小数位数不能超过“{1}”位。" },
            { "DateOnlyValidator_ValidationError", "字段 {0} 必须是有效的日期。" },
            { "TimeOnlyValidator_ValidationError", "字段 {0} 必须是有效的时间。" },
            { "MD5StringValidator_ValidationError", "字段 {0} 不是有效的 MD5 字符串。" },
            { "RequiredValidator_ValidationError", "字段 {0} 是必填项。" },
            { "NotNullValidator_ValidationError", "字段 {0} 不允许为 null 值。" },
            { "NotBlankValidator_ValidationError", "字段 {0} 不能为空或仅包含空白字符。" },
            { "MaxLengthValidator_ValidationError", "字段 {0} 必须是字符串或数组类型，且最大长度为“{1}”。" },
            { "MinLengthValidator_ValidationError", "字段 {0} 必须是字符串或数组类型，且最小长度为“{1}”。" },
            { "LengthValidator_ValidationError", "字段 {0} 必须是字符串或集合类型，且长度介于“{1}”到“{2}”之间。" },
            { "StringLengthValidator_ValidationError", "字段 {0} 必须是字符串，且最大长度为“{1}”。" },
            { "StringLengthValidator_ValidationError_MinimumLength", "字段 {0} 必须是字符串，且长度介于“{2}”到“{1}”之间。" },
            { "EmailAddressValidator_ValidationError", "字段 {0} 不是有效的电子邮件地址。" },
            { "RegularExpressionValidator_ValidationError", "字段 {0} 必须匹配正则表达式“{1}”。" },
            { "AllowedValuesValidator_ValidationError", "字段 {0} 的值不在 AllowedValuesValidator 指定的允许值列表中。" },
            { "DeniedValuesValidator_ValidationError", "字段 {0} 的值属于 DeniedValuesValidator 指定的禁止值之一。" },
            { "Base64StringValidator_ValidationError", "字段 {0} 不是有效的 Base64 编码。" },
            { "UrlValidator_ValidationError", "字段 {0} 不是有效的完整 http 或 https URL。" },
            { "RangeValidator_ValidationError", "字段 {0} 的值必须介于“{1}”和“{2}”之间。" },
            { "RangeValidator_ValidationError_MinExclusive", "字段 {0} 的值必须大于“{1}”且小于或等于“{2}”。" },
            { "RangeValidator_ValidationError_MinExclusive_MaxExclusive", "字段 {0} 的值必须大于“{1}”且小于“{2}”。" },
            { "RangeValidator_ValidationError_MaxExclusive", "字段 {0} 的值必须大于或等于“{1}”且小于“{2}”。" },
            { "JsonValidator_ValidationError", "字段 {0} 必须是有效的 JSON 对象或数组。" },
            { "DateOnlyValidator_ValidationError_Formats", "字段 {0} 必须是符合以下格式之一的有效日期：{1}。" },
            { "DateTimeValidator_ValidationError_Formats", "字段 {0} 必须是符合以下格式之一的有效日期时间：{1}。" },
            {
                "PasswordValidator_ValidationError_Strong",
                "字段 {0} 的密码格式无效。密码长度必须为 12 到 64 个字符，且必须包含大写字母、小写字母、数字和特殊字符。"
            },
            { "AgeValidator_ValidationError_IsAdultOnly", "字段 {0} 必须年满 18 岁。" },
            { "TimeOnlyValidator_ValidationError_Formats", "字段 {0} 必须是符合以下格式之一的有效时间：{1}。" },
            { "UrlValidator_ValidationError_SupportsFtp", "字段 {0} 不是有效的完整 http、https 或 ftp URL。" },
            { "MinValidator_ValidationError", "字段 {0} 必须大于或等于“{1}”。" },
            { "MaxValidator_ValidationError", "字段 {0} 必须小于或等于“{1}”。" },
            { "IpAddressValidator_ValidationError", "字段 {0} 不是有效的 IPv4 地址。" },
            { "IpAddressValidator_ValidationError_AllowIPv6", "字段 {0} 不是有效的 IP 地址（IPv4 或 IPv6）。" },
            { "CompareValidator_ValidationError", "“{0}”与“{1}”不匹配。" },
            { "StringNotContainsValidator_ValidationError", "字段 {0} 不得包含字符串“{1}”。" },
            { "NullValidator_ValidationError", "字段 {0} 必须为 null。" },
            { "EmptyValidator_ValidationError", "字段 {0} 必须为空。" },
            { "EnumValidator_ValidationError", "字段 {0} 必须是枚举 {1} 中定义的值。" },
            { "EnumValidator_ValidationError_SupportFlags", "字段 {0} 必须是枚举 {1} 中定义的有效值组合。" },
            { "HaveLengthValidator_ValidationError", "字段 {0} 必须是字符串或集合类型，且长度恰好为“{1}”。" },
            { "HaveLengthValidator_ValidationError_AllowEmpty", "字段 {0} 必须为空，或长度恰好为“{1}”。" },
            { "FileExtensionsValidator_ValidationError", "字段 {0} 仅接受以下扩展名的文件：{1}。" }
        };

        AddOverrides(chineseMessages);
    }

    /// <summary>
    ///     清除所有已注册的验证消息覆盖项
    /// </summary>
    public static void ClearOverrides() => _overrides.Clear();

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
}