using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ManekiApp.Server.Models.ManekiAppDB
{
    [Table("Subscription", Schema = "public")]
    public partial class Subscription
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int PermissionLevel { get; set; }

        [Required]
        public Guid AuthorPageId { get; set; }

        public AuthorPage AuthorPage { get; set; }

        public ICollection<UserSubscription> UserSubscriptions { get; set; }
    }
}