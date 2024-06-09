using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class UserSubscriptions.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class UserSubscriptions
    {
        /// <summary>
        /// Gets or sets the js runtime.
        /// </summary>
        /// <value>The js runtime.</value>
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        /// <value>The navigation manager.</value>
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        [Inject]
        protected DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the tooltip service.
        /// </summary>
        /// <value>The tooltip service.</value>
        [Inject]
        protected TooltipService TooltipService { get; set; }

        /// <summary>
        /// Gets or sets the context menu service.
        /// </summary>
        /// <value>The context menu service.</value>
        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        /// <summary>
        /// Gets or sets the notification service.
        /// </summary>
        /// <value>The notification service.</value>
        [Inject]
        protected NotificationService NotificationService { get; set; }

        /// <summary>
        /// Gets or sets the security.
        /// </summary>
        /// <value>The security.</value>
        [Inject]
        protected SecurityService Security { get; set; }
        /// <summary>
        /// Gets or sets the maneki application database.
        /// </summary>
        /// <value>The maneki application database.</value>
        [Inject] protected ManekiAppDBService ManekiAppDB { get; set; }


        /// <summary>
        /// The user subscriptions
        /// </summary>
        private IEnumerable<UserSubscription> userSubscriptions = new List<UserSubscription>();
        /// <summary>
        /// The subscriptions
        /// </summary>
        private IEnumerable<Subscription> subscriptions = new List<Subscription>();
        /// <summary>
        /// The authors user follows
        /// </summary>
        private IEnumerable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> authorsUserFollows = new List<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>();

        /// <summary>
        /// Gets the subscription title.
        /// </summary>
        /// <param name="userSubscriptionId">The user subscription identifier.</param>
        /// <returns>System.String.</returns>
        private string GetSubscriptionTitle(Guid userSubscriptionId)
        {

            return subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)?.Title;
        }
        /// <summary>
        /// Gets the subscription price.
        /// </summary>
        /// <param name="userSubscriptionId">The user subscription identifier.</param>
        /// <returns>System.String.</returns>
        private string GetSubscriptionPrice(Guid userSubscriptionId)
        {

            return subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)?.Price + "$";
        }
        /// <summary>
        /// Gets the subscription author.
        /// </summary>
        /// <param name="userSubscriptionId">The user subscription identifier.</param>
        /// <returns>System.String.</returns>
        private string GetSubscriptionAuthor(Guid userSubscriptionId)
        {

            var authorPageId = subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)!.AuthorPageId;
            return authorsUserFollows.FirstOrDefault(a => a.Id == authorPageId)?.Title;
        }

        /// <summary>
        /// Gets the author page.
        /// </summary>
        /// <param name="userSubscriptionId">The user subscription identifier.</param>
        /// <returns>System.Nullable&lt;Guid&gt;.</returns>
        private Guid? GetAuthorPage(Guid userSubscriptionId)
        {
            var authorPageId = subscriptions.FirstOrDefault(s => s.Id == userSubscriptionId)!.AuthorPageId;
            return authorsUserFollows.FirstOrDefault(a => a.Id == authorPageId)?.Id;
        }
        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            var userId = Security.User.Id;
            await GetDataFromDb(userId);
        }

        /// <summary>
        /// Gets the data from database.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        private async Task GetDataFromDb(string userId)
        {
            var userSubscriptionsOData = await GetUserSubscriptionsOData(userId);
            userSubscriptions = userSubscriptionsOData.Value;

            var subscriptionsOData = await GetSubscriptionsODataServiceResult();
            subscriptions = subscriptionsOData.Value;

            var authorsOData = await GetAuthorsODataServiceResult();
            authorsUserFollows = authorsOData.Value;
        }

        /// <summary>
        /// Gets the authors o data service result.
        /// </summary>
        /// <returns>ODataServiceResult&lt;Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorsODataServiceResult()
        {
            var authorsIds = subscriptions.Select(s => s.AuthorPageId).ToList();
            var authorsIdsFilter = string.Join(" or ", authorsIds.Select(id => $"Id eq {id}"));
            var authorsOData = await ManekiAppDB.GetAuthorPages(filter: authorsIdsFilter);
            return authorsOData;
        }

        /// <summary>
        /// Gets the subscriptions o data service result.
        /// </summary>
        /// <returns>ODataServiceResult&lt;Subscription&gt;.</returns>
        private async Task<ODataServiceResult<Subscription>> GetSubscriptionsODataServiceResult()
        {
            var userSubscriptionIds = userSubscriptions.Select(sub => sub.SubscriptionId).ToList();
            var idsFilter = string.Join(" or ", userSubscriptionIds.Select(id => $"Id eq {id}"));
            var subscriptionsOData = await ManekiAppDB.GetSubscriptions(filter: idsFilter);
            return subscriptionsOData;
        }

        /// <summary>
        /// Gets the user subscriptions o data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>ODataServiceResult&lt;UserSubscription&gt;.</returns>
        private async Task<ODataServiceResult<UserSubscription>> GetUserSubscriptionsOData(string userId)
        {
            var filter = $"UserId eq '{userId}'";
            var userSubscriptionsOData = await ManekiAppDB.GetUserSubscriptions(filter: filter);
            return userSubscriptionsOData;
        }


        /// <summary>
        /// Cancels the subscription.
        /// </summary>
        /// <param name="userSubscription">The user subscription.</param>
        private async Task CancelSubscription(UserSubscription userSubscription)
        {
            userSubscription.IsCanceled = true;
            await ManekiAppDB.UpdateUserSubscription(userSubscription.Id, userSubscription);
        }

        /// <summary>
        /// Enables the notifications.
        /// </summary>
        /// <param name="userSubscription">The user subscription.</param>
        private async Task EnableNotifications(UserSubscription userSubscription)
        {
            userSubscription.ReceiveNotifications = true;
            await ManekiAppDB.UpdateUserSubscription(userSubscription.Id, userSubscription);
        }

        /// <summary>
        /// Disables the notifications.
        /// </summary>
        /// <param name="userSubscription">The user subscription.</param>
        private async Task DisableNotifications(UserSubscription userSubscription)
        {
            userSubscription.ReceiveNotifications = false;
            await ManekiAppDB.UpdateUserSubscription(userSubscription.Id, userSubscription);
        }

        /// <summary>
        /// Navigates to author page.
        /// </summary>
        /// <param name="url">The URL.</param>
        void NavigateToAuthorPage(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        /// <summary>
        /// Gets the author profile picture.
        /// </summary>
        /// <param name="userSubscriptionId">The user subscription identifier.</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Gets the color of the gradient.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <returns>System.String.</returns>
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