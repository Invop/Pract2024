using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Data;

namespace ManekiApp.Server.Controllers.ManekiAppDB
{
    /// <summary>
    /// Class ImagesController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Route("odata/ManekiAppDB/Images")]
    public partial class ImagesController : ODataController
    {
        /// <summary>
        /// The context
        /// </summary>
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagesController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ImagesController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <returns>IEnumerable&lt;ManekiApp.Server.Models.ManekiAppDB.Image&gt;.</returns>
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.Image> GetImages()
        {
            var items = this.context.Images.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.Image>();
            this.OnImagesRead(ref items);

            return items;
        }

        /// <summary>
        /// Called when [images read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnImagesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Image> items);

        /// <summary>
        /// Called when [image get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnImageGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.Image> item);

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SingleResult&lt;ManekiApp.Server.Models.ManekiAppDB.Image&gt;.</returns>
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        [HttpGet("/odata/ManekiAppDB/Images(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.Image> GetImage(Guid key)
        {
            var items = this.context.Images.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnImageGet(ref result);

            return result;
        }
        /// <summary>
        /// Called when [image deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnImageDeleted(ManekiApp.Server.Models.ManekiAppDB.Image item);
        /// <summary>
        /// Called when [after image deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterImageDeleted(ManekiApp.Server.Models.ManekiAppDB.Image item);

        /// <summary>
        /// Deletes the image.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("/odata/ManekiAppDB/Images(Id={Id})")]
        public IActionResult DeleteImage(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Images
                    .FirstOrDefault(i => i.Id == key);

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnImageDeleted(item);
                this.context.Images.Remove(item);
                this.context.SaveChanges();
                this.OnAfterImageDeleted(item);

                return new NoContentResult();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [image updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnImageUpdated(ManekiApp.Server.Models.ManekiAppDB.Image item);
        /// <summary>
        /// Called when [after image updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterImageUpdated(ManekiApp.Server.Models.ManekiAppDB.Image item);

        /// <summary>
        /// Puts the image.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("/odata/ManekiAppDB/Images(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PutImage(Guid key, [FromBody] ManekiApp.Server.Models.ManekiAppDB.Image item)
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
                this.OnImageUpdated(item);
                this.context.Images.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Images.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Post");
                this.OnAfterImageUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Patches the image.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="patch">The patch.</param>
        /// <returns>IActionResult.</returns>
        [HttpPatch("/odata/ManekiAppDB/Images(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PatchImage(Guid key, [FromBody] Delta<ManekiApp.Server.Models.ManekiAppDB.Image> patch)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Images.FirstOrDefault(i => i.Id == key);

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnImageUpdated(item);
                this.context.Images.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Images.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Post");
                this.OnAfterImageUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [image created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnImageCreated(ManekiApp.Server.Models.ManekiAppDB.Image item);
        /// <summary>
        /// Called when [after image created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterImageCreated(ManekiApp.Server.Models.ManekiAppDB.Image item);

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.Image item)
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

                this.OnImageCreated(item);
                this.context.Images.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Images.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "Post");

                this.OnAfterImageCreated(item);

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
