// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     空集合、数组和字符串验证器
/// </summary>
public class EmptyValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="EmptyValidator" />
    /// </summary>
    public EmptyValidator() => UseResourceKey(() => nameof(ValidationMessages.EmptyValidator_ValidationError));

    /// <inheritdoc />
    public override bool IsValid(object? value) => value is null || (value.TryGetCount(out var count) && count == 0);
}