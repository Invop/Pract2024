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
    public partial class ManagePlans
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
        protected ManekiAppDBService ManekiService { get; set; }

        protected bool errorVisible;
        protected string error;

        protected List<Subscription> subscriptionsList = new List<Subscription>();
        
        protected override async Task OnInitializedAsync()
        {
            var currentAuthorResult = await GetAuthorPagesOData(Security.User.Id);
            var currentAuthor = currentAuthorResult?.Value?.FirstOrDefault();
            
            if (currentAuthor == null)
            {
                NavigationManager.NavigateTo("/create-author-page");
                return;
            }

            var subscriptionsResult = await GetSubscriptionsForAuthor(currentAuthor.Id);
            var subscriptions = subscriptionsResult?.Value?.ToList();

            subscriptionsList = subscriptions;
        }

        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPagesOData(string userId)
        {
            var filter = $"UserId eq '{userId}'";
            var authorPagesOData = await ManekiService.GetAuthorPages(filter: filter);
            return authorPagesOData;
        }
        
        private async Task<ODataServiceResult<Subscription>> GetSubscriptionsForAuthor(Guid authorPageId)
        {
            var filter = $"AuthorPageId eq {authorPageId} and PermissionLevel ne 0";
            var subscriptions = await ManekiService.GetSubscriptions(filter: filter);
            return subscriptions;
        }
        
        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }

        protected void ShowError(string message)
        {
            error = message;
            errorVisible = true;
        }
    }
}