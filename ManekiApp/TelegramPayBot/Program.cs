

using Hangfire;
using Hangfire.PostgreSql;
using ManekiApp.TelegramPayBot;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5003", "https://localhost:5004");


builder.Services.AddSingleton<UserSubscriptionJobManager>();
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection")));


builder.Services.AddEntityFrameworkNpgsql().AddDbContext<HangfireDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("HangfireDBConnection"));
});

builder.Services.AddHangfire(x =>
    x.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireDBConnection")));

builder.Services.AddHangfireServer();


builder.Services.AddSingleton<TelegramBotRunner>();
var app = builder.Build();
app.UseHangfireDashboard("/dashboard");
app.UseHttpsRedirection();
var botRunner = app.Services.GetRequiredService<TelegramBotRunner>();
Task.Run(async () => await botRunner.StartBotAsync());
app.Run();