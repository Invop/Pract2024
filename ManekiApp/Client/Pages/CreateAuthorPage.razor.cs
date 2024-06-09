using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class CreateAuthorPage.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class CreateAuthorPage
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
        /// The author page
        /// </summary>
        protected ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage = new Server.Models.ManekiAppDB.AuthorPage();
        /// <summary>
        /// The error visible
        /// </summary>
        protected bool errorVisible;
        /// <summary>
        /// The error
        /// </summary>
        protected string error;

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
                await RedirectIfAuthor();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured: {e.Message}");
            }

            authorPage.UserId = Security.User.Id;
        }

        /// <summary>
        /// Redirects if author.
        /// </summary>
        private async Task RedirectIfAuthor()
        {
            string filter = $"UserId eq '{Security.User.Id}'";
            var result = await ManekiService.GetAuthorPages(filter: filter);

            if (result.Value.Any())
            {
                string pageId = result.Value.First().Id.ToString();
                NavigationManager.NavigateTo($"/author-page/{pageId}");
                return;
            }
        }

        /// <summary>
        /// Forms the submit.
        /// </summary>
        /// <param name="authorPage">The author page.</param>
        protected async Task FormSubmit(ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage)
        {
            try
            {
                NormalizeAllTextFields();
                authorPage.SocialLinks = JsonSerializer.Serialize(new SocialLinks());
                await ManekiService.CreateAuthorPage(authorPage);

                var subscriptions = new List<Subscription>
                {
                    new Subscription
                    {
                        Id = Guid.NewGuid(),
                        Title = "Free",
                        Price = 0,
                        Description = "Free subscription level",
                        PermissionLevel = 0,
                        AuthorPageId = authorPage.Id
                    },
                    new Subscription
                    {
                        Id = Guid.NewGuid(),
                        Title = "Standard",
                        Price = 5,
                        Description = "Standard subscription level",
                        PermissionLevel = 1,
                        AuthorPageId = authorPage.Id
                    },
                    new Subscription
                    {
                        Id = Guid.NewGuid(),
                        Title = "Premium",
                        Price = 15,
                        Description = "Premium subscription level",
                        PermissionLevel = 2,
                        AuthorPageId = authorPage.Id
                    },
                    new Subscription
                    {
                        Id = Guid.NewGuid(),
                        Title = "Epic",
                        Price = 25,
                        Description = "Epic subscription level",
                        PermissionLevel = 3,
                        AuthorPageId = authorPage.Id
                    }
                };

                foreach (var subscription in subscriptions)
                {
                    await ManekiService.CreateSubscription(subscription);
                }

                NavigationManager.NavigateTo("author-page");
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
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
            public string Youtube { get; set; } = null;
            /// <summary>
            /// Gets or sets the instagram.
            /// </summary>
            /// <value>The instagram.</value>
            public string Instagram { get; set; } = null;
            /// <summary>
            /// Gets or sets the telegram.
            /// </summary>
            /// <value>The telegram.</value>
            public string Telegram { get; set; } = null;
            /// <summary>
            /// Gets or sets the tik tok.
            /// </summary>
            /// <value>The tik tok.</value>
            public string TikTok { get; set; } = null;
            /// <summary>
            /// Gets or sets the facebook.
            /// </summary>
            /// <value>The facebook.</value>
            public string Facebook { get; set; } = null;
            /// <summary>
            /// Gets or sets the twitter.
            /// </summary>
            /// <value>The twitter.</value>
            public string Twitter { get; set; } = null;
            /// <summary>
            /// Gets or sets the twitch.
            /// </summary>
            /// <value>The twitch.</value>
            public string Twitch { get; set; } = null;
            /// <summary>
            /// Gets or sets the pinterest.
            /// </summary>
            /// <value>The pinterest.</value>
            public string Pinterest { get; set; } = null;
        }

        /// <summary>
        /// Normalizes all text fields.
        /// </summary>
        private void NormalizeAllTextFields()
        {
            authorPage.Title = GetNormalizedString(authorPage.Title);
            authorPage.Description = GetNormalizedString(authorPage.Description);
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