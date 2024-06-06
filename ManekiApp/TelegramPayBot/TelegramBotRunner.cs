using Hangfire;

namespace ManekiApp.TelegramPayBot;

public class TelegramBotRunner
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private TelegramBot bot;
    private readonly UserSubscriptionJobManager _userSubscriptionJobManager;
    public TelegramBotRunner(IServiceProvider serviceProvider, UserSubscriptionJobManager userSubscriptionJobManager)
    {
        _serviceProvider = serviceProvider;
        _userSubscriptionJobManager = userSubscriptionJobManager;
    }

    public async Task StartBotAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var botToken = scope.ServiceProvider.GetRequiredService<IConfiguration>()["BotToken"];
        var serviceScopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
        bot = new TelegramBot(serviceScopeFactory, botToken, _userSubscriptionJobManager);
        await bot.Start();
    }

}