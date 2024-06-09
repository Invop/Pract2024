using Microsoft.AspNetCore.Mvc;

namespace ManekiApp.Server.Controllers
{
    /// <summary>
    /// Class UploadController.
    /// Implements the <see cref="Controller" />
    /// </summary>
    /// <seealso cref="Controller" />
    public partial class UploadController : Controller
    {
        /// <summary>
        /// The environment
        /// </summary>
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        public UploadController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        // Single file upload
        /// <summary>
        /// Singles the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("upload/single")]
        public IActionResult Single(IFormFile file)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload
        /// <summary>
        /// Multiples the specified files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("upload/multiple")]
        public IActionResult Multiple(IFormFile[] files)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload with parameter
        /// <summary>
        /// Posts the specified files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("upload/{id}")]
        public IActionResult Post(IFormFile[] files, int id)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Image file upload (used by HtmlEditor components)
        /// <summary>
        /// Images the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IActionResult.</returns>
        [HttpPost("upload/image")]
        public IActionResult Image(IFormFile file)
        {
            try
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = new FileStream(Path.Combine(environment.WebRootPath, fileName), FileMode.Create))
                {
                    // Save the file
                    file.CopyTo(stream);

                    // Return the URL of the file
                    var url = Url.Content($"~/{fileName}");

                    return Ok(new { Url = url });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
