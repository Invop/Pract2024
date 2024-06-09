namespace ManekiApp.TelegramBot;

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
    /// The bot
    /// </summary>
    private TelegramBot bot;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotRunner"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public TelegramBotRunner(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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

        bot = new TelegramBot(serviceScopeFactory, botToken);
        await bot.Start();
    }

    /// <summary>
    /// Notify users as an asynchronous operation.
    /// </summary>
    /// <param name="authorId">The author identifier.</param>
    /// <param name="postTitle">The post title.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task NotifyUsersAsync(Guid authorId, string postTitle)
    {
        await bot.NotifyUsersAsync(_serviceProvider, authorId, postTitle);
    }
}