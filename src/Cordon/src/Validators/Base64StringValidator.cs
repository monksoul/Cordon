// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     Base64 字符串验证器
/// </summary>
public class Base64StringValidator : ValidatorBase
{
    /// <summary>
    ///     <inheritdoc cref="AttributeValueValidator" />
    /// </summary>
    internal readonly AttributeValueValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="Base64StringValidator" />
    /// </summary>
    public Base64StringValidator()
    {
        _validator = new AttributeValueValidator(new Base64StringAttribute());

        UseResourceKey(() => nameof(ValidationMessages.Base64StringValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value, IValidationContext? validationContext) =>
        _validator.IsValid(value, validationContext);
}