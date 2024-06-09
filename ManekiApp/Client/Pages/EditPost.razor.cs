using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Text.RegularExpressions;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class EditPost.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class EditPost
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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Parameter]
        public string Id { get; set; }

        /// <summary>
        /// The post exists
        /// </summary>
        protected bool PostExists = true;
        /// <summary>
        /// The is author
        /// </summary>
        protected bool isAuthor = true;

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
        /// Gets the post by identifier.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>ODataServiceResult&lt;Server.Models.ManekiAppDB.Post&gt;.</returns>
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.Post>> GetPostById(Guid postId)
        {
            var filter = $"Id eq {postId}";
            var postOData = await ManekiAppDBService.GetPosts(filter: filter, expand: "AuthorPage");
            return postOData;
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
            if (validateGuid(out var postId)) return;

            try
            {
                var postOData = await GetPostById(postId);
                if (postOData.Value.Any())
                {
                    if (!checkIsAuthor(postOData.Value.First()))
                    {
                        isAuthor = false;
                        return;
                    }

                    Post = postOData.Value.First();
                    var subscriptionsOData = await GetSubscriptionsByAuthorPageOData(Post.AuthorPageId);
                    subscriptions = subscriptionsOData.Value.ToList();

                    minLevelValue = Post.MinLevel;
                    currentAuthor = Post.AuthorPage;
                }
                else
                {
                    PostExists = false;
                }
            }
            catch (Exception e)
            {
                errorVisible = true;
                error = e.Message;
            }

        }

        /// <summary>
        /// Checks the is author.
        /// </summary>
        /// <param name="post">The post.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool checkIsAuthor(Post post)
        {
            return post.AuthorPage.UserId.Equals(Security.User.Id);
        }

        /// <summary>
        /// Validates the unique identifier.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool validateGuid(out Guid postId)
        {
            if (!Guid.TryParse(Id, out postId))
            {
                PostExists = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Forms the submit.
        /// </summary>
        /// <param name="post">The post.</param>
        protected async Task FormSubmit(ManekiApp.Server.Models.ManekiAppDB.Post post)
        {
            try
            {
                Post.EditedAt = DateTimeOffset.UtcNow;
                Post.MinLevel = minLevelValue;

                NormalizeAllTextFields();

                await ManekiAppDBService.UpdatePost(Post.Id, Post);

                NavigationManager.NavigateTo($"/post/{Post.Id}");
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
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
        /// Cancels the edit.
        /// </summary>
        private void CancelEdit()
        {
            NavigationManager.NavigateTo($"/post/{Id}");
        }

        /// <summary>
        /// Deletes the post.
        /// </summary>
        private async void DeletePost()
        {
            try
            {
                var confirmOptions = new ConfirmOptions
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No"
                };

                var dialogResult = await DialogService.Confirm("Are you sure?", "You are going to delete a post", confirmOptions);

                if (dialogResult.HasValue && dialogResult.Value)
                {
                    await ManekiAppDBService.DeletePost(Post.Id);
                    NavigationManager.NavigateTo("/");
                }
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
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