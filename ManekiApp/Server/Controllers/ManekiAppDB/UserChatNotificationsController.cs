using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Data;

namespace ManekiApp.Server.Controllers.ManekiAppDB
{
    /// <summary>
    /// Class UserChatNotificationsController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Route("odata/ManekiAppDB/UserChatNotifications")]
    public partial class UserChatNotificationsController : ODataController
    {
        /// <summary>
        /// The context
        /// </summary>
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserChatNotificationsController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UserChatNotificationsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Gets the user chat notifications.
        /// </summary>
        /// <returns>IEnumerable&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatNotification&gt;.</returns>
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> GetUserChatNotifications()
        {
            var items = this.context.UserChatNotifications.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification>();
            this.OnUserChatNotificationsRead(ref items);

            return items;
        }

        /// <summary>
        /// Called when [user chat notifications read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnUserChatNotificationsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> items);

        /// <summary>
        /// Called when [user chat notification get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatNotificationGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> item);

        /// <summary>
        /// Gets the user chat notification.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SingleResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatNotification&gt;.</returns>
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        [HttpGet("/odata/ManekiAppDB/UserChatNotifications(UserId={UserId})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> GetUserChatNotification(string key)
        {
            var items = this.context.UserChatNotifications.Where(i => i.UserId == Uri.UnescapeDataString(key));
            var result = SingleResult.Create(items);

            OnUserChatNotificationGet(ref result);

            return result;
        }
        /// <summary>
        /// Called when [user chat notification deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatNotificationDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        /// <summary>
        /// Called when [after user chat notification deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatNotificationDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);

        /// <summary>
        /// Deletes the user chat notification.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("/odata/ManekiAppDB/UserChatNotifications(UserId={UserId})")]
        public IActionResult DeleteUserChatNotification(string key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.UserChatNotifications
                    .FirstOrDefault(i => i.UserId == Uri.UnescapeDataString(key));

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnUserChatNotificationDeleted(item);
                this.context.UserChatNotifications.Remove(item);
                this.context.SaveChanges();
                this.OnAfterUserChatNotificationDeleted(item);

                return new NoContentResult();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [user chat notification updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatNotificationUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        /// <summary>
        /// Called when [after user chat notification updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatNotificationUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);

        /// <summary>
        /// Puts the user chat notification.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("/odata/ManekiAppDB/UserChatNotifications(UserId={UserId})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PutUserChatNotification(string key, [FromBody] ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null || (item.UserId != Uri.UnescapeDataString(key)))
                {
                    return BadRequest();
                }
                this.OnUserChatNotificationUpdated(item);
                this.context.UserChatNotifications.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserChatNotifications.Where(i => i.UserId == Uri.UnescapeDataString(key));
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
                this.OnAfterUserChatNotificationUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Patches the user chat notification.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="patch">The patch.</param>
        /// <returns>IActionResult.</returns>
        [HttpPatch("/odata/ManekiAppDB/UserChatNotifications(UserId={UserId})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PatchUserChatNotification(string key, [FromBody] Delta<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> patch)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.UserChatNotifications.FirstOrDefault(i => i.UserId == Uri.UnescapeDataString(key));

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnUserChatNotificationUpdated(item);
                this.context.UserChatNotifications.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserChatNotifications.Where(i => i.UserId == Uri.UnescapeDataString(key));
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
                this.OnAfterUserChatNotificationUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [user chat notification created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatNotificationCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        /// <summary>
        /// Called when [after user chat notification created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatNotificationCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item)
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

                this.OnUserChatNotificationCreated(item);
                this.context.UserChatNotifications.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserChatNotifications.Where(i => i.UserId == item.UserId);

                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");

                this.OnAfterUserChatNotificationCreated(item);

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
