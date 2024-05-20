using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("UserSubscription", Schema = "public")]
public class UserSubscription
{
    [Key, Column(Order = 0)]
    public Guid SubscriptionId { get; set; }

    [Key, Column(Order = 1)]
    public string UserId { get; set; }

    [Required]
    public DateTime SubscribedAt { get; set; }

    [Required]
    public DateTime EndsAt { get; set; }

    [Required]
    public bool IsCanceled { get; set; }

    [Required]
    public bool ReceiveNotifications { get; set; }

    [ForeignKey("SubscriptionId")]
    public Subscription Subscription { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
}