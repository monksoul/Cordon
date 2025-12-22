// 版权归百小僧及百签科技（广东）有限公司所有。
// 
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Cordon.Tests;

internal static class Helpers
{
    internal static ValidatorBase GetValidator(ValidationAttribute attribute)
    {
        var validator = attribute.GetType().GetProperty("Validator", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(attribute);

        return (ValidatorBase)validator!;
    }
}