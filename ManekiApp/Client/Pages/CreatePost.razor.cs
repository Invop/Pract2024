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
    public partial class CreatePost
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
        protected ManekiAppDBService ManekiAppDBService { get; set; }

        protected bool isPreview = false;
        protected string PostTime;

        protected bool errorVisible;
        protected string error;
        
        protected Server.Models.ManekiAppDB.AuthorPage currentAuthor;
        protected Server.Models.ManekiAppDB.Post Post = new Post();
        
        protected int minLevelValue = 0;
        protected IEnumerable<Subscription> subscriptions;

        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPagesOData(string userId)
        {
            var filter = $"UserId eq '{userId}'";
            var authorPagesOData = await ManekiAppDBService.GetAuthorPages(filter: filter);
            return authorPagesOData;
        }

        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.Subscription>> GetSubscriptionsByAuthorPageOData(Guid authorPageId)
        {
            var filter = $"AuthorPageId eq {authorPageId}";
            var subscriptionsOData = await ManekiAppDBService.GetSubscriptions(filter: filter);
            return subscriptionsOData;
        }

        protected override async Task OnInitializedAsync()
        {
            var authorPagesOData = await GetAuthorPagesOData(Security.User.Id);
            if (authorPagesOData.Value.Any())
            {
                currentAuthor = authorPagesOData.Value.First();
                var subscriptionsOData = await GetSubscriptionsByAuthorPageOData(currentAuthor.Id);
                subscriptions = subscriptionsOData.Value.ToList();
            }
            else
            {
                NavigationManager.NavigateTo("/create-author-page");
            }
            
        }
        
        protected async Task FormSubmit(ManekiApp.Server.Models.ManekiAppDB.Post post)
        {
            try
            {
                Post.CreatedAt = DateTimeOffset.UtcNow;
                Post.EditedAt = Post.CreatedAt;
                Post.AuthorPageId = currentAuthor.Id;
                Post.MinLevel = minLevelValue;

                await ManekiAppDBService.CreatePost(post);
                
                NavigationManager.NavigateTo($"/post/{Post.Id}");
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }

        private void EnablePreview()
        {
            isPreview = true;
            PostTime = DateTime.Now.ToString("HH:mm");
        }

        private void DisablePreview()
        {
            isPreview = false;
        }
    }
}