// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidatorResultTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validatorResult = new ValidatorResult(true, null, null);
        Assert.True(validatorResult.IsValid);
        Assert.Null(validatorResult.ValidationResults);
        Assert.Null(validatorResult.Instance);

        var validatorResult2 = new ValidatorResult(false, [new ValidationResult("出错了")], new ObjectModel());
        Assert.False(validatorResult2.IsValid);
        Assert.NotNull(validatorResult2.ValidationResults);
        Assert.Single(validatorResult2.ValidationResults);
        Assert.NotNull(validatorResult2.Instance);

        var validatorResult3 =
            new ValidatorResult<ObjectModel>(false, [new ValidationResult("出错了")], new ObjectModel());
        Assert.False(validatorResult3.IsValid);
        Assert.NotNull(validatorResult3.ValidationResults);
        Assert.Single(validatorResult3.ValidationResults);
        Assert.NotNull(validatorResult3.Instance);
    }

    [Fact]
    public void ThrowIfInvalid_ReturnOK()
    {
        var validatorResult = new ValidatorResult(false, [new ValidationResult("出错了")], new ObjectModel());

        var exception = Assert.Throws<ValidationException>(() => validatorResult.ThrowIfInvalid());
        Assert.Equal("出错了", exception.Message);
    }

    public class ObjectModel;
}