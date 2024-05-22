using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.EntityFrameworkCore;

namespace ManekiApp.Server.Data
{
    public partial class ManekiAppDBContext : DbContext
    {
        

        public DbSet<UserVerificationCode> UserVerificationCodes { get; set; }
        public DbSet<AuthorPage> AuthorPages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        
        
        
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
            

            
            OnModelBuilding(builder);
        }
        
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}