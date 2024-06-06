using ManekiApp.Server.Models;
using ManekiApp.Server.Models.ManekiAppDB;
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

    public async Task NotifyUsersAsync(IServiceProvider services,Guid authorPageId)
    {   
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
        var author = await dbContext.AuthorPages.FirstOrDefaultAsync(u => u.Id == authorPageId);
        if (author == null)
        {
            return;
        }

        var authorName = author.Title;

        var subscriptionsIds = dbContext.Subscriptions
            .Where(x => x.AuthorPageId == authorPageId)
            .Select(x=>x.Id)
            .ToList();
        var subscribersIds = dbContext.UserSubscriptions
            .Where(x => subscriptionsIds.Contains(x.SubscriptionId))
            .Where(x=>x.EndsAt >= DateTimeOffset.Now)
            .Where(x=>x.ReceiveNotifications)
            .Select(x => x.UserId)
            .Distinct()
            .ToList();
        var chatIds = dbContext.UserChatNotification
            .Where(x => subscribersIds.Contains(x.UserId) && !string.IsNullOrEmpty(x.TelegramChatId))
            .Select(x => long.Parse(x.TelegramChatId))
            .ToList();

        await Task.WhenAll(chatIds.Select(async chatId =>
        {
            await _client.SendTextMessageAsync(new ChatId(chatId), $"{authorName} Create new post");
        }));


    }
    
    private async Task HandleUpdateAsync(IServiceProvider services, ITelegramBotClient botClient,
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

        var user = await GetUserAsync(context, chatId.ToString(), cancellationToken);
        if (user == null)
        {
            await botClient.SendTextMessageAsync(chatId, "User does not exist.", cancellationToken: cancellationToken);
            return;
        }

        using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        await HandleUserChatNotificationAsync(dbContext, user, chatId.ToString(), cancellationToken);

        if (await IsUserVerifiedAsync(dbContext, user, cancellationToken))
        {
            await botClient.SendTextMessageAsync(chatId, "Your account has already been successfully verified.", cancellationToken: cancellationToken);
            return;
        }

        await HandleUserVerificationCodeAsync(dbContext, botClient, chatId, user, cancellationToken);
    }

    private async Task<ApplicationUser?> GetUserAsync(ApplicationIdentityDbContext context, string chatId, CancellationToken cancellationToken)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.TelegramId == chatId, cancellationToken);
    }

    private async Task HandleUserChatNotificationAsync(ApplicationIdentityDbContext dbContext, ApplicationUser user, string chatId, CancellationToken cancellationToken)
    {
        var userChatNotification = await dbContext.UserChatNotification.FirstOrDefaultAsync(n => n.UserId == user.Id, cancellationToken);

        if (userChatNotification == null)
        {
            userChatNotification = new UserChatNotification
            {
                UserId = user.Id,
                TelegramChatId = chatId
            };
            await dbContext.UserChatNotification.AddAsync(userChatNotification, cancellationToken);
        }
        else if (userChatNotification.TelegramChatId != chatId)
        {
            userChatNotification.TelegramChatId = chatId;
            dbContext.UserChatNotification.Update(userChatNotification);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> IsUserVerifiedAsync(ApplicationIdentityDbContext dbContext, ApplicationUser user, CancellationToken cancellationToken)
    {
        return await dbContext.Users.AnyAsync(c => c.TelegramConfirmed && c.Id == user.Id, cancellationToken);
    }

    private async Task HandleUserVerificationCodeAsync(ApplicationIdentityDbContext dbContext, ITelegramBotClient botClient, long chatId, ApplicationUser user, CancellationToken cancellationToken)
    {
        var existingCode = await dbContext.UserVerificationCodes.FirstOrDefaultAsync(c => c.UserId == user.Id, cancellationToken);
        if (existingCode != null)
        {
            if (existingCode.ExpiryTime <= DateTime.UtcNow)
            {
                existingCode.Code = CodeGenerator.GenerateCode();
                existingCode.ExpiryTime = DateTime.UtcNow.AddMinutes(15);

                dbContext.UserVerificationCodes.Update(existingCode);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            await botClient.SendTextMessageAsync(chatId, $"We've generated a new code for you: {existingCode.Code}", cancellationToken: cancellationToken);
        }
        else
        {
            var newCode = CodeGenerator.GenerateCode();
            var verificationCode = new UserVerificationCode
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Code = newCode,
                ExpiryTime = DateTime.UtcNow.AddMinutes(15)
            };

            await dbContext.UserVerificationCodes.AddAsync(verificationCode, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await botClient.SendTextMessageAsync(chatId, $"We've generated a new code for you: {newCode}", cancellationToken: cancellationToken);
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