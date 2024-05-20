using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("AuthorPage", Schema = "public")]
public class AuthorPage
{
    [Key] [Required] public Guid Id { get; set; }

    [Required] public string Title { get; set; }

    [Required] public string Description { get; set; }
    
    public byte[] ProfileImage { get; set; }
    
    public string SocialLinks { get; set; }

    [NotMapped]
    public Dictionary<string, string> SocialLinksDict
    {
        get => SocialLinks != null ? JsonSerializer.Deserialize<Dictionary<string, string>>(SocialLinks) : null;
        set => SocialLinks = value != null ? JsonSerializer.Serialize(value) : null;
    }
    
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    
    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser UserOwner { get; set; }
}