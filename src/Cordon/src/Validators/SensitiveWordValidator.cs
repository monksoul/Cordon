// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     敏感词验证器
/// </summary>
public class SensitiveWordValidator : ValidatorBase
{
    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        throw new NotImplementedException();
}