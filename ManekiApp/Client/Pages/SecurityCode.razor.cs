using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace ManekiApp.Client.Pages
{
    /// <summary>
    /// Class SecurityCode.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class SecurityCode
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
        /// Verifies the security code.
        /// </summary>
        /// <param name="code">The code.</param>
        async Task VerifySecurityCode(string code)
        {
            if (code.Count() == 6)
            {
                await JSRuntime.InvokeVoidAsync("eval", "document.forms[0].submit()");
            }
        }

        /// <summary>
        /// The message
        /// </summary>
        string message;
        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var uri = new Uri(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).ToString());
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            message = $"We sent a verification code to {query.Get("email")}. Enter the code from the email below.";
        }

        /// <summary>
        /// The sc
        /// </summary>
        RadzenSecurityCode sc;

        /// <summary>
        /// Gets or sets the security.
        /// </summary>
        /// <value>The security.</value>
        [Inject]
        protected SecurityService Security { get; set; }
        /// <summary>
        /// Method invoked after each time the component has been rendered interactively and the UI has finished
        /// updating (for example, after elements have been added to the browser DOM). Any <see cref="T:Microsoft.AspNetCore.Components.ElementReference" />
        /// fields will be populated by the time this runs.
        /// This method is not invoked during prerendering or server-side rendering, because those processes
        /// are not attached to any live browser DOM and are already complete before the DOM is updated.
        /// Note that the component does not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />,
        /// because that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">Set to <c>true</c> if this is the first time <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        /// on this component instance; otherwise <c>false</c>.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        /// are useful for performing interop, or interacting with values received from <c>@ref</c>.
        /// Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        /// once.</remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await sc.FocusAsync();
            }
        }
    }
}
