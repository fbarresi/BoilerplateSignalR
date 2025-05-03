using BoilerplateSignalR.Interfaces.Hardware;
using Microsoft.AspNetCore.Mvc;

namespace BoilerplateSignalR.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PlcController : ControllerBase
{
    private readonly ILogger<PlcController> logger;
    private readonly IPlc plc;

    public PlcController(ILogger<PlcController> logger, IPlc plc)
    {
        this.logger = logger;
        this.plc = plc;
    }
    
    [HttpGet]
    [Route("/variables/{variable}/read")]
    public Task<object> Read([FromRoute]string variable)
    {
        return plc.Read<object>(variable);
    }
    
    [HttpPost]
    [Route("/variables/{variable}/write")]
    public Task Write([FromRoute]string variable, [FromBody]object value)
    {
        return plc.Write(variable, value);
    }
}