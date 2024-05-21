using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ManekiApp.Server.Models.ManekiAppDB
{
    [Table("Post", Schema = "public")]
    public partial class Post
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        public string Title { get; set; }
        
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime EditedAt { get; set; }

        [Required]
        [ForeignKey("AuthorPage")]
        public Guid AuthorPageId { get; set; }

        public AuthorPage AuthorPage { get; set; }

        public ICollection<Image> Images { get; set; }
    }
}