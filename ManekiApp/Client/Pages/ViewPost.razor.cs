using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ManekiApp.Client.Pages
{
    public partial class ViewPost
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

        [Inject] 
        protected ManekiAppDBService ManekiAppDbService { get; set; }
        
        [Parameter]
        public string Id { get; set; }

        protected bool errorVisible;
        protected string error;

        protected bool PostExists = true;
        protected Server.Models.ManekiAppDB.Post Post = new Post();
        protected Server.Models.ManekiAppDB.AuthorPage Author = new Server.Models.ManekiAppDB.AuthorPage();

        protected bool isUserAuthor = false;
        protected bool isContentVisible = true;
        
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.Post>> GetPostById(Guid postId)
        {
            var filter = $"Id eq {postId}";
            var postOData = await ManekiAppDbService.GetPosts(filter: filter);
            return postOData;
        }

        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorById(Guid authorId)
        {
            var filter = $"Id eq {authorId}";
            var authorOData = await ManekiAppDbService.GetAuthorPages(filter: filter);
            return authorOData;
        }
        
        private async Task<ODataServiceResult<UserSubscription>> GetUserSubscriptionsByUserAndAuthor(string userId, Guid authorPageId)
        {
            var filter = $"UserId eq '{userId}' and Subscription/AuthorPageId eq {authorPageId}";
            var userSubscriptionsOData = await ManekiAppDbService.GetUserSubscriptions(filter: filter, expand: "Subscription");
            return userSubscriptionsOData;
        }
        
        protected override async Task OnInitializedAsync()
        {
            if (checkPostExistance(out var postId)) return;
            
            try
            {
                var postOData = await GetPostById(postId);
                if (postOData.Value.Any())
                {
                    Post = postOData.Value.First();
                    var authorOData = await GetAuthorById(Post.AuthorPageId);
                    Author = authorOData.Value.FirstOrDefault();

                    isUserAuthor = Author.UserId.Equals(Security.User.Id);
                    
                    if (Post.MinLevel != 0 && !isUserAuthor)
                    {
                        isContentVisible = await checkUserSubscriptions();
                    }
                    else
                    {
                        isContentVisible = true;
                    }
                }
                else
                {
                    PostExists = false;
                }
            }
            catch (Exception e)
            {
                errorVisible = true;
                error = e.Message;
            }
        }

        private async Task<bool> checkUserSubscriptions()
        {
            var userSubscriptionsOData = await GetUserSubscriptionsByUserAndAuthor(Security.User.Id, Author.Id);
            var userSubscriptions = userSubscriptionsOData.Value.ToList();

            foreach (var subscription in userSubscriptions)
            {
                if (subscription.Subscription.PermissionLevel >= Post.MinLevel &&
                    subscription.EndsAt >= DateTimeOffset.UtcNow) return true;
            }

            return false;
        }

        private bool checkPostExistance(out Guid postId)
        {
            if (!Guid.TryParse(Id, out postId))
            {
                PostExists = false;
                return true;
            }

            return false;
        }

        protected void redirectToAuthorPage()
        {
            NavigationManager.NavigateTo($"/author-page/{Author.Id.ToString()}");
        }

        private void RedirectEditPost()
        {
            NavigationManager.NavigateTo($"/edit-post/{Id}");
        }
    }
}