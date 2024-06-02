namespace ManekiApp.TelegramBot;

public class TelegramBotRunner
{
    private readonly IServiceProvider _serviceProvider;
    private TelegramBot bot;

    public TelegramBotRunner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartBotAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var botToken = scope.ServiceProvider.GetRequiredService<IConfiguration>()["BotToken"];
        var serviceScopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();

        bot = new TelegramBot(serviceScopeFactory, botToken);
        await bot.Start();
    }

    public async Task NotifyUsersAsync(Guid authorId)
    {
        await bot.NotifyUsersAsync(_serviceProvider,authorId);
    }
}