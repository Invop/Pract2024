using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using ManekiApp.Server.Models.ManekiAppDB;

namespace ManekiApp.Client.Pages
{
    public partial class CreateAuthorPage
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
        protected ManekiAppDBService ManekiService { get; set; }

        protected ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage;
        protected bool errorVisible;
        protected string error;
        
        protected override async Task OnInitializedAsync()
        {
            authorPage = new Server.Models.ManekiAppDB.AuthorPage();
            authorPage.UserId = Security.User.Id;
        }
        
        protected async Task FormSubmit(ManekiApp.Server.Models.ManekiAppDB.AuthorPage authorPage)
        {
            try
            {
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
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }
    }
}