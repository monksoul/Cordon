// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ObjectValidationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddObjectValidation_Invalid_Parameters()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() =>
        {
            services.AddObjectValidation((ValidationBuilder)null!);
        });
    }

    [Fact]
    public void AddObjectValidation_ReturnOK()
    {
        var services = new ServiceCollection();

        var validationBuilder = new ValidationBuilder();
        services.AddObjectValidation(validationBuilder);

        Assert.Single(services);
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationDataContext));
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddObjectValidation_Action_Empty_Parameters()
    {
        var services = new ServiceCollection();
        services.AddObjectValidation();

        Assert.Single(services);
        Assert.Contains(services, x => x.ServiceType == typeof(IValidationDataContext));
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddObjectValidation_Action_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddObjectValidation(builder => builder.AddValidator(typeof(ObjectModelValidator1)));

        Assert.Equal(3, services.Count);
        Assert.Contains(services, x => x.ServiceType == typeof(IObjectValidator<ObjectModel>));
        _ = services.BuildServiceProvider();
    }

    [Fact]
    public void AddObjectValidation_Duplicate_ReturnOK()
    {
        var services = new ServiceCollection();
        services.AddObjectValidation(s => s.AddValidator(typeof(ObjectModelValidator1)));
        services.AddObjectValidation(s => s.AddValidator(typeof(ObjectModelValidator1)));
        services.AddObjectValidation(s => s.AddValidator(typeof(ObjectModelValidator1)));

        Assert.Equal(3, services.Count);
        _ = services.BuildServiceProvider();
    }
}