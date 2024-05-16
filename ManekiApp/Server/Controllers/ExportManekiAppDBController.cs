using ManekiApp.Server.Data;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("/export/ManekiAppDB/userverificationcodes/csv")]
        [HttpGet("/export/ManekiAppDB/userverificationcodes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserVerificationCodesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUserVerificationCodes(), Request.Query), fileName);
        }

        [HttpGet("/export/ManekiAppDB/userverificationcodes/excel")]
        [HttpGet("/export/ManekiAppDB/userverificationcodes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserVerificationCodesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUserVerificationCodes(), Request.Query), fileName);
        }
    }
}