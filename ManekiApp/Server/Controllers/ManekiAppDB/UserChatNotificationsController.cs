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
    [Route("odata/ManekiAppDB/UserChatNotifications")]
    public partial class UserChatNotificationsController : ODataController
    {
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        public UserChatNotificationsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> GetUserChatNotifications()
        {
            var items = this.context.UserChatNotifications.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification>();
            this.OnUserChatNotificationsRead(ref items);

            return items;
        }

        partial void OnUserChatNotificationsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> items);

        partial void OnUserChatNotificationGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ManekiAppDB/UserChatNotifications(UserId={UserId})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> GetUserChatNotification(string key)
        {
            var items = this.context.UserChatNotifications.Where(i => i.UserId == Uri.UnescapeDataString(key));
            var result = SingleResult.Create(items);

            OnUserChatNotificationGet(ref result);

            return result;
        }
        partial void OnUserChatNotificationDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        partial void OnAfterUserChatNotificationDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);

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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserChatNotificationUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        partial void OnAfterUserChatNotificationUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);

        [HttpPut("/odata/ManekiAppDB/UserChatNotifications(UserId={UserId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutUserChatNotification(string key, [FromBody]ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item)
        {
            try
            {
                if(!ModelState.IsValid)
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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ManekiAppDB/UserChatNotifications(UserId={UserId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchUserChatNotification(string key, [FromBody]Delta<ManekiApp.Server.Models.ManekiAppDB.UserChatNotification> patch)
        {
            try
            {
                if(!ModelState.IsValid)
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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserChatNotificationCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);
        partial void OnAfterUserChatNotificationCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.UserChatNotification item)
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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
