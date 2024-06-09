using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class Feed.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class Feed
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
        /// Gets or sets the maneki application database service.
        /// </summary>
        /// <value>The maneki application database service.</value>
        [Inject]
        protected ManekiAppDBService ManekiAppDBService { get; set; }

        /// <summary>
        /// The user feed posts
        /// </summary>
        protected List<Post> userFeedPosts = new List<Post>();
        /// <summary>
        /// The user subscriptions
        /// </summary>
        protected IEnumerable<UserSubscription> userSubscriptions = new List<UserSubscription>();
        /// <summary>
        /// The subscriptions
        /// </summary>
        protected IEnumerable<Subscription> subscriptions = new List<Subscription>();
        /// <summary>
        /// The authors user follows
        /// </summary>
        protected IEnumerable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> authorsUserFollows = new List<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>();
        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            if (Security.User != null && !string.IsNullOrEmpty(Security.User.Id))
            {
                var userId = Security.User.Id;
                await LoadFeedData(userId);
            }
        }

        /// <summary>
        /// Loads the feed data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        private async Task LoadFeedData(string userId)
        {
            await LoadUserSubscriptions(userId);
            await LoadAuthorsAndSubscriptions();
            await LoadPostsFromFollowedAuthors();
        }

        /// <summary>
        /// Loads the user subscriptions.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        private async Task LoadUserSubscriptions(string userId)
        // loads user subscriptions from the database
        {
            var filter = $"UserId eq '{userId}'";
            var userSubscriptionsOData = await ManekiAppDBService.GetUserSubscriptions(filter: filter, expand: "Subscription");
            userSubscriptions = userSubscriptionsOData.Value;
        }

        /// <summary>
        /// Loads the authors and subscriptions.
        /// </summary>
        private async Task LoadAuthorsAndSubscriptions()
        // loads authors and their subscriptions from the database
        {
            var subscriptionIds = userSubscriptions.Select(sub => sub.SubscriptionId).ToList();
            if (subscriptionIds.Any())
            {
                var idsFilter = string.Join(" or ", subscriptionIds.Select(id => $"Id eq {id}"));
                var subscriptionsOData = await ManekiAppDBService.GetSubscriptions(filter: idsFilter);
                subscriptions = subscriptionsOData.Value;

                var authorIds = subscriptions.Select(sub => sub.AuthorPageId).Distinct().ToList();
                if (authorIds.Any())
                {
                    var authorsFilter = string.Join(" or ", authorIds.Select(id => $"Id eq {id}"));
                    var authorsOData = await ManekiAppDBService.GetAuthorPages(filter: authorsFilter);
                    authorsUserFollows = authorsOData.Value;
                }
            }
        }

        /// <summary>
        /// Loads the posts from followed authors.
        /// </summary>
        private async Task LoadPostsFromFollowedAuthors()
        // loads posts from authors on which the user is subscribed
        {
            var authorIds = authorsUserFollows.Select(author => author.Id).ToList();
            if (authorIds.Any())
            {
                var postsFilter = string.Join(" or ", authorIds.Select(id => $"AuthorPageId eq {id}"));
                var postsOData = await ManekiAppDBService.GetPosts(filter: postsFilter, expand: "AuthorPage");
                var allPosts = postsOData.Value;

                // apply filtering based on subscription level
                userFeedPosts = allPosts.Where(post =>
                    {
                        var userSubscriptionsToCurrentAuthor = userSubscriptions
                            .Where(sub => sub.Subscription.AuthorPageId == post.AuthorPageId);

                        foreach (var subscription in userSubscriptionsToCurrentAuthor)
                        {
                            if (subscription.Subscription.PermissionLevel >= post.MinLevel &&
                                subscription.EndsAt >= DateTimeOffset.UtcNow)
                            {
                                return true;
                            }
                        }

                        return false;
                    })
                    .OrderByDescending(post => post.CreatedAt)
                    .Take(10)
                    .ToList();
            }
        }
    }
}