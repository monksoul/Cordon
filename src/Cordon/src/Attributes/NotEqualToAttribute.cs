// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     不相等验证特性
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class NotEqualToAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="NotEqualToValidator" />
    internal readonly NotEqualToValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="NotEqualToAttribute" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public NotEqualToAttribute(object? compareValue)
    {
        CompareValue = compareValue;
        _validator = new NotEqualToValidator(compareValue);

        UseResourceKey(() => nameof(ValidationMessages.NotEqualToValidator_ValidationError));
    }

    /// <summary>
    ///     比较的值
    /// </summary>
    public object? CompareValue { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, CompareValue?.ToString() ?? "null");
}