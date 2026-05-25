// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class SensitiveWordAttributeTests
{
    [Fact]
    public void Attribute_Metadata()
    {
        var attributeType = typeof(SensitiveWordAttribute);
        Assert.True(typeof(ValidationAttribute).IsAssignableFrom(attributeType));

        var attributeUsageAttribute = attributeType.GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attributeUsageAttribute);
        Assert.Equal(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
            attributeUsageAttribute.ValidOn);
        Assert.False(attributeUsageAttribute.AllowMultiple);
        Assert.True(attributeUsageAttribute.Inherited);
    }

    [Fact]
    public void New_ReturnOK()
    {
        var attribute = new SensitiveWordAttribute();
        Assert.Null(attribute.DictionaryName);
        Assert.Null(attribute.FilePath);
        Assert.False(attribute.ShowMatchedWords);
        Assert.Null(attribute.ErrorMessage);
        Assert.NotNull(attribute._validator);
        Assert.Null(attribute._validator.DictionaryName);
        Assert.Null(attribute._validator.FilePath);
        Assert.False(attribute._validator.ShowMatchedWords);

        var attribute2 = new SensitiveWordAttribute
            { DictionaryName = "attribute", FilePath = "sensitive_words.txt", ShowMatchedWords = true };
        Assert.Equal("attribute", attribute2.DictionaryName);
        Assert.Equal("sensitive_words.txt", attribute2.FilePath);
        Assert.True(attribute2.ShowMatchedWords);
        Assert.Null(attribute2.ErrorMessage);
        Assert.NotNull(attribute2._validator);
        Assert.Equal("attribute", attribute2._validator.DictionaryName);
        Assert.Equal("sensitive_words.txt", attribute2._validator.FilePath);
        Assert.True(attribute2._validator.ShowMatchedWords);
    }

    [Fact]
    public void IsValid_ReturnOK()
    {
        SensitiveWordSanitizerFactory.GetOrCreateFromPath(SensitiveWordOptions.DefaultDictionaryName,
            "sensitive_words.txt");
        var model = new TestModel { Data = "这是一段正常的文字", Data2 = "这是一段正常的文字" };
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), null, true));

        var model2 = new TestModel { Data = "这里包含敏感词吗", Data2 = "这是一段正常的文字" };
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), null, true));

        var model3 = new TestModel { Data = "这是一段正常的文字", Data2 = "这里包含敏感词吗" };
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), null, true));

        var model4 = new TestModel { Data = "这里包含敏感词吗", Data2 = "你大爷的，TMD!" };
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), null, true));
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(SensitiveWordOptions.DefaultDictionaryName));
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        SensitiveWordSanitizerFactory.GetOrCreateFromPath(SensitiveWordOptions.DefaultDictionaryName,
            "sensitive_words.txt");
        var model = new TestModel { Data = "这是一段正常的文字", Data2 = "这是一段正常的文字" };
        var validationResults = new List<ValidationResult>();
        Assert.True(Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true));
        Assert.Empty(validationResults);

        var model2 = new TestModel { Data = "这里包含敏感词吗", Data2 = "这是一段正常的文字" };
        var validationResults2 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model2, new ValidationContext(model2), validationResults2, true));
        Assert.Single(validationResults2);
        Assert.Equal("The field Data contains sensitive or prohibited words.", validationResults2[0].ErrorMessage);

        var model3 = new TestModel { Data = "这是一段正常的文字", Data2 = "这里包含敏感词吗" };
        var validationResults3 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model3, new ValidationContext(model3), validationResults3, true));
        Assert.Single(validationResults3);
        Assert.Equal("The field Data2 contains sensitive or prohibited words: [敏感词] @ 4..7.",
            validationResults3[0].ErrorMessage);

        var model4 = new TestModel { Data = "这里包含敏感词吗", Data2 = "你大爷的，TMD!" };
        var validationResults4 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model4, new ValidationContext(model4), validationResults4, true));
        Assert.Equal(2, validationResults4.Count);
        Assert.Equal("The field Data contains sensitive or prohibited words.", validationResults4[0].ErrorMessage);
        Assert.Equal("The field Data2 contains sensitive or prohibited words: [你-大-爷] @ 0..3, [TMD!] @ 5..8.",
            validationResults4[1].ErrorMessage);

        var model5 = new TestModel { Data = "这里包含敏感词吗", Data2 = "你大爷的，TMD!", Data3 = "你是八嘎耶鲁吗" };
        var validationResults5 = new List<ValidationResult>();
        Assert.False(Validator.TryValidateObject(model5, new ValidationContext(model5), validationResults5, true));
        Assert.Equal(3, validationResults5.Count);
        Assert.Equal("The field Data contains sensitive or prohibited words.", validationResults5[0].ErrorMessage);
        Assert.Equal("The field Data2 contains sensitive or prohibited words: [你-大-爷] @ 0..3, [TMD!] @ 5..8.",
            validationResults5[1].ErrorMessage);
        Assert.Equal("数据无效", validationResults5[2].ErrorMessage);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(SensitiveWordOptions.DefaultDictionaryName));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        SensitiveWordSanitizerFactory.GetOrCreateFromPath(SensitiveWordOptions.DefaultDictionaryName,
            "sensitive_words.txt");
        var model = new TestModel { Data = "这是一段正常的文字", Data2 = "这是一段正常的文字" };
        Validator.ValidateObject(model, new ValidationContext(model), true);

        var model2 = new TestModel { Data = "这里包含敏感词吗", Data2 = "这是一段正常的文字" };
        var exception =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model2, new ValidationContext(model2), true));
        Assert.Equal("The field Data contains sensitive or prohibited words.", exception.ValidationResult.ErrorMessage);

        var model3 = new TestModel { Data = "这是一段正常的文字", Data2 = "这里包含敏感词吗" };
        var exception2 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model3, new ValidationContext(model3), true));
        Assert.Equal("The field Data2 contains sensitive or prohibited words: [敏感词] @ 4..7.",
            exception2.ValidationResult.ErrorMessage);

        var model4 = new TestModel { Data = "这里包含敏感词吗", Data2 = "你大爷的，TMD!" };
        var exception3 =
            Assert.Throws<ValidationException>(() =>
                Validator.ValidateObject(model4, new ValidationContext(model4), true));
        Assert.Equal("The field Data contains sensitive or prohibited words.",
            exception3.ValidationResult.ErrorMessage);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(SensitiveWordOptions.DefaultDictionaryName));
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var attribute = new SensitiveWordAttribute();
        Assert.Equal("The field data contains sensitive or prohibited words.", attribute.FormatErrorMessage("data"));

        var attribute2 = new SensitiveWordAttribute { ShowMatchedWords = true };
        Assert.Equal("The field data contains sensitive or prohibited words: .", attribute2.FormatErrorMessage("data"));

        var attribute3 = new SensitiveWordAttribute { ShowMatchedWords = true };
        SensitiveWordValidator._lastMatchDetails.Value = ["[敏感词] @ 5..10", "[TMD!] @ 5..8"];
        Assert.Equal("The field data contains sensitive or prohibited words: [敏感词] @ 5..10, [TMD!] @ 5..8.",
            attribute3.FormatErrorMessage("data"));
        SensitiveWordValidator._lastMatchDetails.Value = null;
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var attribute = new SensitiveWordAttribute();
        Assert.Equal("SensitiveWordValidator_ValidationError", attribute.GetResourceKey());

        var attribute2 = new SensitiveWordAttribute { ShowMatchedWords = true };
        Assert.Equal("SensitiveWordValidator_ValidationError_ShowMatchedWords", attribute2.GetResourceKey());
    }

    public class TestModel
    {
        [SensitiveWord] public string? Data { get; set; }

        [SensitiveWord(ShowMatchedWords = true)]
        public string? Data2 { get; set; }

        [SensitiveWord(ErrorMessage = "数据无效")] public string? Data3 { get; set; }
    }
}