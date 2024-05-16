using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ManekiApp.TelegramBot.Models;

public class ApplicationUser : IdentityUser
{
    [JsonIgnore] [IgnoreDataMember] public override string PasswordHash { get; set; }


    public string TelegramId { get; set; }
    public bool TelegramConfirmed { get; set; }

    [NotMapped] public string Password { get; set; }

    [NotMapped] public string ConfirmPassword { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    [NotMapped]
    public string Name
    {
        get => UserName;
        set => UserName = value;
    }

    public ICollection<ApplicationRole> Roles { get; set; }
}