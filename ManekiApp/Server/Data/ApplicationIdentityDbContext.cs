using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using ManekiApp.Server.Models;
using ManekiApp.Server.Models.ManekiAppDB;

namespace ManekiApp.Server.Data
{
    public partial class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<UserVerificationCode> UserVerificationCodes { get; set; }
        public DbSet<AuthorPage> AuthorPages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<UserChatNotification> UserChatNotification { get; set; }
        public DbSet<UserChatPayment> UserChatPayment { get; set; }
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
                .HasPrecision(18,2);
            
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