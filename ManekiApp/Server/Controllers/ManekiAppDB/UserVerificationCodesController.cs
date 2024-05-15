using ManekiApp.Server.Data;
using ManekiApp.Server.Models.ManekiAppDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace ManekiApp.Server.Controllers.ManekiAppDB;

[Route("odata/ManekiAppDB/UserVerificationCodes")]
public partial class UserVerificationCodesController : ODataController
{
    private readonly ManekiAppDBContext context;

    public UserVerificationCodesController(ManekiAppDBContext context)
    {
        this.context = context;
    }


    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
    public IEnumerable<UserVerificationCode> GetUserVerificationCodes()
    {
        var items = context.UserVerificationCodes
            .AsQueryable<UserVerificationCode>();
        OnUserVerificationCodesRead(ref items);

        return items;
    }

    partial void OnUserVerificationCodesRead(
        ref IQueryable<UserVerificationCode> items);

    partial void OnUserVerificationCodeGet(
        ref SingleResult<UserVerificationCode> item);

    [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
    [HttpGet("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
    public SingleResult<UserVerificationCode> GetUserVerificationCode(Guid key)
    {
        var items = context.UserVerificationCodes.Where(i => i.Id == key);
        var result = SingleResult.Create(items);

        OnUserVerificationCodeGet(ref result);

        return result;
    }

    partial void OnUserVerificationCodeDeleted(UserVerificationCode item);
    partial void OnAfterUserVerificationCodeDeleted(UserVerificationCode item);

    [HttpDelete("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
    public IActionResult DeleteUserVerificationCode(Guid key)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var item = context.UserVerificationCodes
                .Where(i => i.Id == key)
                .FirstOrDefault();

            if (item == null) return BadRequest();

            OnUserVerificationCodeDeleted(item);
            context.UserVerificationCodes.Remove(item);
            context.SaveChanges();
            OnAfterUserVerificationCodeDeleted(item);

            return new NoContentResult();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
    }

    partial void OnUserVerificationCodeUpdated(UserVerificationCode item);
    partial void OnAfterUserVerificationCodeUpdated(UserVerificationCode item);

    [HttpPut("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
    [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
    public IActionResult PutUserVerificationCode(Guid key,
        [FromBody] UserVerificationCode item)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (item == null || item.Id != key) return BadRequest();

            OnUserVerificationCodeUpdated(item);
            context.UserVerificationCodes.Update(item);
            context.SaveChanges();

            var itemToReturn = context.UserVerificationCodes.Where(i => i.Id == key);
            Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
            OnAfterUserVerificationCodeUpdated(item);
            return new ObjectResult(SingleResult.Create(itemToReturn));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
    }

    [HttpPatch("/odata/ManekiAppDB/UserVerificationCodes(Id={Id})")]
    [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
    public IActionResult PatchUserVerificationCode(Guid key,
        [FromBody] Delta<UserVerificationCode> patch)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var item = context.UserVerificationCodes.Where(i => i.Id == key).FirstOrDefault();

            if (item == null) return BadRequest();

            patch.Patch(item);

            OnUserVerificationCodeUpdated(item);
            context.UserVerificationCodes.Update(item);
            context.SaveChanges();

            var itemToReturn = context.UserVerificationCodes.Where(i => i.Id == key);
            Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");
            OnAfterUserVerificationCodeUpdated(item);
            return new ObjectResult(SingleResult.Create(itemToReturn));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
    }

    partial void OnUserVerificationCodeCreated(UserVerificationCode item);
    partial void OnAfterUserVerificationCodeCreated(UserVerificationCode item);

    [HttpPost]
    [EnableQuery(MaxExpansionDepth = 10, MaxAnyAllExpressionDepth = 10, MaxNodeCount = 1000)]
    public IActionResult Post([FromBody] UserVerificationCode item)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (item == null) return BadRequest();

            OnUserVerificationCodeCreated(item);
            context.UserVerificationCodes.Add(item);
            context.SaveChanges();

            var itemToReturn = context.UserVerificationCodes.Where(i => i.Id == item.Id);

            Request.QueryString = Request.QueryString.Add("$expand", "AspNetUser");

            OnAfterUserVerificationCodeCreated(item);

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