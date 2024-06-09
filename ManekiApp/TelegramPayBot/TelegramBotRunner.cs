using Hangfire;

namespace ManekiApp.TelegramPayBot;

/// <summary>
/// Class TelegramBotRunner.
/// </summary>
public class TelegramBotRunner
{
    /// <summary>
    /// The service provider
    /// </summary>
    private readonly IServiceProvider _serviceProvider;
    /// <summary>
    /// The background job client
    /// </summary>
    private readonly IBackgroundJobClient _backgroundJobClient;
    /// <summary>
    /// The bot
    /// </summary>
    private TelegramBot bot;
    /// <summary>
    /// The user subscription job manager
    /// </summary>
    private readonly UserSubscriptionJobManager _userSubscriptionJobManager;
    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotRunner"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="userSubscriptionJobManager">The user subscription job manager.</param>
    public TelegramBotRunner(IServiceProvider serviceProvider, UserSubscriptionJobManager userSubscriptionJobManager)
    {
        _serviceProvider = serviceProvider;
        _userSubscriptionJobManager = userSubscriptionJobManager;
    }

    /// <summary>
    /// Start bot as an asynchronous operation.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task StartBotAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var botToken = scope.ServiceProvider.GetRequiredService<IConfiguration>()["BotToken"];
        var serviceScopeFactory = scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>();
        bot = new TelegramBot(serviceScopeFactory, botToken, _userSubscriptionJobManager);
        await bot.Start();
    }

}