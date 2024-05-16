using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(x =>
    x.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage("Server=localhost;Database=Hangfire;User Id=sa;Password=ChangeMe1;TrustServerCertificate=True;")
);


builder.Services.AddHangfireServer(x => x.SchedulePollingInterval = TimeSpan.FromSeconds(1));

var app = builder.Build();
app.UseHangfireDashboard();

app.UseHttpsRedirection();
app.Run();
