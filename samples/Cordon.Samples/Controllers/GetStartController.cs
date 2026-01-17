namespace Cordon.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController
{
    [HttpPost]
    public Task<string> Post([ValidateWith<NameValidator>] string name)
    {
        return Task.FromResult(name);
    }

    [HttpPost /*ValidationOptions(["create"])*/]
    public Task<Custom> Post2(Custom custom)
    {
        return Task.FromResult(custom);
    }

    [HttpPost]
    [ValidationOptions(["create"])]
    public Task<Custom> Post3([ValidateWith<CustomValidator>] Custom custom)
    {
        return Task.FromResult(custom);
    }

    [HttpPost]
    public Task<User> Post4(User user)
    {
        return Task.FromResult(user);
    }

    [HttpPost]
    public Task<Test> Post5(Test test)
    {
        return Task.FromResult(test);
    }
}

public class NameValidator : AbstractValueValidator<string>
{
    public NameValidator()
    {
        Rule().Required().MinLength(3);
    }
}

public class Test : IValidatableObject
{
    public int Id { get; set; }
    public List<string>? Names { get; set; }

    public Test2? Test2 { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return validationContext.With<Test>()
            .RuleFor(u => u.Id).Min(1)
            .RuleForCollection(u => u.Names).EachRules(u => u.Required().MinLength(3)).MinLength(1)
            .RuleFor(u => u.Test2).ChildRules(u =>
                u.RuleForCollection(d => d.Names).EachRules(d => d.Required().MinLength(3)).MinLength(1))
            .ToResults();
    }
}

public class Test2
{
    public List<string>? Names { get; set; }
}