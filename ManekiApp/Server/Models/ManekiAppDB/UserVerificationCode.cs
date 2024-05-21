using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ManekiApp.Server.Models.ManekiAppDB
{
    [Table("UserVerificationCode", Schema = "public")]
    public partial class UserVerificationCode
    {
        [Key]
        [Required]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [Required]
        public int Code { get; set; }

        [Required]
        public DateTime ExpiryTime { get; set; }
    }
}