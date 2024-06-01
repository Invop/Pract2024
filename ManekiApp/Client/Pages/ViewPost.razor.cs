using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ManekiApp.Client.Pages
{
    public partial class ViewPost
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
        protected ManekiAppDBService ManekiAppDbService { get; set; }
        
        [Parameter]
        public string Id { get; set; }

        protected bool errorVisible;
        protected string error;

        protected bool PostExists = true;
        protected Server.Models.ManekiAppDB.Post Post = new Post();
        protected Server.Models.ManekiAppDB.AuthorPage Author = new Server.Models.ManekiAppDB.AuthorPage();
        
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.Post>> GetPostById(Guid postId)
        {
            var filter = $"Id eq {postId}";
            var postOData = await ManekiAppDbService.GetPosts(filter: filter);
            return postOData;
        }

        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorById(Guid authorId)
        {
            var filter = $"Id eq {authorId}";
            var authorOData = await ManekiAppDbService.GetAuthorPages(filter: filter);
            return authorOData;
        }
        
        protected override async Task OnInitializedAsync()
        {
            Guid postId;
            if (!Guid.TryParse(Id, out postId))
            {
                PostExists = false;
                return;
            }

            try
            {
                var postOData = await GetPostById(postId);
                if (postOData.Value.Any())
                {
                    Post = postOData.Value.First();
                    var authorOData = await GetAuthorById(Post.AuthorPageId);
                    Author = authorOData.Value.FirstOrDefault();
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
    }
}