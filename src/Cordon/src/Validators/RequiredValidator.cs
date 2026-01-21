// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     必填验证器
/// </summary>
public class RequiredValidator : ValidatorBase, IHighPriorityValidator
{
    /// <summary>
    ///     <inheritdoc cref="RequiredValidator" />
    /// </summary>
    public RequiredValidator() => UseResourceKey(() => nameof(ValidationMessages.RequiredValidator_ValidationError));

    /// <summary>
    ///     是否允许空字符串
    /// </summary>
    /// <remarks>默认值为：<c>false</c>。</remarks>
    public bool AllowEmptyStrings { get; set; }

    /// <inheritdoc />
    /// <remarks>默认值为：10。</remarks>
    int IHighPriorityValidator.Priority => 10;

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext)
    {
        // 空检查
        if (value is null)
        {
            return false;
        }

        return AllowEmptyStrings || value is not string stringValue || !string.IsNullOrWhiteSpace(stringValue);
    }
}