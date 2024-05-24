using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

public class UserChatPurchases
{
    [Key]
    public Guid UserChatPurchaseId { get; set; }

    // User's Telegram Chat ID where the purchase is made
    [Required]
    public string TelegramChatId { get; set; }

    [ForeignKey("ApplicationUser")]
    public string UserId { get; set; }
    public virtual ApplicationUser ApplicationUser { get; set; }
}