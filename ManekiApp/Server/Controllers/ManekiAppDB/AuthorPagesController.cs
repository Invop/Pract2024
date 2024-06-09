using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Data;

namespace ManekiApp.Server.Controllers.ManekiAppDB
{
    /// <summary>
    /// Class AuthorPagesController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Route("odata/ManekiAppDB/AuthorPages")]
    public partial class AuthorPagesController : ODataController
    {
        /// <summary>
        /// The context
        /// </summary>
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorPagesController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AuthorPagesController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Gets the author pages.
        /// </summary>
        /// <returns>IEnumerable&lt;ManekiApp.Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> GetAuthorPages()
        {
            var items = this.context.AuthorPages.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>();
            this.OnAuthorPagesRead(ref items);

            return items;
        }

        /// <summary>
        /// Called when [author pages read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnAuthorPagesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> items);

        /// <summary>
        /// Called when [author page get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAuthorPageGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> item);

        /// <summary>
        /// Gets the author page.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SingleResult&lt;ManekiApp.Server.Models.ManekiAppDB.AuthorPage&gt;.</returns>
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        [HttpGet("/odata/ManekiAppDB/AuthorPages(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> GetAuthorPage(Guid key)
        {
            var items = this.context.AuthorPages.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnAuthorPageGet(ref result);

            return result;
        }
        /// <summary>
        /// Called when [author page deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAuthorPageDeleted(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        /// <summary>
        /// Called when [after author page deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterAuthorPageDeleted(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

        /// <summary>
        /// Deletes the author page.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("/odata/ManekiAppDB/AuthorPages(Id={Id})")]
        public IActionResult DeleteAuthorPage(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.AuthorPages
                    .FirstOrDefault(i => i.Id == key);

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnAuthorPageDeleted(item);
                this.context.AuthorPages.Remove(item);
                this.context.SaveChanges();
                this.OnAfterAuthorPageDeleted(item);

                return new NoContentResult();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [author page updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAuthorPageUpdated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        /// <summary>
        /// Called when [after author page updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterAuthorPageUpdated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

        /// <summary>
        /// Puts the author page.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("/odata/ManekiAppDB/AuthorPages(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PutAuthorPage(Guid key, [FromBody] ManekiApp.Server.Models.ManekiAppDB.AuthorPage item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.Id != key))
                {
                    return BadRequest();
                }
                this.OnAuthorPageUpdated(item);
                this.context.AuthorPages.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AuthorPages.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
                this.OnAfterAuthorPageUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Patches the author page.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="patch">The patch.</param>
        /// <returns>IActionResult.</returns>
        [HttpPatch("/odata/ManekiAppDB/AuthorPages(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PatchAuthorPage(Guid key, [FromBody] Delta<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> patch)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.AuthorPages.FirstOrDefault(i => i.Id == key);

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnAuthorPageUpdated(item);
                this.context.AuthorPages.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AuthorPages.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
                this.OnAfterAuthorPageUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [author page created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAuthorPageCreated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        /// <summary>
        /// Called when [after author page created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterAuthorPageCreated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.AuthorPage item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null)
                {
                    return BadRequest();
                }

                this.OnAuthorPageCreated(item);
                this.context.AuthorPages.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AuthorPages.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");

                this.OnAfterAuthorPageCreated(item);

                return new ObjectResult(SingleResult.Create(itemToReturn))
                {
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
