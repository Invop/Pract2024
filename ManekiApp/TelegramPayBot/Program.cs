

using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using ManekiApp.TelegramPayBot;
using ManekiApp.TelegramPayBot.Keyboard;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection")));


builder.Services.AddEntityFrameworkNpgsql().AddDbContext<HangfireDbContext>(options => {
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