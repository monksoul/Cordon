var builder = WebApplication.CreateBuilder(args);

// .NET 10 后需显式注册验证服务：https://learn.microsoft.com/zh-cn/aspnet/core/mvc/models/validation?view=aspnetcore-10.0#validation-in-net-10
// 必须在控制器之前注册
builder.Services.AddValidation();

builder.Services.AddControllers(options =>
{
    // options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
}).AddValidationOptions();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapPost("/miniapi", (Custom custom) => { });

app.MapControllers();

app.Run();