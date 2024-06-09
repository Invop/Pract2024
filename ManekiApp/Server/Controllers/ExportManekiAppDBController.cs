using ManekiApp.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace ManekiApp.Server.Controllers
{
    /// <summary>
    /// Class ExportManekiAppDBController.
    /// Implements the <see cref="ManekiApp.Server.Controllers.ExportController" />
    /// </summary>
    /// <seealso cref="ManekiApp.Server.Controllers.ExportController" />
    public partial class ExportManekiAppDBController : ExportController
    {
        /// <summary>
        /// The context
        /// </summary>
        private readonly ManekiAppDBContext context;
        /// <summary>
        /// The service
        /// </summary>
        private readonly ManekiAppDBService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportManekiAppDBController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="service">The service.</param>
        public ExportManekiAppDBController(ManekiAppDBContext context, ManekiAppDBService service)
        {
            this.service = service;
            this.context = context;
        }

        /// <summary>
        /// Exports the author pages to CSV.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/authorpages/csv")]
        [HttpGet("/export/ManekiAppDB/authorpages/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuthorPagesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAuthorPages(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the author pages to excel.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/authorpages/excel")]
        [HttpGet("/export/ManekiAppDB/authorpages/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAuthorPagesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAuthorPages(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the images to CSV.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/images/csv")]
        [HttpGet("/export/ManekiAppDB/images/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportImagesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetImages(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the images to excel.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/images/excel")]
        [HttpGet("/export/ManekiAppDB/images/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportImagesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetImages(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the posts to CSV.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/posts/csv")]
        [HttpGet("/export/ManekiAppDB/posts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPostsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPosts(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the posts to excel.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/posts/excel")]
        [HttpGet("/export/ManekiAppDB/posts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPostsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPosts(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the subscriptions to CSV.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/subscriptions/csv")]
        [HttpGet("/export/ManekiAppDB/subscriptions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubscriptionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetSubscriptions(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the subscriptions to excel.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/subscriptions/excel")]
        [HttpGet("/export/ManekiAppDB/subscriptions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportSubscriptionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetSubscriptions(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the user subscriptions to CSV.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/usersubscriptions/csv")]
        [HttpGet("/export/ManekiAppDB/usersubscriptions/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserSubscriptionsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUserSubscriptions(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the user subscriptions to excel.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/usersubscriptions/excel")]
        [HttpGet("/export/ManekiAppDB/usersubscriptions/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserSubscriptionsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUserSubscriptions(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the user verification codes to CSV.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/userverificationcodes/csv")]
        [HttpGet("/export/ManekiAppDB/userverificationcodes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserVerificationCodesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUserVerificationCodes(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the user verification codes to excel.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/userverificationcodes/excel")]
        [HttpGet("/export/ManekiAppDB/userverificationcodes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserVerificationCodesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUserVerificationCodes(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the user chat payments to CSV.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/userchatpayments/csv")]
        [HttpGet("/export/ManekiAppDB/userchatpayments/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserChatPaymentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUserChatPayments(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the user chat payments to excel.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/userchatpayments/excel")]
        [HttpGet("/export/ManekiAppDB/userchatpayments/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserChatPaymentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUserChatPayments(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the user chat notifications to CSV.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/userchatnotifications/csv")]
        [HttpGet("/export/ManekiAppDB/userchatnotifications/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserChatNotificationsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetUserChatNotifications(), Request.Query, false), fileName);
        }

        /// <summary>
        /// Exports the user chat notifications to excel.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileStreamResult.</returns>
        [HttpGet("/export/ManekiAppDB/userchatnotifications/excel")]
        [HttpGet("/export/ManekiAppDB/userchatnotifications/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportUserChatNotificationsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetUserChatNotifications(), Request.Query, false), fileName);
        }
    }
}
