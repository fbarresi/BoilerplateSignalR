using BoilerplateSignalR.Interfaces.Hardware;
using BoilerplateSignalR.Interfaces.HubClients;
using Microsoft.AspNetCore.SignalR;

namespace BoilerplateSignalR.Logic.Hubs;

public class PlcHub : Hub<IPlcClient>
{
    public async Task Notify(string variableName, string value)
        => await Clients.All.Update(variableName, value);

    public async Task Set(string variableName, object value, IPlc plc)
    {
        await plc.Write(variableName, value);
        await Clients.Caller.Ack(variableName, value);
    }
    
    public async Task Get(string variableName, IPlc plc)
    {
        var value = await plc.Read<string>(variableName);
        await Clients.Caller.Ack(variableName, value);
        await Clients.All.Update(variableName, value);
    }
}