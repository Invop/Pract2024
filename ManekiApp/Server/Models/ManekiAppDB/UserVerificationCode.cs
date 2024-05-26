using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("UserVerificationCode", Schema = "public")]
public class UserVerificationCode
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    public string UserId { get; set; }

    [Required]
    public int Code { get; set; }

    [Required]
    public DateTime ExpiryTime { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
}