// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class CordonServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCordon_Invalid_Parameters()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() =>
        {
            services.AddCordon((ValidationBuilder)null!);
        });
    }

    [Fact]
    public void AddCordon_ReturnOK()
    {
        var services = new ServiceCollection();

        var validationBuilder = new ValidationBuilder();
        services.AddCordon(validationBuilder);

        Assert.Equal(2, services.Count);
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationService));
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddCordon_Action_Empty_Parameters()
    {
        var services = new ServiceCollection();
        services.AddCordon();

        Assert.Equal(2, services.Count);
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationService));
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddCordon_Action_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddCordon(builder => builder.AddValidator(typeof(ObjectModelValidator1)));

        Assert.Equal(5, services.Count);
        Assert.Contains(services, x => x.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.Contains(services, x => x.ServiceType == typeof(ObjectModelValidator1));
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddCordon_Duplicate_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddCordon(s => s.AddValidator(typeof(ObjectModelValidator1)));
        services.AddCordon(s => s.AddValidator(typeof(ObjectModelValidator1)));
        services.AddCordon(s => s.AddValidator(typeof(ObjectModelValidator1)));

        Assert.Equal(5, services.Count);
        _ = services.BuildServiceProvider();
    }
}