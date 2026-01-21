// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <c>null</c> 验证器
/// </summary>
public class NullValidator : ValidatorBase, IHighPriorityValidator
{
    /// <summary>
    ///     <inheritdoc cref="NullValidator" />
    /// </summary>
    public NullValidator() => UseResourceKey(() => nameof(ValidationMessages.NullValidator_ValidationError));

    /// <inheritdoc />
    /// <remarks>默认值为：0。</remarks>
    int IHighPriorityValidator.Priority => 0;

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) => value is null;
}