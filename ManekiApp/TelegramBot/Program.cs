using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManekiApp.TelegramBot;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        var services = new ServiceCollection();
        var startup = new Startup(configuration);
        startup.ConfigureServices(services);

        using var serviceProvider = services.BuildServiceProvider();
        var bot = serviceProvider.GetRequiredService<TelegramBot>();

        await bot.Start();
    }
}