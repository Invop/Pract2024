using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ManekiApp.Server.Models.ManekiAppDB
{
    [Table("UserSubscription", Schema = "public")]
    public partial class UserSubscription
    {
        [Required]
        public Guid SubscriptionId { get; set; }

        public Subscription Subscription { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime SubscribedAt { get; set; }

        [Required]
        public DateTime EndsAt { get; set; }

        public bool IsCanceled { get; set; }

        public bool ReceiveNotifications { get; set; }

        [Key]
        [Required]
        public Guid Id { get; set; }
    }
}