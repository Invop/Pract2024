using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("UserVerificationCode", Schema = "public")]
public class UserVerificationCode
{
    [Key] public Guid Id { get; set; }

    [ForeignKey("User")] public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public int Code { get; set; }

    public DateTime ExpiryTime { get; set; }
}