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
    [Route("odata/ManekiAppDB/UserChatPayments")]
    public partial class UserChatPaymentsController : ODataController
    {
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        public UserChatPaymentsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> GetUserChatPayments()
        {
            var items = this.context.UserChatPayments.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment>();
            this.OnUserChatPaymentsRead(ref items);

            return items;
        }

        partial void OnUserChatPaymentsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> items);

        partial void OnUserChatPaymentGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ManekiAppDB/UserChatPayments(UserId={UserId})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> GetUserChatPayment(string key)
        {
            var items = this.context.UserChatPayments.Where(i => i.UserId == Uri.UnescapeDataString(key));
            var result = SingleResult.Create(items);

            OnUserChatPaymentGet(ref result);

            return result;
        }
        partial void OnUserChatPaymentDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        partial void OnAfterUserChatPaymentDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);

        [HttpDelete("/odata/ManekiAppDB/UserChatPayments(UserId={UserId})")]
        public IActionResult DeleteUserChatPayment(string key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.UserChatPayments
                    .FirstOrDefault(i => i.UserId == Uri.UnescapeDataString(key));

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnUserChatPaymentDeleted(item);
                this.context.UserChatPayments.Remove(item);
                this.context.SaveChanges();
                this.OnAfterUserChatPaymentDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserChatPaymentUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        partial void OnAfterUserChatPaymentUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);

        [HttpPut("/odata/ManekiAppDB/UserChatPayments(UserId={UserId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutUserChatPayment(string key, [FromBody]ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item)
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
                this.OnUserChatPaymentUpdated(item);
                this.context.UserChatPayments.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserChatPayments.Where(i => i.UserId == Uri.UnescapeDataString(key));
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
                this.OnAfterUserChatPaymentUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ManekiAppDB/UserChatPayments(UserId={UserId})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchUserChatPayment(string key, [FromBody]Delta<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.UserChatPayments.FirstOrDefault(i => i.UserId == Uri.UnescapeDataString(key));

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnUserChatPaymentUpdated(item);
                this.context.UserChatPayments.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserChatPayments.Where(i => i.UserId == Uri.UnescapeDataString(key));
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
                this.OnAfterUserChatPaymentUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserChatPaymentCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        partial void OnAfterUserChatPaymentCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item)
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

                this.OnUserChatPaymentCreated(item);
                this.context.UserChatPayments.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserChatPayments.Where(i => i.UserId == item.UserId);

                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");

                this.OnAfterUserChatPaymentCreated(item);

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
