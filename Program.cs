using RaysApps;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RaysApps.Services.Message;
using RaysApps.Services.Theme;
using RaysApps.Services.WebPub;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://webpubsubapi.azurewebsites.net/")
    }); ;
builder.Services.AddScoped<IRaysTheme, RaysThemeService>();
builder.Services.AddScoped<IMessageService, MessagesService>();
// Dependency service to connect and Handle events from Azure WebPubSub service
builder.Services.AddScoped<IRaysAppsWebPubClient, RaysAppsWebPubClient>();

builder.Services.AddMudServices();
await builder.Build().RunAsync();
