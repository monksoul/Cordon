// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Limen.Core.Extensions;

/// <summary>
///     <see cref="Type" /> 拓展类
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    ///     检查类型是否是静态类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsStatic(this Type type) => type is { IsSealed: true, IsAbstract: true };

    /// <summary>
    ///     检查类型是否可实例化
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsInstantiable(this Type type) =>
        type is { IsClass: true, IsAbstract: false }
        && !type.IsStatic();

    /// <summary>
    ///     检查类型是否是整数类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsInteger(this Type type)
    {
        // 如果是枚举或浮点类型则直接返回
        if (type.IsEnum || type.IsDecimal())
        {
            return false;
        }

        // 检查 TypeCode
        return Type.GetTypeCode(type) is TypeCode.Byte
            or TypeCode.SByte
            or TypeCode.Int16
            or TypeCode.Int32
            or TypeCode.Int64
            or TypeCode.UInt16
            or TypeCode.UInt32
            or TypeCode.UInt64;
    }

    /// <summary>
    ///     检查类型是否是小数类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsDecimal(this Type type)
    {
        // 如果是浮点类型则直接返回
        if (type == typeof(decimal)
            || type == typeof(double)
            || type == typeof(float))
        {
            return true;
        }

        // 检查 TypeCode
        return Type.GetTypeCode(type) is TypeCode.Double or TypeCode.Decimal;
    }

    /// <summary>
    ///     检查类型是否是数值类型
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <returns>
    ///     <see cref="bool" />
    /// </returns>
    internal static bool IsNumeric(this Type type) =>
        type.IsInteger()
        || type.IsDecimal();

    /// <summary>
    ///     创建实例属性值设置器
    /// </summary>
    /// <param name="type">
    ///     <see cref="Type" />
    /// </param>
    /// <remarks>不支持 <c>struct</c> 类型设置属性值。</remarks>
    /// <param name="propertyInfo">
    ///     <see cref="PropertyInfo" />
    /// </param>
    /// <returns>
    ///     <see cref="Action{T1, T2}" />
    /// </returns>
    internal static Action<object, object?> CreatePropertySetter(this Type type, PropertyInfo propertyInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_设置_属性名
        var setterMethod = new DynamicMethod(
            $"{type.FullName}_Set_{propertyInfo.Name}",
            null,
            [typeof(object), typeof(object)],
            typeof(TypeExtensions).Module,
            true
        );

        // 获取动态方法的 IL 生成器
        var ilGenerator = setterMethod.GetILGenerator();

        // 获取属性的设置方法，并允许非公开访问
        var setMethod = propertyInfo.GetSetMethod(true);

        // 空检查
        ArgumentNullException.ThrowIfNull(setMethod);

        // 将目标对象加载到堆栈上，并将其转换为所需的类型
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Castclass, type);

        // 将要分配的值加载到堆栈上
        ilGenerator.Emit(OpCodes.Ldarg_1);

        // 检查属性类型是否为值类型
        // 将值转换为属性类型
        // 对值进行拆箱，转换为适当的值类型
        ilGenerator.Emit(propertyInfo.PropertyType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass,
            propertyInfo.PropertyType);

        // 在目标对象上调用设置方法
        ilGenerator.Emit(OpCodes.Callvirt, setMethod);

        // 从动态方法返回
        ilGenerator.Emit(OpCodes.Ret);

        // 创建一个委托并将其转换为适当的 Action 类型
        return (Action<object, object?>)setterMethod.CreateDelegate(typeof(Action<object, object?>));
    }
}