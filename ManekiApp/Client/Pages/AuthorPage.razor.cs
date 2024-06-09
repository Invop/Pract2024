using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Text;
using System.Text.Json;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class AuthorPage.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class AuthorPage
    {
        /// <summary>
        /// Gets or sets the js runtime.
        /// </summary>
        /// <value>The js runtime.</value>
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

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
        [Inject] protected ManekiAppDBService ManekiAppDb { get; set; }

        /// <summary>
        /// Gets or sets the author page identifier.
        /// </summary>
        /// <value>The author page identifier.</value>
        [Parameter] public Guid AuthorPageId { get; set; }

        /// <summary>
        /// The author
        /// </summary>
        protected Server.Models.ManekiAppDB.AuthorPage Author = new();
        /// <summary>
        /// The author subscriptions
        /// </summary>
        protected List<Subscription> AuthorSubscriptions = [];
        /// <summary>
        /// The soc links
        /// </summary>
        protected Dictionary<string, string> SocLinks = new();

        /// <summary>
        /// The is found
        /// </summary>
        protected bool IsFound = true;
        /// <summary>
        /// The is user author
        /// </summary>
        protected bool IsUserAuthor;

        /// <summary>
        /// The posts amount
        /// </summary>
        protected int PostsAmount;
        /// <summary>
        /// The subscribers amount
        /// </summary>
        protected int SubscribersAmount;
        /// <summary>
        /// The paid subscribers amount
        /// </summary>
        protected int PaidSubscribersAmount;

        //PAGINATION ATTRIBUTES
        /// <summary>
        /// The posts
        /// </summary>
        protected IEnumerable<Post> Posts =
            new List<Post>();

        /// <summary>
        /// The pagination posts amount
        /// </summary>
        protected int PaginationPostsAmount;

        /// <summary>
        /// The page size
        /// </summary>
        protected int PageSize = 3;
        /// <summary>
        /// The is loading
        /// </summary>
        protected bool IsLoading;
        /// <summary>
        /// The paging summary format
        /// </summary>
        protected string PagingSummaryFormat = "Displaying page {0} of {1} (total {2} records)";
        /// <summary>
        /// The datalist
        /// </summary>
        protected RadzenDataList<Post> Datalist;

        /// <summary>
        /// The selected tiers
        /// </summary>
        private IEnumerable<int> selectedTiers = new List<int>();
        /// <summary>
        /// The selected years
        /// </summary>
        private IEnumerable<int> selectedYears = new List<int>();
        /// <summary>
        /// The selected sort by
        /// </summary>
        private string selectedSortBy = "From the newest";
        /// <summary>
        /// The search text
        /// </summary>
        private string searchText = string.Empty;

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadAuthorData();
                await UpdateDataList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading author data: {ex.Message}");
            }

        }

        /// <summary>
        /// Loads the author data.
        /// </summary>
        private async Task LoadAuthorData()
        {
            var authorsResult = await ManekiAppDb.GetAuthorPages(filter: $"Id eq {AuthorPageId}", expand: "Subscriptions");
            if (authorsResult.Value.Any())
            {
                Author = authorsResult.Value.First();
                Console.WriteLine($"Loaded Author: {Author.Title}");
                IsUserAuthor = CheckUserAuthor(Author);

                AuthorSubscriptions = Author.Subscriptions.ToList();
                PostsAmount = await GetPostsAmount(Author);
                SubscribersAmount = await GetSubscribersAmount(Author);
                PaidSubscribersAmount = await GetPaidSubscribersAmount(Author);
                SocLinks = await GetSocialLinks();

                //Variable from AuthorPage.razor
                tiers = Author.Subscriptions.ToList();

                Console.WriteLine(!string.IsNullOrEmpty(Author.SocialLinks)
                    ? $"Social Links: {Author.SocialLinks}"
                    : "No social links provided");
            }
            else
            {
                IsFound = false;
                Console.WriteLine("Author is not found");
            }
        }

        /// <summary>
        /// Applies the filter.
        /// </summary>
        private async Task ApplyFilter()
        {
            await UpdateDataList();
        }

        /// <summary>
        /// Updates the data list.
        /// </summary>
        private async Task UpdateDataList()
        {
            await Datalist.FirstPage();
            await Datalist.Reload();
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        async Task LoadData(LoadDataArgs args)
        {
            IsLoading = true;

            string filter = BuildFilter();
            string orderBy = BuildOrderBy();

            var result = await ManekiAppDb.GetPosts(filter: filter, orderby: orderBy, skip: args.Skip, top: args.Top);
            var countFilterResult = await ManekiAppDb.GetPosts(filter: filter, orderby: orderBy, top: 0, count: true);

            PaginationPostsAmount = countFilterResult.Count;
            Posts = result.Value.AsODataEnumerable();

            IsLoading = false;
        }

        /// <summary>
        /// Builds the filter.
        /// </summary>
        /// <returns>System.String.</returns>
        private string BuildFilter()
        {
            var filterBuilder = new StringBuilder($"(AuthorPageId eq {Author.Id})");

            if (selectedTiers.Any())
            {
                filterBuilder.Append(" and (")
                    .Append(BuildTiersFilter())
                    .Append(")");
            }

            if (selectedYears.Any())
            {
                filterBuilder.Append(" and (")
                    .Append(BuildYearsFilter())
                    .Append(")");
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                filterBuilder.Append(BuildSearchTextFilter());
            }

            return filterBuilder.ToString();
        }

        /// <summary>
        /// Builds the tiers filter.
        /// </summary>
        /// <returns>System.String.</returns>
        private string BuildTiersFilter()
        {
            return string.Join(" or ", selectedTiers.Select(tier => $"MinLevel eq {tier}"));
        }

        /// <summary>
        /// Builds the years filter.
        /// </summary>
        /// <returns>System.String.</returns>
        private string BuildYearsFilter()
        {
            return string.Join(" or ", selectedYears.Select(year => $"year(CreatedAt) eq {year}"));
        }

        /// <summary>
        /// Builds the search text filter.
        /// </summary>
        /// <returns>System.String.</returns>
        private string BuildSearchTextFilter()
        {
            var lowerCaseSearchText = searchText.ToLower();
            return $" and (contains(tolower(Title), '{lowerCaseSearchText}') or contains(tolower(Content), '{lowerCaseSearchText}'))";
        }

        /// <summary>
        /// Builds the order by.
        /// </summary>
        /// <returns>System.String.</returns>
        private string BuildOrderBy()
        {
            return selectedSortBy is "From the oldest" ? "CreatedAt" : "CreatedAt desc";
        }



        /// <summary>
        /// Clears the filters.
        /// </summary>
        private async Task ClearFilters()
        {

            selectedTiers = new List<int>();
            selectedYears = new List<int>();
            selectedSortBy = "From the newest";
            searchText = string.Empty;

            await UpdateDataList();
        }


        /// <summary>
        /// Checks the user author.
        /// </summary>
        /// <param name="authorPage">The author page.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CheckUserAuthor(Server.Models.ManekiAppDB.AuthorPage authorPage)
        {
            return Security.User.Id.Equals(Author.UserId);
        }

        /// <summary>
        /// Navigates to edit page.
        /// </summary>
        private void NavigateToEditPage()
        {
            NavigationManager.NavigateTo("/edit-author-page");
        }

        /// <summary>
        /// Gets the social links.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        private async Task<Dictionary<string, string>> GetSocialLinks()
        {
            var toReturn = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(Author.SocialLinks))
            {
                var socialLinks = JsonSerializer.Deserialize<SocialLinks>(Author.SocialLinks);

                //Probably for protecting a telegram chat link from free subscribers
                bool isPaidSubscriber = (await CheckUserSubscriptions(1, Author)) || IsUserAuthor;
                socialLinks.Telegram = isPaidSubscriber ? socialLinks.Telegram : null;

                var icons = new Dictionary<string, string>
                {
                    { "/images/youtube.png", socialLinks.Youtube },
                    { "/images/instagram.png", socialLinks.Instagram },
                    { "/images/telegram.png", socialLinks.Telegram },
                    { "/images/tiktok.png", socialLinks.TikTok },
                    { "/images/facebook.png", socialLinks.Facebook },
                    { "/images/twitter.png", socialLinks.Twitter },
                    { "/images/twitch.png", socialLinks.Twitch },
                    { "/images/pinterest.png", socialLinks.Pinterest }
                };

                toReturn = icons.Where(link => !string.IsNullOrEmpty(link.Value))
                    .ToDictionary(link => link.Key, link => link.Value);
            }
            return toReturn;
        }

        /// <summary>
        /// Opens the link in new tab.
        /// </summary>
        /// <param name="url">The URL.</param>
        private async Task OpenLinkInNewTab(string url)
        {
            var fullUrl = NavigationManager.ToAbsoluteUri(url).ToString();
            await JsRuntime.InvokeVoidAsync("open", fullUrl, "_blank");
        }

        /// <summary>
        /// Gets the posts amount.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <returns>System.Int32.</returns>
        private async Task<int> GetPostsAmount(Server.Models.ManekiAppDB.AuthorPage author)
        {
            return (await GetPostsAmountOData(author)).Count;
        }

        /// <summary>
        /// Gets the subscribers amount.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <returns>System.Int32.</returns>
        private async Task<int> GetSubscribersAmount(Server.Models.ManekiAppDB.AuthorPage author)
        {
            return (await GetSubscribersAmountOData(author)).Count;
        }

        /// <summary>
        /// Gets the paid subscribers amount.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <returns>System.Int32.</returns>
        private async Task<int> GetPaidSubscribersAmount(Server.Models.ManekiAppDB.AuthorPage author)
        {
            return (await GetPaidSubscribersAmountOData(author)).Count;
        }

        /// <summary>
        /// Gets the posts amount o data.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <returns>ODataServiceResult&lt;Post&gt;.</returns>
        private async Task<ODataServiceResult<Post>> GetPostsAmountOData(Server.Models.ManekiAppDB.AuthorPage author)
        {
            var filter = $"AuthorPageId eq {author.Id}";
            var result = await ManekiAppDb.GetPosts(filter: filter, count: true, top: 0);
            return result;
        }

        /// <summary>
        /// Gets the subscribers amount o data.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <returns>ODataServiceResult&lt;UserSubscription&gt;.</returns>
        private async Task<ODataServiceResult<UserSubscription>> GetSubscribersAmountOData(Server.Models.ManekiAppDB.AuthorPage author)
        {
            var subscriptionIds = string.Join(",", author.Subscriptions.Select(s => s.Id));
            var filter = $"SubscriptionId in ({subscriptionIds})";
            var result = await ManekiAppDb.GetUserSubscriptions(filter: filter, count: true, top: 0);
            return result;
        }

        /// <summary>
        /// Gets the paid subscribers amount o data.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <returns>ODataServiceResult&lt;UserSubscription&gt;.</returns>
        private async Task<ODataServiceResult<UserSubscription>> GetPaidSubscribersAmountOData(Server.Models.ManekiAppDB.AuthorPage author)
        {
            var subscriptionIds = string.Join(",", author.Subscriptions.Select(s => s.Id));
            var now = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
            var filter = $"SubscriptionId in ({subscriptionIds}) and Subscription/PermissionLevel gt 0 and EndsAt gt {now}";
            var result = await ManekiAppDb.GetUserSubscriptions(filter: filter, count: true, top: 0);
            return result;
        }

        /// <summary>
        /// Checks the user subscriptions.
        /// </summary>
        /// <param name="minLevel">The minimum level.</param>
        /// <param name="authorPage">The author page.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> CheckUserSubscriptions(int minLevel, ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage)
        {
            var userSubscriptionsOData = await GetUserSubscriptionsByUserAndAuthor(Security.User.Id, authorPage.Id);
            var userSubscriptions = userSubscriptionsOData.Value.ToList();

            foreach (var subscription in userSubscriptions)
            {
                if (subscription.Subscription.PermissionLevel >= minLevel &&
                    subscription.EndsAt >= DateTimeOffset.UtcNow) return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the user subscriptions by user and author.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="authorPageId">The author page identifier.</param>
        /// <returns>ODataServiceResult&lt;UserSubscription&gt;.</returns>
        private async Task<ODataServiceResult<UserSubscription>> GetUserSubscriptionsByUserAndAuthor(string userId, Guid authorPageId)
        {
            var filter = $"UserId eq '{userId}' and Subscription/AuthorPageId eq {authorPageId}";
            var userSubscriptionsOData = await ManekiAppDb.GetUserSubscriptions(filter: filter, expand: "Subscription");
            return userSubscriptionsOData;
        }

        /// <summary>
        /// Navigates to create post.
        /// </summary>
        protected void NavigateToCreatePost()
        {
            NavigationManager.NavigateTo("/create-post");
        }

        /// <summary>
        /// Class SocialLinks.
        /// </summary>
        public class SocialLinks
        {
            /// <summary>
            /// Gets or sets the youtube.
            /// </summary>
            /// <value>The youtube.</value>
            public string Youtube { get; set; } = string.Empty;
            /// <summary>
            /// Gets or sets the instagram.
            /// </summary>
            /// <value>The instagram.</value>
            public string Instagram { get; set; } = string.Empty;
            /// <summary>
            /// Gets or sets the telegram.
            /// </summary>
            /// <value>The telegram.</value>
            public string Telegram { get; set; } = string.Empty;
            /// <summary>
            /// Gets or sets the tik tok.
            /// </summary>
            /// <value>The tik tok.</value>
            public string TikTok { get; set; } = string.Empty;
            /// <summary>
            /// Gets or sets the facebook.
            /// </summary>
            /// <value>The facebook.</value>
            public string Facebook { get; set; } = string.Empty;
            /// <summary>
            /// Gets or sets the twitter.
            /// </summary>
            /// <value>The twitter.</value>
            public string Twitter { get; set; } = string.Empty;
            /// <summary>
            /// Gets or sets the twitch.
            /// </summary>
            /// <value>The twitch.</value>
            public string Twitch { get; set; } = string.Empty;
            /// <summary>
            /// Gets or sets the pinterest.
            /// </summary>
            /// <value>The pinterest.</value>
            public string Pinterest { get; set; } = string.Empty;
        }


    }
}