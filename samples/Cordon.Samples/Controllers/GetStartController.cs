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

    // [HttpPost /*ValidationOptions(["create"])*/]
    // public Task<Custom> Post3([ValidateWith<CustomValidator>] Custom custom)
    // {
    //     return Task.FromResult(custom);
    // }

    [HttpPost]
    public Task<User> Post3(User user)
    {
        return Task.FromResult(user);
    }
}

public class NameValidator : AbstractValueValidator<string>
{
    public NameValidator()
    {
        Rule().Required().MinLength(3);
    }
}