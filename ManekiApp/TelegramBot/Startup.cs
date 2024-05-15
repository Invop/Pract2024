using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManekiApp.TelegramBot;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("ManekiAppDBConnection")));

        services.AddSingleton<TelegramBot>(sp =>
            new TelegramBot(sp.GetRequiredService<ApplicationIdentityDbContext>(), Configuration["BotToken"]));
    }
}