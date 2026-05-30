// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class SensitiveWordSanitizerBuilderTests
{
    [Fact]
    public void New_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        Assert.NotNull(builder._words);
        Assert.Empty(builder._words);
        Assert.NotNull(builder._options);
        Assert.Equal(SensitiveWordOptions.Default, builder._options);
    }

    [Fact]
    public void AddWord_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        Assert.Empty(builder._words);

        builder.AddWord(null).AddWord(string.Empty).AddWord(" ").AddWord("敏感词").AddWord(" 敏感词 ").AddWord("敏感 词")
            .AddWord("fuck").AddWord("FUCK");
        Assert.Equal(3, builder._words.Count);
        Assert.Equal(["敏感词", "敏感 词", "fuck"], builder._words);
    }

    [Fact]
    public void AddWords_Invalid_Parameters()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddWords(null!));
    }

    [Fact]
    public void AddWords_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        builder.AddWords([null, string.Empty, " ", "敏感词", " 敏感词 ", "敏感 词", "fuck", "FUCK"]);
        Assert.Equal(3, builder._words.Count);
        Assert.Equal(["敏感词", "敏感 词", "fuck"], builder._words);
    }

    [Fact]
    public void AddLine_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        builder.AddLine(null)
            .AddLine("# 整行注释")
            .AddLine("#整行注释")
            .AddLine(string.Empty)
            .AddLine(" ")
            .AddLine("\n")
            .AddLine("\r\n")
            .AddLine("# 广告类（使用多种分隔符：竖线、逗号、制表符、分号）")
            .AddLine("加微信 | 兼职刷单, QQ群 ; 淘宝代刷   # 行内注释")
            .AddLine("加微信 | 兼职刷单, QQ群 ; 淘宝代刷   #行内注释")
            .AddLine("敏感词, 违规表述\t暴恐\t涉 政")
            .AddLine("test_bad_word\tillegal_term")
            .AddLine("八 嘎 耶鲁")
            .AddLine("你-大-爷")
            .AddLine("low_high")
            .AddLine("hello_world")
            .AddLine("abc_def")
            .AddLine("TMD!");

        Assert.Equal(16, builder._words.Count);
        Assert.Equal(
        [
            "加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁",
            "你-大-爷", "low_high", "hello_world", "abc_def", "TMD!"
        ], builder._words);
    }

    [Fact]
    public void AddLines_Invalid_Parameters()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddLines(null!));
    }

    [Fact]
    public void AddLines_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        builder.AddLines([
            null, "# 整行注释", "#整行注释", string.Empty, " ", "\n", "\r\n", "# 广告类（使用多种分隔符：竖线、逗号、制表符、分号）",
            "加微信 | 兼职刷单, QQ群 ; 淘宝代刷   # 行内注释", "加微信 | 兼职刷单, QQ群 ; 淘宝代刷   #行内注释", "敏感词, 违规表述\t暴恐\t涉 政",
            "test_bad_word\tillegal_term", "八 嘎 耶鲁", "你-大-爷", "low_high", "hello_world", "abc_def", "TMD!"
        ]);

        Assert.Equal(16, builder._words.Count);
        Assert.Equal(
        [
            "加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁",
            "你-大-爷", "low_high", "hello_world", "abc_def", "TMD!"
        ], builder._words);
    }

    [Fact]
    public void AddStream_Invalid_Parameters()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddStream(null!));

        var stream = new MemoryStream();
        stream.Dispose();

        var exception =
            Assert.Throws<ArgumentException>(() => builder.AddStream(stream));
        Assert.Equal("Stream must be readable. (Parameter 'stream')", exception.Message);
    }

    [Fact]
    public void AddStream_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        using var stream = File.OpenRead("sensitive_words.txt");
        builder.AddStream(stream);

        Assert.Equal(17, builder._words.Count);
        Assert.Equal(
        [
            "加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁",
            "你-大-爷", "low_high", "hello_world", "abc_def", "TMD!", "fuck"
        ], builder._words);
    }

    [Fact]
    public void AddPath_Invalid_Parameters()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddPath(null!));
        Assert.Throws<ArgumentException>(() => builder.AddPath(string.Empty));
        Assert.Throws<ArgumentException>(() => builder.AddPath("  "));

        var exception =
            Assert.Throws<FileNotFoundException>(() => builder.AddPath("/user/not-found.txt"));
        Assert.Equal("Sensitive word file not found.", exception.Message);
        Assert.Equal(Path.GetFullPath("/user/not-found.txt"), exception.FileName);
    }

    [Fact]
    public void AddPath_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        builder.AddPath("sensitive_words.txt");

        Assert.Equal(17, builder._words.Count);
        Assert.Equal(
        [
            "加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁",
            "你-大-爷", "low_high", "hello_world", "abc_def", "TMD!", "fuck"
        ], builder._words);

        builder.AddPath("sensitive_words.txt");
        Assert.Equal(17, builder._words.Count);
    }

    [Fact]
    public void ConfigureOptions_Invalid_Parameters()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        Assert.Throws<ArgumentNullException>(() =>
            builder.ConfigureOptions((Func<SensitiveWordOptions, SensitiveWordOptions>)null!));
    }

    [Fact]
    public void ConfigureOptions_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        builder.ConfigureOptions((SensitiveWordOptions?)null);
        Assert.Same(SensitiveWordOptions.Default, builder._options);

        builder.ConfigureOptions(new SensitiveWordOptions { IgnoreCase = false });
        Assert.False(builder._options.IgnoreCase);

        builder.ConfigureOptions(o => null);
        Assert.Same(SensitiveWordOptions.Default, builder._options);

        builder.ConfigureOptions(o => o with { IgnoreFullwidth = false });
        Assert.False(builder._options.IgnoreFullwidth);
    }

    [Fact]
    public void Clear_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        builder.AddWords(["敏感词"]);
        builder.ConfigureOptions(o => o with { IgnoreCase = false });
        Assert.Single(builder._words);
        Assert.False(builder._options.IgnoreCase);

        builder.Clear();
        Assert.Same(SensitiveWordOptions.Default, builder._options);
        Assert.True(builder._options.IgnoreCase);
    }

    [Fact]
    public void Build_Invalid_Parameters()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        var exception = Assert.Throws<InvalidOperationException>(builder.Build);
        Assert.Equal(
            "Cannot build a SensitiveWordSanitizer with an empty word list. Please add at least one sensitive word via AddWord, AddPath, AddStream, etc.",
            exception.Message);
    }

    [Fact]
    public void Build_ReturnOK()
    {
        var builder = new SensitiveWordSanitizerBuilder();
        builder.AddPath("sensitive_words.txt");
        var sensitiveWordSanitizer = builder.Build();
        Assert.True(sensitiveWordSanitizer.Contains("这里包含敏感词"));
    }

    [Fact]
    public void ResolveFilePath_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerBuilder.ResolveFilePath(null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerBuilder.ResolveFilePath(string.Empty));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerBuilder.ResolveFilePath(" "));
    }

    [Fact]
    public void ResolveFilePath_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);

        Assert.Equal(normalizedPath, SensitiveWordSanitizerBuilder.ResolveFilePath("sensitive_words.txt"));
        Assert.Equal(normalizedPath, SensitiveWordSanitizerBuilder.ResolveFilePath(filePath));
        Assert.Equal(normalizedPath, SensitiveWordSanitizerBuilder.ResolveFilePath(normalizedPath));
    }
}