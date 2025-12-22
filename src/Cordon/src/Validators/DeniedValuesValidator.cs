// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     不允许的值列表验证器
/// </summary>
public class DeniedValuesValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="ValueAnnotationValidator" />
    /// </summary>
    internal readonly ValueAnnotationValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="DeniedValuesValidator" />
    /// </summary>
    /// <param name="values">不允许的值列表</param>
    public DeniedValuesValidator(params object?[] values)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(values);

        Values = values;
        _validator = new ValueAnnotationValidator(new DeniedValuesAttribute(Values));

        UseResourceKey(() => nameof(ValidationMessages.DeniedValuesValidator_ValidationError));
    }

    /// <summary>
    ///     不允许的值列表
    /// </summary>
    public object?[] Values { get; }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}