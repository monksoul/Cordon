// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     空集合、数组和字符串验证器
/// </summary>
public class EmptyValidator : ValidatorBase, IHighPriorityValidator
{
    /// <summary>
    ///     <inheritdoc cref="EmptyValidator" />
    /// </summary>
    public EmptyValidator() => UseResourceKey(() => nameof(ValidationMessages.EmptyValidator_ValidationError));

    /// <inheritdoc />
    /// <remarks>默认值为：20。</remarks>
    int IHighPriorityValidator.Priority => 20;

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        value is null || (value.TryGetCount(out var count) && count == 0);
}