using ManekiApp.Server.Data;
using ManekiApp.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ManekiApp.Server.Controllers
{
    /// <summary>
    /// Class ApplicationUsersController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Authorize]
    [Route("odata/Identity/ApplicationUsers")]
    public partial class ApplicationUsersController : ODataController
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly ApplicationIdentityDbContext context;
        /// <summary>
        /// The user manager
        /// </summary>
        private readonly UserManager<ApplicationUser> userManager;


        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUsersController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userManager">The user manager.</param>
        public ApplicationUsersController(ApplicationIdentityDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        /// <summary>
        /// Called when [users read].
        /// </summary>
        /// <param name="users">The users.</param>
        partial void OnUsersRead(ref IQueryable<ApplicationUser> users);

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;ApplicationUser&gt;.</returns>
        [EnableQuery]
        [HttpGet]
        public IEnumerable<ApplicationUser> Get()
        {
            var users = userManager.Users;
            OnUsersRead(ref users);

            return users;
        }

        /// <summary>
        /// Gets the application user.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SingleResult&lt;ApplicationUser&gt;.</returns>
        [EnableQuery]
        [HttpGet("{Id}")]
        public SingleResult<ApplicationUser> GetApplicationUser(string key)
        {
            var user = context.Users.Where(i => i.Id == key);

            return SingleResult.Create(user);
        }

        /// <summary>
        /// Called when [user deleted].
        /// </summary>
        /// <param name="user">The user.</param>
        partial void OnUserDeleted(ApplicationUser user);

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string key)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnUserDeleted(user);

            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        /// <summary>
        /// Called when [user updated].
        /// </summary>
        /// <param name="user">The user.</param>
        partial void OnUserUpdated(ApplicationUser user);

        /// <summary>
        /// Patches the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        /// <returns>IActionResult.</returns>
        [HttpPatch("{Id}")]
        public async Task<IActionResult> Patch(string key, [FromBody] ApplicationUser data)
        {
            var user = await userManager.FindByIdAsync(key);

            if (user == null)
            {
                return NotFound();
            }

            OnUserUpdated(data);

            IdentityResult result = null;

            user.Roles = null;
            user.TelegramConfirmed = data.TelegramConfirmed;
            result = await userManager.UpdateAsync(user);

            if (data.Roles != null)
            {
                result = await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));

                if (result.Succeeded)
                {
                    result = await userManager.AddToRolesAsync(user, data.Roles.Select(r => r.Name));
                }
            }

            if (!string.IsNullOrEmpty(data.Password))
            {
                result = await userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    result = await userManager.AddPasswordAsync(user, data.Password);
                }

                if (!result.Succeeded)
                {
                    return IdentityError(result);
                }
            }

            if (result != null && !result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        /// <summary>
        /// Called when [user created].
        /// </summary>
        /// <param name="user">The user.</param>
        partial void OnUserCreated(ApplicationUser user);

        /// <summary>
        /// Posts the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IActionResult.</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApplicationUser user)
        {
            var password = user.Password;
            var roles = user.Roles;
            user.Roles = null;
            IdentityResult result = await userManager.CreateAsync(user, password);

            if (result.Succeeded && roles != null)
            {
                result = await userManager.AddToRolesAsync(user, roles.Select(r => r.Name));
            }

            user.Roles = roles;

            if (result.Succeeded)
            {
                OnUserCreated(user);

                return Created($"odata/Identity/Users('{user.Id}')", user);
            }
            else
            {
                return IdentityError(result);
            }
        }

        /// <summary>
        /// Identities the error.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>IActionResult.</returns>
        private IActionResult IdentityError(IdentityResult result)
        {
            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            return BadRequest(new { error = new { message } });
        }
    }
}