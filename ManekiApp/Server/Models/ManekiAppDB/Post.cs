using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mime;

namespace ManekiApp.Server.Models.ManekiAppDB;

[Table("Post", Schema = "public")]
public partial class Post
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    public string Content { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    [Required]
    public DateTimeOffset EditedAt { get; set; }

    [Required]
    public Guid AuthorPageId { get; set; }

    public AuthorPage AuthorPage { get; set; }

    public string Title { get; set; }

    public ICollection<Image> Images { get; set; }

    public int MinLevel { get; set; }

    public Post()
    {
        Id = Guid.NewGuid();
        MinLevel = 0;
    }
}