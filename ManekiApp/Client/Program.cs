using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using ManekiApp.Client;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddRadzenComponents();
builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ManekiApp.Client.ManekiAppDBService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpClient("ManekiApp.Server", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ManekiApp.Server"));
builder.Services.AddScoped<ManekiApp.Client.SecurityService>();
builder.Services.AddScoped<AuthenticationStateProvider, ManekiApp.Client.ApplicationAuthenticationStateProvider>();
var host = builder.Build();
await host.RunAsync();