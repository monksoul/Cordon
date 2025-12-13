namespace Limen.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController
{
    [HttpPost /*ValidationOptions(["create"])*/]
    public Task<User> Post(User user)
    {
        return Task.FromResult(user);
    }
}