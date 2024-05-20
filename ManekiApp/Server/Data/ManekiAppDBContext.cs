using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.EntityFrameworkCore;

namespace ManekiApp.Server.Data
{
    public partial class ManekiAppDBContext : DbContext
    {
        public ManekiAppDBContext()
        {
        }

        public ManekiAppDBContext(DbContextOptions<ManekiAppDBContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserVerificationCode>()
                .HasIndex(p => p.UserId)
                .IsUnique();
            base.OnModelCreating(builder);

            builder.Entity<Post>()
                .HasOne(p => p.AuthorPage)
                .WithMany(ap => ap.Posts)
                .HasForeignKey(p => p.AuthorPageId);
            
            builder.Entity<Subscription>()
                .HasOne(us => us.AuthorPage)
                .WithMany(ap => ap.Subscriptions)
                .HasForeignKey(s => s.AuthorId);
            
            builder.Entity<UserSubscription>()
                .HasKey(us => new { us.SubscriptionId, us.UserId });

            // One-to-many relationship between Subscription and UserSubscription
            builder.Entity<UserSubscription>()
                .HasOne(us => us.Subscription)
                .WithMany(s => s.UserSubscriptions)
                .HasForeignKey(us => us.SubscriptionId);

            // One-to-many relationship between ApplicationUser and UserSubscription
            builder.Entity<UserSubscription>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserSubscriptions)
                .HasForeignKey(us => us.UserId);        }

        public DbSet<UserVerificationCode> UserVerificationCodes { get; set; }
        public DbSet<AuthorPage> AuthorPages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}