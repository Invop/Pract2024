using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("Post", Schema = "public")]
public class Post
{
    [Key] [Required] public Guid Id { get; set; }

    public string Content { get; set; }

    public ICollection<Image> Images { get; set; } = new List<Image>();
    
    [Required] public DateTime CreatedAt { get; set; }
    
    [Required] public DateTime EditedAt { get; set; }
    
    [Required] public Guid AuthorPageId { get; set; }
    
    [ForeignKey("AuthorPageId")]
    public AuthorPage AuthorPage { get; set; }
}