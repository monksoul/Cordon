// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon;

/// <summary>
///     自定义条件不成立时委托验证器
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
public class MustUnlessValidator<T> : PredicateValidator<T>
{
    /// <inheritdoc />
    public MustUnlessValidator(Func<T, bool> condition)
        : base((Func<T, bool>?)condition is null ? null! : u => !condition(u))
    {
    }
}