using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Data;

namespace ManekiApp.Server.Controllers.ManekiAppDB
{
    /// <summary>
    /// Class UserVerificationCodesController.
    /// Implements the <see cref="ODataController" />
    /// </summary>
    /// <seealso cref="ODataController" />
    [Route("odata/ManekiAppDB/UserVerificationCodes")]
    public partial class UserVerificationCodesController : ODataController
    {
        /// <summary>
        /// The context
        /// </summary>
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserVerificationCodesController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UserVerificationCodesController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Gets the user verification codes.
        /// </summary>
        /// <returns>IEnumerable&lt;ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode&gt;.</returns>
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> GetUserVerificationCodes()
        {
            var items = this.context.UserVerificationCodes.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>();
            this.OnUserVerificationCodesRead(ref items);

            return items;
        }

        /// <summary>
        /// Called when [user verification codes read].
        /// </summary>
        /// <param name="items">The items.</param>
        partial void OnUserVerificationCodesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> items);

        /// <summary>
        /// Called when [user verification code get].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserVerificationCodeGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> item);

        /// <summary>
        /// Gets the user verification code.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>SingleResult&lt;ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode&gt;.</returns>
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        [HttpGet("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> GetUserVerificationCode(Guid key)
        {
            var items = this.context.UserVerificationCodes.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnUserVerificationCodeGet(ref result);

            return result;
        }
        /// <summary>
        /// Called when [user verification code deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserVerificationCodeDeleted(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        /// <summary>
        /// Called when [after user verification code deleted].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserVerificationCodeDeleted(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);

        /// <summary>
        /// Deletes the user verification code.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>IActionResult.</returns>
        [HttpDelete("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
        public IActionResult DeleteUserVerificationCode(Guid key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var item = this.context.UserVerificationCodes
                    .Where(i => i.Id == key)
                    .FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                this.OnUserVerificationCodeDeleted(item);
                this.context.UserVerificationCodes.Remove(item);
                this.context.SaveChanges();
                this.OnAfterUserVerificationCodeDeleted(item);

                return new NoContentResult();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [user verification code updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserVerificationCodeUpdated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        /// <summary>
        /// Called when [after user verification code updated].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserVerificationCodeUpdated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);

        /// <summary>
        /// Puts the user verification code.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPut("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PutUserVerificationCode(Guid key, [FromBody] ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item)
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
                this.OnUserVerificationCodeUpdated(item);
                this.context.UserVerificationCodes.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserVerificationCodes.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
                this.OnAfterUserVerificationCodeUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Patches the user verification code.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="patch">The patch.</param>
        /// <returns>IActionResult.</returns>
        [HttpPatch("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult PatchUserVerificationCode(Guid key, [FromBody] Delta<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> patch)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var item = this.context.UserVerificationCodes.Where(i => i.Id == key).FirstOrDefault();

                if (item == null)
                {
                    return BadRequest();
                }
                patch.Patch(item);

                this.OnUserVerificationCodeUpdated(item);
                this.context.UserVerificationCodes.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserVerificationCodes.Where(i => i.Id == key);
                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
                this.OnAfterUserVerificationCodeUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Called when [user verification code created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnUserVerificationCodeCreated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        /// <summary>
        /// Called when [after user verification code created].
        /// </summary>
        /// <param name="item">The item.</param>
        partial void OnAfterUserVerificationCodeCreated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost]
        [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item)
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

                this.OnUserVerificationCodeCreated(item);
                this.context.UserVerificationCodes.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserVerificationCodes.Where(i => i.Id == item.Id);

                Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");

                this.OnAfterUserVerificationCodeCreated(item);

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
