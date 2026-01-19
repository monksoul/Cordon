// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class ValidationBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var builder = new ValidationBuilder();
        Assert.Null(builder._validatorTypes);
        Assert.Equal(typeof(AbstractValidator<>), ValidationBuilder.AbstractValidatorDefinition);
        Assert.Equal(typeof(AbstractValueValidator<>), ValidationBuilder.AbstractValueValidatorDefinition);
    }

    [Fact]
    public void AddValidator_Invalid_Parameters()
    {
        var builder = new ValidationBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.AddValidator(null!));

        var exception = Assert.Throws<ArgumentException>(() => builder.AddValidator(typeof(ObjectModelValidator5)));
        Assert.Equal(
            "Type `Cordon.Tests.ObjectModelValidator5` must be a non-abstract, non-static class to be registered as a validator. (Parameter 'validatorType')",
            exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => builder.AddValidator(typeof(ObjectModel)));
        Assert.Equal(
            "Type `Cordon.Tests.ObjectModel` is not a valid validator; it does not derive from `AbstractValidator<>` or `AbstractValueValidator<>`. (Parameter 'validatorType')",
            exception2.Message);
    }

    [Fact]
    public void AddValidator_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddValidator(typeof(ObjectModelValidator1));

        Assert.NotNull(builder._validatorTypes);
        Assert.Single(builder._validatorTypes);
        Assert.Equal(typeof(ObjectModel), builder._validatorTypes[typeof(ObjectModelValidator1)]);

        builder.AddValidator(typeof(ObjectModelValidator1));

        Assert.NotNull(builder._validatorTypes);
        Assert.Single(builder._validatorTypes);
        Assert.Equal(typeof(ObjectModel), builder._validatorTypes[typeof(ObjectModelValidator1)]);

        builder.AddValidator(typeof(ObjectModelValidator2));

        Assert.NotNull(builder._validatorTypes);
        Assert.Equal(2, builder._validatorTypes.Count);
        Assert.Equal(typeof(ObjectModel), builder._validatorTypes[typeof(ObjectModelValidator2)]);

        builder.AddValidator(typeof(StringTestValidator));

        Assert.NotNull(builder._validatorTypes);
        Assert.Equal(3, builder._validatorTypes.Count);
        Assert.Equal(typeof(string), builder._validatorTypes[typeof(StringTestValidator)]);
    }

    [Fact]
    public void AddValidator_WithGeneric_Invalid_Parameters()
    {
        var builder = new ValidationBuilder();

        var exception = Assert.Throws<ArgumentException>(() => builder.AddValidator<ObjectModelValidator5>());
        Assert.Equal(
            "Type `Cordon.Tests.ObjectModelValidator5` must be a non-abstract, non-static class to be registered as a validator. (Parameter 'validatorType')",
            exception.Message);
    }

    [Fact]
    public void AddValidator_WithGeneric_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddValidator<ObjectModelValidator1>();

        Assert.NotNull(builder._validatorTypes);
        Assert.Single(builder._validatorTypes);
        Assert.Equal(typeof(ObjectModel), builder._validatorTypes[typeof(ObjectModelValidator1)]);

        builder.AddValidator<ObjectModelValidator1>();

        Assert.NotNull(builder._validatorTypes);
        Assert.Single(builder._validatorTypes);
        Assert.Equal(typeof(ObjectModel), builder._validatorTypes[typeof(ObjectModelValidator1)]);

        builder.AddValidator<ObjectModelValidator2>();

        Assert.NotNull(builder._validatorTypes);
        Assert.Equal(2, builder._validatorTypes.Count);
        Assert.Equal(typeof(ObjectModel), builder._validatorTypes[typeof(ObjectModelValidator2)]);
    }

    [Fact]
    public void AddValidators_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationBuilder().AddValidators(null!));

    [Fact]
    public void AddValidators_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddValidators(typeof(ObjectModelValidator1), typeof(ObjectModelValidator2));
        Assert.NotNull(builder._validatorTypes);
        Assert.Equal(2, builder._validatorTypes.Count);
        Assert.Equal(typeof(ObjectModel), builder._validatorTypes[typeof(ObjectModelValidator1)]);
        Assert.Equal(typeof(ObjectModel), builder._validatorTypes[typeof(ObjectModelValidator2)]);
    }

    [Fact]
    public void AddValidatorsFromAssemblies_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationBuilder().AddValidatorsFromAssemblies(null!));

    [Fact]
    public void AddValidatorsFromAssemblies_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddValidatorsFromAssemblies(typeof(ObjectModel).Assembly);

        Assert.NotNull(builder._validatorTypes);
        Assert.Equal(16, builder._validatorTypes.Count);
    }

    [Fact]
    public void ConfigureValidationMessages_Invalid_Parameters()
    {
        var builder = new ValidationBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.ConfigureValidationMessages(null!));
    }

    [Fact]
    public void ConfigureValidationMessages_ReturnOK()
    {
        var builder = new ValidationBuilder();
        Assert.Empty(ValidationMessageProvider._overrides);
        builder.ConfigureValidationMessages(message =>
        {
            message["AgeValidator_ValidationError"] = "字段 {0} 不是有效的年龄。";
            message["BankCardValidator_ValidationError"] = "字段 {0} 不是有效的银行卡号。";
        });

        Assert.Equal(2, ValidationMessageProvider._overrides.Count);
        Assert.Equal("字段 {0} 不是有效的年龄。", ValidationMessageProvider._overrides["AgeValidator_ValidationError"]);
        Assert.Equal("字段 {0} 不是有效的银行卡号。", ValidationMessageProvider._overrides["BankCardValidator_ValidationError"]);

        // 清除单元测试影响
        ValidationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void UseChineseValidationMessages_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.UseChineseValidationMessages();
        Assert.Equal(68, ValidationMessageProvider._overrides.Count);

        // 清除单元测试影响
        ValidationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void ConfigureDataAnnotationValidationMessages_Invalid_Parameters()
    {
        var builder = new ValidationBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.ConfigureDataAnnotationValidationMessages(null!));
    }

    [Fact]
    public void ConfigureDataAnnotationValidationMessages_ReturnOK()
    {
        var builder = new ValidationBuilder();
        Assert.Empty(DataAnnotationMessageProvider._overrides);
        builder.ConfigureDataAnnotationValidationMessages(message =>
        {
            message["RequiredAttribute_ValidationError"] = "字段 {0} 是必填项。";
            message["StringLengthAttribute_ValidationError"] = "字段 {0} 必须是字符串，且最大长度为 {1}。";
        });

        Assert.Equal(2, DataAnnotationMessageProvider._overrides.Count);
        Assert.Equal("字段 {0} 是必填项。", DataAnnotationMessageProvider._overrides["RequiredAttribute_ValidationError"]);
        Assert.Equal("字段 {0} 必须是字符串，且最大长度为 {1}。",
            DataAnnotationMessageProvider._overrides["StringLengthAttribute_ValidationError"]);

        // 清除单元测试影响
        DataAnnotationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void UseChineseDataAnnotationMessages_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.UseChineseDataAnnotationMessages();
        Assert.Equal(24, DataAnnotationMessageProvider._overrides.Count);

        // 清除单元测试影响
        DataAnnotationMessageProvider.ClearOverrides();
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddValidator(typeof(ObjectModelValidator1));

        var services = new ServiceCollection();
        builder.Build(services);

        Assert.Equal(5, services.Count);
        Assert.Contains(services, u => u.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.Contains(services, u => u.ServiceType == typeof(ObjectModelValidator1));
        Assert.Contains(services, u => u.ServiceType == typeof(IValidationService));
        Assert.DoesNotContain(services, u => u.ServiceType == typeof(AbstractValidator<ObjectModel>));

        // 支持重复构建
        builder.Build(services);
        Assert.Equal(5, services.Count);
        Assert.Contains(services, u => u.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.Contains(services, u => u.ServiceType == typeof(ObjectModelValidator1));
        Assert.Contains(services, u => u.ServiceType == typeof(IValidationService));
        Assert.DoesNotContain(services, u => u.ServiceType == typeof(AbstractValidator<ObjectModel>));
    }

    [Fact]
    public void BuildValidatorServices_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationBuilder().BuildValidatorServices(null!));

    [Fact]
    public void BuildValidatorServices_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddValidator(typeof(ObjectModelValidator1));

        var services = new ServiceCollection();
        builder.BuildValidatorServices(services);

        Assert.Equal(3, services.Count);
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.Contains(services, u => u.ServiceType == typeof(ObjectModelValidator1));
        Assert.DoesNotContain(services, u => u.ServiceType == typeof(AbstractValidator<ObjectModel>));

        // 仅在单元测试重复注册
        builder.BuildValidatorServices(services);
        Assert.Equal(6, services.Count);
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.Contains(services, u => u.ServiceType == typeof(ObjectModelValidator1));
        Assert.DoesNotContain(services, u => u.ServiceType == typeof(AbstractValidator<ObjectModel>));
    }

    [Fact]
    public void CreateValidator_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationBuilder.CreateValidator(null!, null!));
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => ValidationBuilder.CreateValidator(serviceProvider, null!));
    }

    [Fact]
    public void CreateValidator_ReturnOK()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var validatorObject = ValidationBuilder.CreateValidator(serviceProvider, typeof(ObjectModelValidator1));
        Assert.NotNull(validatorObject);

        var validator = validatorObject as ObjectModelValidator1;
        Assert.NotNull(validator);
        Assert.NotNull(validator._serviceProvider);
    }

    [Fact]
    public void TryGetValidatedType_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationBuilder.TryGetValidatedType(null!, null!, out _));
        Assert.Throws<ArgumentNullException>(() =>
            ValidationBuilder.TryGetValidatedType(typeof(ObjectModelValidator1), null!, out _));

        var exception = Assert.Throws<ArgumentException>(() =>
            ValidationBuilder.TryGetValidatedType(typeof(ObjectModelValidator1), typeof(IObjectValidator), out _));
        Assert.Equal(
            "The type 'Cordon.IObjectValidator' is not a generic type definition; expected an open generic such as `AbstractValidator<>` or `AbstractValueValidator<>`. (Parameter 'genericTypeDefinition')",
            exception.Message);
    }

    [Theory]
    [InlineData(typeof(ObjectModel), false, null)]
    [InlineData(typeof(ObjectModelValidator1), true, typeof(ObjectModel))]
    [InlineData(typeof(ObjectModelValidator2), true, typeof(ObjectModel))]
    [InlineData(typeof(ObjectModelValidator3), true, typeof(ObjectModel))]
    [InlineData(typeof(ObjectModelValidator4), true, typeof(ObjectModel))]
    [InlineData(typeof(AbstractValidator<ObjectModel>), false, null)]
    [InlineData(typeof(ObjectModelValidator5), false, null)]
    [InlineData(typeof(ObjectModelValidator6), true, typeof(ObjectModel))]
    [InlineData(typeof(object), false, null)]
    public void TryGetValidatedType_ReturnOK(Type type, bool valid, Type? modelType)
    {
        Assert.Equal(valid,
            ValidationBuilder.TryGetValidatedType(type, typeof(AbstractValidator<>), out var modelType1));
        Assert.Equal(modelType, modelType1);
    }
}

public class ObjectModel;

public class ObjectModelValidator1 : AbstractValidator<ObjectModel>;

public class ObjectModelValidator2 : ObjectModelValidator1;

internal class ObjectModelValidator3 : ObjectModelValidator2;

public class ObjectModelValidator4(object arg) : AbstractValidator<ObjectModel>
{
    public object Arg { get; set; } = arg;
}

public abstract class ObjectModelValidator5 : AbstractValidator<ObjectModel>;

public class ObjectModelValidator6 : ObjectModelValidator5;

public class StringTestValidator : AbstractValidator<string>;