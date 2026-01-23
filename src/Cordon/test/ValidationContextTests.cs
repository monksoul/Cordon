// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidationContextTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validationContext = new ValidationContext<ObjectModel>(null!);
        Assert.Null(validationContext.Instance);
        Assert.Equal("ObjectModel", validationContext.DisplayName);
        Assert.Null(validationContext.MemberNames);
        Assert.Null(validationContext.RuleSets);
        Assert.Empty(validationContext.Items);
        Assert.Null(validationContext._serviceProvider);

        var validationContext2 = new ValidationContext<ObjectModel>(new ObjectModel());
        Assert.NotNull(validationContext2.Instance);

        var validationContext3 =
            new ValidationContext<ObjectModel>(new ObjectModel(),
                new Dictionary<object, object?> { { "name", "Furion" } });
        Assert.NotNull(validationContext3.Instance);
        Assert.Single(validationContext3.Items);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();

        var validationContext4 =
            new ValidationContext<ObjectModel>(new ObjectModel(), serviceProvider,
                new Dictionary<object, object?> { { "name", "Furion" } });
        Assert.NotNull(validationContext4.Instance);
        Assert.Single(validationContext4.Items);
        Assert.NotNull(validationContext4._serviceProvider);

        var validationContext5 =
            new ValidationContext<ObjectModel>(new ObjectModel(), serviceProvider.GetService,
                new Dictionary<object, object?> { { "name", "Furion" } });
        Assert.NotNull(validationContext5.Instance);
        Assert.Single(validationContext5.Items);
        Assert.NotNull(validationContext5._serviceProvider);
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var context = new ValidationContext<ObjectModel>(new ObjectModel(), (IServiceProvider?)null, null);
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