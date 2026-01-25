// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class AttributePropertyValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new AttributePropertyValidator<PropertyClassTest>(null!));

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        Assert.NotNull(validator.Property);
        Assert.Equal("Name", validator.Property.Name);

        Assert.NotNull(validator._getter);
        Assert.Equal("Furion", validator._getter(new PropertyClassTest { Name = "Furion" }));

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Null(validator._errorMessageResourceAccessor());

        var validator2 = new AttributePropertyValidator<PropertyClassTest, string?>(u => u.Name);
        Assert.NotNull(validator2.Property);
        Assert.Equal("Name", validator2.Property.Name);
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        Assert.Throws<ArgumentNullException>(() => validator.IsValid(null));
        Assert.Throws<InvalidCastException>(() => validator.IsValid(new { Name = "Furion" }));
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        Assert.False(validator.IsValid(new PropertyClassTest { Name = null }));
        Assert.False(validator.IsValid(new PropertyClassTest { Name = "OK" }));
        Assert.True(validator.IsValid(new PropertyClassTest { Name = "Furion" }));
        Assert.False(validator.IsValid(new PropertyClassTest { Name = "dotnetchina" }));
    }

    [Fact]
    public void GetValidationResults_Invalid_Parameters()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        Assert.Throws<ArgumentNullException>(() => validator.GetValidationResults(null, "data"));
        Assert.Throws<InvalidCastException>(() => validator.GetValidationResults(new { Name = "Furion" }, "data"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        Assert.Null(validator.GetValidationResults(new PropertyClassTest { Name = "Furion" }, "data"));

        var validationResults = validator.GetValidationResults(new PropertyClassTest { Name = null }, "Name");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The Name field is required.", validationResults.First().ErrorMessage);

        var validationResults2 = validator.GetValidationResults(new PropertyClassTest { Name = "OK" }, "Name");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal(
            "The field Name must be a string or collection type with a minimum length of '3' and maximum length of '10'.",
            validationResults2.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults3 = validator.GetValidationResults(new PropertyClassTest { Name = "OK" }, "Name");
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal("数据无效", validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void Validate_Invalid_Parameters()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null, "data"));
        Assert.Throws<InvalidCastException>(() => validator.Validate(new { Name = "Furion" }, "data"));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        validator.Validate(new PropertyClassTest { Name = "Furion" }, "data");

        var exception =
            Assert.Throws<ValidationException>(() => validator.Validate(new PropertyClassTest { Name = null }, "Name"));
        Assert.Equal("The Name field is required.", exception.Message);
        Assert.True(exception.ValidationAttribute is RequiredAttribute);
        Assert.Null(exception.Value);

        var exception2 =
            Assert.Throws<ValidationException>(() => validator.Validate(new PropertyClassTest { Name = "OK" }, "Name"));
        Assert.Equal(
            "The field Name must be a string or collection type with a minimum length of '3' and maximum length of '10'.",
            exception2.Message);
        Assert.True(exception2.ValidationAttribute is LengthAttribute);
        Assert.Equal("OK", exception2.Value);

        validator.ErrorMessage = "数据无效";
        var exception3 =
            Assert.Throws<ValidationException>(() => validator.Validate(new PropertyClassTest { Name = "OK" }, "Name"));
        Assert.Equal("数据无效", exception3.Message);
        Assert.True(exception3.ValidationAttribute is LengthAttribute);
        Assert.Equal("OK", exception3.Value);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        Assert.Null(validator.FormatErrorMessage(null!));

        validator.ErrorMessage = "自定义错误信息";
        Assert.Equal("自定义错误信息", validator.FormatErrorMessage(null!));
    }

    [Fact]
    public void GetValue_Invalid_Parameters()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        Assert.Throws<ArgumentNullException>(() => validator.GetValue(null!));
    }

    [Fact]
    public void GetValue_ReturnOK()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);
        Assert.Null(validator.GetValue(new PropertyClassTest { Name = null }));
        Assert.Equal("Furion", validator.GetValue(new PropertyClassTest { Name = "Furion" }));
        Assert.Equal("OK", validator.GetValue(new PropertyClassTest { Name = "OK" }));

        var validator2 = new AttributePropertyValidator<PropertyClassTest, string?>(u => u.Name);
        Assert.Equal("Furion", validator2.GetValue(new PropertyClassTest { Name = "Furion" }));
    }

    [Fact]
    public void ConvertExpression_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            AttributePropertyValidator<PropertyClassTest, string?>.ConvertExpression(null!));

    [Fact]
    public void ConvertExpression_ReturnOK()
    {
        var expression = AttributePropertyValidator<PropertyClassTest, string?>.ConvertExpression(u => u.Name);
        Assert.NotNull(expression);

        var getter = expression.Compile();
        Assert.Equal("Furion", getter(new PropertyClassTest { Name = "Furion" }));
    }

    [Fact]
    public void GetDisplayName_ReturnOK()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest2>(u => u.Name);
        Assert.Equal("Name", validator.GetDisplayName(null));

        var validator2 = new AttributePropertyValidator<PropertyClassTest2>(u => u.Name1);
        Assert.Equal("名称", validator2.GetDisplayName(null));

        var validator3 = new AttributePropertyValidator<PropertyClassTest2>(u => u.Name2);
        Assert.Equal("名称", validator3.GetDisplayName(null));

        var validator4 = new AttributePropertyValidator<PropertyClassTest2>(u => u.Name2);
        Assert.Equal("新名称", validator4.GetDisplayName("新名称"));
    }

    [Fact]
    public void GetMemberName_ReturnOK()
    {
        var validator = new AttributePropertyValidator<PropertyClassTest2>(u => u.Name);
        Assert.Equal("Name", validator.GetMemberName());
    }

    [Fact]
    public void CreateValidationContext_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            new AttributePropertyValidator<PropertyClassTest>(u => u.Name).CreateValidationContext(null!, null,
                new ValidationContext<PropertyClassTest>(new PropertyClassTest())));

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        var model = new PropertyClassTest();
        var validator = new AttributePropertyValidator<PropertyClassTest>(u => u.Name);

        var validationContext =
            validator.CreateValidationContext(model, null, new ValidationContext<PropertyClassTest>(model));
        Assert.NotNull(validationContext);
        Assert.Empty(validationContext.Items);
        Assert.Null(validationContext.GetService<IServiceProvider>());
        Assert.Equal("Name", validationContext.MemberName);
        Assert.Equal("Name", validationContext.DisplayName);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var validationContext2 = validator.CreateValidationContext(model, "CustomDisplayName",
            new ValidationContext<PropertyClassTest>(model, serviceProvider, null)
            {
                RuleSets = ["login"], MemberNames = ["Model"], DisplayName = "ModelDisplay"
            });
        Assert.NotNull(validationContext2);
        Assert.Single(validationContext2.Items);
        var metadata =
            validationContext2.Items[Constants.ValidationOptionsKey] as ValidationOptionsMetadata;
        Assert.NotNull(metadata);
        Assert.Equal(["login"], (string[]?)metadata.RuleSets!);

        Assert.NotNull(validationContext2.GetService<IServiceProvider>());
        Assert.Equal("Name", validationContext2.MemberName);
        Assert.Equal("CustomDisplayName", validationContext2.DisplayName);
    }
}

public class PropertyClassTest
{
    [Required] [Length(3, 10)] public string? Name { get; set; }
}

public class PropertyClassTest2
{
    public string? Name { get; set; }

    [DisplayName("名称")] public string? Name1 { get; set; }

    [Display(Name = "名称")]
    [DisplayName("名称2")]
    public string? Name2 { get; set; }
}