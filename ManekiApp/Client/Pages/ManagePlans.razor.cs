using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class ManagePlans.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class ManagePlans
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
        /// Gets or sets the maneki service.
        /// </summary>
        /// <value>The maneki service.</value>
        [Inject]
        protected ManekiAppDBService ManekiService { get; set; }

        /// <summary>
        /// The error visible
        /// </summary>
        protected bool errorVisible;
        /// <summary>
        /// The error
        /// </summary>
        protected string error;

        /// <summary>
        /// The subscriptions list
        /// </summary>
        protected List<Subscription> subscriptionsList = new List<Subscription>();

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Gets the author pages o data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>ODataServiceResult&lt;Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPagesOData(string userId)
        {
            var filter = $"UserId eq '{userId}'";
            var authorPagesOData = await ManekiService.GetAuthorPages(filter: filter);
            return authorPagesOData;
        }

        /// <summary>
        /// Gets the subscriptions for author.
        /// </summary>
        /// <param name="authorPageId">The author page identifier.</param>
        /// <returns>ODataServiceResult&lt;Subscription&gt;.</returns>
        private async Task<ODataServiceResult<Subscription>> GetSubscriptionsForAuthor(Guid authorPageId)
        {
            var filter = $"AuthorPageId eq {authorPageId} and PermissionLevel ne 0";
            var subscriptions = await ManekiService.GetSubscriptions(filter: filter);
            return subscriptions;
        }

        /// <summary>
        /// Cancels the click.
        /// </summary>
        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void ShowError(string message)
        {
            error = message;
            errorVisible = true;
        }
    }
}