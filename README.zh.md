# Cordon

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/Cordon/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Cordon.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Cordon) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

Cordon 是一个为 .NET 开发者打造的高表现力数据校验库，通过链式语法与可扩展规则引擎，为数据流动设置精准的“警戒线”。无论是验证
API 输入、表单提交，还是清洗异构数据流，Cordon 都能以极简代码实现企业级校验强度。

## 特性

- **无侵入集成**：无需配置，可直接与现有项目集成。
- **流畅的验证语法**：链式、声明式 API，让验证逻辑清晰易读。
- **全场景覆盖**：支持字段、对象、嵌套结构与集合的完整验证。
- **按场景启用规则**：可针对不同业务需求动态组合验证逻辑。
- **多语言支持**：内置国际化机制，错误信息支持多语言切换。
- **高度可定制**：允许定义专属验证逻辑与验证特性，适配任意业务规则。
- **依赖注入友好**：可直接注册到 .NET 服务容器。
- **支持异步验证**：验证逻辑可异步执行，同步与异步场景均受支持。
- **架构设计**：架构设计灵活，易于使用与扩展。
- **跨平台无依赖**：支持跨平台运行，无需外部依赖。
- **高质量代码保障**：遵循高标准编码规范，拥有高达 `98%` 的单元测试与集成测试覆盖率。
- **`.NET 8+` 兼容性**：可在 `.NET 8` 及更高版本环境中部署使用。

## 安装

```powershell
dotnet add package Cordon
```

## 快速入门

我们在[主页](https://furion.net/docs/cordon/)上有不少例子，这是让您入门的第一个：

```cs
public class User : IValidatableObject
{
    [Min(1, ErrorMessage = "{0} 最小值不能小于 1")]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        return context.With<User>()
            .RuleFor(u => u.Name).NotBlank().MinLength(3).UserName().WithMessage("{0} 不是有效的互联网用户名")
            .RuleFor(u => u.Id).Max(int.MaxValue)
            // 支持规则集（场景）
            .RuleSet("rule", v => 
            {
                v.RuleFor(u => u.Name).EmailAddress().WithMessage("{0} 不是有效的电子邮箱格式");
            }).ToResults();
    }
}
```

[更多文档](https://furion.net/docs/cordon/)

## 文档

您可以在[主页](https://furion.net/docs/cordon/)找到 Cordon 文档。

## 贡献

该存储库的主要目的是继续发展 Cordon 核心，使其更快、更易于使用。Cordon
的开发在 [Gitee](https://gitee.com/dotnetchina/Cordon) 上公开进行，我们感谢社区贡献错误修复和改进。

## 许可证

Cordon 采用 [MIT](./LICENSE) 开源许可证。

[![](./assets/baiqian.svg)](https://baiqian.com)