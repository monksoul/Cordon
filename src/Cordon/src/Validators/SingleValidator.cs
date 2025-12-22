// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     单项验证器
/// </summary>
public class SingleValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="SingleValidator" />
    /// </summary>
    public SingleValidator() => UseResourceKey(() => nameof(ValidationMessages.SingleValidator_ValidationError));

    /// <inheritdoc />
    public override bool IsValid(object? value) => value is null || (value.TryGetCount(out var count) && count == 1);
}