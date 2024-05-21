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
    [Route("odata/ManekiAppDB/AuthorPages")]
    public partial class AuthorPagesController : ODataController
    {
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        public AuthorPagesController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> GetAuthorPages()
        {
            var items = this.context.AuthorPages.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage>();
            this.OnAuthorPagesRead(ref items);

            return items;
        }

        partial void OnAuthorPagesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> items);

        partial void OnAuthorPageGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ManekiAppDB/AuthorPages(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> GetAuthorPage(Guid key)
        {
            var items = this.context.AuthorPages.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnAuthorPageGet(ref result);

            return result;
        }
        partial void OnAuthorPageDeleted(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        partial void OnAfterAuthorPageDeleted(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

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
                    .Where(i => i.Id == key)
                    .FirstOrDefault();

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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAuthorPageUpdated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        partial void OnAfterAuthorPageUpdated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

        [HttpPut("/odata/ManekiAppDB/AuthorPages(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutAuthorPage(Guid key, [FromBody]ManekiApp.Server.Models.ManekiAppDB.AuthorPage item)
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
                this.OnAuthorPageUpdated(item);
                this.context.AuthorPages.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AuthorPages.Where(i => i.Id == key);
                
                this.OnAfterAuthorPageUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ManekiAppDB/AuthorPages(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchAuthorPage(Guid key, [FromBody]Delta<ManekiApp.Server.Models.ManekiAppDB.AuthorPage> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.AuthorPages.Where(i => i.Id == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnAuthorPageUpdated(item);
                this.context.AuthorPages.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AuthorPages.Where(i => i.Id == key);
                
                this.OnAfterAuthorPageUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnAuthorPageCreated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);
        partial void OnAfterAuthorPageCreated(ManekiApp.Server.Models.ManekiAppDB.AuthorPage item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.AuthorPage item)
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

                this.OnAuthorPageCreated(item);
                this.context.AuthorPages.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.AuthorPages.Where(i => i.Id == item.Id);

                

                this.OnAfterAuthorPageCreated(item);

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
