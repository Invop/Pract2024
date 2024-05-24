using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;

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
        using var scope = _serviceScopeFactory.CreateScope();
        if (update.Type == UpdateType.PreCheckoutQuery)
        {
            var preCheckoutQuery = update.PreCheckoutQuery;
            // Check preCheckoutQuery is not null if required
            await botClient.AnswerPreCheckoutQueryAsync(preCheckoutQuery.Id, cancellationToken: cancellationToken);

            Console.WriteLine(1);
            // Check Message is not null if required
            var paymentFromMessage = update.Message?.SuccessfulPayment;
            if (paymentFromMessage != null)
            {
                await botClient.SendTextMessageAsync(chatId, "Thank you for your payment!", cancellationToken: cancellationToken);

            }
        }
        else if (update.Type == UpdateType.ShippingQuery)
        {
            var shippingQuery = update.ShippingQuery;
            if (shippingQuery.InvoicePayload == "Custom-Payload")
            {
                var shippingOptions = new List<ShippingOption>
                {
                    new ShippingOption() { Id = "1", Title = "Shipping Option A", Prices = new[] { new LabeledPrice("A", 100) } },
                    new ShippingOption() { Id = "2", Title = "Shipping Option B", Prices = new[]
                    {
                        new LabeledPrice("B1", 150),
                        new LabeledPrice("B2", 200)
                    }}
                };
                await botClient.AnswerShippingQueryAsync(shippingQuery.Id, shippingOptions, cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.AnswerShippingQueryAsync(shippingQuery.Id, errorMessage: "Something went wrong...", cancellationToken: cancellationToken);
            }
        }
        else if (update.Type == UpdateType.Message && update.Message!.Text != null)
        {
            if (update.Message.Text == "/start")
            {
                var message = update.Message;
                chatId = message.Chat.Id;
                await botClient.SendTextMessageAsync(
                    chatId,
                    "Use /noshipping for an invoice without shipping. USE SAMPLE CARD ONLY 4242 4242 4242 4242",
                    cancellationToken: cancellationToken);
            }
            else if (update.Message.Text == "/noshipping")
            {
                 chatId = update.Message.Chat.Id; // Not sure where chatId is defined initially
                await SendInvoiceAsync(botClient, chatId, cancellationToken);
            }
            else
            {
                Console.WriteLine(1);
            }
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