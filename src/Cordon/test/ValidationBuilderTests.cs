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
            "Type `Cordon.Tests.ObjectModel` is not a valid validator; it does not derive from `AbstractValidator<T>`. (Parameter 'validatorType')",
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
    public void AddValidatorFromAssemblies_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationBuilder().AddValidatorFromAssemblies(null!));

    [Fact]
    public void AddValidatorFromAssemblies_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddValidatorFromAssemblies(typeof(ObjectModel).Assembly);

        Assert.NotNull(builder._validatorTypes);
        Assert.Equal(11, builder._validatorTypes.Count);
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddValidator(typeof(ObjectModelValidator1));

        var services = new ServiceCollection();
        builder.Build(services);

        Assert.Equal(3, services.Count);
        Assert.Contains(services, u => u.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.DoesNotContain(services, u => u.ServiceType == typeof(AbstractValidator<ObjectModel>));

        // 支持重复构建
        builder.Build(services);
        Assert.Equal(3, services.Count);
        Assert.Contains(services, u => u.ServiceType == typeof(IValidationDataContext));
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.DoesNotContain(services, u => u.ServiceType == typeof(AbstractValidator<ObjectModel>));
    }

    [Fact]
    public void BuildObjectValidatorServices_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => new ValidationBuilder().BuildObjectValidatorServices(null!));

    [Fact]
    public void BuildObjectValidatorServices_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddValidator(typeof(ObjectModelValidator1));

        var services = new ServiceCollection();
        builder.BuildObjectValidatorServices(services);

        Assert.Equal(2, services.Count);
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.DoesNotContain(services, u => u.ServiceType == typeof(AbstractValidator<ObjectModel>));

        // 仅在单元测试重复注册
        builder.BuildObjectValidatorServices(services);
        Assert.Equal(4, services.Count);
        Assert.Contains(services, u => u.ServiceType == typeof(IObjectValidator<ObjectModel>));
        Assert.DoesNotContain(services, u => u.ServiceType == typeof(AbstractValidator<ObjectModel>));
    }

    [Fact]
    public void CreateObjectValidator_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationBuilder.CreateObjectValidator(null!, null!));
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        Assert.Throws<ArgumentNullException>(() => ValidationBuilder.CreateObjectValidator(serviceProvider, null!));
    }

    [Fact]
    public void CreateObjectValidator_ReturnOK()
    {
        using var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var validatorObject = ValidationBuilder.CreateObjectValidator(serviceProvider, typeof(ObjectModelValidator1));
        Assert.NotNull(validatorObject);

        var validator = validatorObject as ObjectModelValidator1;
        Assert.NotNull(validator);
        Assert.NotNull(validator._serviceProvider);
    }

    [Fact]
    public void TryGetValidatedType_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => ValidationBuilder.TryGetValidatedType(null!, out _));

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
        Assert.Equal(valid, ValidationBuilder.TryGetValidatedType(type, out var modelType1));
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