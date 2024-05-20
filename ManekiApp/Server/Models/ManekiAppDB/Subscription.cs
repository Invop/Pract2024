using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("Subscription", Schema = "public")]
public class Subscription
{
    [Required] [Key] public Guid Id { get; set; }
    
    [Required] public string Title { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")] 
    [Required] public decimal Price { get; set; }

    [Required] public string Description { get; set; }
    
    [Required] public int PermissionLevel { get; set; }
    
    [Required] 
    [ForeignKey("AuthorPage")]
    public Guid AuthorId { get; set; }
    
    public AuthorPage AuthorPage { get; set; }
    
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}