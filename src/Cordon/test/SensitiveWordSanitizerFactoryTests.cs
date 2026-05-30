// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

[Collection("SensitiveWordTests")]
public class SensitiveWordSanitizerFactoryTests
{
    [Fact]
    public void New_ReturnOK()
    {
        Assert.Equal("SensitiveWords:Default", SensitiveWordSanitizerFactory.DefaultName);
        Assert.NotNull(SensitiveWordSanitizerFactory._instances);
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void Get_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.Get(null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.Get(string.Empty));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.Get(" "));

        var exception = Assert.Throws<InvalidOperationException>(() => SensitiveWordSanitizerFactory.Get("not-found"));
        Assert.Equal(
            "The sensitive word dictionary 'not-found' has not been registered. Please register it using `SensitiveWordSanitizerFactory.GetOrCreate` at application startup.",
            exception.Message);
    }

    [Fact]
    public void Get_ReturnOK()
    {
        const string dictionaryName = nameof(SensitiveWordSanitizerFactory.Get);
        var sensitiveWordSanitizer =
            SensitiveWordSanitizerFactory.GetOrCreate(dictionaryName, builder => builder.AddWord("敏感词"));

        var sensitiveWordSanitizer2 = SensitiveWordSanitizerFactory.Get(dictionaryName);
        var sensitiveWordSanitizer3 = SensitiveWordSanitizerFactory.Get(dictionaryName);
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer2);
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer3);

        Assert.True(SensitiveWordSanitizerFactory.TryRemove(dictionaryName));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void GetOrCreate_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreate(null!));

        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreate(null!, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreate(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreate(" ", null!));
        Assert.Throws<ArgumentNullException>(() =>
            SensitiveWordSanitizerFactory.GetOrCreate(nameof(SensitiveWordSanitizerFactory.GetOrCreate), null!));

        Assert.Throws<ArgumentNullException>(() =>
            SensitiveWordSanitizerFactory.GetOrCreate(null!, (Action<SensitiveWordSanitizerBuilder>)null!));
        Assert.Throws<ArgumentException>(() =>
            SensitiveWordSanitizerFactory.GetOrCreate(string.Empty, (Action<SensitiveWordSanitizerBuilder>)null!));
        Assert.Throws<ArgumentException>(() =>
            SensitiveWordSanitizerFactory.GetOrCreate(" ", (Action<SensitiveWordSanitizerBuilder>)null!));
        Assert.Throws<ArgumentNullException>(() =>
            SensitiveWordSanitizerFactory.GetOrCreate(nameof(SensitiveWordSanitizerFactory.GetOrCreate),
                (Action<SensitiveWordSanitizerBuilder>)null!));

        var exception = Assert.Throws<Exception>(() =>
            SensitiveWordSanitizerFactory.GetOrCreate("exception", () => throw new Exception("出错了")));
        Assert.Equal("出错了", exception.Message);
        Assert.False(SensitiveWordSanitizerFactory._instances.ContainsKey("exception"));
    }

    [Fact]
    public void GetOrCreate_ReturnOK()
    {
        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory.GetOrCreate(builder => builder.AddWord("敏感词"));
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.True(SensitiveWordSanitizerFactory._instances.ContainsKey(SensitiveWordSanitizerFactory.DefaultName));
        Assert.Same(sensitiveWordSanitizer,
            SensitiveWordSanitizerFactory.GetOrCreate(builder => builder.AddWord("敏感词")));

        const string dictionaryName = nameof(SensitiveWordSanitizerFactory.GetOrCreate);
        var sensitiveWordSanitizer2 =
            SensitiveWordSanitizerFactory.GetOrCreate(dictionaryName, builder => builder.AddWord("敏感词"));
        Assert.NotNull(sensitiveWordSanitizer2);
        Assert.True(SensitiveWordSanitizerFactory._instances.ContainsKey(dictionaryName));
        Assert.Same(sensitiveWordSanitizer2,
            SensitiveWordSanitizerFactory.GetOrCreate(dictionaryName, builder => builder.AddWord("敏感词")));

        var factory = () => new SensitiveWordSanitizerBuilder().AddWord("敏感词").Build();
        var sensitiveWordSanitizer3 = SensitiveWordSanitizerFactory.GetOrCreate("factory", factory);
        Assert.NotNull(sensitiveWordSanitizer3);
        Assert.Same(sensitiveWordSanitizer3, SensitiveWordSanitizerFactory.GetOrCreate("Factory", factory));

        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void Refresh_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.Refresh(null!, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.Refresh(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.Refresh(" ", null!));
        Assert.Throws<ArgumentNullException>(() =>
            SensitiveWordSanitizerFactory.Refresh(nameof(SensitiveWordSanitizerFactory.Refresh), null!));

        Assert.Throws<ArgumentNullException>(() =>
            SensitiveWordSanitizerFactory.Refresh(null!, (Action<SensitiveWordSanitizerBuilder>?)null));
        Assert.Throws<ArgumentException>(() =>
            SensitiveWordSanitizerFactory.Refresh(string.Empty));
        Assert.Throws<ArgumentException>(() =>
            SensitiveWordSanitizerFactory.Refresh(" "));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            SensitiveWordSanitizerFactory.Refresh(nameof(SensitiveWordSanitizerFactory.Refresh)));
        Assert.Equal(
            "The sensitive word dictionary 'Refresh' has not been registered. Cannot refresh an unregistered dictionary.",
            exception.Message);

        var exception2 = Assert.Throws<Exception>(() =>
            SensitiveWordSanitizerFactory.Refresh("exception", () => throw new Exception("出错了")));
        Assert.Equal("出错了", exception2.Message);
        Assert.False(SensitiveWordSanitizerFactory._instances.ContainsKey("exception"));
    }

    [Fact]
    public void Refresh_ReturnOK()
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"sensitive_words_{Guid.NewGuid()}.txt");

        try
        {
            File.WriteAllText(tempFilePath, "敏感词\n");
            var sensitiveWordSanitizer =
                SensitiveWordSanitizerFactory.GetOrCreate(builder => builder.AddPath(tempFilePath));
            Assert.NotNull(sensitiveWordSanitizer);
            Assert.True(
                SensitiveWordSanitizerFactory._instances.ContainsKey(SensitiveWordSanitizerFactory.DefaultName));
            Assert.True(sensitiveWordSanitizer.Contains("这里包含敏感词吗？"));
            Assert.False(sensitiveWordSanitizer.Contains("TMD!"));
            Assert.False(sensitiveWordSanitizer.Contains("Fuck!"));

            File.AppendAllText(tempFilePath, "TMD\n");
            SensitiveWordSanitizerFactory.Refresh();
            var refreshedSanitizer = SensitiveWordSanitizerFactory.Get(SensitiveWordSanitizerFactory.DefaultName);
            Assert.True(refreshedSanitizer.Contains("这里包含敏感词吗？"));
            Assert.True(refreshedSanitizer.Contains("TMD!"));
            Assert.False(refreshedSanitizer.Contains("Fuck!"));

            Assert.NotSame(sensitiveWordSanitizer, refreshedSanitizer);

            File.AppendAllText(tempFilePath, "fuck\n");
            SensitiveWordSanitizerFactory.Refresh(SensitiveWordSanitizerFactory.DefaultName);
            var refreshedSanitizer2 = SensitiveWordSanitizerFactory.Get(SensitiveWordSanitizerFactory.DefaultName);
            Assert.True(refreshedSanitizer2.Contains("这里包含敏感词吗？"));
            Assert.True(refreshedSanitizer2.Contains("TMD!"));
            Assert.True(refreshedSanitizer2.Contains("Fuck!"));

            Assert.NotSame(sensitiveWordSanitizer, refreshedSanitizer2);
            Assert.NotSame(refreshedSanitizer, refreshedSanitizer2);

            SensitiveWordSanitizerFactory.Refresh(builder => builder.AddWord("敏感词"));
            var refreshedSanitizer3 = SensitiveWordSanitizerFactory.Get(SensitiveWordSanitizerFactory.DefaultName);
            Assert.True(refreshedSanitizer3.Contains("这里包含敏感词吗？"));
            Assert.False(refreshedSanitizer3.Contains("TMD!"));
            Assert.False(refreshedSanitizer3.Contains("Fuck!"));

            SensitiveWordSanitizerFactory.Refresh(SensitiveWordSanitizerFactory.DefaultName,
                builder => builder.AddWord("敏感词").AddWord("TMD"));
            var refreshedSanitizer4 = SensitiveWordSanitizerFactory.Get(SensitiveWordSanitizerFactory.DefaultName);
            Assert.True(refreshedSanitizer4.Contains("这里包含敏感词吗？"));
            Assert.True(refreshedSanitizer4.Contains("TMD!"));
            Assert.False(refreshedSanitizer4.Contains("Fuck!"));

            SensitiveWordSanitizerFactory.Refresh(SensitiveWordSanitizerFactory.DefaultName,
                () => new SensitiveWordSanitizerBuilder().AddWords(["敏感词", "TMD", "fuck"]).Build());
            var refreshedSanitizer5 = SensitiveWordSanitizerFactory.Get(SensitiveWordSanitizerFactory.DefaultName);
            Assert.True(refreshedSanitizer5.Contains("这里包含敏感词吗？"));
            Assert.True(refreshedSanitizer5.Contains("TMD!"));
            Assert.True(refreshedSanitizer5.Contains("Fuck!"));
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }

        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void GetNames_ReturnOK()
    {
        SensitiveWordSanitizerFactory.GetOrCreate(builder => builder.AddWord("敏感词"));
        SensitiveWordSanitizerFactory.GetOrCreate("GetNames", builder => builder.AddWord("敏感词"));

        Assert.Equal(["SensitiveWords:Default", "GetNames"], SensitiveWordSanitizerFactory.GetNames());
        SensitiveWordSanitizerFactory.Clear();
    }

    [Fact]
    public void TryRemove_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.TryRemove(null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.TryRemove(string.Empty));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.TryRemove(" "));
    }

    [Fact]
    public void TryRemove_ReturnOK()
    {
        Assert.False(SensitiveWordSanitizerFactory.TryRemove("not-found"));
        SensitiveWordSanitizerFactory.GetOrCreate(builder => builder.AddWord("敏感词"));
        Assert.True(SensitiveWordSanitizerFactory.TryRemove(SensitiveWordSanitizerFactory.DefaultName));
        Assert.False(SensitiveWordSanitizerFactory._instances.ContainsKey(SensitiveWordSanitizerFactory.DefaultName));
    }

    [Fact]
    public void Clear_ReturnOK()
    {
        SensitiveWordSanitizerFactory.GetOrCreate(builder => builder.AddWord("敏感词"));
        SensitiveWordSanitizerFactory.Clear();
        Assert.False(SensitiveWordSanitizerFactory._instances.ContainsKey(SensitiveWordSanitizerFactory.DefaultName));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void SanitizerEntry_New_ReturnOK()
    {
        var sensitiveWordSanitizer = new SensitiveWordSanitizerBuilder().AddWord("敏感词").Build();
        var entry = new SensitiveWordSanitizerFactory.SanitizerEntry(() => sensitiveWordSanitizer,
            new Lazy<SensitiveWordSanitizer>(() => sensitiveWordSanitizer));

        Assert.NotNull(entry.Factory);
        Assert.NotNull(entry.LazyInstance);
        Assert.Same(sensitiveWordSanitizer, entry.Factory());
        Assert.Same(sensitiveWordSanitizer, entry.LazyInstance.Value);
    }
}