using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.EntityFrameworkCore;

namespace ManekiApp.Server.Data
{
    /// <summary>
    /// Class ManekiAppDBContext.
    /// Implements the <see cref="DbContext" />
    /// </summary>
    /// <seealso cref="DbContext" />
    public partial class ManekiAppDBContext : DbContext
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
        /// Gets or sets the user chat notifications.
        /// </summary>
        /// <value>The user chat notifications.</value>
        public DbSet<UserChatNotification> UserChatNotifications { get; set; }
        /// <summary>
        /// Gets or sets the user chat payments.
        /// </summary>
        /// <value>The user chat payments.</value>
        public DbSet<UserChatPayment> UserChatPayments { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ManekiAppDBContext"/> class.
        /// </summary>
        /// <remarks>See <see href="https://aka.ms/efcore-docs-dbcontext">DbContext lifetime, configuration, and initialization</see>
        /// for more information and examples.</remarks>
        public ManekiAppDBContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManekiAppDBContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public ManekiAppDBContext(DbContextOptions<ManekiAppDBContext> options) : base(options)
        {
        }

        /// <summary>
        /// Called when [model building].
        /// </summary>
        /// <param name="builder">The builder.</param>
        partial void OnModelBuilding(ModelBuilder builder);

        /// <summary>
        /// Called when [model creating].
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);



            OnModelBuilding(builder);
        }

        /// <summary>
        /// Override this method to set defaults and configure conventions before they run. This method is invoked before
        /// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder)" />.
        /// </summary>
        /// <param name="configurationBuilder">The builder being used to set defaults and configure conventions that will be used to build the model for this context.</param>
        /// <remarks><para>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run. However, it will still run when creating a compiled model.
        /// </para>
        /// <para>
        /// See <see href="https://aka.ms/efcore-docs-pre-convention">Pre-convention model building in EF Core</see> for more information and
        /// examples.
        /// </para></remarks>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}