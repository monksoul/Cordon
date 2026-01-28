// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidatorResultExtensionsTests
{
    [Fact]
    public void ThrowIfInvalid_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ValidatorResultExtensions.ThrowIfInvalid(null!));

    [Fact]
    public void ThrowIfInvalid_ReturnOK()
    {
        List<ValidatorResult> validatorResults =
        [
            new(true, null, null),
            new(false, [new ValidationResult("出错了")], null)
        ];

        var exception = Assert.Throws<ValidationException>(() => validatorResults.ThrowIfInvalid());
        Assert.Equal("出错了", exception.Message);
    }
}