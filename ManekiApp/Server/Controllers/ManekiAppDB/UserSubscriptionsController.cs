using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Data;

namespace ManekiApp.Server.Controllers.ManekiAppDB
{
    /// <summary>
    /// Class UserSubscriptionsController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Route("odata/ManekiAppDB/UserSubscriptions")]
    public partial class UserSubscriptionsController : ODataController
    {
        /// <summary>
        /// The context
        /// </summary>
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSubscriptionsController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UserSubscriptionsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Gets the user subscriptions.
        /// </summary>
        /// <returns>IEnumerable&lt;ManekiApp.Server.Models.ManekiAppDB.UserSubscription&gt;.</returns>
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> GetUserSubscriptions()
        {
            var items = this.context.UserSubscriptions.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription>();
            this.OnUserSubscriptionsRead(ref items);

            return items;
        }

        /// <summary>
        /// Called when [user subscriptions read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnUserSubscriptionsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> items);

        /// <summary>
        /// Called when [user subscription get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserSubscriptionGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> item);

        /// <summary>
        /// Gets the user subscription.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SingleResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserSubscription&gt;.</returns>
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        [HttpGet("/odata/ManekiAppDB/UserSubscriptions(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> GetUserSubscription(Guid key)
        {
            var items = this.context.UserSubscriptions.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnUserSubscriptionGet(ref result);

            return result;
        }
        /// <summary>
        /// Called when [user subscription deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        /// <summary>
        /// Called when [after user subscription deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserSubscriptionDeleted(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);

        /// <summary>
        /// Deletes the user subscription.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
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
                    .FirstOrDefault(i => i.Id == key);

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
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [user subscription updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        /// <summary>
        /// Called when [after user subscription updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserSubscriptionUpdated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);

        /// <summary>
        /// Puts the user subscription.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("/odata/ManekiAppDB/UserSubscriptions(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PutUserSubscription(Guid key, [FromBody] ManekiApp.Server.Models.ManekiAppDB.UserSubscription item)
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
                this.OnUserSubscriptionUpdated(item);
                this.context.UserSubscriptions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserSubscriptions.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Subscription,AspNetUser");
                this.OnAfterUserSubscriptionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Patches the user subscription.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="patch">The patch.</param>
        /// <returns>IActionResult.</returns>
        [HttpPatch("/odata/ManekiAppDB/UserSubscriptions(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PatchUserSubscription(Guid key, [FromBody] Delta<ManekiApp.Server.Models.ManekiAppDB.UserSubscription> patch)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.UserSubscriptions.FirstOrDefault(i => i.Id == key);

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnUserSubscriptionUpdated(item);
                this.context.UserSubscriptions.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserSubscriptions.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Subscription,AspNetUser");
                this.OnAfterUserSubscriptionUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [user subscription created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);
        /// <summary>
        /// Called when [after user subscription created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserSubscriptionCreated(ManekiApp.Server.Models.ManekiAppDB.UserSubscription item);

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.UserSubscription item)
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

                this.OnUserSubscriptionCreated(item);
                this.context.UserSubscriptions.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserSubscriptions.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "Subscription,AspNetUser");

                this.OnAfterUserSubscriptionCreated(item);

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
