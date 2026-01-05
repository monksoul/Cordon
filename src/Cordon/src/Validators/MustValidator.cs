// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     <see cref="MustValidator{T}" /> 内部静态类
/// </summary>
/// <remarks>可通过 <see cref="Must.False" /> 设置不满足条件时的异常消息。</remarks>
public static class Must
{
    /// <summary>
    ///     抛出 <see cref="ValidatorException" /> 异常
    /// </summary>
    /// <param name="message">错误消息</param>
    [DoesNotReturn]
    public static void False(string message) => ValidatorException.Throw(message);

    /// <summary>
    ///     抛出 <see cref="ValidatorException" /> 异常
    /// </summary>
    /// <param name="condition">条件</param>
    /// <param name="message">错误消息</param>
    public static void FalseIf(bool condition, string message)
    {
        if (condition)
        {
            ValidatorException.Throw(message);
        }
    }
}

/// <summary>
///     自定义条件成立时委托验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class MustValidator<T> : PredicateValidator<T>
{
    /// <inheritdoc />
    public MustValidator(Func<T, bool> condition)
        : base(condition)
    {
    }

    /// <inheritdoc />
    public MustValidator(Func<T, ValidationContext<T>, bool> condition)
        : base(condition)
    {
    }
}