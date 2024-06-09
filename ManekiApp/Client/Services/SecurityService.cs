using ManekiApp.Server.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace ManekiApp.Client
{
    /// <summary>
    /// Class SecurityService.
    /// </summary>
    public partial class SecurityService
    {

        /// <summary>
        /// The HTTP client
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// The base URI
        /// </summary>
        private readonly Uri baseUri;

        /// <summary>
        /// The navigation manager
        /// </summary>
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <value>The user.</value>
        public ApplicationUser User { get; private set; } = new ApplicationUser { Name = "Anonymous" };

        /// <summary>
        /// Gets the principal.
        /// </summary>
        /// <value>The principal.</value>
        public ClaimsPrincipal Principal { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityService"/> class.
        /// </summary>
        /// <param name="navigationManager">The navigation manager.</param>
        /// <param name="factory">The factory.</param>
        public SecurityService(NavigationManager navigationManager, IHttpClientFactory factory)
        {
            this.baseUri = new Uri($"{navigationManager.BaseUri}odata/Identity/");
            this.httpClient = factory.CreateClient("ManekiApp.Server");
            this.navigationManager = navigationManager;
        }

        /// <summary>
        /// Determines whether [is in role] [the specified roles].
        /// </summary>
        /// <param name="roles">The roles.</param>
        /// <returns><c>true</c> if [is in role] [the specified roles]; otherwise, <c>false</c>.</returns>
        public bool IsInRole(params string[] roles)
        {
#if DEBUG
            if (User.Name == "admin")
            {
                return true;
            }
#endif

            if (roles.Contains("Everybody"))
            {
                return true;
            }

            if (!IsAuthenticated())
            {
                return false;
            }

            if (roles.Contains("Authenticated"))
            {
                return true;
            }

            return roles.Any(role => Principal.IsInRole(role));
        }

        /// <summary>
        /// Determines whether this instance is authenticated.
        /// </summary>
        /// <returns><c>true</c> if this instance is authenticated; otherwise, <c>false</c>.</returns>
        public bool IsAuthenticated()
        {
            return Principal?.Identity.IsAuthenticated == true;
        }

        /// <summary>
        /// Initialize as an asynchronous operation.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A Task&lt;System.Boolean&gt; representing the asynchronous operation.</returns>
        public async Task<bool> InitializeAsync(AuthenticationState result)
        {
            Principal = result.User;
#if DEBUG
            if (Principal.Identity.Name == "admin")
            {
                User = new ApplicationUser { Name = "Admin" };

                return true;
            }
#endif
            var userId = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null && User?.Id != userId)
            {
                User = await GetUserById(userId);
            }

            return IsAuthenticated();
        }


        /// <summary>
        /// Get authentication state as an asynchronous operation.
        /// </summary>
        /// <returns>A Task&lt;ApplicationAuthenticationState&gt; representing the asynchronous operation.</returns>
        public async Task<ApplicationAuthenticationState> GetAuthenticationStateAsync()
        {
            var uri = new Uri($"{navigationManager.BaseUri}Account/CurrentUser");

            var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, uri));

            return await response.ReadAsync<ApplicationAuthenticationState>();
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        public void Logout()
        {
            navigationManager.NavigateTo("Account/Logout", true);
        }

        /// <summary>
        /// Logins this instance.
        /// </summary>
        public void Login()
        {
            navigationManager.NavigateTo("Login", true);
        }

        /// <summary>
        /// Gets the roles.
        /// </summary>
        /// <returns>IEnumerable&lt;ApplicationRole&gt;.</returns>
        public async Task<IEnumerable<ApplicationRole>> GetRoles()
        {
            var uri = new Uri(baseUri, $"ApplicationRoles");

            uri = uri.GetODataUri();

            var response = await httpClient.GetAsync(uri);

            var result = await response.ReadAsync<ODataServiceResult<ApplicationRole>>();

            return result.Value;
        }

        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>ApplicationRole.</returns>
        public async Task<ApplicationRole> CreateRole(ApplicationRole role)
        {
            var uri = new Uri(baseUri, $"ApplicationRoles");

            var content = new StringContent(ODataJsonSerializer.Serialize(role), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);

            return await response.ReadAsync<ApplicationRole>();
        }

        /// <summary>
        /// Deletes the role.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeleteRole(string id)
        {
            var uri = new Uri(baseUri, $"ApplicationRoles('{id}')");

            return await httpClient.DeleteAsync(uri);
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <returns>IEnumerable&lt;ApplicationUser&gt;.</returns>
        public async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            var uri = new Uri(baseUri, $"ApplicationUsers");


            uri = uri.GetODataUri();

            var response = await httpClient.GetAsync(uri);

            var result = await response.ReadAsync<ODataServiceResult<ApplicationUser>>();

            return result.Value;
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>ApplicationUser.</returns>
        public async Task<ApplicationUser> CreateUser(ApplicationUser user)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers");

            var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);

            return await response.ReadAsync<ApplicationUser>();
        }

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HttpResponseMessage.</returns>
        public async Task<HttpResponseMessage> DeleteUser(string id)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers('{id}')");

            return await httpClient.DeleteAsync(uri);
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ApplicationUser.</returns>
        public async Task<ApplicationUser> GetUserById(string id)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers('{id}')?$expand=Roles");

            var response = await httpClient.GetAsync(uri);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            return await response.ReadAsync<ApplicationUser>();
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>ApplicationUser.</returns>
        public async Task<ApplicationUser> UpdateUser(string id, ApplicationUser user)
        {
            var uri = new Uri(baseUri, $"ApplicationUsers('{id}')");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response.ReadAsync<ApplicationUser>();
        }
        public async Task ChangePassword(string oldPassword, string newPassword)
        {
            var uri = new Uri($"{navigationManager.BaseUri}Account/ChangePassword");

            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "oldPassword", oldPassword },
                { "newPassword", newPassword }
            });

            var response = await httpClient.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();

                throw new ApplicationException(message);
            }
        }
    }
}