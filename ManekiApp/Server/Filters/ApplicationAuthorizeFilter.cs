using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ManekiApp.Server.Filters
{
    /// <summary>
    /// Class ApplicationAuthorizeFilter.
    /// Implements the <see cref="AuthorizeFilter" />
    /// </summary>
    /// <seealso cref="AuthorizeFilter" />
    public class ApplicationAuthorizeFilter : AuthorizeFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationAuthorizeFilter"/> class.
        /// </summary>
        /// <param name="policy">Authorization policy to be used.</param>
        public ApplicationAuthorizeFilter(AuthorizationPolicy policy) : base(policy)
        {
        }

        /// <summary>
        /// Called when [authorization asynchronous].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        /// <inheritdoc />
        public override Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Path.StartsWithSegments("/Account") || context.HttpContext.Request.Path.StartsWithSegments("/Login"))
            {
                return Task.CompletedTask;
            }

            return base.OnAuthorizationAsync(context);
        }
    }
}