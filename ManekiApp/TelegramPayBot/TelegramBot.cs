using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace ManekiApp.TelegramPayBot;

public class TelegramBot
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TelegramBotClient _client;
    private const string PaymentProviderToken = "284685063:TEST:NGJhYzcwNzZmODA3";
    private Message message ;
    private long chatId = 0;
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
            HandleUpdateAsync,
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
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

        switch (update.Type)
        {
            case UpdateType.PreCheckoutQuery:
                await HandlePreCheckoutQueryAsync(botClient, update, cancellationToken);
                break;
            
            case UpdateType.Message:
                await HandleMessageAsync(botClient, update, cancellationToken);
                break;
            case UpdateType.CallbackQuery:
                switch (update.CallbackQuery?.Data)
                {
                    case "/noshipping":
                        await HandleNoShippingCommandAsync(botClient, update, cancellationToken);
                        break;
                    default:
                        Console.WriteLine($"Unhandled callback data: {update.CallbackQuery?.Data}");
                        break;
                }
                break;
            default:
                Console.WriteLine($"Unhandled update type: {update.Type}");
                break;
        }
    }

    private async Task HandlePreCheckoutQueryAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await botClient.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id, cancellationToken: cancellationToken);
    }

    private async Task HandleMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message.SuccessfulPayment != null)
        {
            await HandleSuccessfulPaymentAsync(update);
        }
        else if (update.Message?.Text != null)
        {
            await HandleTextMessageAsync(botClient, update, cancellationToken);
        }
    }

    private async Task HandleSuccessfulPaymentAsync(Update update)
    {
        SuccessfulPayment successfulPayment = update.Message.SuccessfulPayment;
        Console.WriteLine($"Successful payment: {successfulPayment}");
    }

    private async Task HandleTextMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        switch (update.Message.Text)
        {
            case "/start":
                await HandleStartCommandAsync(botClient, update, cancellationToken);
                break;
            
            default:
                Console.WriteLine($"Unhandled message text: {update.Message.Text}");
                break;
        }
    }

    private async Task HandleStartCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;
        string telegramId = message.Chat.Id.ToString();
        bool userExists;
        using(var scope = _serviceScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>(); 
            userExists = context.Users.Any(u => u.TelegramId == telegramId);
        }
        if (userExists)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new [] // First row
                {
                    InlineKeyboardButton.WithCallbackData("No Shipping", "/noshipping"),
                    InlineKeyboardButton.WithCallbackData("Button 2", "Button 2")
                },
                new [] // Second row
                {
                    InlineKeyboardButton.WithCallbackData("Button 3", "Button 3"),
                    InlineKeyboardButton.WithCallbackData("Button 4", "Button 4")
                }
            });

            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "Use /noshipping for an invoice without shipping. USE SAMPLE CARD ONLY 4242 4242 4242 4242",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }
        else 
        {
            // For example, send the error message to the user
            await botClient.SendTextMessageAsync(
                message.Chat.Id,
                "You are not a registered user. Please register on our platform.",
                cancellationToken: cancellationToken);
        }
    }

    private async Task HandleNoShippingCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery?.Message.Chat.Id;
        if(chatId.HasValue)
        {
            await SendInvoiceAsync(botClient, chatId.Value, cancellationToken);
        }
        else
        {
            Console.WriteLine("Chat Id is null");
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

    private async Task SendInvoiceAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
    {
        var prices = new[] { new LabeledPrice("Test", 100) };
        await botClient.SendInvoiceAsync(
            chatId,
            "Payment Example",
            "Payment Example using C# Telegram Bot",
            "Custom-Payload",
            PaymentProviderToken,
            "USD",
            prices,
            cancellationToken: cancellationToken);
    }
}