using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("UserSubscription", Schema = "public")]
public partial class UserSubscription
{
    public Guid SubscriptionId { get; set; }

    public Subscription Subscription { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public DateTime SubscribedAt { get; set; }

    [Required]
    public DateTime EndsAt { get; set; }

    public bool IsCanceled { get; set; }

    public bool ReceiveNotifications { get; set; }

    [Key]
    [Required]
    public Guid Id { get; set; }
    
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
}