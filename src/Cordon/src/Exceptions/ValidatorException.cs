// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     验证器异常类
/// </summary>
public sealed class ValidatorException : Exception
{
    /// <summary>
    ///     <inheritdoc cref="ValidatorException" />
    /// </summary>
    public ValidatorException()
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValidatorException" />
    /// </summary>
    /// <param name="errorMessage">错误信息</param>
    public ValidatorException(string? errorMessage)
        : base(errorMessage)
    {
    }

    /// <summary>
    ///     <inheritdoc cref="ValidatorException" />
    /// </summary>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="innerException">
    ///     <see cref="Exception" />
    /// </param>
    public ValidatorException(string? errorMessage, Exception innerException)
        : base(errorMessage, innerException)
    {
    }

    /// <summary>
    ///     抛出 <see cref="ValidatorException" /> 异常
    /// </summary>
    /// <param name="errorMessage">错误信息</param>
    [DoesNotReturn]
    public static void Throw(string? errorMessage) => throw new ValidatorException(errorMessage);
}