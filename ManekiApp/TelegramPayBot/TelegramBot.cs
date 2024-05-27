
using ManekiApp.Server.Models;
using ManekiApp.Server.Models.ManekiAppDB;
using ManekiApp.TelegramPayBot.Keyboard;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;

namespace ManekiApp.TelegramPayBot
{
    public class TelegramBot
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TelegramBotClient _client;
        private readonly KeyboardService _keyboardService;
        private ApplicationUser? currentUser;
        private const string PaymentProviderToken = "284685063:TEST:NGJhYzcwNzZmODA3";

        public TelegramBot(IServiceScopeFactory serviceScopeFactory, KeyboardService keyboardService, string? botToken)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _keyboardService = keyboardService;
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
        
        private async Task<bool> UserExistsAsync(string telegramId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
                currentUser = await context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
                return currentUser != null;
            }
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
                    var callbackQuery = update.CallbackQuery;
                    var callbackData = callbackQuery?.Data;
                    var chatId = callbackQuery?.Message?.Chat?.Id;
                    if (callbackData.StartsWith("Author"))
                    {
                        var authorIdString = callbackData.Substring("Author".Length);
                        if (Guid.TryParse(authorIdString, out Guid authorId))
                        {
                                await SendSubscriptionLevelsKeyboardAsync(chatId, authorId,
                                    cancellationToken);
                            break;
                        }
                    }
                    else if (callbackData.StartsWith("Subscription"))
                    {
                        var subscriptionIdString = callbackData.Substring("Subscription".Length);
                        if (Guid.TryParse(subscriptionIdString, out Guid subscriptionId))
                        {
                            await SendInvoiceAsync(botClient ,(long)chatId!, subscriptionId,
                                cancellationToken);
                            break;
                        }
                    }
                    
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
                case "Buy Subscription":
                    await SendAuthorsKeyboardAsync(update.Message.Chat.Id, cancellationToken);
                    break;
                case "Back":
                    await ShowMainMenu(update.Message.Chat.Id, cancellationToken);
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

            if (!await UserExistsAsync(telegramId))
            {
                await HandleNonExistingUser(message.Chat.Id, cancellationToken);
                return;
            }
            
            await ShowMainMenu(message.Chat.Id, cancellationToken);
        }

        private async Task ShowMainMenu(long chatId, CancellationToken cancellationToken)
        {
            var mainMenuKeyboard = _keyboardService.GetMainMenuKeyboard();

            await _client.SendTextMessageAsync(
                chatId,
                "USE SAMPLE CARD ONLY 4242 4242 4242 4242",
                replyMarkup: mainMenuKeyboard,
                cancellationToken: cancellationToken);
        }

        private async Task SendAuthorsKeyboardAsync(long chatId, CancellationToken cancellationToken)
        {   
            string telegramId = chatId.ToString();
            if (!await UserExistsAsync(telegramId))
            {
                await HandleNonExistingUser(chatId, cancellationToken);
                return;
            }
    
            List<KeyboardItem> userSubscriptions;
            using(var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>(); 
                userSubscriptions = context.UserSubscriptions
                    .Where(us => currentUser != null &&
                                 us.UserId == currentUser.Id)
                    .Select(us => new KeyboardItem { Text = us.Subscription.AuthorPage.Title, Value = "Author" + us.Subscription.AuthorPage.Id })
                    .ToList();
            }

            var authors = _keyboardService.GetAuthorsKeyboard(userSubscriptions);

            await _client.SendTextMessageAsync(
                chatId,
                "Choose author:",
                replyMarkup: authors,
                cancellationToken: cancellationToken);
        }
        
        private async Task SendSubscriptionLevelsKeyboardAsync(long? chatId, Guid authorId, CancellationToken cancellationToken)
        {
            string telegramId = chatId.ToString();
            if (!await UserExistsAsync(telegramId))
            {
                await HandleNonExistingUser(chatId, cancellationToken);
                return;
            }
            List<KeyboardItem> subscriptions = new List<KeyboardItem>();
            using(var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>(); 
                subscriptions = context.Subscriptions
                    .Where(s=>s.AuthorPageId == authorId)
                    .Where(s => s.PermissionLevel != 0)
                    .Select(s => new KeyboardItem { Text = $"{s.PermissionLevel} {s.Title} {s.Price}$", Value = "Subscription" + s.Id })
                    .ToList();
            }
            var subscriptionsKeyboard = _keyboardService.GetSubscriptionsKeyboard(subscriptions);
            
            await _client.SendTextMessageAsync(
                chatId,
                "Choose Subscription:",
                replyMarkup: subscriptionsKeyboard,
                cancellationToken: cancellationToken);

        }
        
        private async Task HandleNoShippingCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // var chatId = update.CallbackQuery?.Message.Chat.Id;
            // if(chatId.HasValue)
            // {
            //     await SendInvoiceAsync(botClient, chatId.Value, cancellationToken);
            // }
            // else
            // {
            //     Console.WriteLine("Chat Id is null");
            // }
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
        
        private async Task HandleNonExistingUser(long? chatId, CancellationToken cancellationToken)
        {
            // Send an error message if the user does not exist
            await _client.SendTextMessageAsync(
                chatId,
                "You are not a registered user. Please register on our platform.",
                cancellationToken: cancellationToken);
        }

        private async Task SendInvoiceAsync(ITelegramBotClient botClient, long chatId,Guid subscriptionId, CancellationToken cancellationToken)
        {

            Subscription? subscription;
            using(var scope = _serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
                subscription = await context.Subscriptions.FindAsync(subscriptionId);
            }

            if (subscription != null) 
            {
                decimal price = subscription.Price;
                string title = subscription.Title;

                // Create labeled price array for the invoice
                var prices = new[] { new LabeledPrice(title, (int)(price * 100)) };

                // Send the invoice
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
    }
}
