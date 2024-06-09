using ManekiApp.Server.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class UserSettings.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class UserSettings
    {
        /// <summary>
        /// Gets or sets the js runtime.
        /// </summary>
        /// <value>The js runtime.</value>
        [Inject] protected IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        /// <value>The navigation manager.</value>
        [Inject] protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        [Inject] protected DialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the tooltip service.
        /// </summary>
        /// <value>The tooltip service.</value>
        [Inject] protected TooltipService TooltipService { get; set; }

        /// <summary>
        /// Gets or sets the context menu service.
        /// </summary>
        /// <value>The context menu service.</value>
        [Inject] protected ContextMenuService ContextMenuService { get; set; }

        /// <summary>
        /// Gets or sets the notification service.
        /// </summary>
        /// <value>The notification service.</value>
        [Inject] protected NotificationService NotificationService { get; set; }
        /// <summary>
        /// Gets or sets the maneki application database.
        /// </summary>
        /// <value>The maneki application database.</value>
        [Inject] protected ManekiAppDBService ManekiAppDB { get; set; }

        /// <summary>
        /// The old password
        /// </summary>
        protected string oldPassword = "";
        /// <summary>
        /// The new password
        /// </summary>
        protected string newPassword = "";
        /// <summary>
        /// The confirm password
        /// </summary>
        protected string confirmPassword = "";
        /// <summary>
        /// The user
        /// </summary>
        protected ApplicationUser user;
        /// <summary>
        /// The error
        /// </summary>
        protected string error;
        /// <summary>
        /// The error visible
        /// </summary>
        protected bool errorVisible;
        /// <summary>
        /// The success visible
        /// </summary>
        protected bool successVisible;
        /// <summary>
        /// The code
        /// </summary>
        private string code;

        /// <summary>
        /// Gets or sets the security.
        /// </summary>
        /// <value>The security.</value>
        [Inject] protected SecurityService Security { get; set; }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            user = await Security.GetUserById($"{Security.User.Id}");
        }


        /// <summary>
        /// Checks the code.
        /// </summary>
        private async Task CheckCode()
        {
            var verificationCode = await ManekiAppDB.GetUserVerificationCodeByUserId(Security.User.Id);
            if (verificationCode.Code == Convert.ToInt32(code))
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Your telegram id has been confirmed",
                    Duration = 4000
                });
                user.TelegramConfirmed = true;
                await Security.UpdateUser($"{Security.User.Id}", user);
            }

            else
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = "You entered an invalid code",
                    Duration = 4000
                });
        }


        /// <summary>
        /// Forms the submit.
        /// </summary>
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