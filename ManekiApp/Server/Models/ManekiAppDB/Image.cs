using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ManekiApp.Server.Models.ManekiAppDB
{
    [Table("Image", Schema = "public")]
    public partial class Image
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        [Required]
        public byte[] Data { get; set; }

        public string ContentType { get; set; }

        [Required]
        public Guid PostId { get; set; }

        public Post Post { get; set; }
    }
}