using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using ManekiApp.Server.Models;
using ManekiApp.Server.Models.ManekiAppDB;

namespace ManekiApp.Server.Data
{
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
            
            builder.Entity<UserVerificationCode>()
                .HasOne(uvc => uvc.User)
                .WithMany()
                .HasForeignKey(uvc => uvc.UserId)
                .HasConstraintName("ForeignKey_UserVerificationCode_ApplicationUser")
                .IsRequired();

            this.OnModelBuilding(builder);
        }
    }
}