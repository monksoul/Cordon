// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

[Collection("SensitiveWordTests")]
public class SensitiveWordValidatorTests
{
    [Fact]
    public void New_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => new SensitiveWordValidator((string)null!));
        Assert.Throws<ArgumentException>(() => new SensitiveWordValidator(string.Empty));
        Assert.Throws<ArgumentException>(() => new SensitiveWordValidator(" "));

        Assert.Throws<ArgumentNullException>(() => new SensitiveWordValidator((SensitiveWordSanitizer)null!));
        Assert.Throws<ArgumentNullException>(() =>
            new SensitiveWordValidator((Action<SensitiveWordSanitizerBuilder>)null!));
    }

    [Fact]
    public void New_ReturnOK()
    {
        var validator = new SensitiveWordValidator();
        Assert.Null(validator.Sanitizer);
        Assert.Null(validator.DictionaryName);
        Assert.Null(validator.ConfigureBuilder);
        Assert.False(validator.ShowMatchedWords);
        Assert.NotNull(validator._errorMessageResourceAccessor);
        Assert.Equal("The field {0} contains sensitive or prohibited words.",
            validator._errorMessageResourceAccessor());

        var validator2 = new SensitiveWordValidator(SensitiveWordSanitizerFactory.DefaultName);
        Assert.Equal(SensitiveWordSanitizerFactory.DefaultName, validator2.DictionaryName);
        Assert.NotNull(validator2._errorMessageResourceAccessor);
        Assert.Equal("The field {0} contains sensitive or prohibited words.",
            validator2._errorMessageResourceAccessor());

        var validator3 = new SensitiveWordValidator(SensitiveWordSanitizer.Build(["敏感词"]));
        Assert.NotNull(validator3.Sanitizer);
        Assert.NotNull(validator3._errorMessageResourceAccessor);
        Assert.Equal("The field {0} contains sensitive or prohibited words.",
            validator3._errorMessageResourceAccessor());

        var validator4 = new SensitiveWordValidator(builder => builder.AddWord("敏感词"));
        Assert.NotNull(validator4.ConfigureBuilder);
        Assert.NotNull(validator4._errorMessageResourceAccessor);
        Assert.Equal("The field {0} contains sensitive or prohibited words.",
            validator4._errorMessageResourceAccessor());
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("这里包含敏感词吗", false)]
    [InlineData("你大爷的，TMD!", false)]
    [InlineData("这是一段正常的文字", true)]
    public void IsValid_ReturnOK(object? value, bool result)
    {
        var validator = new SensitiveWordValidator(builder => builder.AddPath("sensitive_words.txt"));
        Assert.Equal(result, validator.IsValid(value));

        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void GetValidationResults_ReturnOK()
    {
        var validator = new SensitiveWordValidator(builder => builder.AddPath("sensitive_words.txt"));

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

        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void GetValidationResults_WithShowMatchedWords_ReturnOK()
    {
        var validator =
            new SensitiveWordValidator(builder => builder.AddPath("sensitive_words.txt")) { ShowMatchedWords = true };

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

        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void GetValidationResults_WithShowMatchedWords_MultipleInstances_ReturnOK()
    {
        SensitiveWordSanitizerFactory.GetOrCreate(builder => builder.AddPath("sensitive_words.txt"));
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

        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void Validate_ReturnOK()
    {
        var validator =
            new SensitiveWordValidator(builder => builder.AddPath("sensitive_words.txt"));

        validator.Validate(null, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("这里包含敏感词吗", "data"));
        Assert.Equal("The field data contains sensitive or prohibited words.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("这里包含敏感词吗", "data"));
        Assert.Equal("数据无效", exception2.Message);

        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void Validate_WithShowMatchedWords_ReturnOK()
    {
        var validator =
            new SensitiveWordValidator(builder => builder.AddPath("sensitive_words.txt")) { ShowMatchedWords = true };

        validator.Validate(null, "data");

        var exception = Assert.Throws<ValidationException>(() => validator.Validate("这里包含敏感词吗", "data"));
        Assert.Equal("The field data contains sensitive or prohibited words: [敏感词] @ 4..7.", exception.Message);

        validator.ErrorMessage = "数据无效";
        var exception2 = Assert.Throws<ValidationException>(() => validator.Validate("这里包含敏感词吗", "data"));
        Assert.Equal("数据无效 Matched: [敏感词] @ 4..7", exception2.Message);

        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void GetSanitizer_Invalid_Parameters()
    {
        var validator = new SensitiveWordValidator();
        var exception = Assert.Throws<InvalidOperationException>(validator.GetSanitizer);
        Assert.Equal(
            "No sensitive word source is configured for the SensitiveWordValidator. Either set 'Sanitizer', 'DictionaryName', or 'ConfigureBuilder', or register the default dictionary 'SensitiveWords:Default' via `SensitiveWordSanitizerFactory.GetOrCreate`.",
            exception.Message);

        validator.DictionaryName = "not-found";
        var exception2 = Assert.Throws<InvalidOperationException>(validator.GetSanitizer);
        Assert.Equal(
            "The dictionary 'not-found' has not been registered in the factory. Please register it first, or provide a 'ConfigureBuilder' to build it.",
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

        var sensitiveWordSanitizer2 =
            SensitiveWordSanitizerFactory.GetOrCreate("validator", builder => builder.AddWord("敏感词"));
        var validator2 = new SensitiveWordValidator("validator");
        var sanitizer2 = validator2.GetSanitizer();
        Assert.NotNull(sanitizer2);
        Assert.Same(sensitiveWordSanitizer2, sanitizer2);

        var validator3 = new SensitiveWordValidator(builder => builder.AddWord("敏感词"));
        var sanitizer3 = validator3.GetSanitizer();
        var sensitiveWordSanitizer3 = SensitiveWordSanitizerFactory.Get();
        Assert.NotNull(sanitizer3);
        Assert.Same(sensitiveWordSanitizer3, sanitizer3);
        SensitiveWordSanitizerFactory.Clear();

        var validator4 = new SensitiveWordValidator(builder => builder.AddWord("敏感词")) { DictionaryName = "builder" };
        var sanitizer4 = validator4.GetSanitizer();
        var sensitiveWordSanitizer4 = SensitiveWordSanitizerFactory.Get("builder");
        Assert.NotNull(sanitizer4);
        Assert.Same(sensitiveWordSanitizer4, sanitizer4);

        var sensitiveWordSanitizer5 =
            SensitiveWordSanitizerFactory.GetOrCreate(builder => builder.AddWord("敏感词"));
        var validator5 = new SensitiveWordValidator();
        var sanitizer5 = validator5.GetSanitizer();
        Assert.NotNull(sanitizer5);
        Assert.Same(sensitiveWordSanitizer5, sanitizer5);

        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void FormatErrorMessage_ReturnOK()
    {
        var validator = new SensitiveWordValidator();
        Assert.Equal("The field data contains sensitive or prohibited words.",
            validator.FormatErrorMessage("data", null));

        var validator2 = new SensitiveWordValidator { ShowMatchedWords = true };
        Assert.Equal("The field data contains sensitive or prohibited words: .",
            validator2.FormatErrorMessage("data", []));

        var validator3 = new SensitiveWordValidator { ShowMatchedWords = true };
        Assert.Equal("The field data contains sensitive or prohibited words: [敏感词] @ 5..10, [TMD!] @ 5..8.",
            validator3.FormatErrorMessage("data", [new MatchResult("敏感词", 5, 10), new MatchResult("TMD!", 5, 8)]));
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