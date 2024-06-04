using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

                await sendNotificationRequest(currentAuthor.Id);
                
                NavigationManager.NavigateTo($"/post/{Post.Id}");
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }

        private async Task sendNotificationRequest(Guid authorId)
        {
            string baseAddress = "http://localhost:7033/notify/";

            // Combine the base address with the authorId
            string requestUri = $"{baseAddress}{authorId}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Prepare the request content if necessary
                    HttpContent content = new StringContent("");
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync(requestUri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Notification sent successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send notification. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
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