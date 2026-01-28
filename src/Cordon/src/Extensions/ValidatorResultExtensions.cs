// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <see cref="ValidatorResult" /> 扩展类
/// </summary>
public static class ValidatorResultExtensions
{
    /// <summary>
    ///     验证验证失败时抛出 <see cref="ValidationException" /> 异常
    /// </summary>
    /// <param name="validatorResults"><see cref="ValidatorResult" />列表</param>
    public static void ThrowIfInvalid(this List<ValidatorResult> validatorResults)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(validatorResults);

        // 遍历所有验证器执行结果列表
        foreach (var validatorResult in validatorResults)
        {
            validatorResult.ThrowIfInvalid();
        }
    }
}