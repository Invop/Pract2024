using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.TelegramBot.Models;

[Table("UserVerificationCode", Schema = "public")]
public class UserVerificationCode
{
    [Key] [Required] public Guid Id { get; set; }

    public string UserId { get; set; }

    [Required] public int Code { get; set; }

    [Required] public DateTime ExpiryTime { get; set; }
}