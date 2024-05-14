using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ManekiApp.Client.Pages
{
    public partial class SignUp
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        
        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected IEnumerable<ManekiApp.Server.Models.ApplicationRole> roles;
        protected ManekiApp.Server.Models.ApplicationUser user;
        protected string error;
        protected bool errorVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            user = new ManekiApp.Server.Models.ApplicationUser();

            roles = await Security.GetRoles();
        }

        protected async Task FormSubmit(ManekiApp.Server.Models.ApplicationUser user)
        {
            try
            {
                user.Roles = roles.Where(role => role.NormalizedName=="FREEUSER").ToList();
                await Security.CreateUser(user);
                NavigationManager.NavigateTo("/");
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }
        
    }
}