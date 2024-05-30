using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ManekiApp.Client.Pages
{
    public partial class EditAuthor
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
        
        [Inject]
        protected ManekiAppDBService ManekiAppDBService { get; set; }

        protected SocialLinks socialLinks;
        protected ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage;
        protected string error;
        protected bool errorVisible;

        protected bool isSuccess;
        protected string success;
        protected string lastActionTime;
            
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorPagesOData(string userId)
        {
            var filter = $"UserId eq '{userId}'";
            var authorPagesOData = await ManekiAppDBService.GetAuthorPages(filter: filter);
            return authorPagesOData;
        }


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

        protected async Task FormSubmit(ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage)
        {
            isSuccess = false;
            lastActionTime = DateTime.Now.ToString("HH:mm:ss");
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

        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }

        protected async Task UploadProfileImage(InputFileChangeEventArgs e)
        {
            var file = e.File;
            var buffer = new byte[file.Size];
            await file.OpenReadStream().ReadAsync(buffer);
            authorPage.ProfileImage = Convert.ToBase64String(buffer);
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