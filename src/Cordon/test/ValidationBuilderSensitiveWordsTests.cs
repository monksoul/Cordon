// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

[Collection("SensitiveWordSanitizerTests")]
public class ValidationBuilderSensitiveWordsTests
{
    [Fact]
    public void AddSensitiveWords_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddSensitiveWords("sensitive_words.txt");
        Assert.NotNull(SensitiveWordSanitizerFactory.Get(SensitiveWordOptions.DefaultDictionaryName));

        builder.AddSensitiveWords("sensitive_words.txt", "from_builder");
        Assert.NotNull(SensitiveWordSanitizerFactory.Get("from_builder"));

        builder.AddSensitiveWords(["敏感词"], "from_words");
        Assert.NotNull(SensitiveWordSanitizerFactory.Get("from_words"));

        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        using var stream = File.OpenRead(filePath);

        builder.AddSensitiveWords(stream, "from_stream");
        Assert.NotNull(SensitiveWordSanitizerFactory.Get("from_stream"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove(SensitiveWordOptions.DefaultDictionaryName));
        Assert.NotNull(SensitiveWordSanitizerFactory.Get("from_builder"));
        Assert.NotNull(SensitiveWordSanitizerFactory.Get("from_words"));
        Assert.NotNull(SensitiveWordSanitizerFactory.Get("from_stream"));
    }
}