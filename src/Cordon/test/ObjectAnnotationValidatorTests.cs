// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ObjectAnnotationValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator();
        Assert.True(validator.ValidateAllProperties);
        Assert.Empty(validator.Items);
        Assert.Null(validator._serviceProvider);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Null(validator._errorMessageResourceAccessor());

        var validator2 = new ObjectAnnotationValidator(new Dictionary<object, object?> { { "id", 1 } });
        Assert.Null(validator2._serviceProvider);
        Assert.NotNull(validator2.Items);
        Assert.Single(validator2.Items);

        var services = new ServiceCollection();
        using var serviceProvider = services.BuildServiceProvider();

        var validator3 =
            new ObjectAnnotationValidator(serviceProvider, new Dictionary<object, object?> { { "id", 1 } });
        Assert.NotNull(validator3._serviceProvider);
        Assert.NotNull(validator3.Items);
        Assert.Single(validator3.Items);
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        var validator = new ObjectAnnotationValidator();
        Assert.Throws<ArgumentNullException>(() => validator.IsValid(null));
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator();
        Assert.False(validator.IsValid(new ObjectClassTest { Id = 1, Name = "Furion", Age = 10 }));
        Assert.False(validator.IsValid(new ObjectClassTest { Id = 3, Name = "Furion", Age = 10 }));
        Assert.False(validator.IsValid(new ObjectClassTest { Id = 3, Name = "OK", Age = 10 }));
        Assert.True(validator.IsValid(new ObjectClassTest { Id = 3, Name = "Furion", Age = 18 }));
        Assert.True(validator.IsValid(new ObjectClassTest { Id = 10, Name = "Furion", Age = 18 }));
        Assert.False(validator.IsValid(new ObjectClassTest { Id = 11, Name = "Furion", Age = 18 }));
    }

    [Fact]
    public void IsValid_WithValidateAllProperties_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator { ValidateAllProperties = false };
        Assert.False(validator.IsValid(new ObjectClassTest { Id = 1, Name = "Furion", Age = 10 }));
        Assert.False(validator.IsValid(new ObjectClassTest { Id = 3, Name = "Furion", Age = 10 }));
        Assert.False(validator.IsValid(new ObjectClassTest { Id = 3, Name = "OK", Age = 10 }));
        Assert.True(validator.IsValid(new ObjectClassTest { Id = 3, Name = "Furion", Age = 18 }));
        Assert.True(validator.IsValid(new ObjectClassTest { Id = 10, Name = "Furion", Age = 18 }));
        Assert.True(validator.IsValid(new ObjectClassTest { Id = 11, Name = "Furion", Age = 18 }));
    }

    [Fact]
    public void GetValidationResults_Invalid_Parameters()
    {
        var validator = new ObjectAnnotationValidator();
        Assert.Throws<ArgumentNullException>(() => validator.GetValidationResults(null, "data"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator();
        Assert.Null(validator.GetValidationResults(new ObjectClassTest { Id = 3, Name = "Furion", Age = 18 }, "data"));

        var validationResults =
            validator.GetValidationResults(new ObjectClassTest { Id = 1, Name = "OK", Age = 10 }, "data");
        Assert.NotNull(validationResults);
        Assert.Equal(2, validationResults.Count);
        Assert.Equal("The field Id must be between 3 and 10.", validationResults[0].ErrorMessage);
        Assert.Equal(
            "The field Name must be a string or collection type with a minimum length of '3' and maximum length of '10'.",
            validationResults[1].ErrorMessage);

        var validationResults2 =
            validator.GetValidationResults(new ObjectClassTest { Id = 3, Name = "Furion", Age = 10 }, "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("Age must be greater than 18", validationResults2[0].ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults3 =
            validator.GetValidationResults(new ObjectClassTest { Id = 1, Name = "Furion", Age = 18 }, "data");
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal("数据无效", validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void GetValidationResults_WithValidateAllProperties_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator { ValidateAllProperties = false };
        Assert.Null(validator.GetValidationResults(new ObjectClassTest { Id = 3, Name = "Furion", Age = 18 }, "data"));
        Assert.Null(validator.GetValidationResults(new ObjectClassTest { Id = 1, Name = "OK", Age = 18 }, "data"));

        var validationResults =
            validator.GetValidationResults(new ObjectClassTest { Id = 1, Name = "OK", Age = 10 }, "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("Age must be greater than 18", validationResults[0].ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults3 =
            validator.GetValidationResults(new ObjectClassTest { Id = 1, Name = "Furion", Age = 10 }, "data");
        Assert.NotNull(validationResults3);
        Assert.Equal(2, validationResults3.Count);
        Assert.Equal("数据无效", validationResults3.First().ErrorMessage);
    }

    [Fact]
    public void Validate_Invalid_Parameters()
    {
        var validator = new ObjectAnnotationValidator();
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null, "data"));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator();
        validator.Validate(new ObjectClassTest { Id = 3, Name = "Furion", Age = 18 }, "data");

        var exception = Assert.Throws<ValidationException>(() =>
            validator.Validate(new ObjectClassTest { Id = 1, Name = "OK", Age = 10 }, "data"));
        Assert.Equal("The field Id must be between 3 and 10.", exception.Message);
        Assert.True(exception.ValidationAttribute is RangeAttribute);
        Assert.Equal(1, exception.Value);
        Assert.Equal(["Id"], exception.ValidationResult.MemberNames);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() =>
            validator.Validate(new ObjectClassTest { Id = 1, Name = "OK", Age = 10 }, "data"));
        Assert.Equal("数据无效", exception2.Message);
        Assert.True(exception2.ValidationAttribute is RangeAttribute);
        Assert.Equal(1, exception2.Value);
        Assert.Empty(exception2.ValidationResult.MemberNames);
    }

    [Fact]
    public void Validate_WithValidateAllProperties_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator { ValidateAllProperties = false };
        validator.Validate(new ObjectClassTest { Id = 3, Name = "Furion", Age = 18 }, "data");
        validator.Validate(new ObjectClassTest { Id = 1, Name = "OK", Age = 18 }, "data");

        var exception = Assert.Throws<ValidationException>(() =>
            validator.Validate(new ObjectClassTest { Id = 1, Name = "OK", Age = 10 }, "data"));
        Assert.Equal("Age must be greater than 18", exception.Message);
        Assert.Null(exception.ValidationAttribute);
        Assert.True(exception.Value is ObjectClassTest);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() =>
            validator.Validate(new ObjectClassTest { Id = 1, Name = "Furion", Age = 10 }, "data"));
        Assert.Equal("数据无效", exception2.Message);
        Assert.Null(exception2.ValidationAttribute);
        Assert.True(exception2.Value is ObjectClassTest);
        Assert.Empty(exception2.ValidationResult.MemberNames);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator();
        Assert.Null(validator.FormatErrorMessage(null!));

        validator.ErrorMessage = "自定义错误信息";
        Assert.Equal("自定义错误信息", validator.FormatErrorMessage(null!));
    }

    [Fact]
    public void InitializeServiceProvider_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator();
        Assert.Null(validator._serviceProvider);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);

        Assert.NotNull(validator._serviceProvider);

        validator.InitializeServiceProvider(null);
        Assert.Null(validator._serviceProvider);
    }

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        var validator = new ObjectAnnotationValidator();
        var validationContext = validator.CreateValidationContext(new ObjectClassTest(), null);
        Assert.Equal("ObjectClassTest", validationContext.DisplayName);
        Assert.Null(validationContext.MemberName);

        var validationContext2 = validator.CreateValidationContext(new ObjectClassTest(), "DisplayName");
        Assert.Equal("DisplayName", validationContext2.DisplayName);
        Assert.Null(validationContext2.MemberName);

        var validationContext3 = validator.CreateValidationContext(new ObjectClassTest(), null);
        Assert.Equal("ObjectClassTest", validationContext3.DisplayName);
        Assert.Null(validationContext3.MemberName);

        var serviceProviderField = typeof(ValidationContext).GetField("_serviceProvider",
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(serviceProviderField);

        Assert.Null(validator._serviceProvider);
        Assert.Null(serviceProviderField.GetValue(validationContext3));

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        validator.InitializeServiceProvider(serviceProvider.GetService);
        Assert.NotNull(validator._serviceProvider);

        var validationContext4 = validator.CreateValidationContext(new ObjectClassTest(), null);
        Assert.NotNull(validator._serviceProvider);
        Assert.NotNull(serviceProviderField.GetValue(validationContext4));
    }
}

public class ObjectClassTest : IValidatableObject
{
    [Range(3, 10)] public int Id { get; set; }

    [Required] [Length(3, 10)] public string? Name { get; set; }

    public int Age { get; set; }

    // [Required] [ValidateNever] public string? Never { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Age < 18)
        {
            yield return new ValidationResult("Age must be greater than 18", [nameof(Age)]);
        }
    }
}