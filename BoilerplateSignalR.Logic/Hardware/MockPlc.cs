using System.Reactive;
using System.Reactive.Linq;
using batteries.Extensions;
using BoilerplateSignalR.Interfaces.Hardware;
using BoilerplateSignalR.Interfaces.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BoilerplateSignalR.Logic.Hardware;

public class MockPlc : BackgroundService, IPlc
{
    private readonly ILogger<MockPlc> logger;
    private readonly PlcSettings settings;

    public MockPlc(ILogger<MockPlc> logger, PlcSettings settings)
    {
        this.logger = logger;
        this.settings = settings;
    }
    public IObservable<T> CreateNotification<T>(string variable)
    {
        var random = new Random((int)DateTime.Now.Ticks);
        return Observable.Interval(TimeSpan.FromSeconds(0.5))
                .Select(_ => random.GetData<T>())
            ;
    }

    public IObservable<T> CreateNotification<T>(string variable, TimeSpan cycle) 
        => CreateNotification<T>(variable).CombineLatest(Observable.Timer(cycle), (v, _) => v);


    public Task<T> Read<T>(string variable)
    {
        return Task.FromResult(default(T));
    }

    public Task Write<T>(string variable, T value)
    {
        return Task.FromResult(Unit.Default);
    }

    public IObservable<object> CreateNotification(string variable)
    {
        if(variable.StartsWith("b"))
            return CreateNotification<bool>(variable).Select(i => i as object);
        return CreateNotification<int>(variable).Select(i => i as object);
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting MockPlc");
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping MockPlc");
        return base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Delay(-1, stoppingToken);
    }
}