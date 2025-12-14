// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen;

/// <summary>
///     固定失败验证器
/// </summary>
/// <remarks>仅用于输出指定错误消息。</remarks>
public sealed class FailureValidator : ValidatorBase
{
    /// <inheritdoc />
    public override bool IsValid(object? value) => false;
}