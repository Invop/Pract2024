using ManekiApp.Server.Models.ManekiAppDB;
using Telegram.Bot.Types.ReplyMarkups;

namespace ManekiApp.TelegramPayBot.Keyboard
{
    public class KeyboardService
    {
        public ReplyKeyboardMarkup GetMainMenuKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Buy Subscription" },
                new KeyboardButton[] { "Button 3", "Button 4" }
            })
            {
                ResizeKeyboard = true, // Зменшує клавіатуру до мінімального розміру
                OneTimeKeyboard = false // Клавіатура не ховається після натискання на кнопку
            };
        }

        public InlineKeyboardMarkup GetAuthorsKeyboard(List<KeyboardItem> authors)
        {

            return GenerateInlineKeyboardWithCallback(authors);
        }
        public InlineKeyboardMarkup GetSubscriptionsKeyboard(List<KeyboardItem> subscriptions)
        {
            return GenerateInlineKeyboardWithCallback(subscriptions);
        }

        public InlineKeyboardMarkup GenerateInlineKeyboardWithCallback(List<KeyboardItem> items)
        {
            var buttons = items.Select(item => new[]
            {
                InlineKeyboardButton.WithCallbackData(text: item.Text, callbackData: item.Value)
            }).ToArray();

            return new InlineKeyboardMarkup(buttons);
        }
    }
}