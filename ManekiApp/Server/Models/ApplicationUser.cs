using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ManekiApp.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        [JsonIgnore, IgnoreDataMember]
        public override string PasswordHash { get; set; }
        
        
        public string TelegramId { get; set; }
        public bool TelegramConfirmed { get; set; }
        
        [NotMapped]
        public string Password { get; set; }

        [NotMapped]
        public string ConfirmPassword { get; set; }

        [JsonIgnore, IgnoreDataMember, NotMapped]
        public string Name
        {
            get
            {
                return UserName;
            }
            set
            {
                UserName = value;
            }
        }

        public ICollection<ApplicationRole> Roles { get; set; }
        
        /*
            New attributes
        */
        
        [Required] public string FirstName { get; set; }
        
        [Required] public string LastName { get; set; }
        
        [Required] public string About { get; set; }
        
        public byte[] ProfilePicture { get; set; }
        
        public ICollection<ManekiAppDB.UserSubscription> UserSubscriptions { get; set; }

    }
}