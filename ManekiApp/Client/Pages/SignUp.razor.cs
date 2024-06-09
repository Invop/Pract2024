using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class SignUp.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class SignUp
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
        /// The roles
        /// </summary>
        protected IEnumerable<ManekiApp.Server.Models.ApplicationRole> roles;
        /// <summary>
        /// The user
        /// </summary>
        protected ManekiApp.Server.Models.ApplicationUser user;
        /// <summary>
        /// The error
        /// </summary>
        protected string error;
        /// <summary>
        /// The error visible
        /// </summary>
        protected bool errorVisible;

        /// <summary>
        /// Gets or sets the security.
        /// </summary>
        /// <value>The security.</value>
        [Inject]
        protected SecurityService Security { get; set; }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            user = new ManekiApp.Server.Models.ApplicationUser();

            roles = await Security.GetRoles();
        }

        /// <summary>
        /// Forms the submit.
        /// </summary>
        /// <param name="user">The user.</param>
        protected async Task FormSubmit(ManekiApp.Server.Models.ApplicationUser user)
        {
            try
            {
                user.Roles = roles.Where(role => role.NormalizedName == "FREEUSER").ToList();
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