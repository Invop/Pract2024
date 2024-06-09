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
        
        private async Task EnableNotifications(UserSubscription userSubscription)
        {
            userSubscription.ReceiveNotifications = true;
            await ManekiAppDB.UpdateUserSubscription(userSubscription.Id, userSubscription);
        }
        
        private async Task DisableNotifications(UserSubscription userSubscription)
        {
            userSubscription.ReceiveNotifications = false;
            await ManekiAppDB.UpdateUserSubscription(userSubscription.Id, userSubscription);
        }
        
        void NavigateToAuthorPage(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        private string GetAuthorProfilePicture(Guid userSubscriptionId)
        {
            var placeholderUrl = "https://ui-avatars.com/api/?name=John+Doe";
            
            var subscription = subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId);
            if (subscription == null)
            {
                return placeholderUrl;
            }

            var author = authorsUserFollows.FirstOrDefault(a => a.Id == subscription.AuthorPageId);
            if (author == null)
            {
                return placeholderUrl;
            }

            var pathString = !string.IsNullOrEmpty(author.ProfileImage) ?
                $"data:image/jpeg;base64,{author.ProfileImage}" :
                $"https://ui-avatars.com/api/?name={author.Title}";

            return pathString;
        }

        private string GetGradientColor(UserSubscription subscription)
        {
            Subscription userSubscription = subscriptions.FirstOrDefault(sub => sub.Id == subscription.SubscriptionId);
            int subscriptionPermissionLevel = userSubscription?.PermissionLevel ?? 0;
            
            string style =
                "background: linear-gradient(90deg, rgba(139,152,161,1) 0%, rgba(235,235,235,1) 50%, rgba(194,210,221,1) 100%);";

            if (subscriptionPermissionLevel == 1)
            {
                style = "background: linear-gradient(90deg, rgba(58,172,180,1) 0%, rgba(29,137,253,1) 50%, rgba(119,69,252,1) 100%);";
            }

            if (subscriptionPermissionLevel == 2)
            {
                style = "background: linear-gradient(90deg, rgba(244,179,92,1) 0%, rgba(253,200,29,1) 50%, rgba(252,168,69,1) 100%);";
            }

            if (subscriptionPermissionLevel == 3)
            {
                style = "background: linear-gradient(90deg, rgba(180,58,113,1) 0%, rgba(253,29,29,1) 50%, rgba(252,122,69,1) 100%);";
            }

            style += " height: 40px; display: flex; font-size: 25px; font-weight: 400;";

            return style;
        }
    }
}