using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ManekiApp.TelegramBot.Models;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }

    [JsonIgnore] public ICollection<ApplicationUser> Users { get; set; }
}