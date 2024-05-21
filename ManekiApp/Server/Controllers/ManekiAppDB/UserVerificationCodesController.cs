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
    [Route("odata/ManekiAppDB/UserVerificationCodes")]
    public partial class UserVerificationCodesController : ODataController
    {
        private ManekiApp.Server.Data.ManekiAppDBContext context;

        public UserVerificationCodesController(ManekiApp.Server.Data.ManekiAppDBContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> GetUserVerificationCodes()
        {
            var items = this.context.UserVerificationCodes.AsQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode>();
            this.OnUserVerificationCodesRead(ref items);

            return items;
        }

        partial void OnUserVerificationCodesRead(ref IQueryable<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> items);

        partial void OnUserVerificationCodeGet(ref SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
        public SingleResult<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> GetUserVerificationCode(Guid key)
        {
            var items = this.context.UserVerificationCodes.Where(i => i.Id == key);
            var result = SingleResult.Create(items);

            OnUserVerificationCodeGet(ref result);

            return result;
        }
        partial void OnUserVerificationCodeDeleted(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        partial void OnAfterUserVerificationCodeDeleted(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);

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
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserVerificationCodeUpdated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        partial void OnAfterUserVerificationCodeUpdated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);

        [HttpPut("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutUserVerificationCode(Guid key, [FromBody]ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item)
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
                this.OnUserVerificationCodeUpdated(item);
                this.context.UserVerificationCodes.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserVerificationCodes.Where(i => i.Id == key);
                
                this.OnAfterUserVerificationCodeUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchUserVerificationCode(Guid key, [FromBody]Delta<ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode> patch)
        {
            try
            {
                if(!ModelState.IsValid)
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
                
                this.OnAfterUserVerificationCodeUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnUserVerificationCodeCreated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);
        partial void OnAfterUserVerificationCodeCreated(ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] ManekiApp.Server.Models.ManekiAppDB.UserVerificationCode item)
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

                this.OnUserVerificationCodeCreated(item);
                this.context.UserVerificationCodes.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.UserVerificationCodes.Where(i => i.Id == item.Id);

                

                this.OnAfterUserVerificationCodeCreated(item);

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
