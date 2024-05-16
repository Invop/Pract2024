using Hangfire;
using Hangfire.PostgreSql;
using ManekiApp.TelegramBot;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(x =>
    x.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireDBConnection"))
);


builder.Services.AddDbContext<TgBotDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection")));

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection")));

builder.Services.AddSingleton<TelegramBot>(sp =>
    new TelegramBot(sp.GetRequiredService<IServiceScopeFactory>(), builder.Configuration["BotToken"]));

builder.Services.AddHangfireServer(x => x.SchedulePollingInterval = TimeSpan.FromSeconds(1));

var app = builder.Build();
app.UseHangfireDashboard();

app.UseHttpsRedirection();
var bot = app.Services.GetRequiredService<TelegramBot>();
await bot.Start();

app.Run();