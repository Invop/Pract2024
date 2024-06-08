
using ManekiApp.TelegramBot;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5005", "https://localhost:5006");


builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ManekiAppDBConnection"))
);

builder.Services.AddSingleton<TelegramBotRunner>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:5001", "https://localhost:5006")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});



var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

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
