using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class ApplicationUsers.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class ApplicationUsers
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
        /// The users
        /// </summary>
        protected IEnumerable<ManekiApp.Server.Models.ApplicationUser> users;
        /// <summary>
        /// The grid0
        /// </summary>
        protected RadzenDataGrid<ManekiApp.Server.Models.ApplicationUser> grid0;
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
            users = await Security.GetUsers();
        }

        /// <summary>
        /// Adds the click.
        /// </summary>
        protected async Task AddClick()
        {
            await DialogService.OpenAsync<AddApplicationUser>("Add Application User");

            users = await Security.GetUsers();
        }

        /// <summary>
        /// Rows the select.
        /// </summary>
        /// <param name="user">The user.</param>
        protected async Task RowSelect(ManekiApp.Server.Models.ApplicationUser user)
        {
            await DialogService.OpenAsync<EditApplicationUser>("Edit Application User", new Dictionary<string, object> { { "Id", user.Id } });

            users = await Security.GetUsers();
        }

        /// <summary>
        /// Deletes the click.
        /// </summary>
        /// <param name="user">The user.</param>
        protected async Task DeleteClick(ManekiApp.Server.Models.ApplicationUser user)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this user?") == true)
                {
                    await Security.DeleteUser($"{user.Id}");

                    users = await Security.GetUsers();
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