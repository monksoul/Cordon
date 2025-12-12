// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ValidatorOptionsTests
{
    [Fact]
    public void ValidatorOptions_ReturnOK()
    {
        var validatorOptions = new ValidatorOptions();
        Assert.False(validatorOptions.SuppressAnnotationValidation);
        Assert.True(validatorOptions.ValidateAllProperties);
    }

    [Fact]
    public void PropertyChanged_ReturnOK()
    {
        var validatorOptions = new ValidatorOptions();

        var list = new List<string?>();
        validatorOptions.PropertyChanged += (sender, args) =>
        {
            list.Add(args.PropertyName);
        };

        validatorOptions.SuppressAnnotationValidation = true;
        validatorOptions.ValidateAllProperties = false;

        Assert.Equal(
            [nameof(validatorOptions.SuppressAnnotationValidation), nameof(validatorOptions.ValidateAllProperties)],
            list);
    }
}