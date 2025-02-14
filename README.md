### **Cordon - 数据边界的智能警戒者**

**Cordon** 是一个为 .NET 开发者打造的**高表现力数据校验库**，通过**链式语法**与**可扩展规则引擎**，为数据流动设置精准的“警戒线”。无论是验证 API 输入、表单提交，还是清洗异构数据流，Cordon 都能以**极简代码**实现**企业级校验强度**。

#### 核心特性：

🔹 **开箱即用的规则库**  
内置 50+ 常用校验器（邮箱、电话、范围约束、正则模式等），覆盖典型业务场景。

```csharp
Cordon.For(user)
    .Require(u => u.Name).NotBlank().MaxLength(50)
    .Require(u => u.Email).IsEmail().DomainWhitelist("furion.net")
    .Require(u => u.Age).Between(18, 100);
```

🔹 **无缝扩展自定义逻辑**  
通过 **Lambda 表达式**或继承 `ValidationRule<T>`，注入领域特定规则：

```csharp
// 自定义规则：订单金额必须为 100 的整数倍
Cordon.Configure<Order>()
    .RuleFor(o => o.Amount)
    .Must(amount => amount % 100 == 0)
    .WithMessage("金额需为 100 的整倍数");
```

🔹 **流畅接口 (Fluent Interface)**  
通过链式调用组合复杂规则，代码如自然语言般清晰：

```csharp
Cordon.For(apiRequest)
    .Require(r => r.Timestamp).IsUtc()
    .Require(r => r.IP).IsIpAddress()
    .When(r => r.IsHighRisk).Then(r =>
        r.Require(r => r.Fingerprint).Matches("[A-Za-z0-9]{32}"));
```

🔹 **防御性校验策略**  
支持 **FailFast（快速失败）** 或 **BatchMode（批量收集异常）**，适配调试与生产环境的不同需求。

---

### **为什么选择 Cordon？**

- **🧩 极简集成**：通过 `Install-Package Cordon` 快速接入，零配置启动
- **⚡ 零反射性能**：基于表达式树编译，校验速度媲美手写代码
- **🌐 多语言支持**：内置错误消息本地化，轻松支持 i18n 场景

用 Cordon 为你的数据通道架起智能防线——**让非法数据在边界止步，让合法数据流畅通行**。
