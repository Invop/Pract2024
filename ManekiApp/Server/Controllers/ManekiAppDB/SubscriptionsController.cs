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
    [Route("odata/ManekiAppDB/Subscriptions")]
    public partial class SubscriptionsController : ODataController
    {
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        public SubscriptionsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.Subscription> GetSubscriptions()
        {
            var items = this.context.Subscriptions.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription>();
            this.OnSubscriptionsRead(ref items);

            return items;
        }

        partial void OnSubscriptionsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription> items);

        partial void OnSubscriptionGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.Subscription> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ManekiAppDB/Subscriptions(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.Subscription> GetSubscription(Guid key)
        {
            var items = this.context.Subscriptions.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnSubscriptionGet(ref result);

            return result;
        }
        partial void OnSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        partial void OnAfterSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.Subscription item);

        [HttpDelete("/odata/ManekiAppDB/Subscriptions(Id={Id})")]
        public IActionResult DeleteSubscription(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.Subscriptions
                    .FirstOrDefault(i => i.Id == key);

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnSubscriptionDeleted(item);
                this.context.Subscriptions.Remove(item);
                this.context.SaveChanges();
                this.OnAfterSubscriptionDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        partial void OnAfterSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);

        [HttpPut("/odata/ManekiAppDB/Subscriptions(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutSubscription(Guid key, [FromBody]ManekiApp.Server.Models.ManekiAppDB.Subscription item)
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
                this.OnSubscriptionUpdated(item);
                this.context.Subscriptions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Subscriptions.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AuthorPage");
                this.OnAfterSubscriptionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ManekiAppDB/Subscriptions(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchSubscription(Guid key, [FromBody]Delta<ManekiApp.Server.Models.ManekiAppDB.Subscription> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.Subscriptions.FirstOrDefault(i => i.Id == key);

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnSubscriptionUpdated(item);
                this.context.Subscriptions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Subscriptions.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AuthorPage");
                this.OnAfterSubscriptionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        partial void OnAfterSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.Subscription item)
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

                this.OnSubscriptionCreated(item);
                this.context.Subscriptions.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Subscriptions.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "AuthorPage");

                this.OnAfterSubscriptionCreated(item);

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
