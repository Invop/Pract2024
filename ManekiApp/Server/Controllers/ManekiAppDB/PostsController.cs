using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Data;

namespace ManekiApp.Server.Controllers.ManekiAppDB
{
    /// <summary>
    /// Class PostsController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Route("odata/ManekiAppDB/Posts")]
    public partial class PostsController : ODataController
    {
        /// <summary>
        /// The context
        /// </summary>
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostsController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PostsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Gets the posts.
        /// </summary>
        /// <returns>IEnumerable&lt;ManekiApp.Server.Models.ManekiAppDB.Post&gt;.</returns>
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.Post> GetPosts()
        {
            var items = this.context.Posts.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.Post>();
            this.OnPostsRead(ref items);

            return items;
        }

        /// <summary>
        /// Called when [posts read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnPostsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Post> items);

        /// <summary>
        /// Called when [post get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnPostGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.Post> item);

        /// <summary>
        /// Gets the post.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SingleResult&lt;ManekiApp.Server.Models.ManekiAppDB.Post&gt;.</returns>
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        [HttpGet("/odata/ManekiAppDB/Posts(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.Post> GetPost(Guid key)
        {
            var items = this.context.Posts.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnPostGet(ref result);

            return result;
        }
        /// <summary>
        /// Called when [post deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnPostDeleted(ManekiApp.Server.Models.ManekiAppDB.Post item);
        /// <summary>
        /// Called when [after post deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterPostDeleted(ManekiApp.Server.Models.ManekiAppDB.Post item);

        /// <summary>
        /// Deletes the post.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("/odata/ManekiAppDB/Posts(Id={Id})")]
        public IActionResult DeletePost(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Posts
                    .FirstOrDefault(i => i.Id == key);

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnPostDeleted(item);
                this.context.Posts.Remove(item);
                this.context.SaveChanges();
                this.OnAfterPostDeleted(item);

                return new NoContentResult();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [post updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnPostUpdated(ManekiApp.Server.Models.ManekiAppDB.Post item);
        /// <summary>
        /// Called when [after post updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterPostUpdated(ManekiApp.Server.Models.ManekiAppDB.Post item);

        /// <summary>
        /// Puts the post.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("/odata/ManekiAppDB/Posts(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PutPost(Guid key, [FromBody] ManekiApp.Server.Models.ManekiAppDB.Post item)
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
                this.OnPostUpdated(item);
                this.context.Posts.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Posts.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AuthorPage");
                this.OnAfterPostUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Patches the post.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="patch">The patch.</param>
        /// <returns>IActionResult.</returns>
        [HttpPatch("/odata/ManekiAppDB/Posts(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PatchPost(Guid key, [FromBody] Delta<ManekiApp.Server.Models.ManekiAppDB.Post> patch)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Posts.FirstOrDefault(i => i.Id == key);

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnPostUpdated(item);
                this.context.Posts.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Posts.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AuthorPage");
                this.OnAfterPostUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [post created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnPostCreated(ManekiApp.Server.Models.ManekiAppDB.Post item);
        /// <summary>
        /// Called when [after post created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterPostCreated(ManekiApp.Server.Models.ManekiAppDB.Post item);

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.Post item)
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

                this.OnPostCreated(item);
                this.context.Posts.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Posts.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "AuthorPage");

                this.OnAfterPostCreated(item);

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
