// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ValidationPropertyChangedEventArgsTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var eventArgs = new ValidationPropertyChangedEventArgs(null, null);
        Assert.Null(eventArgs.PropertyName);
        Assert.Null(eventArgs.PropertyValue);

        var eventArgs2 = new ValidationPropertyChangedEventArgs("Name", "Furion");
        Assert.Equal("Name", eventArgs2.PropertyName);
        Assert.Equal("Furion", eventArgs2.PropertyValue);
    }
}