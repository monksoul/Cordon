// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     非 <c>null</c> 验证器
/// </summary>
public class NotNullValidator : ValidatorBase, IHighPriorityValidator
{
    /// <summary>
    ///     <inheritdoc cref="NotNullValidator" />
    /// </summary>
    public NotNullValidator() => UseResourceKey(() => nameof(ValidationMessages.NotNullValidator_ValidationError));

    /// <inheritdoc />
    /// <remarks>默认值为：0。</remarks>
    public int Priority => 0;

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) => value is not null;
}