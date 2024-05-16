using ManekiApp.TelegramBot.Models;
using Microsoft.EntityFrameworkCore;

namespace ManekiApp.TelegramBot;

public partial class TgBotDbContext : DbContext
{
    public TgBotDbContext(DbContextOptions<TgBotDbContext> options) : base(options)
    {
    }

    partial void OnModelBuilding(ModelBuilder builder);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserVerificationCode>()
            .HasIndex(p => p.UserId)
            .IsUnique();
        base.OnModelCreating(builder);
        OnModelBuilding(builder);
    }

    public DbSet<UserVerificationCode> UserVerificationCodes { get; set; }
}