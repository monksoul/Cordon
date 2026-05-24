// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

[Collection("SensitiveWordSanitizerFactoryTests")]
public class SensitiveWordSanitizerFactoryTests
{
    [Fact]
    public void New_ReturnOK()
    {
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
        List<string> words = ["敏感词", "违规表述", "涉政", "暴恐"];

        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory.GetOrCreate("default", words);
        var sensitiveWordSanitizer2 = SensitiveWordSanitizerFactory.Get("default");
        var sensitiveWordSanitizer3 = SensitiveWordSanitizerFactory.Get("default");
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer2);
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer3);

        Assert.True(SensitiveWordSanitizerFactory.TryRemove("default"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void GetOrCreate_Default_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreate(null!, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreate(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreate(" ", null!));
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreate("default", null!));
    }

    [Fact]
    public void GetOrCreate_Default_ReturnOK()
    {
        List<string> words = ["敏感词", "违规表述", "涉政", "暴恐"];

        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory.GetOrCreate("default", words);
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);

        var sensitiveWordSanitizer2 = SensitiveWordSanitizerFactory.GetOrCreate("default", words);
        Assert.Single(SensitiveWordSanitizerFactory._instances);
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer2);

        var sensitiveWordSanitizer3 = SensitiveWordSanitizerFactory.GetOrCreate("DEFAULT", words);
        Assert.Single(SensitiveWordSanitizerFactory._instances);
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer3);

        Assert.True(sensitiveWordSanitizer.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove("default"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void GetOrCreateFromPath_Default_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromPath(null!, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromPath(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromPath(" ", null!));
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromPath("file", null!));
        Assert.Throws<FileNotFoundException>(() =>
            SensitiveWordSanitizerFactory.GetOrCreateFromPath("file", "not-found.txt"));
    }

    [Fact]
    public void GetOrCreateFromPath_Default_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");

        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory.GetOrCreateFromPath("file", filePath);
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);

        var sensitiveWordSanitizer2 = SensitiveWordSanitizerFactory.GetOrCreateFromPath("file", filePath);
        Assert.Single(SensitiveWordSanitizerFactory._instances);
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer2);

        Assert.True(sensitiveWordSanitizer.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove("file"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void GetOrCreateFromPath_FilePathKey_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromPath(null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromPath(string.Empty));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromPath(" "));
        Assert.Throws<FileNotFoundException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromPath("not-found.txt"));
    }

    [Fact]
    public void GetOrCreateFromPath_FilePathKey_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);

        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory.GetOrCreateFromPath(filePath);
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);
        Assert.Equal(normalizedPath, SensitiveWordSanitizerFactory._instances.Keys.FirstOrDefault());

        var sensitiveWordSanitizer2 = SensitiveWordSanitizerFactory.GetOrCreateFromPath(filePath);
        Assert.Single(SensitiveWordSanitizerFactory._instances);
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer2);

        Assert.True(sensitiveWordSanitizer.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove(normalizedPath));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void GetOrCreateFromStream_Default_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromStream(null!, null!));
        Assert.Throws<ArgumentException>(() =>
            SensitiveWordSanitizerFactory.GetOrCreateFromStream(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreateFromStream(" ", null!));
        Assert.Throws<ArgumentNullException>(() =>
            SensitiveWordSanitizerFactory.GetOrCreateFromStream("stream", null!));
    }

    [Fact]
    public void GetOrCreateFromStream_Default_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        using var stream = File.OpenRead(filePath);

        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory.GetOrCreateFromStream("stream", stream);
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);

        var sensitiveWordSanitizer2 = SensitiveWordSanitizerFactory.GetOrCreateFromStream("stream", stream);
        Assert.Single(SensitiveWordSanitizerFactory._instances);
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer2);

        Assert.True(sensitiveWordSanitizer.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove("stream"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void GetOrCreate_Factory_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreate(null!, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreate(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.GetOrCreate(" ", null!));
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.GetOrCreate("factory", null!));

        Assert.Throws<Exception>(() =>
            SensitiveWordSanitizerFactory.GetOrCreate("factory", () => throw new Exception("出错了")));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void GetOrCreate_Factory_ReturnOK()
    {
        List<string> words = ["敏感词", "违规表述", "涉政", "暴恐"];

        var sensitiveWordSanitizer =
            SensitiveWordSanitizerFactory.GetOrCreate("factory", () => SensitiveWordSanitizer.Build(words));
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);

        var sensitiveWordSanitizer2 =
            SensitiveWordSanitizerFactory.GetOrCreate("factory", () => SensitiveWordSanitizer.Build(words));
        Assert.Single(SensitiveWordSanitizerFactory._instances);
        Assert.Same(sensitiveWordSanitizer, sensitiveWordSanitizer2);

        Assert.True(sensitiveWordSanitizer.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove("factory"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void Refresh_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.Refresh(null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.Refresh(string.Empty));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.Refresh(" "));

        var exception =
            Assert.Throws<InvalidOperationException>(() => SensitiveWordSanitizerFactory.Refresh("not-found"));
        Assert.Equal(
            "The sensitive word dictionary 'not-found' has not been registered. Cannot refresh an unregistered dictionary.",
            exception.Message);
    }

    [Fact]
    public void Refresh_ReturnOK()
    {
        var tempFilePath = Path.Combine(Path.GetTempPath(), $"sensitive_words_{Guid.NewGuid()}.txt");
        const string dictName = "refresh_file_test";

        try
        {
            File.WriteAllText(tempFilePath, "敏感词\n");
            var sanitizer = SensitiveWordSanitizerFactory.GetOrCreateFromPath(dictName, tempFilePath);
            Assert.Single(SensitiveWordSanitizerFactory._instances);
            Assert.True(sanitizer.Contains("敏感词"));
            Assert.False(sanitizer.Contains("TMD"));
            Assert.True(sanitizer.Contains("这里包含敏感词吗，TMD!"));

            File.AppendAllText(tempFilePath, "TMD\n");
            SensitiveWordSanitizerFactory.Refresh(dictName);
            Assert.Single(SensitiveWordSanitizerFactory._instances);
            var refreshedSanitizer = SensitiveWordSanitizerFactory.Get(dictName);
            Assert.True(refreshedSanitizer.Contains("敏感词"));
            Assert.True(refreshedSanitizer.Contains("TMD"));
            Assert.True(refreshedSanitizer.Contains("这里包含敏感词吗，TMD!"));

            Assert.NotSame(sanitizer, refreshedSanitizer);
            Assert.Single(SensitiveWordSanitizerFactory._instances);
        }
        finally
        {
            Assert.True(SensitiveWordSanitizerFactory.TryRemove(dictName));
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [Fact]
    public void Refresh_Default_ReturnOK()
    {
        List<string> words = ["敏感词", "违规表述", "涉政", "暴恐"];

        SensitiveWordSanitizerFactory.Refresh("default", words);
        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory._instances["default"];
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);

        Assert.True(sensitiveWordSanitizer.LazyInstance.Value.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove("default"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void RefreshFromPath_Default_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.RefreshFromPath(null!, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.RefreshFromPath(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.RefreshFromPath(" ", null!));
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.RefreshFromPath("file", null!));
        Assert.Throws<FileNotFoundException>(() =>
            SensitiveWordSanitizerFactory.RefreshFromPath("file", "not-found.txt"));
    }

    [Fact]
    public void RefreshFromPath_Default_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");

        SensitiveWordSanitizerFactory.RefreshFromPath("file", filePath);
        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory._instances["file"];
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);

        Assert.True(sensitiveWordSanitizer.LazyInstance.Value.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove("file"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void RefreshFromPath_FilePathKey_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.RefreshFromPath(null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.RefreshFromPath(string.Empty));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.RefreshFromPath(" "));
        Assert.Throws<FileNotFoundException>(() =>
            SensitiveWordSanitizerFactory.RefreshFromPath("not-found.txt"));
    }

    [Fact]
    public void RefreshFromPath_FilePathKey_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);

        SensitiveWordSanitizerFactory.RefreshFromPath(filePath);
        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory._instances[normalizedPath];
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);

        Assert.True(sensitiveWordSanitizer.LazyInstance.Value.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove(normalizedPath));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void RefreshFromStream_Default_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.RefreshFromStream(null!, null!));
        Assert.Throws<ArgumentException>(() =>
            SensitiveWordSanitizerFactory.RefreshFromStream(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.RefreshFromStream(" ", null!));
        Assert.Throws<ArgumentNullException>(() =>
            SensitiveWordSanitizerFactory.RefreshFromStream("stream", null!));
    }

    [Fact]
    public void RefreshFromStream_Default_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        using var stream = File.OpenRead(filePath);

        SensitiveWordSanitizerFactory.RefreshFromStream("stream", stream);
        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory._instances["stream"];
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);

        Assert.True(sensitiveWordSanitizer.LazyInstance.Value.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove("stream"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void Refresh_Factory_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.Refresh(null!, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.Refresh(string.Empty, null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.Refresh(" ", null!));
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.Refresh("factory", null!));

        Assert.Throws<Exception>(() =>
            SensitiveWordSanitizerFactory.Refresh("factory", () => throw new Exception("出错了")));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void Refresh_Factory_ReturnOK()
    {
        List<string> words = ["敏感词", "违规表述", "涉政", "暴恐"];

        SensitiveWordSanitizerFactory.Refresh("factory", () => SensitiveWordSanitizer.Build(words));
        var sensitiveWordSanitizer = SensitiveWordSanitizerFactory._instances["factory"];
        Assert.NotNull(sensitiveWordSanitizer);
        Assert.Single(SensitiveWordSanitizerFactory._instances);

        Assert.True(sensitiveWordSanitizer.LazyInstance.Value.Contains("这里包含敏感词吗"));

        Assert.True(SensitiveWordSanitizerFactory.TryRemove("factory"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
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
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
        Assert.False(SensitiveWordSanitizerFactory.TryRemove("default"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);

        List<string> words = ["敏感词", "违规表述", "涉政", "暴恐"];
        _ = SensitiveWordSanitizerFactory.GetOrCreate("default", words);
        Assert.Single(SensitiveWordSanitizerFactory._instances);
        Assert.True(SensitiveWordSanitizerFactory.TryRemove("default"));
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void Clear_ReturnOK()
    {
        List<string> words = ["敏感词", "违规表述", "涉政", "暴恐"];
        _ = SensitiveWordSanitizerFactory.GetOrCreate("default", words);

        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        _ = SensitiveWordSanitizerFactory.GetOrCreateFromPath("file", filePath);

        Assert.Equal(2, SensitiveWordSanitizerFactory._instances.Count);
        SensitiveWordSanitizerFactory.Clear();
        Assert.Empty(SensitiveWordSanitizerFactory._instances);
    }

    [Fact]
    public void ResolveFilePath_Invalid_Parameters()
    {
        Assert.Throws<ArgumentNullException>(() => SensitiveWordSanitizerFactory.RefreshFromPath(null!));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.RefreshFromPath(string.Empty));
        Assert.Throws<ArgumentException>(() => SensitiveWordSanitizerFactory.RefreshFromPath(" "));
    }

    [Fact]
    public void ResolveFilePath_ReturnOK()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "sensitive_words.txt");
        var normalizedPath = Path.GetFullPath(filePath);

        Assert.Equal(normalizedPath, SensitiveWordSanitizerFactory.ResolveFilePath("sensitive_words.txt"));
        Assert.Equal(normalizedPath, SensitiveWordSanitizerFactory.ResolveFilePath(filePath));
        Assert.Equal(normalizedPath, SensitiveWordSanitizerFactory.ResolveFilePath(normalizedPath));
    }
}