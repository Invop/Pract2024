using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

public class UserNotificationChats
{
    [Key]
    public Guid UserNotificationChatId { get; set; }

    [Required]
    public string TelegramChatId { get; set; }

    [ForeignKey("ApplicationUser")]
    public string UserId { get; set; }
    public virtual ApplicationUser ApplicationUser { get; set; }
}