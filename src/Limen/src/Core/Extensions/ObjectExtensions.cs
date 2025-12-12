// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Core.Extensions;

/// <summary>
///     <see cref="object" /> 拓展类
/// </summary>
internal static class ObjectExtensions
{
    /// <summary>
    ///     尝试获取对象的数量
    /// </summary>
    /// <param name="obj">
    ///     <see cref="object" />
    /// </param>
    /// <param name="count">数量</param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool TryGetCount(this object obj, out int count)
    {
        // 处理可直接获取长度的类型
        switch (obj)
        {
            // 检查对象是否是字符类型
            case char:
                count = 1;
                return true;
            // 检查对象是否是字符串类型
            case string text:
                count = text.Length;
                return true;
            // 检查对象是否实现了 ICollection 接口
            case ICollection collection:
                count = collection.Count;
                return true;
            // 检查对象是否实现了 IEnumerable 接口
            case IEnumerable enumerable:
                // 获取集合枚举数
                var enumerator = enumerable.GetEnumerator();

                try
                {
                    // 检查枚举数是否可以推进到下一个元素
                    if (!enumerator.MoveNext())
                    {
                        count = 0;
                        return true;
                    }

                    // 枚举数循环推进到下一个元素并叠加推进次数
                    var c = 1;
                    while (enumerator.MoveNext())
                    {
                        c++;
                    }

                    count = c;
                    return true;
                }
                finally
                {
                    // 检查枚举数是否实现了 IDisposable 接口
                    if (enumerator is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
        }

        // 反射查找是否存在 Count 属性
        var runtimeProperty = obj.GetType().GetRuntimeProperty("Count");

        // 反射获取 Count 属性值
        if (runtimeProperty is not null && runtimeProperty.CanRead && runtimeProperty.PropertyType == typeof(int))
        {
            count = (int)runtimeProperty.GetValue(obj)!;
            return true;
        }

        count = -1;
        return false;
    }
}