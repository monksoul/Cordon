// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     不相等验证器
/// </summary>
public class NotEqualToValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="NotEqualToValidator" />
    /// </summary>
    /// <param name="compareValue">比较的值</param>
    public NotEqualToValidator(object? compareValue)
    {
        CompareValue = compareValue;

        UseResourceKey(() => nameof(ValidationMessages.NotEqualToValidator_ValidationError));
    }

    /// <summary>
    ///     比较的值
    /// </summary>
    public object? CompareValue { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => value is null || !Equals(value, CompareValue);

    /// <inheritdoc />
    public override string FormatErrorMessage(string name) =>
        string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, CompareValue?.ToString() ?? "null");
}