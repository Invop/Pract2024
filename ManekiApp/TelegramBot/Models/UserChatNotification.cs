using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ManekiApp.TelegramBot.Models;

namespace ManekiApp.Server.Models.ManekiAppDB;

public class UserChatNotification
{
    [Required]
    public string TelegramChatId { get; set; }
     
    [Key]
    [ForeignKey("ApplicationUser")]
    public string UserId { get; set; }
    public virtual ApplicationUser ApplicationUser { get; set; }
}