using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class AddApplicationRole.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class AddApplicationRole
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
        /// The role
        /// </summary>
        protected ManekiApp.Server.Models.ApplicationRole role;
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
            role = new ManekiApp.Server.Models.ApplicationRole();
        }

        /// <summary>
        /// Forms the submit.
        /// </summary>
        /// <param name="role">The role.</param>
        protected async Task FormSubmit(ManekiApp.Server.Models.ApplicationRole role)
        {
            try
            {
                await Security.CreateRole(role);

                DialogService.Close(null);
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }

        /// <summary>
        /// Cancels the click.
        /// </summary>
        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }
    }
}