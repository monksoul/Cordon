// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

[Collection("SensitiveWordValidatorTests")]
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
        Assert.Null(SensitiveWordValidator._lastMatchDetails.Value);
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

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);
        var validator = new SensitiveWordValidator { FilePath = filePath };

        Assert.Null(validator.GetValidationResults(null, "data"));

        var validationResults = validator.GetValidationResults("这里包含敏感词吗", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data contains sensitive or prohibited words.", validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("这里包含敏感词吗", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效", validationResults2.First().ErrorMessage);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(normalizedPath));
    }

    [Fact]
    public void GetValidationResults_WithShowMatchedWords_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);
        var validator = new SensitiveWordValidator { FilePath = filePath, ShowMatchedWords = true };

        Assert.Null(validator.GetValidationResults(null, "data"));

        var validationResults = validator.GetValidationResults("这里包含敏感词吗", "data");
        Assert.NotNull(validationResults);
        Assert.Single(validationResults);
        Assert.Equal("The field data contains sensitive or prohibited words: [敏感词] @ 4..7.",
            validationResults.First().ErrorMessage);

        validator.ErrorMessage = "数据无效";
        var validationResults2 = validator.GetValidationResults("这里包含敏感词吗", "data");
        Assert.NotNull(validationResults2);
        Assert.Single(validationResults2);
        Assert.Equal("数据无效 Matched: [敏感词] @ 4..7", validationResults2.First().ErrorMessage);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(normalizedPath));
    }

    [Fact]
    public void GetValidationResults_WithShowMatchedWords_MultipleInstances_ReturnOK()
    {
        SensitiveWordSanitizerFactory.GetOrCreateFromPath(SensitiveWordOptions.DefaultDictionaryName,
            "sensitive_words.txt");
        var validator1 = new SensitiveWordValidator { ShowMatchedWords = true };
        var validator2 = new SensitiveWordValidator { ShowMatchedWords = true };

        var validationResults1 = validator1.GetValidationResults("这里包含敏感词", "data");
        var validationResults2 = validator2.GetValidationResults("你大爷的，TMD!", "data");

        Assert.NotNull(validationResults1);
        Assert.Equal("The field data contains sensitive or prohibited words: [敏感词] @ 4..7.",
            validationResults1.First().ErrorMessage);

        Assert.NotNull(validationResults2);
        Assert.Equal("The field data contains sensitive or prohibited words: [你-大-爷] @ 0..3, [TMD!] @ 5..8.",
            validationResults2.First().ErrorMessage);

        Assert.True(SensitiveWordSanitizerFactory.TryRemove(SensitiveWordOptions.DefaultDictionaryName));
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);
        var validator = new SensitiveWordValidator { FilePath = filePath };

        validator.Validate(null, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("这里包含敏感词吗", "data"));
        Assert.Equal("The field data contains sensitive or prohibited words.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("这里包含敏感词吗", "data"));
        Assert.Equal("数据无效", exception2.Message);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(normalizedPath));
    }

    [Fact]
    public void Validate_WithShowMatchedWords_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);
        var validator = new SensitiveWordValidator { FilePath = filePath, ShowMatchedWords = true };

        validator.Validate(null, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("这里包含敏感词吗", "data"));
        Assert.Equal("The field data contains sensitive or prohibited words: [敏感词] @ 4..7.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("这里包含敏感词吗", "data"));
        Assert.Equal("数据无效 Matched: [敏感词] @ 4..7", exception2.Message);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(normalizedPath));
    }

    [Fact]
    public void GetSanitizer_Invalid_Parameters()
    {
        var validator = new SensitiveWordValidator();
        var exception = Assert.Throws<InvalidOperationException>(validator.GetSanitizer);
        Assert.Equal(
            "No dictionary source is configured for the SensitiveWordValidator, and the default dictionary 'SensitiveWords:Default' has not been registered. Please either set the 'DictionaryName' or 'FilePath' property, or register the default dictionary via `SensitiveWordSanitizerFactory.GetOrCreateFromPath` at application startup.",
            exception.Message);

        validator.DictionaryName = "file-key";
        validator.FilePath = "mock-file.txt";
        var exception2 = Assert.Throws<InvalidOperationException>(validator.GetSanitizer);
        Assert.Equal(
            "Multiple dictionary sources are configured for the SensitiveWordValidator. Please set only one of 'Sanitizer', 'DictionaryName', or 'FilePath'.",
            exception2.Message);
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

        var validator4 = new SensitiveWordValidator();
        var sensitiveWordSanitizer4 =
            SensitiveWordSanitizerFactory.GetOrCreateFromPath(SensitiveWordOptions.DefaultDictionaryName, filePath);
        var sanitizer4 = validator4.GetSanitizer();
        Assert.Same(sensitiveWordSanitizer4, sanitizer4);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(SensitiveWordOptions.DefaultDictionaryName));
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new SensitiveWordValidator();
        Assert.Equal("The field data contains sensitive or prohibited words.", validator.FormatErrorMessage("data"));

        var validator2 = new SensitiveWordValidator { ShowMatchedWords = true };
        Assert.Equal("The field data contains sensitive or prohibited words: .",
            validator2.FormatErrorMessage("data"));

        var validator3 = new SensitiveWordValidator { ShowMatchedWords = true };
        SensitiveWordValidator._lastMatchDetails.Value = ["[敏感词] @ 5..10", "[TMD!] @ 5..8"];
        Assert.Equal("The field data contains sensitive or prohibited words: [敏感词] @ 5..10, [TMD!] @ 5..8.",
            validator3.FormatErrorMessage("data"));
        SensitiveWordValidator._lastMatchDetails.Value = null;
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