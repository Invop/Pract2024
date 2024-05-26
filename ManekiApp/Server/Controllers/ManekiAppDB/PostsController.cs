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
    [Route("odata/ManekiAppDB/Posts")]
    public partial class PostsController : ODataController
    {
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        public PostsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.Post> GetPosts()
        {
            var items = this.context.Posts.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.Post>();
            this.OnPostsRead(ref items);

            return items;
        }

        partial void OnPostsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Post> items);

        partial void OnPostGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.Post> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ManekiAppDB/Posts(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.Post> GetPost(Guid key)
        {
            var items = this.context.Posts.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnPostGet(ref result);

            return result;
        }
        partial void OnPostDeleted(ManekiApp.Server.Models.ManekiAppDB.Post item);
        partial void OnAfterPostDeleted(ManekiApp.Server.Models.ManekiAppDB.Post item);

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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPostUpdated(ManekiApp.Server.Models.ManekiAppDB.Post item);
        partial void OnAfterPostUpdated(ManekiApp.Server.Models.ManekiAppDB.Post item);

        [HttpPut("/odata/ManekiAppDB/Posts(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutPost(Guid key, [FromBody]ManekiApp.Server.Models.ManekiAppDB.Post item)
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
                this.OnPostUpdated(item);
                this.context.Posts.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Posts.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AuthorPage");
                this.OnAfterPostUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ManekiAppDB/Posts(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchPost(Guid key, [FromBody]Delta<ManekiApp.Server.Models.ManekiAppDB.Post> patch)
        {
            try
            {
                if(!ModelState.IsValid)
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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnPostCreated(ManekiApp.Server.Models.ManekiAppDB.Post item);
        partial void OnAfterPostCreated(ManekiApp.Server.Models.ManekiAppDB.Post item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.Post item)
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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
