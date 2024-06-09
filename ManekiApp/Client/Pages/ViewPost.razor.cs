using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class ViewPost.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class ViewPost
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
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Parameter]
        public string Id { get; set; }

        /// <summary>
        /// The error visible
        /// </summary>
        protected bool errorVisible;
        /// <summary>
        /// The error
        /// </summary>
        protected string error;

        /// <summary>
        /// The post exists
        /// </summary>
        protected bool PostExists = true;
        /// <summary>
        /// The post
        /// </summary>
        protected Server.Models.ManekiAppDB.Post Post = new Post();
        /// <summary>
        /// The author
        /// </summary>
        protected Server.Models.ManekiAppDB.AuthorPage Author = new Server.Models.ManekiAppDB.AuthorPage();

        /// <summary>
        /// The is user author
        /// </summary>
        protected bool isUserAuthor = false;
        /// <summary>
        /// The is content visible
        /// </summary>
        protected bool isContentVisible = true;

        /// <summary>
        /// Gets the post by identifier.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns>ODataServiceResult&lt;Server.Models.ManekiAppDB.Post&gt;.</returns>
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.Post>> GetPostById(Guid postId)
        {
            var filter = $"Id eq {postId}";
            var postOData = await ManekiAppDbService.GetPosts(filter: filter);
            return postOData;
        }

        /// <summary>
        /// Gets the author by identifier.
        /// </summary>
        /// <param name="authorId">The author identifier.</param>
        /// <returns>ODataServiceResult&lt;Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorById(Guid authorId)
        {
            var filter = $"Id eq {authorId}";
            var authorOData = await ManekiAppDbService.GetAuthorPages(filter: filter);
            return authorOData;
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
            var userSubscriptionsOData = await ManekiAppDbService.GetUserSubscriptions(filter: filter, expand: "Subscription");
            return userSubscriptionsOData;
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
            if (checkPostExistance(out var postId)) return;

            try
            {
                var postOData = await GetPostById(postId);
                if (postOData.Value.Any())
                {
                    Post = postOData.Value.First();
                    var authorOData = await GetAuthorById(Post.AuthorPageId);
                    Author = authorOData.Value.FirstOrDefault();

                    isUserAuthor = Author.UserId.Equals(Security.User.Id);

                    if (Post.MinLevel != 0 && !isUserAuthor)
                    {
                        isContentVisible = await checkUserSubscriptions();
                    }
                    else
                    {
                        isContentVisible = true;
                    }
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
        /// Checks the user subscriptions.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private async Task<bool> checkUserSubscriptions()
        {
            var userSubscriptionsOData = await GetUserSubscriptionsByUserAndAuthor(Security.User.Id, Author.Id);
            var userSubscriptions = userSubscriptionsOData.Value.ToList();

            foreach (var subscription in userSubscriptions)
            {
                if (subscription.Subscription.PermissionLevel >= Post.MinLevel &&
                    subscription.EndsAt >= DateTimeOffset.UtcNow) return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the post existance.
        /// </summary>
        /// <param name="postId">The post identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool checkPostExistance(out Guid postId)
        {
            if (!Guid.TryParse(Id, out postId))
            {
                PostExists = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Redirects to author page.
        /// </summary>
        protected void redirectToAuthorPage()
        {
            NavigationManager.NavigateTo($"/author-page/{Author.Id.ToString()}");
        }

        /// <summary>
        /// Redirects the edit post.
        /// </summary>
        private void RedirectEditPost()
        {
            NavigationManager.NavigateTo($"/edit-post/{Id}");
        }
    }
}