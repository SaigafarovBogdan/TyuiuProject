using Microsoft.AspNetCore.Mvc;

namespace ServerProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;

        private readonly ILogger<UploadController> _logger;

        public UploadController(ILogger<UploadController> logger, IWebHostEnvironment _environment)
        {
            _logger = logger;
            environment = _environment;
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload()
        {
            if (!HttpContext.Request.Form.Files.Any())
                return BadRequest("Нет файла для загрузки.");
            foreach (var file in HttpContext.Request.Form.Files)
            {
                var path = Path.Combine(environment.WebRootPath, "uploads", file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Ok();
        }
    }
}
