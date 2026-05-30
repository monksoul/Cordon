// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

[Collection("SensitiveWordTests")]
public class ValidationBuilderSensitiveWordsTests
{
    [Fact]
    public void AddSensitiveWords_ReturnOK()
    {
        var builder = new ValidationBuilder();
        builder.AddSensitiveWords(u => u.AddPath("sensitive_words.txt"));
        Assert.NotNull(SensitiveWordSanitizerFactory.Get(SensitiveWordSanitizerFactory.DefaultName));

        builder.AddSensitiveWords("builder", u => u.AddPath("sensitive_words.txt"));
        Assert.NotNull(SensitiveWordSanitizerFactory.Get("builder"));

        builder.AddSensitiveWords("custom", () => SensitiveWordSanitizer.Build(["敏感词"]));
        Assert.NotNull(SensitiveWordSanitizerFactory.Get("custom"));

        SensitiveWordSanitizerFactory.Clear();
    }
}