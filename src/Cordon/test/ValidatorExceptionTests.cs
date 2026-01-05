// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidatorExceptionTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var exception = new ValidatorException();
        Assert.NotNull(exception);

        var exception2 = new ValidatorException("错误消息");
        Assert.Equal("错误消息", exception2.Message);

        var exception3 = new ValidatorException("错误消息", new Exception("内部错误消息"));
        Assert.Equal("错误消息", exception3.Message);
        Assert.NotNull(exception3.InnerException);
        Assert.Equal("内部错误消息", exception3.InnerException.Message);
    }

    [Fact]
    public void Throw_ReturnOK()
    {
        var exception = Assert.Throws<ValidatorException>(() => ValidatorException.Throw("错误消息"));
        Assert.Equal("错误消息", exception.Message);
    }
}