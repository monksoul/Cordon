# Limen

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/Limen/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Limen.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Limen) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

Limen is a highly expressive data validation library built for .NET developers. With its fluent syntax and extensible
rule engine, it establishes precise "guardrails" for data flows. Whether validating API inputs, form submissions, or
cleansing heterogeneous data streams, Limen enables enterprise-grade validation with minimal code.

## Features

- **Non-intrusive Integration**: No configuration required—integrate directly into existing projects.
- **Fluent Validation Syntax**: Chainable, declarative APIs that make validation logic clear and readable.
- **Comprehensive Coverage**: Full validation support for fields, objects, nested structures, and collections.
- **Scenario-based Rule Activation**: Dynamically compose validation logic based on business requirements.
- **Multilingual Support**: Built-in internationalization with error messages available in multiple languages.
- **Highly Customizable**: Define custom validation logic and validation attributes to fit any business rule.
- **Dependency Injection Friendly**: Register directly into the .NET service container.
- **Asynchronous Validation Support**: Validation logic can be executed asynchronously; both sync and async scenarios
  are fully supported.
- **Flexible Architecture**: Designed for ease of use, extension, and maintainability.
- **Cross-platform and Dependency-free**: Runs on all major platforms with zero external dependencies.
- **High-Quality Code Assurance**: Built under strict coding standards, with 98% unit and integration test coverage.
- **.NET 8+ Compatibility**: Fully compatible with .NET 8 and later versions.

## Installation

```powershell
dotnet add package Limen
```

## Getting Started

We have many examples on our [homepage](https://furion.net/docs/limen/). Here's your first one to get you started:

```cs
public class User : IValidatableObject
{
    [Min(1)]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        return context.ValidateUsing<User>(validator =>
        {
            validator.RuleFor(u => u.Name).NotBlank().MinLength(3).UserName().WithMessage("{0} is not a valid internet username")
                     .RuleFor(u => u.Id).Max(int.MaxValue)
                     .RuleSet("rule", () => 
                     {
                         validator.RuleFor(u => u.Name).EmailAddress().WithMessage("{0} is not a valid email address format");
                     });
        });
    }
}
```

[More Documentation](https://furion.net/docs/limen/)

## Documentation

You can find the Limen documentation on our [homepage](https://furion.net/docs/limen/).

## Contributing

The main purpose of this repository is to continue developing the core of Limen, making it faster and easier to use.
The development of Limen is publicly hosted on [Gitee](https://gitee.com/dotnetchina/Limen), and we appreciate
community contributions for bug fixes and improvements.

## License

Limen is released under the [MIT](./LICENSE) open source license.

[![](./assets/baiqian.svg)](https://baiqian.com)