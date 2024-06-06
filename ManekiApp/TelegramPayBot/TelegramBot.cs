
using Hangfire;
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
        private ApplicationUser? currentUser;
        private const string PaymentProviderToken = "284685063:TEST:NGJhYzcwNzZmODA3";
        private readonly UserSubscriptionJobManager _userSubscriptionJobManager;

        public TelegramBot(IServiceScopeFactory serviceScopeFactory, string botToken, UserSubscriptionJobManager userSubscriptionJobManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _client = new TelegramBotClient(botToken);
            _userSubscriptionJobManager = userSubscriptionJobManager;
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
                await HandleSuccessfulPaymentAsync(update,update.Message.From.Id.ToString(),update.Message.Chat.Id,cancellationToken);
            }
            else if (update.Message?.Text != null)
            {
                await HandleTextMessageAsync(botClient, update, cancellationToken);
            }

        }

        private async Task HandleSuccessfulPaymentAsync(Update update, string telegramId,long chatId, CancellationToken cancellationToken)
        {

            if (!await UserExistsAsync(telegramId))
            {
                await HandleNonExistingUser(chatId, cancellationToken);
                return;
            }
            SuccessfulPayment successfulPayment = update.Message.SuccessfulPayment;

            string invoicePayload = successfulPayment.InvoicePayload;
            if (invoicePayload.StartsWith("subscriptionId:"))
            {
                if (Guid.TryParse(invoicePayload.Substring("subscriptionId:".Length), out var subscriptionId))
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
                    var subscription = await context.Subscriptions
                        .FirstOrDefaultAsync(s => s.Id == subscriptionId);

                    if (subscription == null)
                    {
                        // Handle case where subscription does not exist
                        throw new Exception("Subscription not found");
                    }

                    var existingUserSubscription = await context.UserSubscriptions
                        .Include(us => us.Subscription)
                        .FirstOrDefaultAsync(us => us.UserId == currentUser.Id 
                                                   && us.Subscription.AuthorPageId == subscription.AuthorPageId);

                    if (existingUserSubscription != null)
                    {
                        var currentSubscription = existingUserSubscription.SubscriptionId;

                        bool isUpgradeOrDowngrade = currentSubscription != subscription.Id;

                        if (!isUpgradeOrDowngrade)
                        {
                            // Extend the existing subscription by 1 month
                            existingUserSubscription.EndsAt = existingUserSubscription.EndsAt.AddMonths(1);
                            Console.WriteLine("Extended");
                        }
                        else
                        {
                            // Update the existing subscription
                            existingUserSubscription.SubscriptionId = subscriptionId;
                            existingUserSubscription.EndsAt = DateTime.UtcNow.AddMonths(1);
                            existingUserSubscription.SubscribedAt = DateTime.UtcNow;
                            Console.WriteLine("Updated");
                        }
                        
                        _userSubscriptionJobManager.ScheduleUserSubscriptionDeletionJob(existingUserSubscription);
                    }
                    else
                    {
                        // No existing subscription to the author, create a new one
                        var newUserSubscription = new UserSubscription
                        {
                            Id = Guid.NewGuid(),
                            SubscriptionId = subscriptionId,
                            UserId = currentUser.Id,
                            SubscribedAt = DateTime.UtcNow,
                            EndsAt = DateTime.UtcNow.AddMonths(1),
                            ReceiveNotifications = true,
                            IsCanceled = false,
                            JobId = String.Empty
                        };
                        _userSubscriptionJobManager.ScheduleUserSubscriptionDeletionJob(newUserSubscription);
                        context.UserSubscriptions.Add(newUserSubscription);
                        Console.WriteLine("Created");
                    }

                    await context.SaveChangesAsync();

                }
            }
        }
        
        private async Task HandleTextMessageAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message.Text.StartsWith("/start subscription"))
            {
                await HandleStartSubscriptionCommandAsync(botClient, update, cancellationToken);
            }
        }

        private async Task HandleStartSubscriptionCommandAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            var command = "/start subscription";
            var subscriptionId = Guid.Parse(message.Text.Substring(command.Length).Trim());
            string telegramId = message.Chat.Id.ToString();

            if (!await UserExistsAsync(telegramId))
            {
                await HandleNonExistingUser(message.Chat.Id, cancellationToken);
                return;
            }
            await _client.SendTextMessageAsync(
                message.Chat.Id,
                "USE SAMPLE CARD ONLY 4242 4242 4242 4242",
                cancellationToken: cancellationToken);
            await SendInvoiceAsync(botClient ,message.Chat.Id, subscriptionId,
                    cancellationToken);
            
            
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
                if (price<=0)
                {
                    return;
                }
                // Create labeled price array for the invoice
                var prices = new[] { new LabeledPrice(title, (int)(price * 100)) };
                string invoicePayload = $"subscriptionId:{subscriptionId}";
                // Send the invoice
                await botClient.SendInvoiceAsync(
                    chatId,
                    "Payment Example",
                    "Payment Example using C# Telegram Bot",
                    invoicePayload,
                    PaymentProviderToken,
                    "USD",
                    prices,
                    cancellationToken: cancellationToken);
            }
        }

        public async Task DeleteUserSubscription(Guid id)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
            var userSubscription = await context.UserSubscriptions.FindAsync(id);

            if (userSubscription != null)
            {
                context.UserSubscriptions.Remove(userSubscription);
                await context.SaveChangesAsync();
            }
        }
    }
}
