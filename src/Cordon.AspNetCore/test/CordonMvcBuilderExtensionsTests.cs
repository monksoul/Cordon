// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.AspNetCore.Tests;

public class CordonMvcBuilderExtensionsTests
{
    [Fact]
    public void AddCordon_ReturnOK()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers().AddCordon();

        Assert.Contains(builder.Services, u => u.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(builder.Services, u => u.ServiceType == typeof(IStringLocalizer<>));
        Assert.Contains(builder.Services, u => u.ServiceType == typeof(IStringLocalizerFactory));
        Assert.Contains(builder.Services,
            u => u.ServiceType == typeof(IConfigureOptions<MvcDataAnnotationsLocalizationOptions>));
        using var app = builder.Build();

        var mvcOptions = app.Services.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.NotNull(mvcOptions.ModelValidatorProviders);
        Assert.Equal(3, mvcOptions.ModelValidatorProviders.Count);
        Assert.Equal(typeof(ValidationOptionsModelValidatorProvider),
            mvcOptions.ModelValidatorProviders.First().GetType());

        Assert.NotNull(mvcOptions.Filters);
        Assert.Equal(2, mvcOptions.Filters.Count);
        Assert.Equal(typeof(ValidationOptionsAsyncPageFilter),
            mvcOptions.Filters.Last().GetType());
    }

    [Fact]
    public void AddCordon_Duplicate_ReturnOK()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers().AddCordon().AddCordon();

        Assert.Contains(builder.Services, u => u.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(builder.Services, u => u.ServiceType == typeof(IStringLocalizer<>));
        Assert.Contains(builder.Services, u => u.ServiceType == typeof(IStringLocalizerFactory));
        Assert.Contains(builder.Services,
            u => u.ServiceType == typeof(IConfigureOptions<MvcDataAnnotationsLocalizationOptions>));
        using var app = builder.Build();

        var mvcOptions = app.Services.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.NotNull(mvcOptions.ModelValidatorProviders);
        Assert.Equal(3, mvcOptions.ModelValidatorProviders.Count);
        Assert.Equal(typeof(ValidationOptionsModelValidatorProvider),
            mvcOptions.ModelValidatorProviders.First().GetType());

        Assert.NotNull(mvcOptions.Filters);
        Assert.Equal(2, mvcOptions.Filters.Count);
        Assert.Equal(typeof(ValidationOptionsAsyncPageFilter),
            mvcOptions.Filters.Last().GetType());
    }
}