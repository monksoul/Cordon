// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     允许的值列表验证器
/// </summary>
public class AllowedValuesValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="AllowedValuesValidator" />
    /// </summary>
    /// <param name="values">允许的值列表</param>
    public AllowedValuesValidator(params object?[] values)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(values);

        Values = values;

        UseResourceKey(() => nameof(ValidationMessages.AllowedValuesValidator_ValidationError));
    }

    /// <summary>
    ///     允许的值列表
    /// </summary>
    public object?[] Values { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        Values.Any(allowed => allowed?.Equals(value) ?? value is null);
}