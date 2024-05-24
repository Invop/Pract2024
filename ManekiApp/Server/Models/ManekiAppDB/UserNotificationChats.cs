using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

public class UserNotificationChats
{
    [Required]
    public string TelegramChatId { get; set; }
     
    [Key]
    [ForeignKey("ApplicationUser")]
    public string UserId { get; set; }
    public virtual ApplicationUser ApplicationUser { get; set; }
}