using BoilerplateSignalR.Interfaces.Hardware;
using BoilerplateSignalR.Interfaces.Settings;
using BoilerplateSignalR.Logic.Hardware;
using BoilerplateSignalR.Logic.Hubs;
using BoilerplateSignalR.Logic.Workers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

// Log

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// Fix listening port by adding kestrel config to your settings
// "Kestrel": {
//     "Endpoints": {
//         "Https": {
//             "Url": "https://*:8443",
//             "Certificate": {
//                 "Subject": "my-fancy-cert",
//                 "Store": "MY",
//                 "Location": "LocalMachine",
//                 "AllowInvalid": "true"
//             }
//         }
//     }
// }

// Options

builder.Services.AddOptions<ApplicationSettings>()
    .BindConfiguration("ApplicationSettings")
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddScoped(resolver => resolver.GetRequiredService<IOptions<ApplicationSettings>>().Value);

builder.Services.AddOptions<PlcSettings>()
    .BindConfiguration("PlcSettings")
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<PlcSettings>>().Value);

// Services

// backgroud services

builder.Services.AddSingleton<MockPlc>();
builder.Services.AddSingleton<IHostedService, MockPlc>(
    serviceProvider => serviceProvider.GetService<MockPlc>());
builder.Services.AddSingleton<IPlc, MockPlc>(
    serviceProvider => serviceProvider.GetService<MockPlc>());

builder.Services.AddHostedService<ClockWorker>();


// scoped services

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddControllers()
                .AddNewtonsoftJson();

builder.Services.AddHealthChecks();

builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "BoilerplateSignalR API",
        Description = "REST API to BoilerplateSignalR",
        Contact = new OpenApiContact
        {
            Name = "BoilerplateSignalR corporation",
            Email = "info@BoilerplateSignalR.com",
        }
    });
});

// Metrics

AppMetricsServiceCollectionExtensions.AddMetrics(builder.Services);
builder.Services.AddMetricsEndpoints();
builder.Services.AddMetricsTrackingMiddleware();
builder.Services.AddMvcCore().AddMetricsCore();

// allow run as Service

builder.Host.UseWindowsService()
            .UseSystemd()
            ;

//


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
//put your webpage into wwwroot folder for serving a self-hosted webpage
// learn more here: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-9.0
app.UseDefaultFiles();
app.UseStaticFiles();

// in case of static web ui you can simply use allow any origin
// app.UseCors(options => options.AllowAnyOrigin());

app.UseCors(options =>
    options.AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed((host) => true)
        .AllowCredentials()
);

app.MapControllers();
app.MapHealthChecks("/health");

// Hubs
app.MapHub<ClockHub>("/hubs/clock");
app.MapHub<PlcHub>("/hubs/plc");

app.UseMetricsAllMiddleware();
app.UseMetricsAllEndpoints();

app.Run();
