// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class SensitiveWordValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new SensitiveWordValidator(null!));
        Assert.Throws<ArgumentNullException>(() => new SensitiveWordValidator((Stream)null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new SensitiveWordValidator();
        Assert.Null(validator.Sanitizer);
        Assert.Null(validator.DictionaryName);
        Assert.Null(validator.FilePath);
        Assert.False(validator.ShowMatchedWords);
        Assert.Null(validator._lastMatchDetails);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} contains sensitive or prohibited words.",
            validator._errorMessageResourceAccessor());

        var validator2 = new SensitiveWordValidator { ShowMatchedWords = true };
        Assert.True(validator2.ShowMatchedWords);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} contains sensitive or prohibited words: {1}.",
            validator2._errorMessageResourceAccessor());

        var validator3 = new SensitiveWordValidator(SensitiveWordSanitizer.Build(["敏感词"]));
        Assert.NotNull(validator3.Sanitizer);
        Assert.NotNull(validator3._errorMessageResourceAccessor);
        Assert.Equal("The field {0} contains sensitive or prohibited words.",
            validator3._errorMessageResourceAccessor());

        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        using var stream = File.OpenRead(filePath);
        var validator4 = new SensitiveWordValidator(stream);
        Assert.NotNull(validator4.Sanitizer);
        Assert.Null(validator4.DictionaryName);
        Assert.Empty(SensitiveWordSanitizerFactory._instances);

        Assert.NotNull(validator4._errorMessageResourceAccessor);
        Assert.Equal("The field {0} contains sensitive or prohibited words.",
            validator4._errorMessageResourceAccessor());

        var validator5 = new SensitiveWordValidator(stream, "validator_test");
        Assert.NotNull(validator5.Sanitizer);
        Assert.Null(validator5.DictionaryName);
        Assert.Single(SensitiveWordSanitizerFactory._instances);
        Assert.Same(validator5.Sanitizer, SensitiveWordSanitizerFactory.Get("validator_test"));
        SensitiveWordSanitizerFactory.TryRemove("validator_test");
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("这里包含敏感词吗", false)]
    [InlineData("你大爷的，TMD!", false)]
    [InlineData("这是一段正常的文字", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);
        var validator = new SensitiveWordValidator { FilePath = filePath };
        Assert.Equal(result, validator.IsValid(value));
        SensitiveWordSanitizerFactory.TryRemove(normalizedPath); // null 或空白字符串时，GetSanitizer() 还未调用
    }
    //
    // [Theory]
    // [InlineData(null, true)]
    // [InlineData(0, false)]
    // [InlineData(10, false)]
    // [InlineData(17, false)]
    // [InlineData(18, true)]
    // [InlineData(30, true)]
    // [InlineData(100, true)]
    // [InlineData(120, true)]
    // [InlineData(121, false)]
    // public void IsValid_WithShowMatchedWords_ReturnOK(object? value, bool result)
    // {
    //     var validator = new SensitiveWordValidator { ShowMatchedWords = true };
    //     Assert.Equal(result, validator.IsValid(value));
    // }
    //
    // [Theory]
    // [InlineData(null, true)]
    // [InlineData("0", true)]
    // [InlineData("30", true)]
    // [InlineData("100", true)]
    // [InlineData("120", true)]
    // [InlineData("121", false)]
    // [InlineData("30.00", false)]
    // [InlineData("-1", false)]
    // public void IsValid_WithAllowStringValues_ReturnOK(object? value, bool result)
    // {
    //     var validator = new SensitiveWordValidator { AllowStringValues = true };
    //     Assert.Equal(result, validator.IsValid(value));
    // }
    //
    // [Fact]
    // public void GetValidationResults_ReturnOK()
    // {
    //     var validator = new SensitiveWordValidator();
    //     Assert.Null(validator.GetValidationResults(30, "data"));
    //
    //     var validationResults = validator.GetValidationResults(121, "data");
    //     Assert.NotNull(validationResults);
    //     Assert.Single(validationResults);
    //     Assert.Equal("The field data contains sensitive or prohibited words.", validationResults.First().ErrorMessage);
    //
    //     validator.ErrorMessage = "数据无效";
    //     var validationResults2 = validator.GetValidationResults(121, "data");
    //     Assert.NotNull(validationResults2);
    //     Assert.Single(validationResults2);
    //     Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    // }
    //
    // [Fact]
    // public void GetValidationResults_WithShowMatchedWords_ReturnOK()
    // {
    //     var validator = new SensitiveWordValidator { ShowMatchedWords = true };
    //     Assert.Null(validator.GetValidationResults(30, "data"));
    //
    //     var validationResults = validator.GetValidationResults(16, "data");
    //     Assert.NotNull(validationResults);
    //     Assert.Single(validationResults);
    //     Assert.Equal("The field data contains sensitive or prohibited words: {1}.", validationResults.First().ErrorMessage);
    //
    //     validator.ErrorMessage = "数据无效";
    //     var validationResults2 = validator.GetValidationResults(16, "data");
    //     Assert.NotNull(validationResults2);
    //     Assert.Single(validationResults2);
    //     Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    // }
    //
    // [Fact]
    // public void GetValidationResults_WithAllowStringValues_ReturnOK()
    // {
    //     var validator = new SensitiveWordValidator { AllowStringValues = true };
    //     Assert.Null(validator.GetValidationResults("30", "data"));
    //
    //     var validationResults = validator.GetValidationResults("121", "data");
    //     Assert.NotNull(validationResults);
    //     Assert.Single(validationResults);
    //     Assert.Equal("The field data contains sensitive or prohibited words.", validationResults.First().ErrorMessage);
    //
    //     validator.ErrorMessage = "数据无效";
    //     var validationResults2 = validator.GetValidationResults("121", "data");
    //     Assert.NotNull(validationResults2);
    //     Assert.Single(validationResults2);
    //     Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
    // }
    //
    // [Fact]
    // public void Validate_ReturnOK()
    // {
    //     var validator = new SensitiveWordValidator();
    //     validator.Validate(30, "data");
    //
    //     var exception = Assert.Throws<ValidationException>(() => validator.Validate(121, "data"));
    //     Assert.Equal("The field data contains sensitive or prohibited words.", exception.Message);
    //
    //     validator.ErrorMessage = "数据无效";
    //     var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(121, "data"));
    //     Assert.Equal("数据无效", exception2.Message);
    // }
    //
    // [Fact]
    // public void Validate_WithShowMatchedWords_ReturnOK()
    // {
    //     var validator = new SensitiveWordValidator { ShowMatchedWords = true };
    //     validator.Validate(30, "data");
    //
    //     var exception = Assert.Throws<ValidationException>(() => validator.Validate(16, "data"));
    //     Assert.Equal("The field data contains sensitive or prohibited words: {1}.", exception.Message);
    //
    //     validator.ErrorMessage = "数据无效";
    //     var exception2 = Assert.Throws<ValidationException>(() => validator.Validate(16, "data"));
    //     Assert.Equal("数据无效", exception2.Message);
    // }
    //
    // [Fact]
    // public void Validate_WithAllowStringValues_ReturnOK()
    // {
    //     var validator = new SensitiveWordValidator { AllowStringValues = true };
    //     validator.Validate("30", "data");
    //
    //     var exception = Assert.Throws<ValidationException>(() => validator.Validate("121", "data"));
    //     Assert.Equal("The field data contains sensitive or prohibited words.", exception.Message);
    //
    //     validator.ErrorMessage = "数据无效";
    //     var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("121", "data"));
    //     Assert.Equal("数据无效", exception2.Message);
    // }

    [Fact]
    public void GetSanitizer_Invalid_Parameters()
    {
        var validator = new SensitiveWordValidator();
        var exception = Assert.Throws<InvalidOperationException>(validator.GetSanitizer);
        Assert.Equal(
            "No dictionary source is configured for the SensitiveWordValidator. Please set the 'Sanitizer', 'DictionaryName', or 'FilePath' property, or provide a Stream or Sanitizer via the constructor.",
            exception.Message);
    }

    [Fact]
    public void GetSanitizer_ReturnOK()
    {
        var sensitiveWordSanitizer1 = SensitiveWordSanitizer.Build(["敏感词"]);
        var validator = new SensitiveWordValidator(sensitiveWordSanitizer1);
        var sanitizer = validator.GetSanitizer();
        Assert.NotNull(sanitizer);
        Assert.Same(sensitiveWordSanitizer1, sanitizer);

        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);

        var sensitiveWordSanitizer2 = SensitiveWordSanitizerFactory.GetOrCreateFromPath("validator_get", filePath);
        var validator2 = new SensitiveWordValidator { DictionaryName = "validator_get" };
        var sanitizer2 = validator2.GetSanitizer();
        Assert.NotNull(sanitizer2);
        Assert.Same(sensitiveWordSanitizer2, sanitizer2);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove("validator_get"));

        var validator3 = new SensitiveWordValidator { FilePath = filePath };
        var sanitizer3 = validator3.GetSanitizer();
        Assert.NotNull(sanitizer3);
        var sensitiveWordSanitizer3 = SensitiveWordSanitizerFactory.Get(normalizedPath);
        Assert.Same(sensitiveWordSanitizer3, sanitizer3);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(normalizedPath));
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new SensitiveWordValidator();
        Assert.Equal("The field data contains sensitive or prohibited words.", validator.FormatErrorMessage("data"));

        var validator2 = new SensitiveWordValidator { ShowMatchedWords = true };
        Assert.Equal("The field data contains sensitive or prohibited words: .",
            validator2.FormatErrorMessage("data"));

        var validator3 = new SensitiveWordValidator
        {
            ShowMatchedWords = true, _lastMatchDetails = ["[敏感词] @ 5..10", "[TMD!] @ 5..8"]
        };
        Assert.Equal("The field data contains sensitive or prohibited words: [敏感词] @ 5..10, [TMD!] @ 5..8.",
            validator3.FormatErrorMessage("data"));
        validator3._lastMatchDetails = null;
    }

    [Fact]
    public void GetResourceKey_ReturnOK()
    {
        var validator = new SensitiveWordValidator();
        Assert.Equal("SensitiveWordValidator_ValidationError", validator.GetResourceKey());

        var validator2 = new SensitiveWordValidator { ShowMatchedWords = true };
        Assert.Equal("SensitiveWordValidator_ValidationError_ShowMatchedWords", validator2.GetResourceKey());
    }
}