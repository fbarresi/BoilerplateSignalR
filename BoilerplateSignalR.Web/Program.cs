using BoilerplateSignalR.Logic.Hubs;
using BoilerplateSignalR.Logic.Workers;
using BoilerplateSignalR.Web.Settings;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

//Log

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

// Add settings to the containers
builder.Services.AddOptions<WebAppSettings>()
    .BindConfiguration("WebAppSettings")
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<WebAppSettings>>().Value);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHealthChecks();

// uncomment here to allow serving local instance of SignalR
// builder.Services.AddSignalR();
//
// builder.Services.AddHostedService<ClockWorker>();

// Add usage over service
builder.Host.UseWindowsService()
            .UseSystemd();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(options => options.AllowAnyOrigin());

// app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// uncomment here to allow serving local instance of SignalR
// app.MapHub<ClockHub>("/hubs/clock");

app.Run();