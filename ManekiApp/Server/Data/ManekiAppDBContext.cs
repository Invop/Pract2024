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

            base.OnModelCreating(builder);
            this.OnModelBuilding(builder);
        }

        public DbSet<UserVerificationCode> UserVerificationCodes { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}