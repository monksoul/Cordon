// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class CompareValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new CompareValidator<ObjectModel>(null!, (string)null!));
        Assert.Throws<ArgumentException>(() => new CompareValidator<ObjectModel>(null!, string.Empty));
        Assert.Throws<ArgumentException>(() => new CompareValidator<ObjectModel>(null!, "  "));

        Assert.Throws<ArgumentNullException>(() =>
            new CompareValidator<ObjectModel>(null!, (Expression<Func<ObjectModel, object?>>)null!));

        Assert.Throws<ArgumentException>(() =>
            new CompareValidator<ObjectModel>(null!, "UnknownProperty"));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var model = new ObjectModel { Password = "password", ConfirmPassword = "password1" };

        var validator = new CompareValidator<ObjectModel>(u => u.Password, u => u.ConfirmPassword);
        Assert.NotNull(validator._propertyGetter);
        Assert.Equal("password", validator._propertyGetter(model));
        Assert.NotNull(validator._otherPropertyGetter);
        Assert.Equal("password1", validator._otherPropertyGetter(model));
        Assert.NotNull(validator.Property);
        Assert.Equal("Password", validator.Property.Name);
        Assert.NotNull(validator.OtherProperty);
        Assert.Equal("ConfirmPassword", validator.OtherProperty.Name);

        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("'{0}' and '{1}' do not match.", validator._errorMessageResourceAccessor());

        var validator2 =
            new CompareValidator<ObjectModel>(u => u.Password, nameof(ObjectModel.ConfirmPassword));
        Assert.NotNull(validator2._propertyGetter);
        Assert.Equal("password", validator2._propertyGetter(model));
        Assert.NotNull(validator2._otherPropertyGetter);
        Assert.Equal("password1", validator2._otherPropertyGetter(model));
        Assert.NotNull(validator2.Property);
        Assert.Equal("Password", validator2.Property.Name);
        Assert.NotNull(validator2.OtherProperty);
        Assert.Equal("ConfirmPassword", validator2.OtherProperty.Name);
    }

    [Fact]
    public void IsValid_Invalid_Parameters()
    {
        var validator = new CompareValidator<ObjectModel>(u => u.Password, u => u.ConfirmPassword);
        Assert.Throws<ArgumentNullException>(() => validator.IsValid(null));
    }

    [Theory]
    [InlineData(null, null, true)]
    [InlineData(null, "password1", false)]
    [InlineData("password", null, false)]
    [InlineData("password", "password1", false)]
    [InlineData("password", "password", true)]
    public void IsValid_ReturnOK(string? value1, string? value2, bool result)
    {
        var model = new ObjectModel { Password = value1, ConfirmPassword = value2 };

        var validator = new CompareValidator<ObjectModel>(u => u.Password, u => u.ConfirmPassword);
        Assert.Equal(result, validator.IsValid(model));
    }

    [Fact]
    public void GetValidationResults_Invalid_Parameters()
    {
        var validator = new CompareValidator<ObjectModel>(u => u.Password, u => u.ConfirmPassword);
        Assert.Throws<ArgumentNullException>(() => validator.GetValidationResults(null, "data"));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new CompareValidator<ObjectModel>(u => u.Password, u => u.ConfirmPassword);
        Assert.Null(
            validator.GetValidationResults(new ObjectModel { Password = "password", ConfirmPassword = "password" },
                "Password"));

        var validationResults =
            validator.GetValidationResults(new ObjectModel { Password = "password", ConfirmPassword = "password1" },
                "Password");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("'Password' and 'ConfirmPassword' do not match.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 =
            validator.GetValidationResults(new ObjectModel { Password = "password", ConfirmPassword = "password1" },
                "Password");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    }

    [Fact]
    public void Validate_Invalid_Parameters()
    {
        var validator = new CompareValidator<ObjectModel>(u => u.Password, u => u.ConfirmPassword);
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null, "data"));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator = new CompareValidator<ObjectModel>(u => u.Password, u => u.ConfirmPassword);
        validator.Validate(new ObjectModel { Password = "password", ConfirmPassword = "password" },
            "Password");

        var exception = Assert.Throws<ValidationException>(() =>
            validator.Validate(new ObjectModel { Password = "password", ConfirmPassword = "password1" }, "Password"));
        Assert.Equal("'Password' and 'ConfirmPassword' do not match.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() =>
            validator.Validate(new ObjectModel { Password = "password", ConfirmPassword = "password1" }, "Password"));
        Assert.Equal("数据无效", exception2.Message);
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new CompareValidator<ObjectModel>(u => u.Password, u => u.ConfirmPassword);
        Assert.Equal("'Password' and 'ConfirmPassword' do not match.", validator.FormatErrorMessage("Password"));
    }

    [Fact]
    public void GetDisplayNameForProperty_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() =>
            CompareValidator<ObjectModel>.GetDisplayNameForProperty(null!));

    [Fact]
    public void GetDisplayNameForProperty_ReturnOK()
    {
        var property = typeof(ObjectModel).GetProperty(nameof(ObjectModel.Password));
        Assert.NotNull(property);
        Assert.Equal("Password", CompareValidator<ObjectModel>.GetDisplayNameForProperty(property));

        var property2 = typeof(ObjectModel).GetProperty(nameof(ObjectModel.NewPassword));
        Assert.NotNull(property2);
        Assert.Equal("NPassword", CompareValidator<ObjectModel>.GetDisplayNameForProperty(property2));
    }

    [Fact]
    public void CreatePropertySelector_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CompareValidator<ObjectModel>.CreatePropertySelector(null!));

        Assert.Throws<ArgumentException>(() =>
            CompareValidator<ObjectModel>.CreatePropertySelector(string.Empty));

        Assert.Throws<ArgumentException>(() =>
            CompareValidator<ObjectModel>.CreatePropertySelector(" "));
    }

    [Fact]
    public void CreatePropertySelector_ReturnOK()
    {
        var selector = CompareValidator<ObjectModel>.CreatePropertySelector("Password");
        Assert.NotNull(selector);
        var getter = selector.Compile();
        Assert.Equal("password", getter(new ObjectModel { Password = "password" }));
    }

    public class ObjectModel
    {
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        [Display(Name = "NPassword")] public string? NewPassword { get; set; }
    }
}