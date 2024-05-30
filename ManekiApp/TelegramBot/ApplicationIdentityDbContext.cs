using ManekiApp.Server.Models.ManekiAppDB;
using ManekiApp.TelegramBot.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManekiApp.TelegramBot;

public partial class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options)
    {
    }

    public ApplicationIdentityDbContext()
    {
    }

    partial void OnModelBuilding(ModelBuilder builder);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<IdentityUserRole<string>>();

        OnModelBuilding(builder);
    }

    public DbSet<UserVerificationCode> UserVerificationCodes { get; set; }
    
    public DbSet<UserChatNotification> UserChatNotification { get; set; }
}