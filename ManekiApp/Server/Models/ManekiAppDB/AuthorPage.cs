using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ManekiApp.Server.Models.ManekiAppDB
{
    [Table("AuthorPage", Schema = "public")]
    public partial class AuthorPage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public byte[] ProfileImage { get; set; }

        public string SocialLinks { get; set; }

        [Required]
        public string UserId { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}