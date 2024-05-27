

using Microsoft.EntityFrameworkCore;
using ManekiApp.TelegramPayBot;
using ManekiApp.TelegramPayBot.Keyboard;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection")));


builder.Services.AddSingleton<KeyboardService>();


builder.Services.AddSingleton<TelegramBot>(sp =>
    new TelegramBot(
        sp.GetRequiredService<IServiceScopeFactory>(),
        sp.GetRequiredService<KeyboardService>(),
        builder.Configuration["BotToken"])
);


var app = builder.Build();

app.UseHttpsRedirection();
var bot = app.Services.GetRequiredService<TelegramBot>();
await bot.Start();

app.Run();