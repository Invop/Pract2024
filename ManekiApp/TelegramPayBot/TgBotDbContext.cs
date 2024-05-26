using Microsoft.EntityFrameworkCore;

namespace ManekiApp.TelegramPayBot;

public partial class TgBotDbContext : DbContext
{
    public TgBotDbContext(DbContextOptions<TgBotDbContext> options) : base(options)
    {
    }

    partial void OnModelBuilding(ModelBuilder builder);

    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);
        OnModelBuilding(builder);
    }
}