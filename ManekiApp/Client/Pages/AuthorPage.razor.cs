using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

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
        private IEnumerable<Subscription> subscriptions = new List<Subscription>();

        protected override async Task OnInitializedAsync()
        {
            var authorPage = await ManekiAppDB.GetAuthorPageById(id: AuthorPageId);
            if (Security.User.Id != authorPage.UserId)
            {

                var filter = $"AuthorPageId eq {AuthorPageId}";
                var subscriptionsOData = await ManekiAppDB.GetSubscriptions(filter: filter);
                subscriptions = subscriptionsOData.Value;

            }
        }
        private void NavigateToEditPage()
        {
            NavigationManager.NavigateTo("/edit-author-page");
        }
    }
}