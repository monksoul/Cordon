// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Tests;

public class ObjectValidatorEnumerableExtensionsTests
{
    [Fact]
    public void IsValid_ReturnOK()
    {
        List<IObjectValidator<VModel>> validators = [new VModelValidator1(), new VModelValidator2()];

        Assert.False(validators.IsValid(new VModel()));
        Assert.False(validators.IsValid(new VModel { Id = 1 }));
        Assert.True(validators.IsValid(new VModel { Id = 1, Name = "Furion" }));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        List<IObjectValidator<VModel>> validators = [new VModelValidator1(), new VModelValidator2()];

        var validationResult = validators.GetValidationResults(new VModel());
        Assert.NotNull(validationResult);
        Assert.Equal(2, validationResult.Count);
        Assert.Equal(["The field Id must be greater than or equal to '1'.", "The Name field is required."],
            validationResult.Select(u => u.ErrorMessage));

        var validationResult2 = validators.GetValidationResults(new VModel { Id = 1 });
        Assert.NotNull(validationResult2);
        Assert.Single(validationResult2);
        Assert.Equal("The Name field is required.", validationResult2[0].ErrorMessage);

        Assert.Null(validators.GetValidationResults(new VModel { Id = 1, Name = "Furion" }));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        List<IObjectValidator<VModel>> validators = [new VModelValidator1(), new VModelValidator2()];

        var exception = Assert.Throws<ValidationException>(() => validators.Validate(new VModel()));
        Assert.Equal("The field Id must be greater than or equal to '1'.", exception.Message);

        var exception2 = Assert.Throws<ValidationException>(() => validators.Validate(new VModel { Id = 1 }));
        Assert.Equal("The Name field is required.", exception2.Message);

        validators.Validate(new VModel { Id = 1, Name = "Furion" });
    }

    public class VModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class VModelValidator1 : AbstractValidator<VModel>
    {
        public VModelValidator1() => RuleFor(u => u.Id).Min(1);
    }


    public class VModelValidator2 : AbstractValidator<VModel>
    {
        public VModelValidator2() => RuleFor(u => u.Name).Required().MinLength(3);
    }
}