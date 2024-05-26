

using Microsoft.EntityFrameworkCore;
using ManekiApp.TelegramPayBot;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<TgBotDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection")));

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection")));

builder.Services.AddSingleton<TelegramBot>(sp =>
    new TelegramBot(sp.GetRequiredService<IServiceScopeFactory>(), builder.Configuration["BotToken"]));


var app = builder.Build();

app.UseHttpsRedirection();
var bot = app.Services.GetRequiredService<TelegramBot>();
await bot.Start();

app.Run();