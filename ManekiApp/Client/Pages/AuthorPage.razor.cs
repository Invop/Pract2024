using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManekiApp.Server.Models;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ManekiApp.Client.Pages
{
    public partial class AuthorPage
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        
        [Inject] protected ManekiAppDBService ManekiAppDB { get; set; }
        [Parameter]
        public Guid AuthorPageId { get; set; }

        private Guid authorId;
        private IEnumerable<Subscription> subscriptions = new List<Subscription>();

        private Server.Models.ManekiAppDB.AuthorPage author = new Server.Models.ManekiAppDB.AuthorPage();
        
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadAuthorData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading author data: {ex.Message}");
            }
            
        }

        private async Task LoadAuthorData()
        {
            author = await ManekiAppDB.GetAuthorPageById(id: AuthorPageId);
            if (author != null)
            {
                Console.WriteLine($"Loaded Author: {author.Title}");
                var filter = $"AuthorPageId eq {AuthorPageId}";
                var subscriptionsOData = await ManekiAppDB.GetSubscriptions(filter: filter);
                subscriptions = subscriptionsOData.Value;
                author.Subscriptions = subscriptions.ToList();
                author.Posts = (await ManekiAppDB.GetPosts(filter: filter)).Value.ToList();
                if (!string.IsNullOrEmpty(author.SocialLinks))
                {
                    Console.WriteLine($"Social Links: {author.SocialLinks}");
                }
                else
                {
                    Console.WriteLine("No social links provided");
                }
            }
            else
            {
                Console.WriteLine("Author is null");
            }
        }

        private void NavigateToEditPage()
        {
            NavigationManager.NavigateTo($"/edit-author-page/{AuthorPageId}");        
        }
        
        private IEnumerable<string> GetSocialLinks()
        {
            var links = new List<string>();
            if (!string.IsNullOrEmpty(author.SocialLinks))
            {
                var socialLinks = author.SocialLinks.Split(',');
                var icons = new Dictionary<string, string>
                {
                    { "youtube", "/images/youtube.png" },
                    { "instagram", "/images/instagram.png" },
                    { "telegram", "/images/telegram.png" },
                    { "tiktok", "/images/tiktok.png" },
                    { "facebook", "/images/facebook.png" },
                    { "twitter", "/images/twitter.png" },
                    { "twitch", "/images/twitch.png" },
                    { "pinterest", "/images/pinterest.png" }
                };

                foreach (var social in socialLinks)
                {
                    var key = social.Split(':')[0];
                    if (icons.ContainsKey(key))
                    {
                        links.Add(icons[key]);
                    }
                }
            }
            return links;
        }
        
    }
}