// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class AttributeObjectValidatorTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var validator = new AttributeObjectValidator();
        Assert.True(validator.ValidateAllProperties);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Null(validator._errorMessageResourceAccessor());
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        var validator = new AttributeObjectValidator();
        Assert.Throws<ArgumentNullException>(() => validator.IsValid(null));
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        var validator = new AttributeObjectValidator();
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
        var validator = new AttributeObjectValidator { ValidateAllProperties = false };
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
        var validator = new AttributeObjectValidator();
        Assert.Throws<ArgumentNullException>(() => validator.GetValidationResults(null, "data"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new AttributeObjectValidator();
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
        var validator = new AttributeObjectValidator { ValidateAllProperties = false };
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
        var validator = new AttributeObjectValidator();
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null, "data"));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new AttributeObjectValidator();
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
        var validator = new AttributeObjectValidator { ValidateAllProperties = false };
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
        var validator = new AttributeObjectValidator();
        Assert.Null(validator.FormatErrorMessage(null!));

        validator.ErrorMessage = "自定义错误信息";
        Assert.Equal("自定义错误信息", validator.FormatErrorMessage(null!));
    }

    [Fact]
    public void CreateValidationContext_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => AttributeObjectValidator.CreateValidationContext(null!, null));

    [Fact]
    public void CreateValidationContext_ReturnOK()
    {
        var model = new ObjectClassTest();
        var validationContext = AttributeObjectValidator.CreateValidationContext(model, null);
        Assert.NotNull(validationContext);
        Assert.Empty(validationContext.Items);
        Assert.Null(validationContext.GetService<IServiceProvider>());
        Assert.Null(validationContext.MemberName);
        Assert.Equal("ObjectClassTest", validationContext.DisplayName);

        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var validationContext2 = AttributeObjectValidator.CreateValidationContext(model,
            new ValidationContext<ObjectClassTest>(model, serviceProvider, null)
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
        Assert.Equal("Model", validationContext2.MemberName);
        Assert.Equal("ModelDisplay", validationContext2.DisplayName);
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