// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidatorBaseExtensionsTests
{
    [Fact]
    public void WithErrorMessage_Invalid_Parameters()
    {
        var validator = new AgeValidator();
        Assert.Throws<ArgumentNullException>(() => validator.WithErrorMessage(null!, null!));
        Assert.Throws<ArgumentNullException>(() => validator.WithErrorMessage(typeof(TestValidationMessages), null!));
        Assert.Throws<ArgumentException>(() =>
            validator.WithErrorMessage(typeof(TestValidationMessages), string.Empty));
        Assert.Throws<ArgumentException>(() =>
            validator.WithErrorMessage(typeof(TestValidationMessages), "  "));
    }

    [Fact]
    public void WithErrorMessage_ReturnOK()
    {
        var validator = new AgeValidator();
        validator.WithErrorMessage("自定义错误消息");
        Assert.Equal("自定义错误消息", validator.ErrorMessage);

        validator.WithErrorMessage(null);
        Assert.Null(validator.ErrorMessage);

        var validator2 = new AgeValidator();
        validator2.WithErrorMessage(typeof(TestValidationMessages), "TestValidator_ValidationError2");
        Assert.Equal(typeof(TestValidationMessages), validator2.ErrorMessageResourceType);
        Assert.Equal("TestValidator_ValidationError2", validator2.ErrorMessageResourceName);
    }

    [Fact]
    public void WithMessage_Invalid_Parameters()
    {
        var validator = new AgeValidator();
        Assert.Throws<ArgumentNullException>(() => validator.WithMessage(null!, null!));
        Assert.Throws<ArgumentNullException>(() => validator.WithMessage(typeof(TestValidationMessages), null!));
        Assert.Throws<ArgumentException>(() =>
            validator.WithMessage(typeof(TestValidationMessages), string.Empty));
        Assert.Throws<ArgumentException>(() =>
            validator.WithMessage(typeof(TestValidationMessages), "  "));
    }

    [Fact]
    public void WithMessage_ReturnOK()
    {
        var validator = new AgeValidator();
        validator.WithMessage("自定义错误消息");
        Assert.Equal("自定义错误消息", validator.ErrorMessage);

        validator.WithMessage(null);
        Assert.Null(validator.ErrorMessage);

        var validator2 = new AgeValidator();
        validator2.WithMessage(typeof(TestValidationMessages), "TestValidator_ValidationError2");
        Assert.Equal(typeof(TestValidationMessages), validator2.ErrorMessageResourceType);
        Assert.Equal("TestValidator_ValidationError2", validator2.ErrorMessageResourceName);
    }
}