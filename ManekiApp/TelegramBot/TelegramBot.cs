using ManekiApp.TelegramBot.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ManekiApp.TelegramBot;

public class TelegramBot
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TelegramBotClient _client;

    public TelegramBot(IServiceScopeFactory serviceScopeFactory, string? botToken)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _client = new TelegramBotClient(botToken);
    }

    public async Task Start()
    {
        using CancellationTokenSource cts = new();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _client.StartReceiving(
            async (botClient, update, cancellationToken) =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                await HandleUpdateAsync(scope.ServiceProvider, botClient, update, cancellationToken);
            },
            HandlePollingErrorAsync,
            receiverOptions,
            cts.Token
        );

        var me = await _client.GetMeAsync();

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        // Send cancellation request to stop bot
        cts.Cancel();
    }

    private static async Task HandleUpdateAsync(IServiceProvider services, ITelegramBotClient botClient,
        Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        // Get relevant services
        var context = services.GetRequiredService<ApplicationIdentityDbContext>();
        var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();

        var user = await context.Users.FirstOrDefaultAsync(u => u.TelegramId == chatId.ToString(), cancellationToken);

        if (user == null)
        {
            await botClient.SendTextMessageAsync(chatId, "User does not exist.", cancellationToken: cancellationToken);
            return;
        }

        using (var scope = scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

            // Get existing code or generate a new one
            var verificationCode = await dbContext
                .UserVerificationCodes
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ExpiryTime > DateTime.UtcNow, cancellationToken);

            if (verificationCode == null)
            {
                var newCode = CodeGenerator.GenerateCode();
                verificationCode = new UserVerificationCode
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Code = newCode,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(15)
                };

                await dbContext.UserVerificationCodes.AddAsync(verificationCode, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                await botClient.SendTextMessageAsync(chatId, $"Generated a new code: {newCode}",
                    cancellationToken: cancellationToken);
                return;
            }

            await botClient.SendTextMessageAsync(chatId, $"Your current code is: {verificationCode.Code}",
                cancellationToken: cancellationToken);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}