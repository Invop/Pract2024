using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System.Text;
using System.Text.Json;

namespace ManekiApp.Client.Pages
{
    public partial class AuthorPage
    {
        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

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

        [Inject] protected ManekiAppDBService ManekiAppDb { get; set; }

        [Parameter] public Guid AuthorPageId { get; set; }

        protected Server.Models.ManekiAppDB.AuthorPage Author = new();
        protected List<Subscription> AuthorSubscriptions = [];
        protected Dictionary<string, string> SocLinks = new();

        protected bool IsFound = true;
        protected bool IsUserAuthor;

        protected int PostsAmount;
        protected int SubscribersAmount;
        protected int PaidSubscribersAmount;

        //PAGINATION ATTRIBUTES
        protected IEnumerable<Post> Posts =
            new List<Post>();

        protected int PaginationPostsAmount;

        protected int PageSize = 3;
        protected bool IsLoading;
        protected string PagingSummaryFormat = "Displaying page {0} of {1} (total {2} records)";
        protected RadzenDataList<Post> Datalist;

        private IEnumerable<int> selectedTiers = new List<int>();
        private IEnumerable<int> selectedYears = new List<int>();
        private string selectedSortBy = "From the newest";
        private string searchText = string.Empty;

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

        private async Task LoadAuthorData()
        {
            Author = await ManekiAppDb.GetAuthorPageById(id: AuthorPageId, expand: "Subscriptions");
            if (Author != null)
            {
                Console.WriteLine($"Loaded Author: {Author.Title}");
                IsUserAuthor = CheckUserAuthor(Author);

                AuthorSubscriptions = Author.Subscriptions.Where(sub => sub.PermissionLevel > 0).ToList();
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
                Console.WriteLine("Author is null");
            }
        }

        private async Task ApplyFilter()
        {
            await UpdateDataList();
        }

        private async Task UpdateDataList()
        {
            await Datalist.FirstPage();
            await Datalist.Reload();
        }

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

        private string BuildTiersFilter()
        {
            return string.Join(" or ", selectedTiers.Select(tier => $"MinLevel eq {tier}"));
        }

        private string BuildYearsFilter()
        {
            return string.Join(" or ", selectedYears.Select(year => $"year(CreatedAt) eq {year}"));
        }

        private string BuildSearchTextFilter()
        {
            var lowerCaseSearchText = searchText.ToLower();
            return $" and (contains(tolower(Title), '{lowerCaseSearchText}') or contains(tolower(Content), '{lowerCaseSearchText}'))";
        }

        private string BuildOrderBy()
        {
            return selectedSortBy is "From the oldest" ? "CreatedAt" : "CreatedAt desc";
        }



        private async Task ClearFilters()
        {

            selectedTiers = new List<int>();
            selectedYears = new List<int>();
            selectedSortBy = "From the newest";
            searchText = string.Empty;

            await UpdateDataList();
        }


        private bool CheckUserAuthor(Server.Models.ManekiAppDB.AuthorPage authorPage)
        {
            return Security.User.Id.Equals(Author.UserId);
        }

        private void NavigateToEditPage()
        {
            NavigationManager.NavigateTo("/edit-author-page");
        }

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

        private async Task OpenLinkInNewTab(string url)
        {
            var fullUrl = NavigationManager.ToAbsoluteUri(url).ToString();
            await JsRuntime.InvokeVoidAsync("open", fullUrl, "_blank");
        }

        private async Task<int> GetPostsAmount(Server.Models.ManekiAppDB.AuthorPage author)
        {
            return (await GetPostsAmountOData(author)).Count;
        }

        private async Task<int> GetSubscribersAmount(Server.Models.ManekiAppDB.AuthorPage author)
        {
            return (await GetSubscribersAmountOData(author)).Count;
        }

        private async Task<int> GetPaidSubscribersAmount(Server.Models.ManekiAppDB.AuthorPage author)
        {
            return (await GetPaidSubscribersAmountOData(author)).Count;
        }

        private async Task<ODataServiceResult<Post>> GetPostsAmountOData(Server.Models.ManekiAppDB.AuthorPage author)
        {
            var filter = $"AuthorPageId eq {author.Id}";
            var result = await ManekiAppDb.GetPosts(filter: filter, count: true, top: 0);
            return result;
        }

        private async Task<ODataServiceResult<UserSubscription>> GetSubscribersAmountOData(Server.Models.ManekiAppDB.AuthorPage author)
        {
            var subscriptionIds = string.Join(",", author.Subscriptions.Select(s => s.Id));
            var filter = $"SubscriptionId in ({subscriptionIds})";
            var result = await ManekiAppDb.GetUserSubscriptions(filter: filter, count: true, top: 0);
            return result;
        }

        private async Task<ODataServiceResult<UserSubscription>> GetPaidSubscribersAmountOData(Server.Models.ManekiAppDB.AuthorPage author)
        {
            var subscriptionIds = string.Join(",", author.Subscriptions.Select(s => s.Id));
            var now = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
            var filter = $"SubscriptionId in ({subscriptionIds}) and Subscription/PermissionLevel gt 0 and EndsAt gt {now}";
            var result = await ManekiAppDb.GetUserSubscriptions(filter: filter, count: true, top: 0);
            return result;
        }

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
        
        private async Task<ODataServiceResult<UserSubscription>> GetUserSubscriptionsByUserAndAuthor(string userId, Guid authorPageId)
        {
            var filter = $"UserId eq '{userId}' and Subscription/AuthorPageId eq {authorPageId}";
            var userSubscriptionsOData = await ManekiAppDb.GetUserSubscriptions(filter: filter, expand: "Subscription");
            return userSubscriptionsOData;
        }

        public class SocialLinks
        {
            public string Youtube { get; set; } = string.Empty;
            public string Instagram { get; set; } = string.Empty;
            public string Telegram { get; set; } = string.Empty;
            public string TikTok { get; set; } = string.Empty;
            public string Facebook { get; set; } = string.Empty;
            public string Twitter { get; set; } = string.Empty;
            public string Twitch { get; set; } = string.Empty;
            public string Pinterest { get; set; } = string.Empty;
        }


    }
}