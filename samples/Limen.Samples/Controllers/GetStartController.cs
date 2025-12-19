namespace Limen.Samples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GetStartController
{
    [HttpPost /*ValidationOptions(["create"])*/]
    public Task<Custom> Post(Custom custom)
    {
        return Task.FromResult(custom);
    }
}