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
    [Route("odata/ManekiAppDB/UserSubscriptions")]
    public partial class UserSubscriptionsController : ODataController
    {
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        public UserSubscriptionsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> GetUserSubscriptions()
        {
            var items = this.context.UserSubscriptions.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>();
            this.OnUserSubscriptionsRead(ref items);

            return items;
        }

        partial void OnUserSubscriptionsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> items);

        partial void OnUserSubscriptionGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ManekiAppDB/UserSubscriptions(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> GetUserSubscription(Guid key)
        {
            var items = this.context.UserSubscriptions.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnUserSubscriptionGet(ref result);

            return result;
        }
        partial void OnUserSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        partial void OnAfterUserSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);

        [HttpDelete("/odata/ManekiAppDB/UserSubscriptions(Id={Id})")]
        public IActionResult DeleteUserSubscription(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.UserSubscriptions
                    .Where(i => i.Id == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnUserSubscriptionDeleted(item);
                this.context.UserSubscriptions.Remove(item);
                this.context.SaveChanges();
                this.OnAfterUserSubscriptionDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        partial void OnAfterUserSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);

        [HttpPut("/odata/ManekiAppDB/UserSubscriptions(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutUserSubscription(Guid key, [FromBody]ManekiApp.Server.Models.ManekiAppDB.UserSubscription item)
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
                this.OnUserSubscriptionUpdated(item);
                this.context.UserSubscriptions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserSubscriptions.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Subscription");
                this.OnAfterUserSubscriptionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ManekiAppDB/UserSubscriptions(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchUserSubscription(Guid key, [FromBody]Delta<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.UserSubscriptions.Where(i => i.Id == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnUserSubscriptionUpdated(item);
                this.context.UserSubscriptions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserSubscriptions.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Subscription");
                this.OnAfterUserSubscriptionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        partial void OnAfterUserSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.UserSubscription item)
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

                this.OnUserSubscriptionCreated(item);
                this.context.UserSubscriptions.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserSubscriptions.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "Subscription");

                this.OnAfterUserSubscriptionCreated(item);

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
