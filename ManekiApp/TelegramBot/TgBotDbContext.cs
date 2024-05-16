using Microsoft.EntityFrameworkCore;

namespace ManekiApp.TelegramBot;

public class TgBotDbContext : DbContext
{
    public TgBotDbContext(DbContextOptions<TgBotDbContext> options) : base(options)
    {
    }
}