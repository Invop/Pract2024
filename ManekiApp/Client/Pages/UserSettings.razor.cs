using ManekiApp.Server.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages
{
    public partial class UserSettings
    {
        [Inject] protected IJSRuntime JSRuntime { get; set; }

        [Inject] protected NavigationManager NavigationManager { get; set; }

        [Inject] protected DialogService DialogService { get; set; }

        [Inject] protected TooltipService TooltipService { get; set; }

        [Inject] protected ContextMenuService ContextMenuService { get; set; }

        [Inject] protected NotificationService NotificationService { get; set; }
        [Inject] protected ManekiAppDBService ManekiAppDB { get; set; }

        protected string oldPassword = "";
        protected string newPassword = "";
        protected string confirmPassword = "";
        protected ApplicationUser user;
        protected string error;
        protected bool errorVisible;
        protected bool successVisible;
        private string code;

        [Inject] protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            user = await Security.GetUserById($"{Security.User.Id}");
        }


        private async Task CheckCode()
        {
            var verificationCode = await ManekiAppDB.GetUserVerificationCodeByUserId(Security.User.Id);
            if (verificationCode.Code == Convert.ToInt32(code))
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Info, Summary = $"{verificationCode.Code}",
                    Detail = $"Info Detail{code}", Duration = 4000
                });
            else
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error, Summary = $"{verificationCode.Code}",
                    Detail = $"Info Detail{code}", Duration = 4000
                });
        }


        protected async Task FormSubmit()
        {
            try
            {
                await Security.ChangePassword(oldPassword, newPassword);
                successVisible = true;
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }
    }
}