

using Hangfire;
using Hangfire.PostgreSql;
using ManekiApp.Server.Models.ManekiAppDB;
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
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("https://localhost:5001", "https://localhost:5004")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});



builder.Services.AddSingleton<TelegramBotRunner>();
var app = builder.Build();
app.UseHangfireDashboard("/dashboard");
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);
app.MapPost("/setJobIdForFreeSub", async (UserSubscription userSubscription, IServiceProvider services) =>
{
    var context = services.GetRequiredService<ApplicationIdentityDbContext>();
    var jobManager = services.GetRequiredService<UserSubscriptionJobManager>();
    userSubscription.JobId = jobManager.ScheduleUserSubscriptionDeletionJob(userSubscription, userSubscription.SubscriptionId);
    context.UserSubscriptions.Update(userSubscription);
    await context.SaveChangesAsync();
    return Results.Accepted();
});



var botRunner = app.Services.GetRequiredService<TelegramBotRunner>();
Task.Run(async () => await botRunner.StartBotAsync());
app.Run();