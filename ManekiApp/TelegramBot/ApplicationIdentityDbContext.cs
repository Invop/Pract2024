using ManekiApp.Server.Models;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManekiApp.TelegramBot;

/// <summary>
/// Class ApplicationIdentityDbContext.
/// Implements the <see cref="Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext{ManekiApp.Server.Models.ApplicationUser, ManekiApp.Server.Models.ApplicationRole, System.String}" />
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext{ManekiApp.Server.Models.ApplicationUser, ManekiApp.Server.Models.ApplicationRole, System.String}" />
public partial class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationIdentityDbContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationIdentityDbContext"/> class.
    /// </summary>
    public ApplicationIdentityDbContext()
    {
    }

    /// <summary>
    /// Called when [model building].
    /// </summary>
    /// <param name="builder">The builder.</param>
    partial void OnModelBuilding(ModelBuilder builder);

    /// <summary>
    /// Configures the schema needed for the identity framework.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<IdentityUserRole<string>>();

        OnModelBuilding(builder);
    }

    /// <summary>
    /// Gets or sets the user verification codes.
    /// </summary>
    /// <value>The user verification codes.</value>
    public DbSet<UserVerificationCode> UserVerificationCodes { get; set; }
    /// <summary>
    /// Gets or sets the author pages.
    /// </summary>
    /// <value>The author pages.</value>
    public DbSet<AuthorPage> AuthorPages { get; set; }
    /// <summary>
    /// Gets or sets the posts.
    /// </summary>
    /// <value>The posts.</value>
    public DbSet<Post> Posts { get; set; }
    /// <summary>
    /// Gets or sets the subscriptions.
    /// </summary>
    /// <value>The subscriptions.</value>
    public DbSet<Subscription> Subscriptions { get; set; }
    /// <summary>
    /// Gets or sets the user subscriptions.
    /// </summary>
    /// <value>The user subscriptions.</value>
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    /// <summary>
    /// Gets or sets the user chat notification.
    /// </summary>
    /// <value>The user chat notification.</value>
    public DbSet<UserChatNotification> UserChatNotification { get; set; }
}