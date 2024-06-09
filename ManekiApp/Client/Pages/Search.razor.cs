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
    public partial class Search
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

        protected IEnumerable<Server.Models.ManekiAppDB.AuthorPage> authors =
            new List<Server.Models.ManekiAppDB.AuthorPage>();

        protected int pageSize = 3;
        protected int authorsAmount;
        protected bool isLoading;
        protected string pagingSummaryFormat = "Displaying page {0} of {1} (total {2} records)";
        
        protected string searchFieldValue;
        protected string titleToSearch = "";
        
        protected bool errorVisible;
        protected string error;

        protected RadzenDataList<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> datalist;
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
        
        private async Task<IEnumerable<Server.Models.ManekiAppDB.AuthorPage>> GetAuthors(int? skip, int? top, string title = "")
        {
            return (await GetAuthorsOData(skip, top, title)).Value.ToList();
        }
        
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorsOData(int? skip, int? top, string title)
        {
            string filter = string.IsNullOrEmpty(title) ? string.Empty : $"contains(ToLower(Title), '{title.ToLower()}')";
            var result = await ManekiAppDbService.GetAuthorPages(filter: filter, skip: skip, top: top, expand: "Subscriptions");
            return result;
        }
        
        private async Task<int> GetAuthorsAmount(string title)
        {
            return (await GetAuthorsAmountOData(title)).Count;
        }
        
        private async Task<ODataServiceResult<Server.Models.ManekiAppDB.AuthorPage>> GetAuthorsAmountOData(string title)
        {
            string filter = string.IsNullOrEmpty(title) ? string.Empty : $"contains(ToLower(Title), '{title.ToLower()}')";
            var result = await ManekiAppDbService.GetAuthorPages(filter: filter, count: true, top: 0);
            return result;
        }

        private async void ApplySearch()
        { 
            titleToSearch = searchFieldValue;
            await datalist.FirstPage();
            await datalist.Reload();
        }
    }
}