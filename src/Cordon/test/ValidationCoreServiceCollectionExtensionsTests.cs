// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidationCoreServiceCollectionExtensionsTests
{
    [Fact]
    public void AddValidationCore_Invalid_Parameters()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() =>
        {
            services.AddValidationCore((ValidationBuilder)null!);
        });
    }

    [Fact]
    public void AddValidationCore_ReturnOK()
    {
        var services = new ServiceCollection();

        var validationBuilder = new ValidationBuilder();
        services.AddValidationCore(validationBuilder);

        Assert.Equal(2, services.Count);
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationService));
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddValidationCore_Action_Empty_Parameters()
    {
        var services = new ServiceCollection();
        services.AddValidationCore();

        Assert.Equal(2, services.Count);
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationService));
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddValidationCore_Action_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddValidationCore(builder => builder.AddValidator(typeof(ObjectModelValidator1)));

        Assert.Equal(5, services.Count);
        Assert.Contains(services, x => x.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.Contains(services, x => x.ServiceType == typeof(ObjectModelValidator1));
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddValidationCore_Duplicate_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddValidationCore(s => s.AddValidator(typeof(ObjectModelValidator1)));
        services.AddValidationCore(s => s.AddValidator(typeof(ObjectModelValidator1)));
        services.AddValidationCore(s => s.AddValidator(typeof(ObjectModelValidator1)));

        Assert.Equal(5, services.Count);
        _ = services.BuildServiceProvider();
    }
}