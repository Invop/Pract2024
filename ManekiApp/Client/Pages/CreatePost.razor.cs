using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Text.RegularExpressions;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class CreatePost.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class CreatePost
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
        /// The is preview
        /// </summary>
        protected bool isPreview = false;
        /// <summary>
        /// The post time
        /// </summary>
        protected string PostTime;

        /// <summary>
        /// The error visible
        /// </summary>
        protected bool errorVisible;
        /// <summary>
        /// The error
        /// </summary>
        protected string error;

        /// <summary>
        /// The current author
        /// </summary>
        protected Server.Models.ManekiAppDB.AuthorPage currentAuthor;
        /// <summary>
        /// The post
        /// </summary>
        protected Server.Models.ManekiAppDB.Post Post = new Post();

        /// <summary>
        /// The minimum level value
        /// </summary>
        protected int minLevelValue = 0;
        /// <summary>
        /// The subscriptions
        /// </summary>
        protected IEnumerable<Subscription> subscriptions;

        /// <summary>
        /// Gets the author pages o data.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>ODataServiceResult&lt;Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPagesOData(string userId)
        {
            var filter = $"UserId eq '{userId}'";
            var authorPagesOData = await ManekiAppDBService.GetAuthorPages(filter: filter);
            return authorPagesOData;
        }

        /// <summary>
        /// Gets the subscriptions by author page o data.
        /// </summary>
        /// <param name="authorPageId">The author page identifier.</param>
        /// <returns>ODataServiceResult&lt;Server.Models.ManekiAppDB.Subscription&gt;.</returns>
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.Subscription>> GetSubscriptionsByAuthorPageOData(Guid authorPageId)
        {
            var filter = $"AuthorPageId eq {authorPageId}";
            var subscriptionsOData = await ManekiAppDBService.GetSubscriptions(filter: filter);
            return subscriptionsOData;
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
            var authorPagesOData = await GetAuthorPagesOData(Security.User.Id);
            if (authorPagesOData.Value.Any())
            {
                currentAuthor = authorPagesOData.Value.First();
                var subscriptionsOData = await GetSubscriptionsByAuthorPageOData(currentAuthor.Id);
                subscriptions = subscriptionsOData.Value.ToList();
            }
            else
            {
                NavigationManager.NavigateTo("/create-author-page");
            }

        }

        /// <summary>
        /// Forms the submit.
        /// </summary>
        /// <param name="post">The post.</param>
        protected async Task FormSubmit(ManekiApp.Server.Models.ManekiAppDB.Post post)
        {
            try
            {
                Post.CreatedAt = DateTimeOffset.UtcNow;
                Post.EditedAt = Post.CreatedAt;
                Post.AuthorPageId = currentAuthor.Id;
                Post.MinLevel = minLevelValue;

                NormalizeAllTextFields();

                await ManekiAppDBService.CreatePost(post);

                try
                {
                    await SendNotificationRequest(currentAuthor.Id);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while sending notification request: {e.Message}");
                }

                NavigationManager.NavigateTo($"/post/{Post.Id}");
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }

        /// <summary>
        /// Sends the notification request.
        /// </summary>
        /// <param name="authorId">The author identifier.</param>
        private async Task SendNotificationRequest(Guid authorId)
        {
            var client = new HttpClient();
            var response = await client.PostAsync($"https://localhost:5006/notify?authorId={authorId}&title={Post.Title}", null);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Notification sent successfully.");
            }
            else
            {
                Console.WriteLine($"Failed to send notification. Status code: {response.StatusCode}");
            }
        }

        /// <summary>
        /// Enables the preview.
        /// </summary>
        private void EnablePreview()
        {
            isPreview = true;
            PostTime = DateTime.Now.ToString("HH:mm");
        }

        /// <summary>
        /// Disables the preview.
        /// </summary>
        private void DisablePreview()
        {
            isPreview = false;
        }

        /// <summary>
        /// Normalizes all text fields.
        /// </summary>
        private void NormalizeAllTextFields()
        {
            Post.Title = GetNormalizedString(Post.Title);
        }

        /// <summary>
        /// Gets the normalized string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.String.</returns>
        public static string GetNormalizedString(string input)
        {
            string pattern = @"\s+";
            string normalizedString = Regex.Replace(input, pattern, " ").Trim();
            return normalizedString;
        }

        /// <summary>
        /// Validates the string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ValidateString(string text)
        {
            return !(string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text));
        }
    }
}