using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class EditAuthor.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class EditAuthor
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
        /// The social links
        /// </summary>
        protected SocialLinks socialLinks;
        /// <summary>
        /// The author page
        /// </summary>
        protected ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage;
        /// <summary>
        /// The error
        /// </summary>
        protected string error;
        /// <summary>
        /// The error visible
        /// </summary>
        protected bool errorVisible;

        /// <summary>
        /// The is success
        /// </summary>
        protected bool isSuccess;
        /// <summary>
        /// The success
        /// </summary>
        protected string success;
        /// <summary>
        /// The last action time
        /// </summary>
        protected string lastActionTime;

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
                authorPage = authorPagesOData.Value.First();

                //Should replace it with try/catch?
                if (String.IsNullOrEmpty(authorPage.SocialLinks))
                {
                    authorPage.SocialLinks = JsonSerializer.Serialize(new SocialLinks());
                }

                socialLinks = JsonSerializer.Deserialize<SocialLinks>(authorPage.SocialLinks);
            }
            else
            {
                NavigationManager.NavigateTo("/create-author-page");
            }

        }

        /// <summary>
        /// Forms the submit.
        /// </summary>
        /// <param name="authorPage">The author page.</param>
        protected async Task FormSubmit(ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage)
        {
            isSuccess = false;
            lastActionTime = DateTime.Now.ToString("HH:mm:ss");
            NormalizeAllTextFields();
            try
            {
                authorPage.SocialLinks = JsonSerializer.Serialize(socialLinks);
                await ManekiAppDBService.UpdateAuthorPage(authorPage.Id, authorPage);
                isSuccess = true;
                success = $"{lastActionTime} – Your author page has been updated!";
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = $"{lastActionTime} – {ex.Message}";
            }
        }

        /// <summary>
        /// Cancels the click.
        /// </summary>
        protected async Task CancelClick()
        {
            NavigationManager.NavigateTo($"/author-page/{authorPage.Id}");
        }

        /// <summary>
        /// Uploads the profile image.
        /// </summary>
        /// <param name="e">The <see cref="InputFileChangeEventArgs"/> instance containing the event data.</param>
        protected async Task UploadProfileImage(InputFileChangeEventArgs e)
        {
            var file = e.File;
            var buffer = new byte[file.Size];
            await file.OpenReadStream().ReadAsync(buffer);
            authorPage.ProfileImage = Convert.ToBase64String(buffer);
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