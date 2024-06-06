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
    public partial class EditPost
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
        
        [Parameter]
        public string Id { get; set; }

        protected bool PostExists = true;
        protected bool isAuthor = true;
        
        protected bool isPreview = false;
        protected string PostTime;

        protected bool errorVisible;
        protected string error;
        
        protected Server.Models.ManekiAppDB.AuthorPage currentAuthor;
        protected Server.Models.ManekiAppDB.Post Post = new Post();
        
        protected int minLevelValue = 0;
        protected IEnumerable<Subscription> subscriptions;

        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.Post>> GetPostById(Guid postId)
        {
            var filter = $"Id eq {postId}";
            var postOData = await ManekiAppDBService.GetPosts(filter: filter, expand: "AuthorPage");
            return postOData;
        }

        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.Subscription>> GetSubscriptionsByAuthorPageOData(Guid authorPageId)
        {
            var filter = $"AuthorPageId eq {authorPageId}";
            var subscriptionsOData = await ManekiAppDBService.GetSubscriptions(filter: filter);
            return subscriptionsOData;
        }
        
        protected override async Task OnInitializedAsync()
        {
            if (validateGuid(out var postId)) return;
            
            try
            {
                var postOData = await GetPostById(postId);
                if (postOData.Value.Any())
                {
                    if (!checkIsAuthor(postOData.Value.First()))
                    {
                        isAuthor = false;
                        return;
                    }
                    
                    Post = postOData.Value.First();
                    var subscriptionsOData = await GetSubscriptionsByAuthorPageOData(Post.AuthorPageId);
                    subscriptions = subscriptionsOData.Value.ToList();
                    
                    minLevelValue = Post.MinLevel;
                    currentAuthor = Post.AuthorPage;
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

        private bool checkIsAuthor(Post post)
        {
            return post.AuthorPage.UserId.Equals(Security.User.Id);
        }

        private bool validateGuid(out Guid postId)
        {
            if (!Guid.TryParse(Id, out postId))
            {
                PostExists = false;
                return true;
            }

            return false;
        }
        
        protected async Task FormSubmit(ManekiApp.Server.Models.ManekiAppDB.Post post)
        {
            try
            {
                Post.EditedAt = DateTimeOffset.UtcNow;
                Post.MinLevel = minLevelValue;

                await ManekiAppDBService.UpdatePost(Post.Id, Post);
                
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

        private void CancelEdit()
        {
            NavigationManager.NavigateTo($"/post/{Id}");
        }

        private async void DeletePost()
        {
            try
            {
                var confirmOptions = new ConfirmOptions 
                { 
                    OkButtonText = "Yes", 
                    CancelButtonText = "No" 
                };

                var dialogResult = await DialogService.Confirm("Are you sure?", "You are going to delete a post", confirmOptions);

                if (dialogResult.HasValue && dialogResult.Value)
                {
                    await ManekiAppDBService.DeletePost(Post.Id);
                    NavigationManager.NavigateTo("/");
                }
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }
    }
}