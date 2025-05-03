namespace BoilerplateSignalR.Interfaces.HubClients;

public interface IPlcClient
{
    Task Update(string name, string value);
    Task Ack(string variableName, object value);
}