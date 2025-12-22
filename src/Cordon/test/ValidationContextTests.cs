// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidationContextTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationContext<ObjectModel>(null!, null, null));

    [Fact]
    public void New_ReturnOK()
    {
        var context = new ValidationContext<ObjectModel>(new ObjectModel(), null, null);
        Assert.NotNull(context.Instance);
        Assert.Null(context._serviceProvider);
        Assert.NotNull(context.Items);
        Assert.Empty(context.Items);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var context2 =
            new ValidationContext<ObjectModel>(new ObjectModel(), serviceProvider,
                new Dictionary<object, object?> { { "name", "Furion" } });
        Assert.NotNull(context2.Instance);
        Assert.NotNull(context2._serviceProvider);
        Assert.NotNull(context2.Items);
        Assert.Single(context2.Items);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var context = new ValidationContext<ObjectModel>(new ObjectModel(), null, null);
        Assert.Null(context._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        context.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(context._serviceProvider);

        context.InitializeServiceProvider(null);
        Assert.Null(context._serviceProvider);
    }

    [Fact]
    public void GetService_ReturnOK()
    {
        using var serviceProvider =
            new ServiceCollection().AddTransient<IMyService, MyService>().BuildServiceProvider();

        var context =
            new ValidationContext<ObjectModel>(new ObjectModel(), serviceProvider, new Dictionary<object, object?>());
        Assert.NotNull(context.GetService(typeof(IMyService)));
        Assert.NotNull(context.GetService<IMyService>());
    }

    public class ObjectModel;

    public class MyService : IMyService;

    public interface IMyService;
}