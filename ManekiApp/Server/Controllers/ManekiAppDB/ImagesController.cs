using System;
using System.Net;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ManekiApp.Server.Controllers.ManekiAppDB
{
    [Route("odata/ManekiAppDB/Images")]
    public partial class ImagesController : ODataController
    {
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        public ImagesController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.Image> GetImages()
        {
            var items = this.context.Images.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.Image>();
            this.OnImagesRead(ref items);

            return items;
        }

        partial void OnImagesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Image> items);

        partial void OnImageGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.Image> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ManekiAppDB/Images(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.Image> GetImage(Guid key)
        {
            var items = this.context.Images.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnImageGet(ref result);

            return result;
        }
        partial void OnImageDeleted(ManekiApp.Server.Models.ManekiAppDB.Image item);
        partial void OnAfterImageDeleted(ManekiApp.Server.Models.ManekiAppDB.Image item);

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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnImageUpdated(ManekiApp.Server.Models.ManekiAppDB.Image item);
        partial void OnAfterImageUpdated(ManekiApp.Server.Models.ManekiAppDB.Image item);

        [HttpPut("/odata/ManekiAppDB/Images(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutImage(Guid key, [FromBody]ManekiApp.Server.Models.ManekiAppDB.Image item)
        {
            try
            {
                if(!ModelState.IsValid)
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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ManekiAppDB/Images(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchImage(Guid key, [FromBody]Delta<ManekiApp.Server.Models.ManekiAppDB.Image> patch)
        {
            try
            {
                if(!ModelState.IsValid)
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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnImageCreated(ManekiApp.Server.Models.ManekiAppDB.Image item);
        partial void OnAfterImageCreated(ManekiApp.Server.Models.ManekiAppDB.Image item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.Image item)
        {
            try
            {
                if(!ModelState.IsValid)
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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
