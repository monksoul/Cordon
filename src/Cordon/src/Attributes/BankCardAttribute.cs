// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace System.ComponentModel.DataAnnotations;

/// <summary>
///     银行卡号验证特性（Luhn 算法）
/// </summary>
/// <remarks>
///     <see href="https://baike.baidu.com/item/Luhn算法/22799984">Luhn 算法</see>
///     <see href="https://www.ee.unb.ca/cgi-bin/tervo/luhn.pl">Luhn 算法在线测试</see>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class BankCardAttribute : ValidationBaseAttribute
{
    /// <inheritdoc cref="BankCardValidator" />
    internal readonly BankCardValidator _validator;

    /// <summary>
    ///     <inheritdoc cref="BankCardAttribute" />
    /// </summary>
    public BankCardAttribute()
    {
        _validator = new BankCardValidator();

        UseResourceKey(() => nameof(ValidationMessages.BankCardValidator_ValidationError));
    }

    /// <inheritdoc />
    public override bool IsValid(object? value) => _validator.IsValid(value);
}