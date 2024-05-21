using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using ManekiApp.Server.Data;

namespace ManekiApp.Server.Controllers
{
    public partial class ExportManekiAppDBController : ExportController
    {
        private readonly ManekiAppDBContext context;
        private readonly ManekiAppDBService service;

        public ExportManekiAppDBController(ManekiAppDBContext context, ManekiAppDBService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/ManekiAppDB/authorpages/csv")]
        [HttpGet("/export/ManekiAppDB/authorpages/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuthorPagesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAuthorPages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/authorpages/excel")]
        [HttpGet("/export/ManekiAppDB/authorpages/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuthorPagesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAuthorPages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/images/csv")]
        [HttpGet("/export/ManekiAppDB/images/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportImagesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetImages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/images/excel")]
        [HttpGet("/export/ManekiAppDB/images/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportImagesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetImages(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/posts/csv")]
        [HttpGet("/export/ManekiAppDB/posts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPostsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPosts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/posts/excel")]
        [HttpGet("/export/ManekiAppDB/posts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPostsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPosts(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/subscriptions/csv")]
        [HttpGet("/export/ManekiAppDB/subscriptions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubscriptionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSubscriptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/subscriptions/excel")]
        [HttpGet("/export/ManekiAppDB/subscriptions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubscriptionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSubscriptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/usersubscriptions/csv")]
        [HttpGet("/export/ManekiAppDB/usersubscriptions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserSubscriptionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUserSubscriptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/usersubscriptions/excel")]
        [HttpGet("/export/ManekiAppDB/usersubscriptions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserSubscriptionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUserSubscriptions(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/userverificationcodes/csv")]
        [HttpGet("/export/ManekiAppDB/userverificationcodes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserVerificationCodesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUserVerificationCodes(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ManekiAppDB/userverificationcodes/excel")]
        [HttpGet("/export/ManekiAppDB/userverificationcodes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserVerificationCodesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUserVerificationCodes(), Request.Query, false), fileName);
        }
    }
}
