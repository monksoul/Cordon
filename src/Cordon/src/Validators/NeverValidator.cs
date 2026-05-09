// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     永不通过验证器
/// </summary>
/// <remarks>仅用于输出指定错误信息。</remarks>
public sealed class NeverValidator : ValidatorBase
{
    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) => false;
}