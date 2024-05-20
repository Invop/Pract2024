using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB
{
    [Table("Image", Schema = "public")]
    public class Image
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public byte[] Data { get; set; }

        public string ContentType { get; set; }

        [Required]
        [ForeignKey("Post")]
        public Guid PostId { get; set; }

        public Post Post { get; set; }
    }
}