using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ManekiApp.Client.Pages
{
    public partial class UserSubscriptions
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


        private IEnumerable<UserSubscription> userSubscriptions = new List<UserSubscription>();
        private IEnumerable<Subscription> subscriptions = new List<Subscription>();
        private IEnumerable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> authorsUserFollows = new List<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>();

        private string GetSubscriptionTitle(Guid userSubscriptionId)
        {
            
            return subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)?.Title;
        }
        private string GetSubscriptionPrice(Guid userSubscriptionId)
        {
            
            return subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)?.Price + "$";
        }
        private string GetSubscriptionAuthor(Guid userSubscriptionId)
        {
            
            var authorPageId =  subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)!.AuthorPageId;
            return authorsUserFollows.FirstOrDefault(a => a.Id == authorPageId)?.Title;
        }

        private Guid? GetAuthorPage(Guid userSubscriptionId)
        {
            var authorPageId =  subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)!.AuthorPageId;
            return authorsUserFollows.FirstOrDefault(a => a.Id == authorPageId)?.Id;
        }
        protected override async Task OnInitializedAsync()
        {
            var userId = Security.User.Id;
            await GetDataFromDb(userId);
        }

        private async Task GetDataFromDb(string userId)
        {
            var userSubscriptionsOData = await GetUserSubscriptionsOData(userId);
            userSubscriptions = userSubscriptionsOData.Value;
            
            var subscriptionsOData = await GetSubscriptionsODataServiceResult();
            subscriptions = subscriptionsOData.Value;

            var authorsOData = await GetAuthorsODataServiceResult();
            authorsUserFollows = authorsOData.Value;
        }
        
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorsODataServiceResult()
        {
            var authorsIds = subscriptions.Select(s => s.AuthorPageId).ToList();
            var authorsIdsFilter = string.Join(" or ", authorsIds.Select(id => $"Id eq {id}"));
            var authorsOData = await ManekiAppDB.GetAuthorPages(filter: authorsIdsFilter);
            return authorsOData;
        }

        private async Task<ODataServiceResult<Subscription>> GetSubscriptionsODataServiceResult()
        {
            var userSubscriptionIds = userSubscriptions.Select(sub => sub.SubscriptionId).ToList();
            var idsFilter = string.Join(" or ", userSubscriptionIds.Select(id => $"Id eq {id}"));
            var subscriptionsOData = await ManekiAppDB.GetSubscriptions(filter: idsFilter);
            return subscriptionsOData;
        }

        private async Task<ODataServiceResult<UserSubscription>> GetUserSubscriptionsOData(string userId)
        {
            var filter = $"UserId eq '{userId}'";
            var userSubscriptionsOData = await ManekiAppDB.GetUserSubscriptions(filter: filter);
            return userSubscriptionsOData;
        }
        

        private async Task CancelSubscription(UserSubscription userSubscription)
        {
            userSubscription.IsCanceled = true;
            await ManekiAppDB.UpdateUserSubscription(userSubscription.Id, userSubscription);
        }
        
        void NavigateToAuthorPage(string url)
        {
            NavigationManager.NavigateTo(url);
        }
    }
}