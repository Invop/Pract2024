using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ManekiApp.Server.Models.ManekiAppDB;

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
            base.OnModelCreating(builder);

            builder.Entity<ManekiApp.Server.Models.ManekiAppDB.Image>()
              .HasOne(i => i.Post)
              .WithMany(i => i.Images)
              .HasForeignKey(i => i.PostId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<ManekiApp.Server.Models.ManekiAppDB.Post>()
              .HasOne(i => i.AuthorPage)
              .WithMany(i => i.Posts)
              .HasForeignKey(i => i.AuthorPageId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<ManekiApp.Server.Models.ManekiAppDB.Subscription>()
              .HasOne(i => i.AuthorPage)
              .WithMany(i => i.Subscriptions)
              .HasForeignKey(i => i.AuthorPageId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>()
              .HasOne(i => i.Subscription)
              .WithMany(i => i.UserSubscriptions)
              .HasForeignKey(i => i.SubscriptionId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<ManekiApp.Server.Models.ManekiAppDB.Subscription>()
              .Property(p => p.Price)
              .HasPrecision(18,2);
            this.OnModelBuilding(builder);
        }

        public DbSet<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> AuthorPages { get; set; }

        public DbSet<ManekiApp.Server.Models.ManekiAppDB.Image> Images { get; set; }

        public DbSet<ManekiApp.Server.Models.ManekiAppDB.Post> Posts { get; set; }

        public DbSet<ManekiApp.Server.Models.ManekiAppDB.Subscription> Subscriptions { get; set; }

        public DbSet<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> UserSubscriptions { get; set; }

        public DbSet<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> UserVerificationCodes { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}