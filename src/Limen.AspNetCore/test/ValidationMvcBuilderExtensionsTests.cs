// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.AspNetCore.Tests;

public class ValidationMvcBuilderExtensionsTests
{
    [Fact]
    public void AddValidationOptions_ReturnOK()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers().AddValidationOptions();

        Assert.Contains(builder.Services, u => u.ServiceType == typeof(IValidationDataContext));
        using var app = builder.Build();

        var mvcOptions = app.Services.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.NotNull(mvcOptions.ModelValidatorProviders);
        Assert.Equal(3, mvcOptions.ModelValidatorProviders.Count);
        Assert.Equal(typeof(ValidationOptionsModelValidatorProvider),
            mvcOptions.ModelValidatorProviders.First().GetType());
    }

    [Fact]
    public void AddValidationOptions_Duplicate_ReturnOK()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers().AddValidationOptions().AddValidationOptions();

        Assert.Contains(builder.Services, u => u.ServiceType == typeof(IValidationDataContext));
        using var app = builder.Build();

        var mvcOptions = app.Services.GetRequiredService<IOptions<MvcOptions>>().Value;
        Assert.NotNull(mvcOptions.ModelValidatorProviders);
        Assert.Equal(3, mvcOptions.ModelValidatorProviders.Count);
        Assert.Equal(typeof(ValidationOptionsModelValidatorProvider),
            mvcOptions.ModelValidatorProviders.First().GetType());
    }
}