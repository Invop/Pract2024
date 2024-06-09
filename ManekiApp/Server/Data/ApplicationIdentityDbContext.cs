using ManekiApp.Server.Models;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManekiApp.Server.Data
{
    /// <summary>
    /// Class ApplicationIdentityDbContext.
    /// Implements the <see cref="Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext{ManekiApp.Server.Models.ApplicationUser, ManekiApp.Server.Models.ApplicationRole, System.String}" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext{ManekiApp.Server.Models.ApplicationUser, ManekiApp.Server.Models.ApplicationRole, System.String}" />
    public partial class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
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
        /// Gets or sets the images.
        /// </summary>
        /// <value>The images.</value>
        public DbSet<Image> Images { get; set; }
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
        /// <summary>
        /// Gets or sets the user chat payment.
        /// </summary>
        /// <value>The user chat payment.</value>
        public DbSet<UserChatPayment> UserChatPayment { get; set; }
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

            builder.Entity<UserVerificationCode>()
                .HasOne(uvc => uvc.User)
                .WithMany()
                .HasForeignKey(uvc => uvc.UserId)
                .HasConstraintName("ForeignKey_UserVerificationCode_ApplicationUser")
                .IsRequired();

            builder.Entity<Image>()
                .HasOne(i => i.Post)
                .WithMany(i => i.Images)
                .HasForeignKey(i => i.PostId)
                .HasPrincipalKey(i => i.Id);

            builder.Entity<Post>()
                .HasOne(i => i.AuthorPage)
                .WithMany(i => i.Posts)
                .HasForeignKey(i => i.AuthorPageId)
                .HasPrincipalKey(i => i.Id);

            builder.Entity<Subscription>()
                .HasOne(i => i.AuthorPage)
                .WithMany(i => i.Subscriptions)
                .HasForeignKey(i => i.AuthorPageId)
                .HasPrincipalKey(i => i.Id);

            builder.Entity<UserSubscription>()
                .HasOne(i => i.Subscription)
                .WithMany(i => i.UserSubscriptions)
                .HasForeignKey(i => i.SubscriptionId)
                .HasPrincipalKey(i => i.Id);

            builder.Entity<Subscription>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<ApplicationUser>()
                .HasOne(a => a.AuthorPage)
                .WithOne(b => b.User)
                .HasForeignKey<AuthorPage>(b => b.UserId)
                .HasConstraintName("ForeignKey_AuthorPage_ApplicationUser");

            builder.Entity<ApplicationUser>()
                .HasMany(a => a.UserSubscriptions)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .HasConstraintName("ForeignKey_UserSubscriptions_ApplicationUser");


            this.OnModelBuilding(builder);
        }
    }
}