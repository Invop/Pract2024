using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(x =>
    x.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireDBConnection"))
);


builder.Services.AddHangfireServer(x => x.SchedulePollingInterval = TimeSpan.FromSeconds(1));

var app = builder.Build();
app.UseHangfireDashboard();

app.UseHttpsRedirection();
app.Run();