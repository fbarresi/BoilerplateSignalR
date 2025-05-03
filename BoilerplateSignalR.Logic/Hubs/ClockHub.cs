using BoilerplateSignalR.Interfaces.HubClients;
using Microsoft.AspNetCore.SignalR;

namespace BoilerplateSignalR.Logic.Hubs;

public class ClockHub : Hub<IClock>
{
    public async Task SendTimeToClients(DateTime dateTime)
    {
        await Clients.All.ShowTime(dateTime);
    }
}