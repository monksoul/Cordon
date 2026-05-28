// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

public class SensitiveWordSanitizerTests
{
    private const string userText =
        @"测试开始：加微信，加-wechat；兼职刷单：日结300元。QQ群123456，淘宝代刷。违规表述、暴恐、涉政。还有涉 政（带空格），涉-政，涉　政（全角空格），涉\t政（制表符）。敏感词测试：敏感 词、敏-感词、敏感词！test bad word 和 illegal term，八嘎耶鲁（无空格），你大爷，low high，hello-world! Hello World。abc*def。另外重叠匹配：low_high low high。最后去TMD!。ｆｕｃｋ the bad words。Ⓕⓤc⒦ the bad words。";

    [Fact]
    public void New_ReturnOK()
    {
        Assert.All([
            '_',
            '-',
            ' ',
            '\t',
            '　',
            '\r',
            '\n'
        ], c => Assert.True(SensitiveWordSanitizer.IgnoredSeparators.Contains(c)));

        Assert.NotNull(SensitiveWordSanitizer.SkipMap);
        Assert.Equal(65536, SensitiveWordSanitizer.SkipMap.Length);
    }

    [Fact]
    public void CreateFromPath_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizer.CreateFromPath(null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizer.CreateFromPath(string.Empty));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizer.CreateFromPath("  "));

        var exception =
            Assert.Throws<FileNotFoundException>(() => SensitiveWordSanitizer.CreateFromPath("/user/not-found.txt"));
        Assert.Equal("Sensitive word file not found.", exception.Message);
        Assert.Equal("/user/not-found.txt", exception.FileName);
    }

    [Fact]
    public void CreateFromPath_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromPath(filePath);
        Assert.NotNull(sensitiveWordSanitizer);

        Assert.NotNull(sensitiveWordSanitizer._options);
        Assert.True(sensitiveWordSanitizer._options.IgnoreCase);
        Assert.True(sensitiveWordSanitizer._options.IgnoreSymbol);
        Assert.NotNull(sensitiveWordSanitizer._root);
        Assert.Equal('加', sensitiveWordSanitizer._root.Children.FirstOrDefault().Key);
        Assert.Equal(16, sensitiveWordSanitizer._root.Children.Count);

        var sensitiveWordSanitizer2 = SensitiveWordSanitizer.CreateFromPath(filePath,
            new SensitiveWordOptions { IgnoreCase = false, IgnoreSymbol = false });
        Assert.NotNull(sensitiveWordSanitizer2._options);
        Assert.False(sensitiveWordSanitizer2._options.IgnoreCase);
        Assert.False(sensitiveWordSanitizer2._options.IgnoreSymbol);
    }

    [Fact]
    public void CreateFromStream_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizer.CreateFromStream(null!));

        var stream = new MemoryStream();
        stream.Dispose();

        var exception =
            Assert.Throws<ArgumentException>(() => SensitiveWordSanitizer.CreateFromStream(stream));
        Assert.Equal("Stream must be readable. (Parameter 'stream')", exception.Message);
    }

    [Fact]
    public void CreateFromStream_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        using var stream = File.OpenRead(filePath);

        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromStream(stream);
        Assert.NotNull(sensitiveWordSanitizer);

        Assert.True(sensitiveWordSanitizer._options.IgnoreCase);
        Assert.True(sensitiveWordSanitizer._options.IgnoreSymbol);
        Assert.NotNull(sensitiveWordSanitizer._root);
        Assert.Equal('加', sensitiveWordSanitizer._root.Children.FirstOrDefault().Key);
        Assert.Equal(16, sensitiveWordSanitizer._root.Children.Count);

        var sensitiveWordSanitizer2 = SensitiveWordSanitizer.CreateFromStream(stream,
            new SensitiveWordOptions { IgnoreCase = false, IgnoreSymbol = false });
        Assert.False(sensitiveWordSanitizer2._options.IgnoreCase);
        Assert.False(sensitiveWordSanitizer2._options.IgnoreSymbol);
    }

    [Fact]
    public void Build_Invalid_Parameters() =>
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizer.Build(null!));

    [Fact]
    public void Build_ReturnOK()
    {
        List<string> words = ["敏感词", "违规表述", "涉政", "暴恐"];
        var sensitiveWordSanitizer = SensitiveWordSanitizer.Build(words);

        Assert.True(sensitiveWordSanitizer._options.IgnoreCase);
        Assert.True(sensitiveWordSanitizer._options.IgnoreSymbol);
        Assert.NotNull(sensitiveWordSanitizer._root);
        Assert.Equal('敏', sensitiveWordSanitizer._root.Children.FirstOrDefault().Key);

        var sensitiveWordSanitizer2 = SensitiveWordSanitizer.Build(words,
            new SensitiveWordOptions { IgnoreCase = false, IgnoreSymbol = false });
        Assert.False(sensitiveWordSanitizer2._options.IgnoreCase);
        Assert.False(sensitiveWordSanitizer2._options.IgnoreSymbol);
    }

    [Fact]
    public void AddWord_ReturnOK()
    {
        var hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        SensitiveWordSanitizer.AddWord("敏感词", hashSet);
        Assert.Single(hashSet);
        Assert.Equal("敏感词", hashSet.ElementAt(0));

        SensitiveWordSanitizer.AddWord(" 敏感词 ", hashSet);
        Assert.Single(hashSet);
        Assert.Equal("敏感词", hashSet.ElementAt(0));

        SensitiveWordSanitizer.AddWord(string.Empty, hashSet);
        Assert.Single(hashSet);
        Assert.Equal("敏感词", hashSet.ElementAt(0));

        SensitiveWordSanitizer.AddWord("  ", hashSet);
        Assert.Single(hashSet);
        Assert.Equal("敏感词", hashSet.ElementAt(0));

        SensitiveWordSanitizer.AddWord("低俗", hashSet);
        Assert.Equal(2, hashSet.Count);
        Assert.Equal("低俗", hashSet.ElementAt(1));
    }

    [Fact]
    public void ParseLine_ReturnOK()
    {
        var hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        SensitiveWordSanitizer.ParseLine("# 整行注释", hashSet);
        Assert.Empty(hashSet);

        SensitiveWordSanitizer.ParseLine("", hashSet);
        Assert.Empty(hashSet);

        SensitiveWordSanitizer.ParseLine("\n", hashSet);
        Assert.Empty(hashSet);

        SensitiveWordSanitizer.ParseLine("\r\n", hashSet);
        Assert.Empty(hashSet);

        SensitiveWordSanitizer.ParseLine("# 广告类（使用多种分隔符：竖线、逗号、制表符、分号）", hashSet);
        Assert.Empty(hashSet);

        SensitiveWordSanitizer.ParseLine("加微信 | 兼职刷单, QQ群 ; 淘宝代刷   # 行内注释", hashSet);
        Assert.Equal(4, hashSet.Count);
        Assert.Equal(["加微信", "兼职刷单", "QQ群", "淘宝代刷"], hashSet);

        SensitiveWordSanitizer.ParseLine("敏感词, 违规表述\t暴恐\t涉 政", hashSet);
        Assert.Equal(8, hashSet.Count);
        Assert.Equal(["加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政"], hashSet);

        SensitiveWordSanitizer.ParseLine("test_bad_word\tillegal_term", hashSet);
        Assert.Equal(10, hashSet.Count);
        Assert.Equal(["加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term"],
            hashSet);

        SensitiveWordSanitizer.ParseLine("八 嘎 耶鲁", hashSet);
        Assert.Equal(11, hashSet.Count);
        Assert.Equal(
            ["加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁"],
            hashSet);

        SensitiveWordSanitizer.ParseLine("你-大-爷", hashSet);
        Assert.Equal(12, hashSet.Count);
        Assert.Equal(
        [
            "加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁", "你-大-爷"
        ], hashSet);

        SensitiveWordSanitizer.ParseLine("low_high", hashSet);
        Assert.Equal(13, hashSet.Count);
        Assert.Equal(
        [
            "加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁",
            "你-大-爷", "low_high"
        ], hashSet);

        SensitiveWordSanitizer.ParseLine("hello_world", hashSet);
        Assert.Equal(14, hashSet.Count);
        Assert.Equal(
        [
            "加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁",
            "你-大-爷", "low_high", "hello_world"
        ], hashSet);

        SensitiveWordSanitizer.ParseLine("abc_def", hashSet);
        Assert.Equal(15, hashSet.Count);
        Assert.Equal(
        [
            "加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁",
            "你-大-爷", "low_high", "hello_world", "abc_def"
        ], hashSet);

        SensitiveWordSanitizer.ParseLine("TMD!", hashSet);
        Assert.Equal(16, hashSet.Count);
        Assert.Equal(
        [
            "加微信", "兼职刷单", "QQ群", "淘宝代刷", "敏感词", "违规表述", "暴恐", "涉 政", "test_bad_word", "illegal_term", "八 嘎 耶鲁",
            "你-大-爷", "low_high", "hello_world", "abc_def", "TMD!"
        ], hashSet);
    }

    [Fact]
    public void ShouldSkip_ReturnOK()
    {
        Assert.True(SensitiveWordSanitizer.ShouldSkip(' '));
        Assert.True(SensitiveWordSanitizer.ShouldSkip('。'));
        Assert.True(SensitiveWordSanitizer.ShouldSkip('，'));
        Assert.True(SensitiveWordSanitizer.ShouldSkip('|'));
    }

    [Fact]
    public void FindMatches_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromPath(filePath);

        Assert.Empty(sensitiveWordSanitizer.FindMatches(null));
        Assert.Empty(sensitiveWordSanitizer.FindMatches(string.Empty));

        var matchResults = sensitiveWordSanitizer.FindMatches(userText);
        Assert.Equal(26, matchResults.Length);
        Assert.Equal(
            [
                "加微信", "兼职刷单", "QQ群", "淘宝代刷", "违规表述", "暴恐", "涉 政", "涉 政", "涉 政", "涉 政", "敏感词", "敏感词", "敏感词", "敏感词",
                "test_bad_word", "illegal_term", "八 嘎 耶鲁", "你-大-爷", "low_high", "hello_world", "hello_world", "abc_def",
                "low_high", "low_high", "TMD!", "fuck"
            ],
            matchResults.Select(u => u.Word));

        Assert.Equal(
            [
                "[加微信] @ 5..8", "[兼职刷单] @ 18..22", "[QQ群] @ 30..33", "[淘宝代刷] @ 40..44", "[违规表述] @ 45..49",
                "[暴恐] @ 50..52", "[涉 政] @ 53..55", "[涉 政] @ 58..61", "[涉 政] @ 67..70", "[涉 政] @ 71..74",
                "[敏感词] @ 91..94", "[敏感词] @ 97..101", "[敏感词] @ 102..106", "[敏感词] @ 107..110",
                "[test_bad_word] @ 111..124", "[illegal_term] @ 127..139", "[八 嘎 耶鲁] @ 140..144", "[你-大-爷] @ 150..153",
                "[low_high] @ 154..162", "[hello_world] @ 163..174", "[hello_world] @ 176..187", "[abc_def] @ 188..195",
                "[low_high] @ 203..211", "[low_high] @ 212..220", "[TMD!] @ 224..227", "[fuck] @ 229..233"
            ],
            matchResults.Select(u => u.ToString()));

        var matchResults2 = sensitiveWordSanitizer.FindMatches("这里呢，是否包含敏__感__词呢？");
        Assert.Single(matchResults2);
        Assert.Equal(["敏感词"], matchResults2.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 8..15"], matchResults2.Select(u => u.ToString()));

        var matchResults3 = sensitiveWordSanitizer.FindMatches("这里面包含敏\r感\n词吗？");
        Assert.Single(matchResults3);
        Assert.Equal(["敏感词"], matchResults3.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 5..10"], matchResults3.Select(u => u.ToString()));

        var matchResults4 = sensitiveWordSanitizer.FindMatches("这里面包含敏\t感\n词吗？");
        Assert.Single(matchResults4);
        Assert.Equal(["敏感词"], matchResults4.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 5..10"], matchResults4.Select(u => u.ToString()));

        var matchResults5 = sensitiveWordSanitizer.FindMatches("Low High");
        Assert.Single(matchResults5);
        Assert.Equal(["low_high"], matchResults5.Select(u => u.Word));
        Assert.Equal(["[low_high] @ 0..8"], matchResults5.Select(u => u.ToString()));

        var matchResults6 = sensitiveWordSanitizer.FindMatches("这里面包含敏*感*词吗？");
        Assert.Single(matchResults6);
        Assert.Equal(["敏感词"], matchResults6.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 5..10"], matchResults6.Select(u => u.ToString()));

        var matchResults7 = sensitiveWordSanitizer.FindMatches("这里面包含TMD吗？");
        Assert.Single(matchResults7);
        Assert.Equal(["TMD!"], matchResults7.Select(u => u.Word));
        Assert.Equal(["[TMD!] @ 5..8"], matchResults7.Select(u => u.ToString()));

        var matchResults8 = sensitiveWordSanitizer.FindMatches("这里面包含tmd吗？");
        Assert.Single(matchResults8);
        Assert.Equal(["TMD!"], matchResults8.Select(u => u.Word));
        Assert.Equal(["[TMD!] @ 5..8"], matchResults8.Select(u => u.ToString()));
    }

    [Fact]
    public void FindMatches_SetIgnoreCaseFalse_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer =
            SensitiveWordSanitizer.CreateFromPath(filePath, new SensitiveWordOptions { IgnoreCase = false });

        var matchResults1 = sensitiveWordSanitizer.FindMatches("这里呢，是否包含敏__感__词呢？");
        Assert.Single(matchResults1);
        Assert.Equal(["敏感词"], matchResults1.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 8..15"], matchResults1.Select(u => u.ToString()));

        var matchResults2 = sensitiveWordSanitizer.FindMatches("这里面包含敏\r感\n词吗？");
        Assert.Single(matchResults2);
        Assert.Equal(["敏感词"], matchResults2.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 5..10"], matchResults2.Select(u => u.ToString()));

        var matchResults3 = sensitiveWordSanitizer.FindMatches("这里面包含敏\t感\n词吗？");
        Assert.Single(matchResults3);
        Assert.Equal(["敏感词"], matchResults3.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 5..10"], matchResults3.Select(u => u.ToString()));

        var matchResults4 = sensitiveWordSanitizer.FindMatches("Low High");
        Assert.Empty(matchResults4);

        var matchResults5 = sensitiveWordSanitizer.FindMatches("这里面包含敏*感*词吗？");
        Assert.Single(matchResults5);
        Assert.Equal(["敏感词"], matchResults5.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 5..10"], matchResults5.Select(u => u.ToString()));

        var matchResults6 = sensitiveWordSanitizer.FindMatches("这里面包含TMD吗？");
        Assert.Single(matchResults6);
        Assert.Equal(["TMD!"], matchResults6.Select(u => u.Word));
        Assert.Equal(["[TMD!] @ 5..8"], matchResults6.Select(u => u.ToString()));

        var matchResults7 = sensitiveWordSanitizer.FindMatches("这里面包含tmd吗？");
        Assert.Empty(matchResults7);
    }

    [Fact]
    public void FindMatches_SetIgnoreCaseFalse_And_SetIgnoreSymbolFalse_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromPath(filePath,
            new SensitiveWordOptions { IgnoreCase = false, IgnoreSymbol = false });

        var matchResults1 = sensitiveWordSanitizer.FindMatches("这里呢，是否包含敏__感__词呢？");
        Assert.Single(matchResults1);
        Assert.Equal(["敏感词"], matchResults1.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 8..15"], matchResults1.Select(u => u.ToString()));

        var matchResults2 = sensitiveWordSanitizer.FindMatches("这里面包含敏\r感\n词吗？");
        Assert.Single(matchResults2);
        Assert.Equal(["敏感词"], matchResults2.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 5..10"], matchResults2.Select(u => u.ToString()));

        var matchResults3 = sensitiveWordSanitizer.FindMatches("这里面包含敏\t感\n词吗？");
        Assert.Single(matchResults3);
        Assert.Equal(["敏感词"], matchResults3.Select(u => u.Word));
        Assert.Equal(["[敏感词] @ 5..10"], matchResults3.Select(u => u.ToString()));

        var matchResults4 = sensitiveWordSanitizer.FindMatches("Low High");
        Assert.Empty(matchResults4);

        var matchResults5 = sensitiveWordSanitizer.FindMatches("这里面包含敏*感*词吗？");
        Assert.Empty(matchResults5);

        var matchResults6 = sensitiveWordSanitizer.FindMatches("这里面包含TMD吗？");
        Assert.Empty(matchResults6);

        var matchResults7 = sensitiveWordSanitizer.FindMatches("这里面包含tmd吗？");
        Assert.Empty(matchResults7);
    }

    [Fact]
    public void FindFirst_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromPath(filePath);

        Assert.Null(sensitiveWordSanitizer.FindFirst(null));
        Assert.Null(sensitiveWordSanitizer.FindFirst(string.Empty));

        var matchResult = sensitiveWordSanitizer.FindFirst(userText);
        Assert.NotNull(matchResult);
        Assert.Equal("加微信", matchResult.Value.Word);
        Assert.Equal("[加微信] @ 5..8", matchResult.Value.ToString());

        var matchResult2 = sensitiveWordSanitizer.FindFirst("这里呢，是否包含敏__感__词呢？");
        Assert.NotNull(matchResult2);
        Assert.Equal("敏感词", matchResult2.Value.Word);
        Assert.Equal("[敏感词] @ 8..15", matchResult2.Value.ToString());

        var matchResult3 = sensitiveWordSanitizer.FindFirst("这里面包含敏\r感\n词吗？");
        Assert.NotNull(matchResult3);
        Assert.Equal("敏感词", matchResult3.Value.Word);
        Assert.Equal("[敏感词] @ 5..10", matchResult3.Value.ToString());

        var matchResult4 = sensitiveWordSanitizer.FindFirst("这里面包含敏\t感\n词吗？");
        Assert.NotNull(matchResult4);
        Assert.Equal("敏感词", matchResult4.Value.Word);
        Assert.Equal("[敏感词] @ 5..10", matchResult4.Value.ToString());

        var matchResult5 = sensitiveWordSanitizer.FindFirst("Low High");
        Assert.NotNull(matchResult5);
        Assert.Equal("low_high", matchResult5.Value.Word);
        Assert.Equal("[low_high] @ 0..8", matchResult5.Value.ToString());

        var matchResult6 = sensitiveWordSanitizer.FindFirst("这里面包含敏*感*词吗？");
        Assert.NotNull(matchResult6);
        Assert.Equal("敏感词", matchResult6.Value.Word);
        Assert.Equal("[敏感词] @ 5..10", matchResult6.Value.ToString());

        var matchResult7 = sensitiveWordSanitizer.FindFirst("这里面包含TMD吗？");
        Assert.NotNull(matchResult7);
        Assert.Equal("TMD!", matchResult7.Value.Word);
        Assert.Equal("[TMD!] @ 5..8", matchResult7.Value.ToString());

        var matchResult8 = sensitiveWordSanitizer.FindFirst("这里面包含tmd吗？");
        Assert.NotNull(matchResult8);
        Assert.Equal("TMD!", matchResult8.Value.Word);
        Assert.Equal("[TMD!] @ 5..8", matchResult8.Value.ToString());
    }

    [Fact]
    public void FindFirst_SetIgnoreCaseFalse_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer =
            SensitiveWordSanitizer.CreateFromPath(filePath, new SensitiveWordOptions { IgnoreCase = false });

        var matchResult1 = sensitiveWordSanitizer.FindFirst("这里呢，是否包含敏__感__词呢？");
        Assert.NotNull(matchResult1);
        Assert.Equal("敏感词", matchResult1.Value.Word);
        Assert.Equal("[敏感词] @ 8..15", matchResult1.Value.ToString());

        var matchResult2 = sensitiveWordSanitizer.FindFirst("这里面包含敏\r感\n词吗？");
        Assert.NotNull(matchResult2);
        Assert.Equal("敏感词", matchResult2.Value.Word);
        Assert.Equal("[敏感词] @ 5..10", matchResult2.Value.ToString());

        var matchResult3 = sensitiveWordSanitizer.FindFirst("这里面包含敏\t感\n词吗？");
        Assert.NotNull(matchResult3);
        Assert.Equal("敏感词", matchResult3.Value.Word);
        Assert.Equal("[敏感词] @ 5..10", matchResult3.Value.ToString());

        var matchResult4 = sensitiveWordSanitizer.FindFirst("Low High");
        Assert.Null(matchResult4);

        var matchResult5 = sensitiveWordSanitizer.FindFirst("这里面包含敏*感*词吗？");
        Assert.NotNull(matchResult5);
        Assert.Equal("敏感词", matchResult5.Value.Word);
        Assert.Equal("[敏感词] @ 5..10", matchResult5.Value.ToString());

        var matchResult6 = sensitiveWordSanitizer.FindFirst("这里面包含TMD吗？");
        Assert.NotNull(matchResult6);
        Assert.Equal("TMD!", matchResult6.Value.Word);
        Assert.Equal("[TMD!] @ 5..8", matchResult6.Value.ToString());

        var matchResults7 = sensitiveWordSanitizer.FindFirst("这里面包含tmd吗？");
        Assert.Null(matchResults7);
    }

    [Fact]
    public void FindFirst_SetIgnoreCaseFalse_And_SetIgnoreSymbolFalse_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromPath(filePath,
            new SensitiveWordOptions { IgnoreCase = false, IgnoreSymbol = false });

        var matchResult1 = sensitiveWordSanitizer.FindFirst("这里呢，是否包含敏__感__词呢？");
        Assert.NotNull(matchResult1);
        Assert.Equal("敏感词", matchResult1.Value.Word);
        Assert.Equal("[敏感词] @ 8..15", matchResult1.Value.ToString());

        var matchResult2 = sensitiveWordSanitizer.FindFirst("这里面包含敏\r感\n词吗？");
        Assert.NotNull(matchResult2);
        Assert.Equal("敏感词", matchResult2.Value.Word);
        Assert.Equal("[敏感词] @ 5..10", matchResult2.Value.ToString());

        var matchResult3 = sensitiveWordSanitizer.FindFirst("这里面包含敏\t感\n词吗？");
        Assert.NotNull(matchResult3);
        Assert.Equal("敏感词", matchResult3.Value.Word);
        Assert.Equal("[敏感词] @ 5..10", matchResult3.Value.ToString());

        var matchResult4 = sensitiveWordSanitizer.FindFirst("Low High");
        Assert.Null(matchResult4);

        var matchResult5 = sensitiveWordSanitizer.FindFirst("这里面包含敏*感*词吗？");
        Assert.Null(matchResult5);

        var matchResult6 = sensitiveWordSanitizer.FindFirst("这里面包含TMD吗？");
        Assert.Null(matchResult6);

        var matchResult7 = sensitiveWordSanitizer.FindFirst("这里面包含tmd吗？");
        Assert.Null(matchResult7);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("这是一个正常的字符串", false)]
    [InlineData("有需要添加微信", true)]
    [InlineData("你大爷的", true)]
    [InlineData("low high", true)]
    [InlineData("Low High", true)]
    [InlineData("这里面包含敏*感*词吗？", true)]
    [InlineData("这里呢，是否包含敏__感__词呢？", true)]
    [InlineData("这里面包含敏\r感\n词吗？", true)]
    [InlineData("这里面包含敏\t感\n词吗？", true)]
    [InlineData("这里面包含TMD吗？", true)]
    [InlineData("这里面包含tmd吗？", true)]
    public void Contains_ReturnOK(string? text, bool result)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromPath(filePath);

        Assert.Equal(result, sensitiveWordSanitizer.Contains(text));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("这是一个正常的字符串", false)]
    [InlineData("有需要添加微信", true)]
    [InlineData("你大爷的", true)]
    [InlineData("low high", true)]
    [InlineData("Low High", false)]
    [InlineData("这里面包含敏*感*词吗？", true)]
    [InlineData("这里呢，是否包含敏__感__词呢？", true)]
    [InlineData("这里面包含敏\r感\n词吗？", true)]
    [InlineData("这里面包含敏\t感\n词吗？", true)]
    [InlineData("这里面包含TMD吗？", true)]
    [InlineData("这里面包含tmd吗？", false)]
    public void Contains_SetIgnoreCaseFalse_ReturnOK(string? text, bool result)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer =
            SensitiveWordSanitizer.CreateFromPath(filePath, new SensitiveWordOptions { IgnoreCase = false });

        Assert.Equal(result, sensitiveWordSanitizer.Contains(text));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("这是一个正常的字符串", false)]
    [InlineData("有需要添加微信", true)]
    [InlineData("你大爷的", true)]
    [InlineData("low high", true)]
    [InlineData("Low High", false)]
    [InlineData("这里面包含敏*感*词吗？", false)]
    [InlineData("这里呢，是否包含敏__感__词呢？", true)]
    [InlineData("这里面包含敏\r感\n词吗？", true)]
    [InlineData("这里面包含敏\t感\n词吗？", true)]
    [InlineData("这里面包含TMD吗？", false)]
    [InlineData("这里面包含tmd吗？", false)]
    public void Contains_SetIgnoreCaseFalse_And_SetIgnoreSymbolFalse_ReturnOK(string? text, bool result)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromPath(filePath,
            new SensitiveWordOptions { IgnoreCase = false, IgnoreSymbol = false });

        Assert.Equal(result, sensitiveWordSanitizer.Contains(text));
    }

    [Fact]
    public void Replace_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromPath(filePath);

        var newText = sensitiveWordSanitizer.Replace(userText);
        Assert.Equal(
            "测试开始：***，加-wechat；****：日结300元。***123456，****。****、**、**。还有***（带空格），***，***（全角空格），涉\\t政（制表符）。***测试：****、****、***！************* 和 ************，****（无空格），***，********，***********! ***********。*******。另外重叠匹配：******** ********。最后去***!。**** the bad words。Ⓕⓤc⒦ the bad words。",
            newText);
    }

    [Fact]
    public void Replace_SetIgnoreCaseFalse_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer =
            SensitiveWordSanitizer.CreateFromPath(filePath, new SensitiveWordOptions { IgnoreCase = false });

        var newText1 = sensitiveWordSanitizer.Replace("这里呢，是否包含敏__感__词呢？");
        Assert.Equal("这里呢，是否包含*******呢？", newText1);

        var newText2 = sensitiveWordSanitizer.Replace("这里面包含敏\r感\n词吗？");
        Assert.Equal("这里面包含*****吗？", newText2);

        var newText3 = sensitiveWordSanitizer.Replace("这里面包含敏\t感\n词吗？");
        Assert.Equal("这里面包含*****吗？", newText3);

        var newText4 = sensitiveWordSanitizer.Replace("Low High");
        Assert.Equal("Low High", newText4);

        var newText5 = sensitiveWordSanitizer.Replace("这里面包含敏*感*词吗？");
        Assert.Equal("这里面包含*****吗？", newText5);
    }

    [Fact]
    public void Replace_SetIgnoreCaseFalse_And_SetIgnoreSymbolFalse_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var sensitiveWordSanitizer = SensitiveWordSanitizer.CreateFromPath(filePath,
            new SensitiveWordOptions { IgnoreCase = false, IgnoreSymbol = false });

        var newText1 = sensitiveWordSanitizer.Replace("这里呢，是否包含敏__感__词呢？");
        Assert.Equal("这里呢，是否包含*******呢？", newText1);

        var newText2 = sensitiveWordSanitizer.Replace("这里面包含敏\r感\n词吗？");
        Assert.Equal("这里面包含*****吗？", newText2);

        var newText3 = sensitiveWordSanitizer.Replace("这里面包含敏\t感\n词吗？");
        Assert.Equal("这里面包含*****吗？", newText3);

        var newText4 = sensitiveWordSanitizer.Replace("Low High");
        Assert.Equal("Low High", newText4);

        var newText5 = sensitiveWordSanitizer.Replace("这里面包含敏*感*词吗？");
        Assert.Equal("这里面包含敏*感*词吗？", newText5);
    }

    [Theory]
    [InlineData('ｆ', 'f')]
    [InlineData('ｕ', 'u')]
    [InlineData('１', '1')]
    [InlineData('２', '2')]
    [InlineData('Ⓕ', 'F')]
    [InlineData('ⓤ', 'u')]
    [InlineData('c', 'c')]
    [InlineData('⒦', 'k')]
    public void NormalizeChar_ReturnOK(char c, char result) => Assert.Equal(result,
        SensitiveWordSanitizer.NormalizeChar(c, SensitiveWordOptions.Default));

    [Fact]
    public void InitSkipMap_ReturnOK()
    {
        var skipMap = SensitiveWordSanitizer.InitSkipMap();
        Assert.NotNull(skipMap);
        Assert.Equal(65536, skipMap.Length);
    }

    [Fact]
    public void TrieNode_New_ReturnOK()
    {
        var node = new SensitiveWordSanitizer.TrieNode { IsEnd = true };
        node.Fail = node;
        node.Children.Add('敏', new SensitiveWordSanitizer.TrieNode { IsEnd = true });
        node.MatchedWords.Add(("敏感词", 3));
        node.MatchedWordSet.Add("敏感词");

        Assert.NotNull(node.Fail);
        Assert.True(node.IsEnd);
        Assert.Single(node.Children);
        Assert.Single(node.MatchedWords);
        Assert.Single(node.MatchedWordSet);
    }
}