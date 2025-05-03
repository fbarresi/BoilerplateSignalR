using Microsoft.AspNetCore.Mvc;

namespace BoilerplateSignalR.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> logger;

    public TestController(ILogger<TestController> logger)
    {
        this.logger = logger;
    }
    
    [HttpGet]
    [Route("try")]
    public Task Get()
    {
        return Task.CompletedTask;
    }
}