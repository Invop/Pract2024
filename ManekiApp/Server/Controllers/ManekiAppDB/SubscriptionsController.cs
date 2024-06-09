using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Data;

namespace ManekiApp.Server.Controllers.ManekiAppDB
{
    /// <summary>
    /// Class SubscriptionsController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Route("odata/ManekiAppDB/Subscriptions")]
    public partial class SubscriptionsController : ODataController
    {
        /// <summary>
        /// The context
        /// </summary>
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SubscriptionsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Gets the subscriptions.
        /// </summary>
        /// <returns>IEnumerable&lt;ManekiApp.Server.Models.ManekiAppDB.Subscription&gt;.</returns>
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.Subscription> GetSubscriptions()
        {
            var items = this.context.Subscriptions.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription>();
            this.OnSubscriptionsRead(ref items);

            return items;
        }

        /// <summary>
        /// Called when [subscriptions read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnSubscriptionsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.Subscription> items);

        /// <summary>
        /// Called when [subscription get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnSubscriptionGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.Subscription> item);

        /// <summary>
        /// Gets the subscription.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SingleResult&lt;ManekiApp.Server.Models.ManekiAppDB.Subscription&gt;.</returns>
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        [HttpGet("/odata/ManekiAppDB/Subscriptions(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.Subscription> GetSubscription(Guid key)
        {
            var items = this.context.Subscriptions.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnSubscriptionGet(ref result);

            return result;
        }
        /// <summary>
        /// Called when [subscription deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        /// <summary>
        /// Called when [after subscription deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.Subscription item);

        /// <summary>
        /// Deletes the subscription.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [subscription updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        /// <summary>
        /// Called when [after subscription updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);

        /// <summary>
        /// Puts the subscription.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("/odata/ManekiAppDB/Subscriptions(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PutSubscription(Guid key, [FromBody] ManekiApp.Server.Models.ManekiAppDB.Subscription item)
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
                this.OnSubscriptionUpdated(item);
                this.context.Subscriptions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Subscriptions.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AuthorPage");
                this.OnAfterSubscriptionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Patches the subscription.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="patch">The patch.</param>
        /// <returns>IActionResult.</returns>
        [HttpPatch("/odata/ManekiAppDB/Subscriptions(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PatchSubscription(Guid key, [FromBody] Delta<ManekiApp.Server.Models.ManekiAppDB.Subscription> patch)
        {
            try
            {
                if (!ModelState.IsValid)
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [subscription created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);
        /// <summary>
        /// Called when [after subscription created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.Subscription item);

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.Subscription item)
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
