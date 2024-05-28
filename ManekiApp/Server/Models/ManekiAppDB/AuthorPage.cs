using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("AuthorPage", Schema = "public")]
public partial class AuthorPage
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public string ProfileImage { get; set; }

    public string SocialLinks { get; set; }

    [Required]
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
    
    public ICollection<Post> Posts { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; }

    public AuthorPage()
    {
        this.Id = Guid.NewGuid();
    }
}