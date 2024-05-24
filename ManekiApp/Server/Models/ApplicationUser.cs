using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using ManekiApp.Server.Models.ManekiAppDB;
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
        
        public virtual UserVerificationCode UserVerificationCode { get; set; }
        public virtual AuthorPage AuthorPage { get; set; }
        
        public virtual UserChatPurchases UserChatPurchases { get; set; }
        
        public virtual UserNotificationChats UserNotificationChats { get; set; }
        public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }
}