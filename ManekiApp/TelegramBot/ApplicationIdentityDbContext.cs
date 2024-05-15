using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
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
}

public class ApplicationRole : IdentityRole
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }

    [JsonIgnore] public ICollection<ApplicationUser> Users { get; set; }
}

public class ApplicationUser : IdentityUser
{
    [JsonIgnore] [IgnoreDataMember] public override string PasswordHash { get; set; }


    public string TelegramId { get; set; }
    public bool TelegramConfirmed { get; set; }

    [NotMapped] public string Password { get; set; }

    [NotMapped] public string ConfirmPassword { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    [NotMapped]
    public string Name
    {
        get => UserName;
        set => UserName = value;
    }

    public ICollection<ApplicationRole> Roles { get; set; }
}