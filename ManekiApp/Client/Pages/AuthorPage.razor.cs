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

        [Parameter] public Guid AuthorPageId { get; set; }

        protected Server.Models.ManekiAppDB.AuthorPage author = new Server.Models.ManekiAppDB.AuthorPage();
        protected List<Subscription> authorSubscriptions = new List<Subscription>();
        protected Dictionary<string, string> socLinks = new Dictionary<string, string>();

        protected bool isFound = true;
        protected bool isUserAuthor = false;

        protected int postsAmount;
        protected int subscribersAmount;
        protected int paidSubscribersAmount;

        //PAGINATION ATTRIBUTES
        protected IEnumerable<Server.Models.ManekiAppDB.Post> posts =
            new List<Server.Models.ManekiAppDB.Post>();

        protected IEnumerable<Server.Models.ManekiAppDB.Post> filteredPosts =
            new List<Server.Models.ManekiAppDB.Post>();

        protected int paginationPostsAmount;

        protected int pageSize = 3;
        protected bool isLoading;
        protected string pagingSummaryFormat = "Displaying page {0} of {1} (total {2} records)";
        protected RadzenDataList<ManekiApp.Server.Models.ManekiAppDB.Post> datalist;

        private IEnumerable<int> selectedTiers = new List<int>();
        private IEnumerable<int> selectedYears = new List<int>();
        private IEnumerable<string> selectedSortBy = new List<string>();
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
            author = await ManekiAppDB.GetAuthorPageById(id: AuthorPageId, expand: "Subscriptions");
            if (author != null)
            {
                Console.WriteLine($"Loaded Author: {author.Title}");
                isUserAuthor = CheckUserAuthor(author);

                authorSubscriptions = author.Subscriptions.Where(sub => sub.PermissionLevel > 0).ToList();
                postsAmount = await GetPostsAmount(author);
                subscribersAmount = await GetSubscribersAmount(author);
                paidSubscribersAmount = await GetPaidSubscribersAmount(author);
                socLinks = GetSocialLinks();

                if (!string.IsNullOrEmpty(author.SocialLinks))
                {
                    Console.WriteLine($"Social Links: {author.SocialLinks}");
                }
                else
                {
                    Console.WriteLine("No social links provided");
                }
            }
            else
            {
                isFound = false;
                Console.WriteLine("Author is null");
            }
        }

        private async Task ApplyFilter(object args)
        {

            await UpdateDataList();
        }

        private async Task UpdateDataList()
        {
            await datalist.FirstPage();
            await datalist.Reload();
        }

        async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;

            string filter = BuildFilter();
            string orderby = BuildOrderBy();

            var result = await ManekiAppDB.GetPosts(filter: filter, orderby: orderby);

            paginationPostsAmount = result.Count;
            posts = result.Value.AsODataEnumerable();

            isLoading = false;
        }

        private string BuildFilter()
        {
            var filterBuilder = new StringBuilder($"(AuthorPageId eq {author.Id})");

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
            return string.Join(" and ", selectedTiers.Select(tier => $"MinLevel ge {tier}"));
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
            return selectedSortBy != null && selectedSortBy.Contains("From the oldest") ? "CreatedAt" : "CreatedAt desc";
        }



        private async Task ClearFilters()
        {

            selectedTiers = new List<int>();
            selectedYears = new List<int>();
            selectedSortBy = new List<string>();
            searchText = string.Empty;

            await UpdateDataList();
        }


        private bool CheckUserAuthor(Server.Models.ManekiAppDB.AuthorPage authorPage)
        {
            return Security.User.Id.Equals(author.UserId);
        }

        private void NavigateToEditPage()
        {
            NavigationManager.NavigateTo($"/edit-author-page");
        }

        private Dictionary<string, string> GetSocialLinks()
        {
            var toReturn = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(author.SocialLinks))
            {
                var socialLinks = JsonSerializer.Deserialize<SocialLinks>(author.SocialLinks);
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
            await JSRuntime.InvokeVoidAsync("open", fullUrl, "_blank");
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
            var result = await ManekiAppDB.GetPosts(filter: filter, count: true, top: 0);
            return result;
        }

        private async Task<ODataServiceResult<UserSubscription>> GetSubscribersAmountOData(Server.Models.ManekiAppDB.AuthorPage author)
        {
            var subscriptionIds = string.Join(",", author.Subscriptions.Select(s => s.Id));
            var filter = $"SubscriptionId in ({subscriptionIds})";
            var result = await ManekiAppDB.GetUserSubscriptions(filter: filter, count: true, top: 0);
            return result;
        }

        private async Task<ODataServiceResult<UserSubscription>> GetPaidSubscribersAmountOData(Server.Models.ManekiAppDB.AuthorPage author)
        {
            var subscriptionIds = string.Join(",", author.Subscriptions.Select(s => s.Id));
            var now = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
            var filter = $"SubscriptionId in ({subscriptionIds}) and Subscription/PermissionLevel gt 0 and EndsAt gt {now}";
            var result = await ManekiAppDB.GetUserSubscriptions(filter: filter, count: true, top: 0);
            return result;
        }



        public class SocialLinks
        {
            public string Youtube { get; set; } = null;
            public string Instagram { get; set; } = null;
            public string Telegram { get; set; } = null;
            public string TikTok { get; set; } = null;
            public string Facebook { get; set; } = null;
            public string Twitter { get; set; } = null;
            public string Twitch { get; set; } = null;
            public string Pinterest { get; set; } = null;
        }


    }
}