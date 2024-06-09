using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class Search.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class Search
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
        protected ManekiAppDBService ManekiAppDbService { get; set; }

        /// <summary>
        /// The authors
        /// </summary>
        protected IEnumerable<Server.Models.ManekiAppDB.AuthorPage> authors =
            new List<Server.Models.ManekiAppDB.AuthorPage>();

        /// <summary>
        /// The page size
        /// </summary>
        protected int pageSize = 3;
        /// <summary>
        /// The authors amount
        /// </summary>
        protected int authorsAmount;
        /// <summary>
        /// The is loading
        /// </summary>
        protected bool isLoading;
        /// <summary>
        /// The paging summary format
        /// </summary>
        protected string pagingSummaryFormat = "Displaying page {0} of {1} (total {2} records)";

        /// <summary>
        /// The search field value
        /// </summary>
        protected string searchFieldValue;
        /// <summary>
        /// The title to search
        /// </summary>
        protected string titleToSearch = "";

        /// <summary>
        /// The error visible
        /// </summary>
        protected bool errorVisible;
        /// <summary>
        /// The error
        /// </summary>
        protected string error;

        /// <summary>
        /// The datalist
        /// </summary>
        protected RadzenDataList<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> datalist;
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
                await base.OnInitializedAsync();
                authorsAmount = await GetAuthorsAmount(titleToSearch);
                authors = await GetAuthors(0, pageSize, titleToSearch);
            }
            catch (Exception e)
            {
                errorVisible = true;
                error = e.Message;
            }
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        async Task LoadData(LoadDataArgs args)
        {
            isLoading = true;

            try
            {
                authorsAmount = await GetAuthorsAmount(titleToSearch);
                authors = await GetAuthors(args.Skip, args.Top, titleToSearch);
            }
            catch (Exception e)
            {
                errorVisible = true;
                error = e.Message;
            }

            isLoading = false;
        }

        /// <summary>
        /// Gets the authors.
        /// </summary>
        /// <param name="skip">The skip.</param>
        /// <param name="top">The top.</param>
        /// <param name="title">The title.</param>
        /// <returns>IEnumerable&lt;Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        private async Task<IEnumerable<Server.Models.ManekiAppDB.AuthorPage>> GetAuthors(int? skip, int? top, string title = "")
        {
            return (await GetAuthorsOData(skip, top, title)).Value.ToList();
        }

        /// <summary>
        /// Gets the authors o data.
        /// </summary>
        /// <param name="skip">The skip.</param>
        /// <param name="top">The top.</param>
        /// <param name="title">The title.</param>
        /// <returns>ODataServiceResult&lt;Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorsOData(int? skip, int? top, string title)
        {
            string filter = string.IsNullOrEmpty(title) ? string.Empty : $"contains(ToLower(Title), '{title.ToLower()}')";
            var result = await ManekiAppDbService.GetAuthorPages(filter: filter, skip: skip, top: top, expand: "Subscriptions");
            return result;
        }

        /// <summary>
        /// Gets the authors amount.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>System.Int32.</returns>
        private async Task<int> GetAuthorsAmount(string title)
        {
            return (await GetAuthorsAmountOData(title)).Count;
        }

        /// <summary>
        /// Gets the authors amount o data.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>ODataServiceResult&lt;Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorsAmountOData(string title)
        {
            string filter = string.IsNullOrEmpty(title) ? string.Empty : $"contains(ToLower(Title), '{title.ToLower()}')";
            var result = await ManekiAppDbService.GetAuthorPages(filter: filter, count: true, top: 0);
            return result;
        }

        /// <summary>
        /// Applies the search.
        /// </summary>
        private async void ApplySearch()
        {
            titleToSearch = searchFieldValue;
            await datalist.FirstPage();
            await datalist.Reload();
        }
    }
}