using ManekiApp.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ManekiApp.Server.Controllers
{
    /// <summary>
    /// Class ApplicationRolesController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Authorize]
    [Route("odata/Identity/ApplicationRoles")]
    public partial class ApplicationRolesController : ODataController
    {
        /// <summary>
        /// The role manager
        /// </summary>
        private readonly RoleManager<ApplicationRole> roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRolesController"/> class.
        /// </summary>
        /// <param name="roleManager">The role manager.</param>
        public ApplicationRolesController(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        /// <summary>
        /// Called when [roles read].
        /// </summary>
        /// <param name="roles">The roles.</param>
        partial void OnRolesRead(ref IQueryable<ApplicationRole> roles);

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;ApplicationRole&gt;.</returns>
        [AllowAnonymous]
        [EnableQuery]
        [HttpGet]
        public IEnumerable<ApplicationRole> Get()
        {
            var roles = roleManager.Roles;
            OnRolesRead(ref roles);

            return roles;
        }

        /// <summary>
        /// Called when [role created].
        /// </summary>
        /// <param name="role">The role.</param>
        partial void OnRoleCreated(ApplicationRole role);

        /// <summary>
        /// Posts the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApplicationRole role)
        {
            if (role == null)
            {
                return BadRequest();
            }

            OnRoleCreated(role);

            var result = await roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                var message = string.Join(", ", result.Errors.Select(error => error.Description));

                return BadRequest(new { error = new { message } });
            }

            return Created($"odata/Identity/Roles('{role.Id}')", role);
        }

        /// <summary>
        /// Called when [role deleted].
        /// </summary>
        /// <param name="role">The role.</param>
        partial void OnRoleDeleted(ApplicationRole role);

        /// <summary>
        /// Deletes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Delete(string key)
        {
            var role = await roleManager.FindByIdAsync(key);

            if (role == null)
            {
                return NotFound();
            }

            OnRoleDeleted(role);

            var result = await roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                var message = string.Join(", ", result.Errors.Select(error => error.Description));

                return BadRequest(new { error = new { message } });
            }

            return new NoContentResult();
        }
    }
}