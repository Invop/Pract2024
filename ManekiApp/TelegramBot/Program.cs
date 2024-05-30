
using ManekiApp.TelegramBot;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection"))
);

builder.Services.AddSingleton<TelegramBotRunner>();

var app = builder.Build();


app.UseHttpsRedirection();
var botRunner = app.Services.GetRequiredService<TelegramBotRunner>();
Task.Run(async () => await botRunner.StartBotAsync());
// Map the /notify endpoint
app.MapPost("/notify", async (Guid authorId) =>
{
    var notificationService = app.Services.GetRequiredService<TelegramBotRunner>();
    await notificationService.NotifyUsersAsync(authorId);
    return Results.Accepted();
});

app.Run();