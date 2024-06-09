using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Data;

namespace ManekiApp.Server.Controllers.ManekiAppDB
{
    /// <summary>
    /// Class UserChatPaymentsController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Route("odata/ManekiAppDB/UserChatPayments")]
    public partial class UserChatPaymentsController : ODataController
    {
        /// <summary>
        /// The context
        /// </summary>
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserChatPaymentsController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UserChatPaymentsController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Gets the user chat payments.
        /// </summary>
        /// <returns>IEnumerable&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatPayment&gt;.</returns>
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> GetUserChatPayments()
        {
            var items = this.context.UserChatPayments.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment>();
            this.OnUserChatPaymentsRead(ref items);

            return items;
        }

        /// <summary>
        /// Called when [user chat payments read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnUserChatPaymentsRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> items);

        /// <summary>
        /// Called when [user chat payment get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatPaymentGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> item);

        /// <summary>
        /// Gets the user chat payment.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SingleResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserChatPayment&gt;.</returns>
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        [HttpGet("/odata/ManekiAppDB/UserChatPayments(UserId={UserId})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> GetUserChatPayment(string key)
        {
            var items = this.context.UserChatPayments.Where(i => i.UserId == Uri.UnescapeDataString(key));
            var result = SingleResult.Create(items);

            OnUserChatPaymentGet(ref result);

            return result;
        }
        /// <summary>
        /// Called when [user chat payment deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatPaymentDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        /// <summary>
        /// Called when [after user chat payment deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatPaymentDeleted(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);

        /// <summary>
        /// Deletes the user chat payment.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [user chat payment updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatPaymentUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        /// <summary>
        /// Called when [after user chat payment updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatPaymentUpdated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);

        /// <summary>
        /// Puts the user chat payment.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("/odata/ManekiAppDB/UserChatPayments(UserId={UserId})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PutUserChatPayment(string key, [FromBody] ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item)
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
                this.OnUserChatPaymentUpdated(item);
                this.context.UserChatPayments.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserChatPayments.Where(i => i.UserId == Uri.UnescapeDataString(key));
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
                this.OnAfterUserChatPaymentUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Patches the user chat payment.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="patch">The patch.</param>
        /// <returns>IActionResult.</returns>
        [HttpPatch("/odata/ManekiAppDB/UserChatPayments(UserId={UserId})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PatchUserChatPayment(string key, [FromBody] Delta<ManekiApp.Server.Models.ManekiAppDB.UserChatPayment> patch)
        {
            try
            {
                if (!ModelState.IsValid)
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [user chat payment created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserChatPaymentCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);
        /// <summary>
        /// Called when [after user chat payment created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserChatPaymentCreated(ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item);

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.UserChatPayment item)
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
