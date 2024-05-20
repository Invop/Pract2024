using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("UserSubscription", Schema = "public")]
public class UserSubscription
{
    [Key]
    public Guid Id { get; set; }
    
    [Required] 
    [ForeignKey("Subscription")]
    public Guid SubscriptionId { get; set; }

    [Required] 
    [ForeignKey("ApplicationUser")]
    public string UserId { get; set; }

    [Required]
    public DateTime SubscribedAt { get; set; }

    [Required]
    public DateTime EndsAt { get; set; }

    [Required]
    public bool IsCanceled { get; set; }

    [Required]
    public bool ReceiveNotifications { get; set; }
    
    public Subscription Subscription { get; set; }

    public ApplicationUser ApplicationUser { get; set; }
}