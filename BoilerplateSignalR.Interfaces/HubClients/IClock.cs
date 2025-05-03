namespace BoilerplateSignalR.Interfaces.HubClients;

public interface IClock
{
    Task ShowTime(DateTime currentTime);
}