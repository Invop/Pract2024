using Microsoft.EntityFrameworkCore;

namespace ManekiApp.TelegramPayBot;

public class HangfireDbContext : DbContext
{
    public HangfireDbContext(DbContextOptions<HangfireDbContext> options)
        : base(options) { }
}