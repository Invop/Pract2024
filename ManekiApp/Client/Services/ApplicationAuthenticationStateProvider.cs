using ManekiApp.Server.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ManekiApp.Client
{
    /// <summary>
    /// Class ApplicationAuthenticationStateProvider.
    /// Implements the <see cref="AuthenticationStateProvider" />
    /// </summary>
    /// <seealso cref="AuthenticationStateProvider" />
    public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
    {
        /// <summary>
        /// The security service
        /// </summary>
        private readonly SecurityService securityService;
        /// <summary>
        /// The authentication state
        /// </summary>
        private ApplicationAuthenticationState authenticationState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationAuthenticationStateProvider"/> class.
        /// </summary>
        /// <param name="securityService">The security service.</param>
        public ApplicationAuthenticationStateProvider(SecurityService securityService)
        {
            this.securityService = securityService;
        }

        /// <summary>
        /// Asynchronously gets an <see cref="T:Microsoft.AspNetCore.Components.Authorization.AuthenticationState" /> that describes the current user.
        /// </summary>
        /// <returns>A Task&lt;AuthenticationState&gt; representing the asynchronous operation.</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();

            try
            {
                var state = await GetApplicationAuthenticationStateAsync();

                if (state.IsAuthenticated)
                {
                    identity = new ClaimsIdentity(state.Claims.Select(c => new Claim(c.Type, c.Value)), "ManekiApp.Server");
                }
            }
            catch (HttpRequestException ex)
            {
            }

            var result = new AuthenticationState(new ClaimsPrincipal(identity));

            await securityService.InitializeAsync(result);

            return result;
        }

        /// <summary>
        /// Get application authentication state as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;ApplicationAuthenticationState&gt; representing the asynchronous operation.</returns>
        private async Task<ApplicationAuthenticationState> GetApplicationAuthenticationStateAsync()
        {
            if (authenticationState == null)
            {
                authenticationState = await securityService.GetAuthenticationStateAsync();
            }

            return authenticationState;
        }
    }
}