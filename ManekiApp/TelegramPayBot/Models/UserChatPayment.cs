using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.TelegramPayBot.Models;

public class UserChatPayment
{
    [Required]
    public string TelegramChatId { get; set; }
     
    [Key]
    [ForeignKey("ApplicationUser")]
    public string UserId { get; set; }
    public virtual ApplicationUser ApplicationUser { get; set; }
}